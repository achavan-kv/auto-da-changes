SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS OFF
GO

IF EXISTS (SELECT * FROM SysObjects
           WHERE Name = 'DN_SalesCommissions_GetBasicDetails' AND Type = 'P')
BEGIN
    DROP PROCEDURE DN_SalesCommissions_GetBasicDetails
END
GO


CREATE PROCEDURE DN_SalesCommissions_GetBasicDetails

--------------------------------------------------------------------------------
--
-- Project      : CoSACS .NET
-- File Name    : DN_SalesCommissions_GetBasicDetails.prc
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Sales Commissions/Spiffs get details for enquiry
-- Author       : John Croft
-- Date         : 16 October 2006
--
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 10/11/2006 pc   Added ability to query for all users, added commission percentage field
-- 04/07/2007 jec  Add branch number parameter
--03/07/2008 jec  UAT441, UAT442, UAT443 
-- 22/07/2009  jec CR1035 Enhancements
-- 24/09/2009 IP UAT(441) Changed column name returned from Commission Amount to Commission
-- 13/11/2009 FA UAT 678 check for null tax amount
-- 17/12/2009 FA same error as UAT 678 in commission % calculation - fixed everywhere.
-- 20/07/2011 jec CR1254 RI Changes
-- 21/09/2011 jec #8184  sales commission calculation is incorrect when the Tax type is exclusive		
-- 28/09/2011 ip #8184 - Do not need to subtract tax if inclusive, as tax already subtracted from Delivery Amount 
--						when calculating Commission %
-- 23/12/2011 jec #9355 Total Commission Percentage & Correct incorrect merge & fix from 5.13.
-- 08/05/2012 jec #9783 CR9439 - sales commission enquiry
-- 17/05/2012 jec #10118/#9783 Commission percentage added to SalesCommission table
-- 25/09/2012 ip #10691 - LW75224 - included buffno in select as (select distinct) prevented delivery for identical replacement
--						 from being displayed. (Merged from CoSACS 6.5)
-- 04/03/2014 jec #17587 - Change column heading from 'Agreement Date' to 'Booking Date'
--------------------------------------------------------------------------------
    -- Parameters
	@piBranchNo			char(3),		-- CR1035 jec 22/07/09
    @piEmpeeNo          int,
    @piDateFrom         datetime,
    @piDateTo           datetime,
    @piAccountNo        varchar(12),
	@piAgreementNo		int,
    @piSumDets          char(1),
    @piCategory			varchar(15),		-- This is Product Group
    @return             INTEGER OUTPUT



AS  --DECLARE
    -- Local variables

Declare @CommissionType char(2),
        @CommStartDate datetime,
        @CommRunDate datetime,
        @loseRebate char(4),
        @loseRepoMths int,
        @loseCancel char(4),
		@runno int,
		@CatOrDept BIT,				-- RI
		@AgrmtTaxType char(1),		-- 21/09/11			
		@datestart datetime,
		@datefinish datetime,
		@agreementTaxType char(1)

set @return=0

select @CatOrDept = value from CountryMaintenance where codename='RIDispCatAsDept'		-- RI 
select @AgrmtTaxType = value from CountryMaintenance where codename='agrmttaxtype'

set @agreementTaxType = (SELECT value FROM CountryMaintenance WHERE CodeName = 'agrmttaxtype')

-- convert description to Code
declare @category table (CatCode varchar(3))
insert into @category 
select category from codecat where category in('PCE','PCF','PCW','PCO')
						and  (category =case
		when @piCategory='Electrical' then 'PCE'
		when @piCategory='Furniture' then 'PCF'
		when @piCategory='Workstation' then 'PCW'
		when @piCategory='Other' then 'PCO'
		end
		or @piCategory	='All')

--    Get commissions totals summarised by rundate
if @piAccountNo='000000000000' and @piSumDets='S'
    Begin
    
    Select @piCategory as 'ProductGroup',Rundate,CAST(ROUND(sum(CommissionAmount),2) as DECIMAL(12,2)) as Commission		-- #9783
    from SalesCommission s 
    inner join Admin.[User] u on s.Employee= u.id	-- jec 04/07/07
	inner join stockitem st on s.ItemId =st.itemId	-- RI s.itemno =st.itemno 
					and s.StockLocn=st.stocklocn
	left outer join code co on co.code=cast(st.category as char(4)) 
						and co.category in(select MAX(category) from code co2 where co.code=co2.code and co2.category in('PCE','PCF','PCW','PCO'))	-- #9783 req because warranty categories are sometimes in PCE and PCF - causes double counting
				inner join @category ca on isnull(co.category,'PCO') = ca.CatCode	
            where (@piEmpeeno = -99 OR Employee=@piEmpeeno) --change 9/11/2006 sum for all employees
                and Rundate between @piDateFrom and @piDateTo
				and (@piBranchNo=cast(u.branchno as char(3)) or @piBranchNo='All')	-- #9783
            Group by /*Employee,*/Rundate --Don't think we need to group by employee if filtering on them? [PC]


    End   

--    Get commissions details by rundate
if @piAccountNo='000000000000' and @piSumDets='D'
    Begin
    Select AcctNo as 'Account No.',AgrmtNo as 'Agreement No.',CommissionType as Type,ROUND(sum(CommissionAmount),2) as Commission	-- #9783
        from SalesCommission
            where Employee=@piEmpeeno
                and Rundate = @piDateFrom 
            Group by AcctNo,AgrmtNo,CommissionType
	
    End  

--    Get commissions details by rundate/account number
if @piAccountNo='999999999999' and @piSumDets='D'
    Begin
    Select s.AcctNo as 'Account No.',s.AgrmtNo as 'Agreement No.',case when s.AgrmtNo>1 then a.dateagrmt else DATEADD(dd, DATEDIFF(dd,0,ac.dateacctopen), 0) end as 'Agreement/ Account Open Date', -- #17587 'Agreement Date',
			CAST(Round(sum(CommissionAmount),2)  as DECIMAL(12,2)) as Commission,@piCategory as 'ProductGroup'				-- #9783
        from SalesCommission s 
		inner join acct ac on s.AcctNo=ac.acctno			-- #17587 
        inner join Admin.[User] u on s.Employee= u.id
				inner join stockitem st on s.ItemId =st.ItemId			-- RI s.itemno =st.itemno 	
						and s.StockLocn=st.stocklocn
				--left outer join code co on co.code=cast(st.category as char(4)) and co.category in('PCE','PCF','PCW','PCO')			-- RI
				left outer join code co on co.code=cast(st.category as char(4)) 
						and co.category in(select MAX(category) from code co2 where co.code=co2.code and co2.category in('PCE','PCF','PCW','PCO'))		-- #9783 req because warranty categories are sometimes in PCE and PCF - causes double counting
				inner join @category ca on isnull(co.category,'PCO') = ca.CatCode		-- PCO if not on Code table
				,agreement a		-- jec 04/07/07
            where (@piEmpeeno = -99 or Employee=@piEmpeeno) --Added to correctly calc case for all users
                and Rundate = @piDateFrom
				and s.AcctNo=a.Acctno
				and s.AgrmtNo=a.Agrmtno
				and (@piBranchNo=cast(u.branchno as char(3)) or @piBranchNo='All')	-- CR1035 jec 22/07/09
            Group by s.AcctNo,s.AgrmtNo,case when s.AgrmtNo>1 then a.dateagrmt else DATEADD(dd, DATEDIFF(dd,0,ac.dateacctopen), 0) end	-- #17587 a.dateagrmt
    End  
--    Get commissions totals summarised by account no - specific salesperson rights
if @piAccountNo!='000000000000' and @piSumDets='S' and @piEmpeeno>=0 -- change 02/11/06
    Begin

    Select Rundate,s.AcctNo as 'Account No.',s.AgrmtNo as 'Agreement No.',case when s.AgrmtNo>1 then a.dateagrmt else DATEADD(dd, DATEDIFF(dd,0,ac.dateacctopen), 0) end as 'Agreement/ Account Open Date',	 -- #17587  'Agreement Date',
				ROUND(sum(CommissionAmount),2) as Commission		-- #9783
        from SalesCommission s,agreement a,	acct ac 			-- #17587 
            where s.AcctNo=@piAccountNo
				and (s.AgrmtNo=@piAgreementNo or @piAgreementNo=0)
                and (Employee=@piEmpeeno or @piEmpeeno<0)	-- change 01/11/06
				and s.AcctNo=a.Acctno
				and s.AgrmtNo=a.Agrmtno 
				and Rundate between @piDateFrom and @piDateTo	--change 02/11/06
				and s.AcctNo=ac.Acctno						-- #17587 
            Group by Rundate,s.AcctNo,s.AgrmtNo,case when s.AgrmtNo>1 then a.dateagrmt else DATEADD(dd, DATEDIFF(dd,0,ac.dateacctopen), 0) end	-- #17587 a.dateagrmt
		
    End  

--    Get commissions totals summarised by account no - all salesperson rights
if @piAccountNo!='000000000000' and @piSumDets='S' and @piEmpeeno<0 -- change 02/11/06
    Begin

    Select u.FullName as 'Employee',Rundate,s.AcctNo as 'Account No.',s.AgrmtNo as 'Agreement No.',case when s.AgrmtNo>1 then a.dateagrmt else DATEADD(dd, DATEDIFF(dd,0,ac.dateacctopen), 0) end as 'Agreement/ Account Open Date',	 -- #17587 'Agreement Date',
				ROUND(sum(CommissionAmount),2) as Commission	-- #9783
        from SalesCommission s
        INNER JOIN Admin.[User] u ON u.id = s.Employee
        INNER JOIN agreement a ON s.AcctNo=a.Acctno
		inner join acct ac on s.AcctNo=ac.acctno			-- #17587 
            where s.AcctNo=@piAccountNo
				and (s.AgrmtNo=@piAgreementNo or @piAgreementNo=0)
                and (Employee=@piEmpeeno or @piEmpeeno<0)	-- change 01/11/06
				and s.AgrmtNo=a.Agrmtno 
				and Rundate between @piDateFrom and @piDateTo	--change 02/11/06
            Group by u.FullName,Rundate,s.AcctNo,s.AgrmtNo,case when s.AgrmtNo>1 then a.dateagrmt else DATEADD(dd, DATEDIFF(dd,0,ac.dateacctopen), 0) end	-- #17587 a.dateagrmt
    End  

--    Get commissions details by account no
if @piAccountNo!='000000000000' and @piSumDets='D' 
    Begin
	-- set date range of commissions
	set @datefinish=(select max(datestart) 
			from interfacecontrol
				where interface='Commissions'
				and result='P')
	set @runno=(select max(runno) 
			from interfacecontrol
				where interface='Commissions'
				and result='P')
	set @datestart=(select max(datestart) 
			from interfacecontrol
				where interface='Commissions'
				and runno=(select max(i2.runno) 
							from interfacecontrol i2
								where i2.interface='Commissions'
								and i2.result='P'
								and i2.runno<@runno)
				and result='P')
	 
    Select Distinct 
			--s.Employee,
			@piCategory as 'ProductGroup',s.AcctNo as 'Account No.',	--- For excel CR1035
			u.fullname as 'Employee', 
			--s.Itemno,
			i.IUPC as Itemno,			-- RI
			i.Itemno as CourtsCode,			-- RI
			-- delamount net of tax jec 03/07/08  UAT443
			-- UAT 678 - problem with null values
			--case			-- #9783 remove
			--	when @agreementTaxType = 'I' then
			--		cast(d.transvalue as decimal(9,2)) - cast( ISNULL(l.taxamt,0) as decimal(9,2))	-- jec 03/07/08
			--else
			--		cast(d.transvalue as decimal(9,2)) END as 'Delivery Amount',
			CAST(s.NetCommissionValue as DECIMAL(12,2)) as 'Delivery Amount',		-- #9783
			-- order swapped #9783
			--cast(	round(	cast(commissionAmount as decimal(11,4)) /
			--				cast(NetCommissionValue		  as decimal(11,4)) * 100
			--		, 3) -- jec 03/07/08  round to two dec place				 
			--as decimal(9,4) ) as [Total Commission %], -- #9783
			CommissionPcent as [Total Commission %],		-- #9783 
            --cast(CommissionAmount as decimal(9,2)) as Commission,		-- jec 03/07/08 --IP - 24/09/09 - UAT(441)
            CommissionAmount as Commission,					-- #9783
			case		-- #9783	Calc for Commission no longer valid/required
				when CommissionAmount = 0 then 0
			-- NetCommissionValue is commissionable value i.e price less discount/tax etc.
				when NetCommissionValue!=0 then						-- jec 06/07/07
					case
						when @agreementTaxType = 'I' then
							cast(	round(	cast(commissionAmount as decimal(11,4))/(1.00+ (UpliftRate/100))  /		-- jec 16/07/09 UAT441
										(	cast(transvalue			  as decimal(11,4))-cast( ISNULL(l.taxamt,0) as decimal(9,3)) ) * 100	-- fa 17/12/09 same as UAT678 - problem with 0 tax
									, 3)		-- jec 03/07/08  round to two dec place
							as decimal(9,4) )		-- #9783
							--as [Total Commission %], --added 16/07/2009
							--	cast(	round(	cast(commissionAmount as decimal(11,4)) /
							--				cast(NetCommissionValue		  as decimal(11,4)) * 100
							--		, 1) -- jec 03/07/08  round to one dec place
							--as decimal(9,2) )  
					else
							cast(	round(	cast(commissionAmount as decimal(11,4))/(1.00+ (UpliftRate/100))  /		-- jec 16/07/09 UAT441
										(	cast(transvalue			  as decimal(11,4)) ) * 100	-- fa 17/12/09 same as UAT678 - problem with 0 tax
									, 3)		-- jec 03/07/08  round to two dec place
							as decimal(9,4) ) end		-- #9783
							--as [Total Commission %], --added 16/07/2009
							--	cast(	round(	cast(commissionAmount as decimal(11,4)) /
							--				cast(NetCommissionValue		  as decimal(11,4)) * 100
							--		, 1) -- jec 03/07/08  round to one dec place
							--as decimal(9,2) )
			else 
				case when @agreementTaxType = 'I' then

					-- UAT441 - show standard commission % (excluding uplift%)
					cast(	round(	cast(commissionAmount as decimal(11,4))/(1.00+ (UpliftRate/100))  /		-- jec 03/07/08 UAT441
								(	cast(transvalue			  as decimal(11,4))-cast( ISNULL(l.taxamt,0) as decimal(9,3)) ) * 100	-- fa 17/12/09 same as UAT678 - problem with 0 tax
							, 3)		-- jec 03/07/08  round to one dec place
					as decimal(9,3) ) --added 09/11/2006
				else

					-- UAT441 - show standard commission % (excluding uplift%)
					cast(	round(	cast(commissionAmount as decimal(11,4))/(1.00+ (UpliftRate/100))  /		-- jec 03/07/08 UAT441
								(	cast(transvalue			  as decimal(11,4)) ) * 100	-- fa 17/12/09 same as UAT678 - problem with 0 tax
							, 3)		-- jec 03/07/08  round to one dec place
					as decimal(9,3) ) end --added 09/11/2006

			end	as [Commission %],
			


			-- Uplift Commission Rate  - jec 22/06/07  CR36 Enhancements
			cast(UpliftRate as decimal(9,3)) as 'Uplift Commission %Rate',
			-- UAT441 - show total commission % (including uplift%)
				--	cast(	round(	cast(commissionAmount as decimal(11,4)) /
				--				cast(NetCommissionValue		  as decimal(11,4)) * 100
				--		, 3) -- jec 03/07/08  round to two dec place				 
				--as decimal(9,3) ) as [Total Commission %], --added 16/07/2009			
			d.datedel as 'Delivery Date', case 
					when s.CommissionType ='P' then 'Product'		
					when s.CommissionType ='PL' then 'Product Class'							-- RI
					when s.CommissionType ='PS' then 'Product SubClass'							-- RI
					when s.CommissionType ='PC' and @CatOrDept = 0 then 'Product Category'		-- RI
					when s.CommissionType ='PC' and @CatOrDept = 1 then 'Product Department'	-- RI  
					when s.CommissionType ='SP' then 'SPIFF'
					when s.CommissionType ='LS' then 'Linked SPIFF'
					when s.CommissionType ='TT' then 'Terms Type'
					when s.CommissionType ='TS' then 'Terms Type SPIFF'
					when s.CommissionType ='ES' then 'Extra SPIFF'
					else s.CommissionType
			End as 'Commission Type'
			,i.ItemDescr1,i.ItemDescr2,d.delorcoll as 'Del/Col',
			d.contractno as 'Contract No.',
			isnull(co.codedescript,'Not Categorised') as 'Category', 	-- CR1035
			d.buffno	-- IP - 25/09/12 - #10691 - LW75224	
        from Delivery d left outer join SalesCommission s 
					on s.AcctNo=d.AcctNo and 
						--s.itemno=d.ItemNo and
						s.ItemId=d.ItemId and			-- RI 
						S.AgrmtNo=d.AgrmtNo and 
						S.Buffno=d.BuffNo and
						s.StockLocn=d.StockLocn
left outer join lineItem l			-- required for taxamt jec 03/07/08
					on l.AcctNo=d.AcctNo and 
						--l.itemno=d.ItemNo and
						l.ItemId=d.ItemId and			-- RI  
						l.AgrmtNo=d.AgrmtNo and 					
						l.StockLocn=d.StockLocn,
				StockItem i 
			left outer join code co on CAST(i.category as varchar(4))= co.code and co.category like 'PC%', -- RI 
						
				Admin.[User] u 
            where s.AcctNo=@piAccountNo
				and s.AgrmtNo=@piAgreementNo
				and rundate=@pidatefrom			-- added 12/01/07 jec
                --and s.ItemNo=i.ItemNo
                and s.ItemId=i.ItemId				-- RI
                and s.StockLocn=i.stocklocn
				and s.Employee=u.id
				and ((d.transvalue>0 and s.CommissionAmount>0) or (d.transvalue<=0 and s.CommissionAmount<0))	-- jec 28/01/10
				-- and Rundate = @piDateFrom 

                --and Rundate between @piDateFrom and @piDateTo
            --Group by AcctNo,AgrmtNo

    End
 SET @return = @@ERROR

      
GO
GRANT EXECUTE ON DN_SalesCommissions_GetBasicDetails TO PUBLIC
GO
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO
-- end end end end end end end end end end end end end end end end end end end end end end end 
