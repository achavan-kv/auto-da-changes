
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS OFF
GO

IF EXISTS (SELECT * FROM SysObjects
           WHERE Name = 'DN_EOD_Commissions_Calculation' AND Type = 'P')
BEGIN
    DROP PROCEDURE DN_EOD_Commissions_Calculation
END
GO


CREATE PROCEDURE DN_EOD_Commissions_Calculation

--------------------------------------------------------------------------------
--===============================================================================================================================
----version <003>
--================================================================================================================================
--
-- Project      : CoSACS .NET
-- File Name    : DN_EOD_Commissions_Calculation.prc
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : EOD Calculate Sales Commissions/Spiffs
-- Author       : John Croft
-- Date         : 11 October 2006
--
--  
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 13/02/07	Jec Contract Number added to Sales Commission table for Warranties
-- 26/06/07 - 06/07/07 Jec Cater for Credit & Cash percentage rates (CR36 enhancements)
-- 30/11/07 - UAT 61 fix - Collection of item with linked discount
-- 14/05/08 - UAT test - move code due to error "invalid obect name #templineitemaudit" when @agrmtTaxType!='I'
-- 16/05/08	- jec UAT429 - taxamt +/- signed incorrectly when Agrmttaxtype=I
-- 04/08/08 - jec UAT507 - make extra spiff -ve on Repo or Collection
-- 03/10/08 - jec UAT541 - Calculate tax on Repossessions
-- 24/04/09 - jec UAT652 - Correct error.
-- 17/06/09 - jec 71324 - Commission only being calculated on one warranty when there is more than one (same itemno)
-- 17/07/09 - jec  Change decimal (9,2) to (15,2) 
-- 21/07/09  jec CR1035 Enhancements
-- 09/11/09 - jec 71868 - Add new country parameter
-- 25/01/10 - jec 72111 - Commission for repossession of warranties where transvalue=0
-- 14/07/11  jec CR1254 RI Changes
-- 12/09/11  jec 73681 #4471 Commission not calculated for identical product delivered following day with diff stock location
-- 03/01/12  jec #9355 Total Commission Percentage
-- 09/03/12  jec #9676 LW74344 - Sales commissions
-- 10/05/12  jec #9782 CR9439 - new uplift calculation
-- 17/05/12  jec #10118/#9782 Commission percentage added to SalesCommission table
--				 #10131 net delivery amount is changed after processing a cancellation in the GRT screen
-- 25/09/12  ip  #10691 - LW75224 - Commission was not being calculated for a delivery processed for an Identical Replacement. (Merged from CoSACS 6.5)
-- 16/01/14  ip  #17003 - Sales Commission - Commissionable value incorrect when multiple discounts linked to an item.
-- 17/01/14  ip  #17004 - Incorrect commisionable value when processing an Identical Replacement. Commissionable value not reduced by tax on item.
-- 19/02/14  ip  #17456 - Incorrect commisionable value when processing Delivery & Collection in the same run.
-- 14/03/14  ip  #17716 - Sales Commission for Cash & Go Return
-- 20/03/14  ip  #17882 - Sales Commission incorrect when Goods Return Exchange processed.
-- 28/04/14  ip  #17983 - Always return any linked discounts and no longer restrict by datetrans being within this run.
-- 29/04/14	 ip  #18182 - Discount TaxAmt was being updated incorrectly if the same discount was used during Exchange for different items.
------------------------------------------------------------------------------------------------------------
    -- Parameters
    @piRunNo            int,
    @return             INTEGER OUTPUT

AS  --DECLARE
    -- Local variables
set nocount on 

Declare @CommissionType char(2),
        @CommStartDate datetime,
        @CommRunDate datetime,
        @loseRebate char(5),
        @loseRepoMths int,
        @loseCancel char(5),
		@taxtype char(1),
		@taxrate numeric(5,2),		-- jec 03/10/08
		@agrmtTaxType char(1),		-- jec 08/01/07
		@SalesCommNetTax char(5)	-- jec 09/11/09

set @return=0

 -- Update rates without itemId   (Rate can be added before item exists)		-- RI
    UPDATE SalesCommissionRates
		set itemId=ISNULL(s.Id,0)
	from SalesCommissionRates c, StockInfo s 
	Where (c.itemId=0)
		and c.CommissionType in('P','SP','PL','PS')
		and ((((c.CommissionType='P' and ISNULL(s.IUPC,'')=c.ItemText) 
				or (c.CommissionType='SP' and ISNULL(s.IUPC,'')=c.ItemText)
				or (c.CommissionType='PL' and ISNULL(s.Class,'')=c.ItemText)
				or (c.CommissionType='PS' and ISNULL(s.SubClass,'')=c.ItemText)
			) and s.RepossessedItem=0))	-- Regular item Id
	
	UPDATE SalesCommissionRates
		set repoItemId=ISNULL(s.Id,0)
	from SalesCommissionRates c, StockInfo s 
	Where (c.repoItemId=0)
		and c.CommissionType in('P','SP','PL','PS')
		and ((((c.CommissionType='P' and ISNULL(s.IUPC,'')=c.ItemText) 
				or (c.CommissionType='SP' and ISNULL(s.IUPC,'')=c.ItemText)
				or (c.CommissionType='PL' and ISNULL(s.Class,'')=c.ItemText)
				or (c.CommissionType='PS' and ISNULL(s.SubClass,'')=c.ItemText)
			) and s.RepossessedItem=1))	-- Repossessed item Id
		
	 -- Update ItemId  - if Item exists
	 UPDATE SalesCommissionMultiSpiffRates
		set ItemId1=ISNULL(s.Id,0) 
	 from SalesCommissionMultiSpiffRates m INNER JOIN StockInfo s on m.Item1=s.iupc
	 Where m.Item1 !='' and m.ItemId1=0
	 
	 UPDATE SalesCommissionMultiSpiffRates
		set ItemId2=ISNULL(s.Id,0) 
	 from SalesCommissionMultiSpiffRates m INNER JOIN StockInfo s on m.Item2=s.iupc
	 Where m.Item2 !='' and m.ItemId2=0
	 
	 UPDATE SalesCommissionMultiSpiffRates
		set ItemId3=ISNULL(s.Id,0) 
	 from SalesCommissionMultiSpiffRates m INNER JOIN StockInfo s on m.Item3=s.iupc
	 Where m.Item3 !='' and m.ItemId3=0
	 
	 UPDATE SalesCommissionMultiSpiffRates
		set ItemId4=ISNULL(s.Id,0) 
	 from SalesCommissionMultiSpiffRates m INNER JOIN StockInfo s on m.Item4=s.iupc
	 Where m.Item4 !='' and m.ItemId4=0
	 
	 UPDATE SalesCommissionMultiSpiffRates
		set ItemId5=ISNULL(s.Id,0) 
	 from SalesCommissionMultiSpiffRates m INNER JOIN StockInfo s on m.Item5=s.iupc
	 Where m.Item5 !='' and m.ItemId5=0
	 
	 	

create table #tempCommissions
(
employee int,
Rundate datetime,
AcctNo char(12),
AgrmtNo int,
TermsType varchar(2),
CommissionType varchar(2),
SpiffType varchar(2),
ItemNo varchar(18),
ItemId INT,				-- RI
RepoItem BIT,			-- RI
KitNo varchar(18),
KitId INT,				-- RI
StockLocn smallint,
CommissionDate datetime,
DelorColl char(1),
TransValue money,
TaxAmt money,
CommissionAmount money,			-- #9355
SpiffAmount money,
Buffno int,
ContractNo varchar(10),
UpliftRate float,
AccType CHAR(1),
BranchNo char(3),		-- CR1035
CommissionPcent MONEY,	-- #9782
SpiffCommissionPcent MONEY,
Quantity int,               --#37529 
QuantityBefore int          --#37529 
)	
-- if commission per account type is false set cash percentage (percentagecash) = credit percentage (percentage)
-- this is to ensure that if different commissions were set up when commission per account type was true that they are bought in line.

if (select value from countrymaintenance
			where codename='comperacctype')='false'
	Begin
		update salescommissionrates
				set percentagecash=percentage, repopercentagecash=repopercentage		-- RI
		where (percentage!=percentagecash or repopercentage!=repopercentagecash)		-- RI
			and datefrom <=getdate()
			and dateto>getdate()
	End

-- if Repossessed products commission rate(ComRepoItem) is false set repo percentages repopercentage/repopercentagecash = regular percentages (percentage/percentagecash)
-- this is to ensure that if different commissions were set up when ComRepoItem was true that they are bought in line.

if (select value from countrymaintenance								-- RI
			where codename='ComRepoItem')='false'
	Begin
		update salescommissionrates
				set repopercentage=percentage, repopercentagecash=percentagecash		
		where (percentage!=repopercentage or percentagecash!=repopercentagecash)		
			and datefrom <=getdate()
			and dateto>getdate()
	End
	
-- get dates
set @CommStartDate = (select datestart from InterfaceControl
                            where interface='COMMISSIONS' and RunNo=(select max(RunNo) from InterfaceControl
                                    where interface='COMMISSIONS' and result='P'))
set @CommRunDate = (select datestart from InterfaceControl
                            where interface='COMMISSIONS' and RunNo=@piRunNo)

-- get Country Maintenance parameters
set @loseRebate = (select value from countryMaintenance
                        where CodeName='loserebatecomm')
set @loseRepoMths =(select value from countryMaintenance
                        where CodeName='loseRepocomm')
set @loseCancel =(select value from countryMaintenance
                        where CodeName='loseCancelcomm')
set @taxtype = (select taxtype from country)
set @agrmtTaxType = (select agrmtTaxType from country)	-- jec 08/01/07
set @taxrate=(select value from countrymaintenance where CodeName = 'taxrate')	-- jec 03/10/08
set @SalesCommNetTax = (select value from countryMaintenance
                        where CodeName='SalesCommNetTax')

-- load deliveries into temp table
-- "Cash & Go" Accounts
insert into #tempCommissions (employee,Rundate,AcctNo,AgrmtNo,TermsType,CommissionType,SpiffType,ItemNo,ItemId,RepoItem,KitNo,KitId,			-- RI
			StockLocn,CommissionDate,DelorColl,TransValue,TaxAmt,CommissionAmount,SpiffAmount,BuffNo,ContractNo,UpliftRate,AccType,
			BranchNo,CommissionPcent, SpiffCommissionPcent, Quantity, QuantityBefore)  --#37529 - Added Quantity and QuantityBefore	-- CR1035

select a.empeenoSale,@CommRunDate,d.acctno,d.agrmtno,' ',' ',' ',s.IUPC,d.itemId,s.RepossessedItem,' ',0,			-- RI
			d.stocklocn,a.Dateagrmt,d.delorcoll,d.transvalue,0,0,0,d.Buffno,d.ContractNo,0,' ',
			SUBSTRING(d.acctno,1,3),0, 0, d.quantity, 0	 --#37529		-- CR1035
            from agreement a,delivery d, StockInfo s			-- RI
                where a.acctno=d.acctno
                and a.agrmtno=d.agrmtno
                and a.agrmtno>1    -- Cash & Go
                and d.datetrans>@CommStartDate
				and d.datetrans<=@CommRunDate
				and d.ItemId=s.ID					-- RI
                and isnull(d.ftnotes,' ')!='XX'    -- datafix postings
                and (delorcoll='D'    -- delivery
                    or (d.delorcoll = 'C' and @loseCancel='true'    -- Cancel/Collection and commission lost on Cancelations (country parameter)
                        and exists(select * from delivery d2 
                            where d.acctno=d2.acctno	--and d.itemno=d2.itemno
								and d.ItemId=d2.ItemId				-- RI  
                                and d2.delorcoll = 'D' and isnull(d2.ftnotes,' ')!='XX'
                                and dateadd(m,@loseRepoMths,d2.datedel)>d.datedel))) -- delivery date + mths > Canceldate = lose commission  
SET @return = @@ERROR

if @@Error=0
    Begin    
-- Cash or Credit Accounts
    insert into #tempCommissions (employee,Rundate,AcctNo,AgrmtNo,TermsType,CommissionType,SpiffType,ItemNo,ItemId,RepoItem,KitNo,KitId,			-- RI
			StockLocn,CommissionDate,DelorColl,TransValue,TaxAmt,CommissionAmount,SpiffAmount,BuffNo,ContractNo,UpliftRate,AccType,
			BranchNo,CommissionPcent, SpiffCommissionPcent, Quantity, QuantityBefore) --#37529 - Added Quantity and QuantityBefore	-- CR1035

    select a.empeenoSale,@CommRunDate,d.acctno,d.agrmtno,' ',' ',' ',s.IUPC,d.ItemId,s.RepossessedItem,' ',0,			-- RI
			d.stocklocn,ac.Dateacctopen,d.delorcoll,d.transvalue,0,0,0,d.Buffno,d.ContractNo,0,' ',
			SUBSTRING(d.acctno,1,3),0, 0, d.quantity, 0  --#37529		-- CR1035
            from agreement a,delivery d,acct ac,StockInfo s			-- RI
                where a.acctno=d.acctno
                and a.agrmtno=d.agrmtno
                and a.acctno=ac.acctno
                and ac.accttype != 'S'    -- Cash or Credit
                and d.ItemId=s.ID					-- RI
				and (s.IUPC not in('ADDDR','ADDCR','RB')				-- RI
						or (s.IUPC ='RB' and @loseRebate='true')) -- Rebate, and commission lost on Rebates (country parameter)
                and d.datetrans>@CommStartDate
				and d.datetrans<=@CommRunDate
                and isnull(d.ftnotes,' ')!='XX'    -- datafix postings
                and ((d.delorcoll = 'D'    -- deliveries
					and d.transvalue>0	-- true delivery not collection of DT	jec 15/01/07 (Discount coding required - see below)
					and (	--IP - 25/09/12 - #10691 - LW75224 
							not exists(select * from delivery d3	-- previous delivery of item
                            where d.acctno=d3.acctno	--and d.itemno=d3.itemno 
								and d.ItemId=d3.ItemId					-- RI
								and d.stocklocn=d3.stocklocn		-- 73861  jec 12/09/11 
								and d.contractno=d3.contractno		-- 71324  jec 17/06/09 include contract no 
                                and d3.delorcoll = 'D' and isnull(d3.ftnotes,' ')!='XX'	--CHANGED--
								and d3.datetrans<d.datetrans) 	--)	--  jec 19/01/07 re: replaced on same day 
                    or exists(select * from delivery d4		--IP - 25/09/12 #10691 - LW75224 - where there has been previous delivery but with collection in between
                            where d.acctno=d4.acctno	
								and d.ItemId=d4.ItemId			
								and d.stocklocn=d4.stocklocn	
								and d.contractno=d4.contractno	
                                and d4.delorcoll = 'D' and isnull(d4.ftnotes,' ')!='XX'	
								and d4.datetrans<d.datetrans 
								and exists(select * from delivery d5
									where d.acctno = d5.acctno
										and d.ItemId=d5.ItemId				
										and d.stocklocn=d5.stocklocn		
										and d.contractno=d5.contractno		
										AND d5.delorcoll = 'C' and isnull(d5.ftnotes,' ')!='XX'
										AND d5.datetrans > d4.datetrans -- collection processed after previous delivery
										AND d5.datetrans < d.datetrans -- collection processed before this delivery
										)
									)
							
						 ) --IP - 25/09/12 #10691 - LW75224 
						 
                    or (s.IUPC='RB' and @loseRebate='true') ) -- Re-instated 02/07/07 Rebate and commission lost on Rebates (country parameter)

                    or (d.delorcoll = 'R' and @loseRepoMths>0    -- Repossessions and commission lost on Repos (country parameter)
                        and exists(select * from delivery d2 
                            where d.acctno=d2.acctno	--and d.itemno=d2.itemno
								and d.ItemId=d2.ItemId 
                                and d2.delorcoll = 'D' and isnull(d2.ftnotes,' ')!='XX'
                                and dateadd(m,@loseRepoMths,d2.datedel)>d.datedel)) -- delivery date + mths > repodate = lose commission

                    or (d.delorcoll = 'C' and @loseCancel='true'    -- Cancel/Collection and commission lost on Cancelations (country parameter)
                        and exists(select * from delivery d2 
                            where d.acctno=d2.acctno	--and d.itemno=d2.itemno
								and d.ItemId=d2.ItemId 
                                and d2.delorcoll = 'D' and isnull(d2.ftnotes,' ')!='XX'
                                and dateadd(m,@loseRepoMths,d2.datedel)>d.datedel))	--) -- delivery date + mths > Canceldate = lose commission
					
					-- Discounts			jec 16/01/07
					-- This code has been added/replicated due to above change on 15/01/07 re: collection of DT	
					or (d.delorcoll = 'D'
						and exists(select category from stockitem s
							where d.ItemId=s.ID			--d.itemno=s.itemno
								and d.stocklocn=s.stocklocn
								and s.category in (select code from code where category = 'PCDIS')) --IP - 29/10/09 - CoSACS Improvement - Remove hardcoded discounts & warranties
						)	

					-- Cancellations on TermsType			jec 15/01/07	
					or (d.delorcoll = 'D' and @loseCancel='true'    -- Cancel/Collection and commission lost on Cancelations (country parameter)
                        and s.IUPC='DT'			-- RI
                        and d.transvalue<0
						and exists(select * from delivery d2 
                            where d.acctno=d2.acctno	--and d.itemno=d2.itemno
								and d.ItemId=d2.ItemId				-- RI
                                and d2.delorcoll = 'D' and isnull(d2.ftnotes,' ')!='XX'
                                and dateadd(m,@loseRepoMths,d2.datedel)>d.datedel))) -- delivery date + mths > Canceldate = lose commission
    SET @return = @@ERROR          
    End

if @@Error=0
    Begin    
	-- Exchange Items		jec 24/01/07	
    insert into  #tempCommissions (employee,Rundate,AcctNo,AgrmtNo,TermsType,CommissionType,SpiffType,ItemNo,ItemId,RepoItem,KitNo,KitId,			-- RI
			StockLocn,CommissionDate,DelorColl,TransValue,TaxAmt,CommissionAmount,SpiffAmount,BuffNo,ContractNo,UpliftRate,AccType,
			BranchNo,CommissionPcent, SpiffCommissionPcent, Quantity, QuantityBefore)  --#37529 - Added Quantity and QuantityBefore	-- CR1035

	select a.empeenoSale,@CommRunDate,d.acctno,d.agrmtno,' ',' ',' ',s.IUPC,d.ItemId,s.RepossessedItem,' ',0,			-- RI
			d.stocklocn,ac.Dateacctopen,d.delorcoll,d.transvalue,0,0,0,d.BuffNo,d.ContractNo,0,' ',
			SUBSTRING(d.acctno,1,3),0, 0, d.quantity, 0	 --#37529		-- CR1035
            from agreement a,delivery d,acct ac,StockInfo s			-- RI
                where a.acctno=d.acctno
                and a.agrmtno=d.agrmtno
                and a.acctno=ac.acctno
                and ac.accttype != 'S'    -- Cash or Credit
                and d.ItemId=s.ID					-- RI
				and s.IUPC not in('ADDDR','ADDCR','RB')				-- RI
				and d.datetrans>@CommStartDate
				and d.datetrans<=@CommRunDate
                and isnull(d.ftnotes,' ')!='XX'    -- datafix postings
				and (d.delorcoll = 'C' -- collection/cancellation
                        and exists(select * from collectionreason c 
                            where d.acctno=c.acctno		--and d.itemno=c.itemno
								and d.ItemId=c.ItemId				-- RI
								and d.stocklocn=c.stocklocn  and c.collecttype='E'
								and d.datetrans>c.dateauthorised)
						-- not already included due to Cancel/Collection and commission lost on Cancelations (country parameter)
						and not exists(select * from #tempCommissions t
							where d.acctno=t.acctno				--and d.itemno=t.itemno
								and d.ItemId=t.ItemId			-- RI
								and d.stocklocn=t.stocklocn and d.delorcoll='C')
				 -- Termstype on Exchanges			jec 24/01/07	
					or (d.delorcoll = 'D' 
                        and s.IUPC='DT'			-- RI
                        and d.transvalue<0	-- Collect Service charge
						and not exists(select * from #tempCommissions t 
                            where d.acctno=t.acctno		--and d.itemno=t.itemno
								and d.ItemId=t.ItemId			-- RI 
                                and t.delorcoll = 'D' and d.transvalue=t.transvalue) )
				-- Termstype on Exchanges			jec 24/01/07	
					or (d.delorcoll = 'D'						
                        and s.IUPC='DT'			-- RI 
                        and d.transvalue>0	-- Deliver Service charge
						and not exists(select * from #tempCommissions t 
                            where d.acctno=t.acctno			--and d.itemno=t.itemno 
								and d.ItemId=t.ItemId			-- RI
                                and t.delorcoll = 'D' and d.transvalue=t.transvalue) )
                                
				)

	SET @return = @@ERROR          
    End

-- Update Account Type
if @@Error=0
	Begin
	UPDATE #tempCommissions
		set AccType=a.AcctType
	from #tempCommissions t join acct a on t.acctno=a.acctno
	SET @return = @@ERROR 

	End 

-- Update Uplift % rate for Salespersons
if @@Error=0
	Begin
	UPDATE #tempCommissions
		set UpliftRate=c.UpliftCommissionRate
	from #tempCommissions t join courtsperson c on t.employee=c.empeeno
	SET @return = @@ERROR 

	End  

-- Update Warranty commission date = date of delivery
if @@Error=0
	Begin
	update #tempCommissions
		set commissiondate=d.datetrans
	from stockitem s,delivery d,#tempCommissions t
		where s.category in(select distinct code from code where category = 'WAR')	-- Warranty --IP - 29/10/09 - CoSACS Improvement - Remove hardcoded discounts & warranties
			and s.ItemID=d.ItemId		-- RI
			and d.ItemId=t.ItemId		-- RI
			and d.acctno=t.acctno
			and d.agrmtno=t.agrmtno
			and d.ContractNo=t.ContractNo		-- jec 13/02/07  
	SET @return = @@ERROR 

	End  

-- Update Warranty transvalue for Repossessions where transvalue=0  jec 25/01/10
if @@Error=0
	Begin
	update #tempCommissions
		set TransValue=d.TransValue*-1
	from stockitem s,delivery d,#tempCommissions t
		where s.category in(select distinct code from code where category = 'WAR')	-- Warranty
			and s.ItemID=d.ItemId		-- RI
			and d.ItemId=t.ItemId		-- RI		
			and d.acctno=t.acctno
			and d.agrmtno=t.agrmtno
			and d.ContractNo=t.ContractNo
			and d.delorcoll='D'
			and t.DelorColl in('R','C')
			and t.transvalue=0
					  
	SET @return = @@ERROR 

	End 
	
-- following code moved to here before check on @agrmtTaxType='I' as #templineitemaudit used later when @agrmtTaxType!='I'  -- jec 14/05/08 UAT testing
-- Update taxamt for Collections from lineitemaudit (can't use lineitem taxamt as it is zero when item collected)
if @@Error=0
	Begin
	select l.acctno,l.agrmtno,l.itemno,l.stocklocn,CAST(0 as money) as ordval,CAST(0 as money) as ValueAfter,
		CAST(0 as money) as TaxAmtBefore,MAX(a.datechange) as datechange,l.contractno,l.ItemId, a.QuantityBefore   --#37529			-- RI
			into #templineitemaudit
			from #tempCommissions t inner join lineitem l on l.acctno=t.acctno
						and l.agrmtno=t.agrmtno
						and l.ItemId=t.ItemId			-- RI
						and l.contractno=t.contractno			-- #10131
						and l.stocklocn=t.stocklocn	
					inner join lineitemaudit a on l.acctno=a.acctno
						and l.agrmtno=a.agrmtno
						and l.ItemId=a.ItemId			-- RI
						and l.contractno=a.contractno			-- #10131
						and l.stocklocn=a.stocklocn
						and a.Datechange>@CommStartDate			-- #10131	
			where a.source='GRTCancel'				-- #10131  l.ordval=a.ValueAfter
			and a.TaxAmtBefore!=0
			and (t.DelorColl!='D'		-- not deliveries
				-- Collection of Discount is signified by delivery of +ve value
				or a.TaxAmtBefore<0		-- Discount		jec 30/11/07
				and exists(select category from stockitem s
							where t.ItemId=s.ItemId			-- RI t.itemno=s.itemno
								and t.stocklocn=s.stocklocn
								and s.category in (select code from code where category = 'PCDIS') ) )	-- Discount		jec 30/11/07 --IP - 29/10/09 - CoSACS Improvement - Remove hardcoded discounts & warranties
			group by l.acctno,l.agrmtno,l.itemno,l.stocklocn,l.contractno,l.ItemId, a.QuantityBefore  --#37529			-- RI

		SET @return = @@ERROR 

		--#17882 - TaxAmt for Exchange items.
		insert into #templineitemaudit
		select l.acctno,l.agrmtno,l.itemno,l.stocklocn,CAST(0 as money) as ordval,CAST(0 as money) as ValueAfter,
		l.TaxAmtBefore as TaxAmtBefore,@CommStartDate as datechange,l.contractno,l.ItemId, l.QuantityBefore     --#37529				
			from #tempCommissions t inner join lineitemaudit l on l.acctno=t.acctno
						and l.agrmtno=t.agrmtno
						and l.ItemId=t.ItemId			
						and l.contractno=t.contractno			
						and l.stocklocn=t.stocklocn		
			where l.source = 'GRTExchange'
			and l.TaxAmtBefore!=0
			and not exists(select * from #templineitemaudit a2
								where a2.acctno=t.acctno
						and a2.agrmtno=t.agrmtno
						and a2.ItemId=t.ItemId			
						and a2.contractno=t.contractno			
						and a2.stocklocn=t.stocklocn)		
			and (t.DelorColl!='D')		-- not deliveries
			and t.AgrmtNo = 1
			group by l.acctno,l.agrmtno,l.itemno,l.stocklocn,l.contractno,l.ItemId, l.TaxAmtBefore, l.QuantityBefore  --#37529

		--Identical Replacement - Get the QuantityBefore and TaxAmtBefore from Lineitemaudit
		--Item collected is removed from Revise screen, therefore can get the record from 
		--Lineitemaudit.
		insert into #templineitemaudit
		select l.acctno,l.agrmtno,l.itemno,l.stocklocn,CAST(0 as money) as ordval,CAST(0 as money) as ValueAfter,
		l.TaxAmtBefore as TaxAmtBefore,getdate() as datechange,l.contractno,l.ItemId, l.QuantityBefore				
			from #tempCommissions t inner join lineitemaudit l on l.acctno=t.acctno
						and l.agrmtno=t.agrmtno
						and l.ItemId=t.ItemId			
						and l.contractno=t.contractno			
						and l.stocklocn=t.stocklocn		
						and l.ValueBefore != 0
			inner join delivery d on t.AcctNo = d.acctno
			and d.agrmtno = t.AgrmtNo
			and d.itemid = t.ItemId
			and d.stocklocn = t.StockLocn
			and d.contractno = t.ContractNo
			and d.buffno = t.Buffno
			where not exists(select * from #templineitemaudit a2
								where a2.acctno=t.acctno
						and a2.agrmtno=t.agrmtno
						and a2.ItemId=t.ItemId			
						and a2.contractno=t.contractno			
						and a2.stocklocn=t.stocklocn)
			and l.source = 'Revise'
			and l.datechange = (select min(l2.DateChange)
									from lineitemaudit l2
									where l2.acctno = l.acctno
									and l2.agrmtno = l.agrmtno
									and l2.itemid = l.itemid
									and l2.stocklocn = l.stocklocn
									and l2.contractno = l.contractno
									and l2.datechange > d.datetrans
									and l2.source = l.source)		
			and (t.DelorColl!='D')		-- not deliveries
			and t.AgrmtNo = 1			-- not Cash & Go
			group by l.acctno,l.agrmtno,l.itemno,l.stocklocn,l.contractno,l.ItemId, l.TaxAmtAfter, l.QuantityBefore, l.TaxAmtBefore       

		
		--#17004 - For Identical Replacement there is no LineItemAudit record. Therefore retrieve TaxAmt from LineItem.
		insert into #templineitemaudit
		select l.acctno,l.agrmtno,l.itemno,l.stocklocn,CAST(0 as money) as ordval,CAST(0 as money) as ValueAfter,
		l.taxamt as TaxAmtBefore,@CommStartDate as datechange,l.contractno,l.ItemId, l.quantity	as QuantityBefore       --#37529				
			from #tempCommissions t inner join lineitem l on l.acctno=t.acctno
						and l.agrmtno=t.agrmtno
						and l.ItemId=t.ItemId			
						and l.contractno=t.contractno			
						and l.stocklocn=t.stocklocn		
			where not exists(select * from #templineitemaudit a2
								where a2.acctno=t.acctno
						and a2.agrmtno=t.agrmtno
						and a2.ItemId=t.ItemId			
						and a2.contractno=t.contractno			
						and a2.stocklocn=t.stocklocn)		
			and (t.DelorColl!='D')		-- not deliveries
			and t.AgrmtNo = 1			--#17716 - Not Cash & Go
			group by l.acctno,l.agrmtno,l.itemno,l.stocklocn,l.contractno,l.ItemId, l.taxamt, l.quantity    --#37529


	    --#17716 -- Cash & Go - Select tax amount from Lineitemaudit if Collection for Cash & Go
		insert into #templineitemaudit
		select l.acctno,l.agrmtno,l.itemno,l.stocklocn,CAST(0 as money) as ordval,CAST(0 as money) as ValueAfter,
		l.TaxAmtAfter as TaxAmtBefore,@CommStartDate as datechange,l.contractno,l.ItemId, l.QuantityBefore				
			from #tempCommissions t inner join lineitemaudit l on l.acctno=t.acctno
						and l.agrmtno=t.agrmtno
						and l.ItemId=t.ItemId			
						and l.contractno=t.contractno			
						and l.stocklocn=t.stocklocn		
						and l.ValueAfter != 0
			where not exists(select * from #templineitemaudit a2
								where a2.acctno=t.acctno
						and a2.agrmtno=t.agrmtno
						and a2.ItemId=t.ItemId			
						and a2.contractno=t.contractno			
						and a2.stocklocn=t.stocklocn)		
			and (t.DelorColl!='D')		-- not deliveries
			and t.AgrmtNo > 1			-- Cash & Go
			group by l.acctno,l.agrmtno,l.itemno,l.stocklocn,l.contractno,l.ItemId, l.TaxAmtAfter, l.QuantityBefore     --#37529

	End
-- end of moved code -- jec 14/05/08 UAT testing


--#38329 - This update should cover the following scenarios that have ocurred
--1. Goods Return was saved for Cancellation. The return was not processed immediately (different day)
-- The lineitemaudit record where the datechange >  @CommStartDate is not true as this run is now after the change.
--2. The QuantityBefore was not being retrieved correctly for items that do not have a taxamt as the values in this
-- instance are retrieved from Lineitem. But there is a record in Lineitemaudit, therefore update the QuantityBefore.

update a
set 
    QuantityBefore = la.QuantityBefore,
    TaxAmtBefore = la.TaxAmtBefore,
    datechange = la.Datechange
from 
    #templineitemaudit a
inner join 
    #tempCommissions t on a.acctno = t.AcctNo
    and a.agrmtno = t.AgrmtNo
    and a.ItemID = t.ItemId
    and a.stocklocn = t.StockLocn
    and a.contractno = t.ContractNo
inner join 
    LineitemAudit la on la.acctno = a.acctno
    and la.agrmtno = a.agrmtno
    and la.ItemID = a.ItemID
    and la.stocklocn = a.stocklocn
    and la.contractno = a.contractno
    and la.Datechange = (select 
                            max(la2.Datechange)
						 from 
                            lineitemaudit la2
						 where 
                            la2.acctno = la.acctno
						    and la2.agrmtno = la.agrmtno
						    and la2.ItemID = la.ItemID
						    and la2.stocklocn = la.stocklocn
						    and la2.contractno = la.contractno
						    and la2.source in ('GRTExchange', 'GRTCancel'))
where 
    t.DelorColl = 'C'
    and (a.QuantityBefore = 0 or a.TaxAmtBefore = 0)
    and (la.QuantityBefore != 0 or la.TaxAmtBefore !=0)


--#37529
 update  #tempCommissions
        set QuantityBefore = l.QuantityBefore       
			from #templineitemaudit l, #tempCommissions t
		where l.ItemId=t.ItemId					-- RI l.itemno=t.itemno
			and l.acctno=t.acctno
			and l.agrmtno=t.agrmtno
			and l.ContractNo=t.ContractNo
			and (t.DelorColl!='D'
				-- Collection of Discount is signified by delivery of +ve value				
				or t.TransValue>0			-- Discount		jec 30/11/07
				and exists(select category from stockitem s
							where t.ItemId=s.ItemId				-- RI t.itemno=s.itemno
								and t.stocklocn=s.stocklocn
								and s.category in (select code from code where category = 'PCDIS') ) )	-- Discount		jec 30/11/07 --IP - 29/10/09 - CoSACS Improvement - Remove hardcoded discounts & warranties
			and t.TransValue!=0					-- avoid divide by 0


-- Update taxamt if Country.taxtype = I (Stock tax included) and Country.agrmtTaxType = I (agreement tax included)
if @@Error=0          	-- Stock tax type removed jec 26/06/07
			and @agrmtTaxType='I'
			and @SalesCommNetTax = 'true'		-- Commission Exclusive of tax jec 09/11/09
	Begin
		-- Update taxamt for Deliveries from lineitem
		update #tempCommissions
		--set taxamt=l.taxamt --	removed jec 16/05/08 uat429 taxamt had wrong sign --* (t.TransValue/ABS(t.TransValue))	-- jec 05/07/07
        set taxamt= (t.TransValue - (t.TransValue / (1 + si.taxrate / 100)))
			from lineitem l,#tempCommissions t, stockinfo si
		where l.ItemId=t.ItemId					-- RI l.itemno=t.itemno
			and l.acctno=t.acctno
			and l.agrmtno=t.agrmtno
            and l.ItemID = si.Id
			and l.ContractNo=t.ContractNo		-- jec 13/02/07
			and t.DelorColl='D'
			and t.TransValue!=0					-- avoid divide by 0
  
		-- update table with tax from lineitemaudit			
		UPDATE #templineitemaudit
			set ordval=la.ValueAfter,ValueAfter=la.ValueAfter,TaxAmtBefore=la.TaxAmtBefore		-- #10131
			from #templineitemaudit t,lineitemaudit la
			where t.acctno=la.acctno
			and t.agrmtno=la.agrmtno
			and t.ItemId=la.ItemId			-- RI
			and t.stocklocn=la.stocklocn
			and t.datechange=la.datechange

		update  #tempCommissions
        set taxamt=((l.TaxAmtBefore / l.QuantityBefore) * ABS(t.Quantity)) * (t.TransValue/ABS(t.TransValue))     --#37529                                     
			from #templineitemaudit l, #tempCommissions t
		where l.ItemId=t.ItemId					-- RI l.itemno=t.itemno
			and l.acctno=t.acctno
			and l.agrmtno=t.agrmtno
			and l.ContractNo=t.ContractNo
			and (t.DelorColl!='D'
				-- Collection of Discount is signified by delivery of +ve value				
				or t.TransValue>0			-- Discount		jec 30/11/07
				and exists(select category from stockitem s
							where t.ItemId=s.ItemId				-- RI t.itemno=s.itemno
								and t.stocklocn=s.stocklocn
								and s.category in (select code from code where category = 'PCDIS') ) )	-- Discount		jec 30/11/07 --IP - 29/10/09 - CoSACS Improvement - Remove hardcoded discounts & warranties
			and t.TransValue!=0					-- avoid divide by 0
            and l.QuantityBefore !=0

		-- end code added 05/07/07 jec
		
		-- Set tax amount for Repossessions -- jec 03/10/08 Repos are not in lineitemaudit
		update  #tempCommissions
			set taxamt=t.TransValue-(t.TransValue*100/(100+@taxrate))
		from lineitem l, #tempCommissions t
		where l.ItemId=t.ItemId					-- RI l.itemno=t.itemno
			and l.acctno=t.acctno
			and l.agrmtno=t.agrmtno
			and l.ContractNo=t.ContractNo		-- jec 13/02/07
			and t.DelorColl='R'
			and @taxrate!=0					-- avoid divide by 0
			
	-- #10131 this code pertains to warranties where items delivered and collected within current commissions date range - lineitem taxamt=0 - get taxamt from lineitemaudit	
		if exists(select * from #tempCommissions t INNER JOIN stockinfo s on t.itemid=s.id and CAST(s.category as VARCHAR(4)) in (select code from code c where c.category='WAR')
					where taxamt=0 and delorcoll='D' )				
		UPDATE #tempCommissions
			set taxamt=l.TaxAmtBefore			
		from #tempCommissions t INNER JOIN #templineitemaudit l on l.ItemId=t.ItemId		
				and l.acctno=t.acctno
				and l.agrmtno=t.agrmtno
				and l.ContractNo=t.ContractNo		
				and t.DelorColl='D'
		where taxamt=0	
				
	End

-- Flag any Items belonging to a Kit
if @@Error=0
    Begin    
        update #tempCommissions
        set KitNo=k.kitno, KitId=k.KitId			-- RI
        from #tempCommissions t,KitClineItem k
        where t.acctno=k.acctno
        and t.ItemId=k.ComponentId			-- RI
		and t.transvalue=cast(k.OrdVal as money)   -- re-instated 17/01/07	-- !!need to fix data
        SET @return = @@ERROR          
    End

-- Apply discounts linked to Products
if @@Error=0
    Begin 

   select distinct l.origbr, l.acctno, l.agrmtno, l.itemno, l.itemsupptext, l.quantity, l.delqty, l.stocklocn, case when la.ValueBefore != 0 then la.ValueBefore else la.ValueAfter end as price, case when la.ValueBefore != 0 then la.ValueBefore else la.ValueAfter end as ordval, l.datereqdel,
				l.timereqdel, l.dateplandel, l.delnotebranch, l.qtydiff, l.itemtype, l.notes, case when la.TaxAmtBefore != 0 then la.TaxAmtBefore else la.TaxAmtAfter end as TaxAmt, l.isKit, l.deliveryaddress, l.parentitemno, 
				l.parentlocation, l.contractno, l.expectedreturndate, l.deliveryprocess, l.deliveryarea, l.DeliveryPrinted, l.assemblyrequired, l.damaged, l.OrderNo, 
				l.Orderlineno, l.PrintOrder, l.taxrate, l.ItemID, l.ParentItemID, l.SalesBrnNo, l.ID, l.Express, l.WarrantyGroupId
                into #tempdiscounts --#17456
			from lineitem l,#tempCommissions t,stockitem s, delivery d, LineitemAudit la
			where l.parentitemId=t.ItemId				 -- RI l.parentitemno=t.itemno		--l.itemno=t.itemno -- changed so linked discount is applied when item is cancelled (jec 03/07/07)
			and l.acctno=t.acctno
			and l.agrmtno=t.agrmtno
			and l.ContractNo=t.ContractNo		-- jec 13/02/07  
			and l.parentitemId != 0			-- RI
			and l.ItemId=s.ItemId				-- RI
			and l.stocklocn=s.stocklocn
			and l.acctno=d.acctno					-- #9676
			and l.ItemId=d.ItemId					-- #9676
			and l.agrmtno=d.agrmtno					-- #9676
            and l.parentitemId=d.parentitemId		-- #9676
			and l.stocklocn=d.stocklocn				-- #9676
			and l.ContractNo=d.ContractNo			-- #9676
			and s.category in (select code from code where category = 'PCDIS') -- Discounts --IP - 29/10/09 - CoSACS Improvement - Remove hardcoded discounts & warranties
			and l.agrmtno = 1 --#17716
            and la.AcctNo = l.acctno
            and la.AgrmtNo = l.agrmtno
            and la.ItemId = l.ItemId
            and la.StockLocn = l.stocklocn
            and la.contractno = l.contractno
            and la.Datechange = (select min(la2.Datechange)
                                    from lineitemaudit la2
                                    where la2.acctno = la.acctno
                                    and la2.agrmtno = la.agrmtno
                                    and la2.ItemID = la.ItemID
                                    and la2.stocklocn = la.stocklocn
                                    and la2.contractno = la.contractno
                                    and la2.Datechange >@CommStartDate)

            
        insert into #tempdiscounts
        select distinct l.origbr, l.acctno, l.agrmtno, l.itemno, l.itemsupptext, l.quantity, l.delqty, l.stocklocn, case when la.ValueBefore != 0 then la.ValueBefore else la.ValueAfter end as price, case when la.ValueBefore != 0 then la.ValueBefore else la.ValueAfter end as ordval, l.datereqdel,
				l.timereqdel, l.dateplandel, l.delnotebranch, l.qtydiff, l.itemtype, l.notes, case when la.TaxAmtBefore != 0 then la.TaxAmtBefore else la.TaxAmtAfter end as TaxAmt, l.isKit, l.deliveryaddress, l.parentitemno, 
				l.parentlocation, l.contractno, l.expectedreturndate, l.deliveryprocess, l.deliveryarea, l.DeliveryPrinted, l.assemblyrequired, l.damaged, l.OrderNo, 
				l.Orderlineno, l.PrintOrder, l.taxrate, l.ItemID, l.ParentItemID, l.SalesBrnNo, l.ID, l.Express, l.WarrantyGroupId
        from lineitem l,#tempCommissions t,stockitem s, delivery d, LineitemAudit la
			where l.parentitemId=t.ItemId				 -- RI l.parentitemno=t.itemno		--l.itemno=t.itemno -- changed so linked discount is applied when item is cancelled (jec 03/07/07)
			and l.acctno=t.acctno
			and l.agrmtno=t.agrmtno
			and l.ContractNo=t.ContractNo		-- jec 13/02/07  
			and l.parentitemId != 0			-- RI
			and l.ItemId=s.ItemId				-- RI
			and l.stocklocn=s.stocklocn
			and l.acctno=d.acctno					-- #9676
			and l.ItemId=d.ItemId					-- #9676
			and l.agrmtno=d.agrmtno					-- #9676
            and l.parentitemId=d.parentitemId		-- #9676
			and l.stocklocn=d.stocklocn				-- #9676
			and l.ContractNo=d.ContractNo			-- #9676
			and s.category in (select code from code where category = 'PCDIS') -- Discounts --IP - 29/10/09 - CoSACS Improvement - Remove hardcoded discounts & warranties
			and l.agrmtno = 1 --#17716
            and la.AcctNo = l.acctno
            and la.AgrmtNo = l.agrmtno
            and la.ItemId = l.ItemId
            and la.StockLocn = l.stocklocn
            and la.contractno = l.contractno
            and la.Datechange = (select max(la2.Datechange)
                                    from lineitemaudit la2
                                    where la2.acctno = la.acctno
                                    and la2.agrmtno = la.agrmtno
                                    and la2.ItemID = la.ItemID
                                    and la2.stocklocn = la.stocklocn
                                    and la2.contractno = la.contractno)
            and not exists(select * from #tempdiscounts t2
                                where t2.acctno = l.acctno
                                and t2.agrmtno = l.agrmtno
                                and t2.ItemID = l.ItemId
                                and t2.stocklocn = l.StockLocn
                                and t2.contractno = l.contractno)



		--#17716 - Cash & Go - Select discounts
        insert into #tempdiscounts
		select distinct l.origbr, l.acctno, l.agrmtno, l.itemno, l.itemsupptext, l.quantity, l.delqty, l.stocklocn,  case when la.ValueBefore != 0 then la.ValueBefore else la.ValueAfter end as price, case when la.ValueBefore != 0 then la.ValueBefore else la.ValueAfter end as ordval, l.datereqdel,
				l.timereqdel, l.dateplandel, l.delnotebranch, l.qtydiff, l.itemtype, l.notes,  case when la.TaxAmtBefore != 0 then la.TaxAmtBefore else la.TaxAmtAfter end as TaxAmt, l.isKit, l.deliveryaddress, l.parentitemno, 
				l.parentlocation, l.contractno, l.expectedreturndate, l.deliveryprocess, l.deliveryarea, l.DeliveryPrinted, l.assemblyrequired, l.damaged, l.OrderNo, 
				l.Orderlineno, l.PrintOrder, l.taxrate, l.ItemID, l.ParentItemID, l.SalesBrnNo, l.ID, l.Express, l.WarrantyGroupId
		from lineitem l inner join #tempCommissions t on l.parentitemId=t.ItemId
		and l.acctno=t.acctno
		and l.agrmtno=t.agrmtno
		and l.ContractNo=t.ContractNo		-- jec 13/02/07  
		and l.parentitemId != 0			-- RI
		inner join stockitem s on l.ItemId=s.ItemId				-- RI
		and l.stocklocn=s.stocklocn
		inner join delivery d on l.acctno=d.acctno					-- #9676
		and l.ItemId=d.ItemId					-- #9676
		and l.agrmtno=d.agrmtno					-- #9676
		and l.parentitemId=d.parentitemId		-- #9676
		and l.stocklocn=d.stocklocn				-- #9676
		and l.ContractNo=d.ContractNo			-- #9676
		inner join lineitemaudit la on la.acctno = l.acctno
		and la.agrmtno = l.agrmtno
		and la.ItemID = l.ItemID 
		and la.ParentItemID = l.ParentItemID
		and la.stocklocn = l.stocklocn
		and la.contractno = l.contractno
		where la.Datechange = (select min(la2.Datechange)
                                    from lineitemaudit la2
                                    where la2.acctno = la.acctno
                                    and la2.agrmtno = la.agrmtno
                                    and la2.ItemID = la.ItemID
                                    and la2.stocklocn = la.stocklocn
                                    and la2.contractno = la.contractno
                                    and la2.Datechange >@CommStartDate)

		and s.category in (select code from code where category = 'PCDIS') -- Discounts --IP - 29/10/09 - CoSACS Improvement - Remove hardcoded discounts & warranties
		and l.agrmtno > 1 --#17716


		-- set up tax amount		--jec 30/11/07
		Update #tempdiscounts
			set TaxAmt=TaxAmtBefore
		From #tempdiscounts d INNER JOIN #templineitemaudit l 
					on d.acctno = l.acctno and d.agrmtno = l.agrmtno 
						and d.ItemId = l.ItemId				-- RI
						and d.stocklocn = l.stocklocn
		where d.taxamt = 0			--#18182
	 

		-- reduce price by discount	
		--select acctno, parentitemid,agrmtno, sum(price) as discValue 
		--					Into ##tempdiscounts from #tempdiscounts
		--					group by acctno, parentitemid,agrmtno

		update #tempCommissions set QuantityBefore = 1 where TransValue!=0 and DelorColl != 'D' and QuantityBefore = 0
	
			--#17003
	   ;with discVal as (select acctno, parentitemid,agrmtno, sum(price) as discValue 
							from #tempdiscounts
							group by acctno, parentitemid,agrmtno)

        update  #tempCommissions
        set transvalue= case -- #37529
                            when t.DelorColl != 'D'
                            then transvalue + ((t.TransValue/ABS(t.TransValue)) * ((l.discValue / ABS(T.QuantityBefore)) * ABS(t.Quantity))) 
                            else transvalue + ((t.TransValue/ABS(t.TransValue)) * l.discValue)
                        end
			from discVal l, #tempCommissions t			--#17003
		where l.parentitemId=t.ItemId						-- RI l.parentitemno=t.itemno
			and l.acctno=t.acctno
			and l.agrmtno=t.agrmtno
			and t.TransValue!=0 --IP - 04/09/08 - UAT5.1 - UAT(545) - prevent divide by zero

		--#17456 - Update ItemTax where Delivery/Collection in the same run 
		;WITH CollectionTax as 
		(
			select AcctNo, ItemId, StockLocn, ContractNo, DelorColl, TaxAmt
			from #tempCommissions
			where DelorColl = 'C'
		)

		update #tempCommissions
		set TaxAmt = c.TaxAmt * -1
		from CollectionTax c inner join #tempCommissions t on c.AcctNo = t.AcctNo
		and c.ItemId = t.ItemId
		and c.StockLocn = t.StockLocn
		and c.ContractNo = t.ContractNo
		and t.DelorColl = 'D'
		and t.TaxAmt = 0

		if @agrmtTaxType='I'		-- Stock tax type removed jec 26/06/07
			and @SalesCommNetTax ='true'	-- Commission inclusive of tax jec 09/11/09
		Begin	

		--#17003
		   ;with discTax as (select acctno, parentitemid,agrmtno, sum(taxamt) as discTax
							from #tempdiscounts
							group by acctno, parentitemid,agrmtno)

		update  #tempCommissions
        set taxamt= case        --#37529 
                        when t.DelorColl != 'D'
                        then t.taxamt+ (((l.discTax / t.QuantityBefore) * (t.TransValue/ABS(t.TransValue))) * ABS(t.Quantity))   	  --#17003		-- jec 05/07/07     --ILYAS
                        else t.taxamt+ (l.discTax * (t.TransValue/ABS(t.TransValue)))   --#17003		-- jec 05/07/07     
                    end  
			from discTax l, #tempCommissions t		--#17003
		where l.parentitemId=t.ItemId							-- RI l.parentitemno=t.itemno
			and l.acctno=t.acctno
			and l.agrmtno=t.agrmtno
			and t.TransValue!=0 --IP - 04/09/08 - UAT5.1 - UAT(545) - prevent divide by zero
		End
		-- delete linked discount (excluded from further commissions)
		
		delete #tempCommissions 
		from #tempCommissions t,#tempdiscounts d
				where d.ItemId=t.ItemId					-- RI d.itemno=t.itemno
						and d.acctno=t.acctno
						and d.agrmtno=t.agrmtno
						and t.kitId=0			-- do not delete discount if part of kit
	End

-- Flag any Items belonging to a Kit - was here. Moved to before linked discounts (jec 16/01/07)

-- Update termstype from ACCT
if @@Error=0
    Begin  
        update #tempCommissions
        set TermsType=a.Termstype
        from #tempCommissions t,Acct a, StockInfo s			-- RI
        where t.acctno=a.acctno
			and t.ItemId=s.ID					-- RI
			and s.IUPC in('DT','RB')			-- RI
            SET @return = @@ERROR          
    End
-- Calculate Commissions - Product
-- for Kit products commission is based on % applied to Kit
if @@Error=0
    Begin 
        update #tempCommissions
        set commissiontype=c.CommissionType,
            CommissionAmount= case				-- RI
				when AccType in ('C','S') and t.RepoItem=0 then cast((t.transValue-t.taxamt)*
							case when c.PercentageCash>0 then (c.PercentageCash+UpliftRate) else c.PercentageCash end/100 as float)                -- #9782
				when AccType  not in ('C','S') and t.RepoItem=0 then cast((t.transValue-t.taxamt)*
							case when c.Percentage>0 then (c.Percentage+UpliftRate) else c.Percentage end/100 as float)                -- #9782
				when AccType in ('C','S') and t.RepoItem=1 then cast((t.transValue-t.taxamt)*
							case when c.RepoPercentageCash>0 then (c.RepoPercentageCash+UpliftRate) else c.RepoPercentageCash end/100 as float)                -- #9782
				when AccType  not in ('C','S') and t.RepoItem=1 then cast((t.transValue-t.taxamt)*
							case when c.RepoPercentage>0 then (c.RepoPercentage+UpliftRate) else c.RepoPercentage end/100 as float)                -- #9782
				End,
			CommissionPcent= case				-- #9782
				when AccType in ('C','S') and t.RepoItem=0 then 
							case when c.PercentageCash>0 then (c.PercentageCash+UpliftRate) else c.PercentageCash end               -- #9782
				when AccType  not in ('C','S') and t.RepoItem=0 then 
							case when c.Percentage>0 then (c.Percentage+UpliftRate) else c.Percentage end              -- #9782
				when AccType in ('C','S') and t.RepoItem=1 then 
							case when c.RepoPercentageCash>0 then (c.RepoPercentageCash+UpliftRate) else c.RepoPercentageCash end               -- #9782
				when AccType  not in ('C','S') and t.RepoItem=1 then 
							case when c.RepoPercentage>0 then (c.RepoPercentage+UpliftRate) else c.RepoPercentage end               -- #9782				
				End
        from #tempCommissions t,SalesCommissionRates c
        where c.CommissionType='P '
            and t.commissiontype=' '
            and t.KitId=c.ItemId				-- RI
            and t.CommissionDate between c.datefrom and c.dateto    
        SET @return = @@ERROR          
    End
-- Calculate Commissions - Product
if @@Error=0
    Begin 
        update #tempCommissions
        set commissiontype=c.CommissionType,
        CommissionAmount= case				-- RI
				when AccType in ('C','S') and t.RepoItem=0 then cast((t.transValue-t.taxamt)*
							case when c.PercentageCash>0 then (c.PercentageCash+UpliftRate) else c.PercentageCash end/100 as float)                -- #9782
				when AccType  not in ('C','S') and t.RepoItem=0 then cast((t.transValue-t.taxamt)*
							case when c.Percentage>0 then (c.Percentage+UpliftRate) else c.Percentage end/100 as float)                -- #9782
				when AccType in ('C','S') and t.RepoItem=1 then cast((t.transValue-t.taxamt)*
							case when c.RepoPercentageCash>0 then (c.RepoPercentageCash+UpliftRate) else c.RepoPercentageCash end/100 as float)                -- #9782
				when AccType  not in ('C','S') and t.RepoItem=1 then cast((t.transValue-t.taxamt)*
							case when c.RepoPercentage>0 then (c.RepoPercentage+UpliftRate) else c.RepoPercentage end/100 as float)                -- #9782
				End,
			CommissionPcent= case				-- #9782
				when AccType in ('C','S') and t.RepoItem=0 then 
							case when c.PercentageCash>0 then (c.PercentageCash+UpliftRate) else c.PercentageCash end               -- #9782
				when AccType  not in ('C','S') and t.RepoItem=0 then 
							case when c.Percentage>0 then (c.Percentage+UpliftRate) else c.Percentage end              -- #9782
				when AccType in ('C','S') and t.RepoItem=1 then 
							case when c.RepoPercentageCash>0 then (c.RepoPercentageCash+UpliftRate) else c.RepoPercentageCash end               -- #9782
				when AccType  not in ('C','S') and t.RepoItem=1 then 
							case when c.RepoPercentage>0 then (c.RepoPercentage+UpliftRate) else c.RepoPercentage end               -- #9782				
				End
        from #tempCommissions t,SalesCommissionRates c
        where c.CommissionType='P '
        and t.commissiontype=' '
        and t.ItemId=c.ItemId				-- RI
        and t.CommissionDate between c.datefrom and c.dateto     

        SET @return = @@ERROR          
    End
    
-- Calculate Commissions - SKU		-- RI
if @@Error=0
    Begin 
        update #tempCommissions
        set commissiontype=c.CommissionType,
        CommissionAmount= case				-- RI
				when AccType in ('C','S') and t.RepoItem=0 then cast((t.transValue-t.taxamt)*
							case when c.PercentageCash>0 then (c.PercentageCash+UpliftRate) else c.PercentageCash end/100 as float)                -- #9782
				when AccType  not in ('C','S') and t.RepoItem=0 then cast((t.transValue-t.taxamt)*
							case when c.Percentage>0 then (c.Percentage+UpliftRate) else c.Percentage end/100 as float)                -- #9782
				when AccType in ('C','S') and t.RepoItem=1 then cast((t.transValue-t.taxamt)*
							case when c.RepoPercentageCash>0 then (c.RepoPercentageCash+UpliftRate) else c.RepoPercentageCash end/100 as float)                -- #9782
				when AccType  not in ('C','S') and t.RepoItem=1 then cast((t.transValue-t.taxamt)*
							case when c.RepoPercentage>0 then (c.RepoPercentage+UpliftRate) else c.RepoPercentage end/100 as float)                -- #9782
				End,
			CommissionPcent= case				-- #9782
				when AccType in ('C','S') and t.RepoItem=0 then 
							case when c.PercentageCash>0 then (c.PercentageCash+UpliftRate) else c.PercentageCash end               -- #9782
				when AccType  not in ('C','S') and t.RepoItem=0 then 
							case when c.Percentage>0 then (c.Percentage+UpliftRate) else c.Percentage end              -- #9782
				when AccType in ('C','S') and t.RepoItem=1 then 
							case when c.RepoPercentageCash>0 then (c.RepoPercentageCash+UpliftRate) else c.RepoPercentageCash end               -- #9782
				when AccType  not in ('C','S') and t.RepoItem=1 then 
							case when c.RepoPercentage>0 then (c.RepoPercentage+UpliftRate) else c.RepoPercentage end               -- #9782				
				End
        from #tempCommissions t,SalesCommissionRates c, StockInfo s
        where c.CommissionType='SK'
        and t.commissiontype=' '
        and t.ItemId=s.ID 
        and s.SKU=c.ItemText				-- SKU 			
        and t.CommissionDate between c.datefrom and c.dateto     

        SET @return = @@ERROR          
    End
    
-- Calculate Commissions - Product SubClass		-- RI
if @@Error=0
    Begin 
        update #tempCommissions
        set commissiontype=c.CommissionType,
            CommissionAmount= case				-- RI
				when AccType in ('C','S') and t.RepoItem=0 then cast((t.transValue-t.taxamt)*
							case when c.PercentageCash>0 then (c.PercentageCash+UpliftRate) else c.PercentageCash end/100 as float)                -- #9782
				when AccType  not in ('C','S') and t.RepoItem=0 then cast((t.transValue-t.taxamt)*
							case when c.Percentage>0 then (c.Percentage+UpliftRate) else c.Percentage end/100 as float)                -- #9782
				when AccType in ('C','S') and t.RepoItem=1 then cast((t.transValue-t.taxamt)*
							case when c.RepoPercentageCash>0 then (c.RepoPercentageCash+UpliftRate) else c.RepoPercentageCash end/100 as float)                -- #9782
				when AccType  not in ('C','S') and t.RepoItem=1 then cast((t.transValue-t.taxamt)*
							case when c.RepoPercentage>0 then (c.RepoPercentage+UpliftRate) else c.RepoPercentage end/100 as float)                -- #9782
				End,
			CommissionPcent= case				-- #9782
				when AccType in ('C','S') and t.RepoItem=0 then 
							case when c.PercentageCash>0 then (c.PercentageCash+UpliftRate) else c.PercentageCash end               -- #9782
				when AccType  not in ('C','S') and t.RepoItem=0 then 
							case when c.Percentage>0 then (c.Percentage+UpliftRate) else c.Percentage end              -- #9782
				when AccType in ('C','S') and t.RepoItem=1 then 
							case when c.RepoPercentageCash>0 then (c.RepoPercentageCash+UpliftRate) else c.RepoPercentageCash end               -- #9782
				when AccType  not in ('C','S') and t.RepoItem=1 then 
							case when c.RepoPercentage>0 then (c.RepoPercentage+UpliftRate) else c.RepoPercentage end               -- #9782				
				End
        from #tempCommissions t,SalesCommissionRates c, StockInfo s
        where c.CommissionType='PS'
        and t.commissiontype=' ' 
        and t.ItemId= s.ID						
        and ISNULL(s.SubClass,'')=c.itemText		
        and t.CommissionDate between c.datefrom and c.dateto     
        
        SET @return = @@ERROR          
    End
      
-- Calculate Commissions - Product Class

if @@Error=0
    Begin 
        update #tempCommissions
        set commissiontype=c.CommissionType,
            CommissionAmount= case				-- RI
				when AccType in ('C','S') and t.RepoItem=0 then cast((t.transValue-t.taxamt)*
							case when c.PercentageCash>0 then (c.PercentageCash+UpliftRate) else c.PercentageCash end/100 as float)                -- #9782
				when AccType  not in ('C','S') and t.RepoItem=0 then cast((t.transValue-t.taxamt)*
							case when c.Percentage>0 then (c.Percentage+UpliftRate) else c.Percentage end/100 as float)                -- #9782
				when AccType in ('C','S') and t.RepoItem=1 then cast((t.transValue-t.taxamt)*
							case when c.RepoPercentageCash>0 then (c.RepoPercentageCash+UpliftRate) else c.RepoPercentageCash end/100 as float)                -- #9782
				when AccType  not in ('C','S') and t.RepoItem=1 then cast((t.transValue-t.taxamt)*
							case when c.RepoPercentage>0 then (c.RepoPercentage+UpliftRate) else c.RepoPercentage end/100 as float)                -- #9782
				End,
			CommissionPcent= case				-- #9782
				when AccType in ('C','S') and t.RepoItem=0 then 
							case when c.PercentageCash>0 then (c.PercentageCash+UpliftRate) else c.PercentageCash end               -- #9782
				when AccType  not in ('C','S') and t.RepoItem=0 then 
							case when c.Percentage>0 then (c.Percentage+UpliftRate) else c.Percentage end              -- #9782
				when AccType in ('C','S') and t.RepoItem=1 then 
							case when c.RepoPercentageCash>0 then (c.RepoPercentageCash+UpliftRate) else c.RepoPercentageCash end               -- #9782
				when AccType  not in ('C','S') and t.RepoItem=1 then 
							case when c.RepoPercentage>0 then (c.RepoPercentage+UpliftRate) else c.RepoPercentage end               -- #9782				
				End
        from #tempCommissions t,SalesCommissionRates c, StockInfo s
        where c.CommissionType='PL'
        and t.commissiontype=' ' 
        and t.ItemId= s.ID						-- RI
        and ISNULL(s.Class,'')=c.itemText		-- RI
        and t.CommissionDate between c.datefrom and c.dateto     
        
        SET @return = @@ERROR          
    End
    
  -- Calculate Commissions - Termstype (based on service charge) -- moved to before Category commission so TT takes priority over PC  // jec 21/07/11
if @@Error=0
    Begin
        update #tempCommissions
        set commissiontype=c.CommissionType,CommissionAmount=case
            when c.percentage >0 then
            cast((t.transValue-t.taxamt)*c.Percentage/100 as float)
            else
			cast((t.transValue/abs(t.transValue))*c.value as float) -- 15/01/07 jec
            end,
            CommissionPcent= c.percentage				-- #9782
        from #tempCommissions t,SalesCommissionRates c, StockInfo s		-- RI
        where c.CommissionType='TT'
        and t.commissiontype=' '
        and t.ItemId=s.ID				-- RI
        and s.IUPC='DT'					-- RI
        and t.termstype=c.itemText
        and t.CommissionDate between c.datefrom and c.dateto 
		and t.TransValue!=0 --IP - 04/09/08 - UAT5.1 - UAT(545) - prevent divide by zero
		
		-- the following code removes duplicate TermsType commission on cancellations that 
		-- happens when a commission value rather that percentage is specified for Termstype commission (jec 15/01/07)
				
		-- get count by acctno/Commissionvalue
		select acctno,CommissionAmount,sum(transvalue) as 'DTvalue',count(*) as 'lines' into #tempcommissionsTT_count
		from #tempcommissions
		where CommissionAmount!=0
		group by acctno,CommissionAmount
		-- set commission value to zero for duplicate lines (except line containing lowest transvalue)
		update #tempCommissions		
		set CommissionAmount=0
		from #tempCommissions t,#tempcommissionsTT_count c, StockInfo s
		where t.acctno=c.acctno
			and c.lines>1		-- more than one line with same amount
			and t.CommissionAmount=c.CommissionAmount
			and t.ItemId=s.ID				-- RI
			and s.IUPC='DT'					-- RI
			and t.transvalue!=(select min(transvalue) from #tempCommissions t2, StockInfo s2			-- RI
						where t.acctno = t2.acctno
							and t.CommissionAmount=t2.CommissionAmount
							and t2.ItemId=s2.ID				-- RI
							and s2.IUPC='DT')					-- RI
		-- set DT value on commission line = sum of DT value		(jec 16/01/07)
		-- this code is redundant as the transvalue is not included in the SalesCommission table
		-- the transvalue shown in the Commission Enquiry screen comes from Delivery table
		update #tempCommissions		
		set transvalue=DTvalue
		from #tempCommissions t,#tempcommissionsTT_count c, StockInfo s			-- RI
		where t.acctno=c.acctno
			and c.lines>1		-- more than one line with same amount
			and t.CommissionAmount=c.CommissionAmount
			and t.ItemId=s.ID				-- RI
			and s.IUPC='DT'					-- RI

        SET @return = @@ERROR          
    End 
        -- Calculate  negative Commissions - Rebates (based on service charge)
if @@Error=0
    Begin
        update #tempCommissions
        set commissiontype=c.CommissionType,CommissionAmount=case
            when c.percentage >0 then
            cast((t.transValue-t.taxamt)*c.Percentage/100 as float)
            else
			-- this is required for value to determine if the commission should be -ve i.e deducted (minus value /pos value) = -1
			cast((t.transValue/abs(t.transValue))*c.value as float)                -- #9355 -- 15/01/07 jec
            end,
            CommissionPcent= c.percentage				-- #9782
        from #tempCommissions t,SalesCommissionRates c, StockInfo s			-- RI
        where c.CommissionType='TT'
        and t.commissiontype=' '
        and t.ItemId=s.ID				-- RI
		and s.IUPC='RB'					-- RI
        and t.termstype=c.itemText
        and t.CommissionDate between c.datefrom and c.dateto 
		and t.TransValue!=0 --IP - 04/09/08 - UAT5.1 - UAT(545) - prevent divide by zero
        
        SET @return = @@ERROR          
    End 


-- Calculate Commissions - Product Category

if @@Error=0
    Begin
        update #tempCommissions
        set commissiontype=c.CommissionType,
            CommissionAmount= case				-- RI
				when AccType in ('C','S') and t.RepoItem=0 then cast((t.transValue-t.taxamt)*
							case when c.PercentageCash>0 then (c.PercentageCash+UpliftRate) else c.PercentageCash end/100 as float)                -- #9782
				when AccType  not in ('C','S') and t.RepoItem=0 then cast((t.transValue-t.taxamt)*
							case when c.Percentage>0 then (c.Percentage+UpliftRate) else c.Percentage end/100 as float)                -- #9782
				when AccType in ('C','S') and t.RepoItem=1 then cast((t.transValue-t.taxamt)*
							case when c.RepoPercentageCash>0 then (c.RepoPercentageCash+UpliftRate) else c.RepoPercentageCash end/100 as float)                -- #9782
				when AccType  not in ('C','S') and t.RepoItem=1 then cast((t.transValue-t.taxamt)*
							case when c.RepoPercentage>0 then (c.RepoPercentage+UpliftRate) else c.RepoPercentage end/100 as float)                -- #9782
				End,
			CommissionPcent= case				-- #9782
				when AccType in ('C','S') and t.RepoItem=0 then 
							case when c.PercentageCash>0 then (c.PercentageCash+UpliftRate) else c.PercentageCash end                -- #9782
				when AccType  not in ('C','S') and t.RepoItem=0 then 
							case when c.Percentage>0 then (c.Percentage+UpliftRate) else c.Percentage end             -- #9782
				when AccType in ('C','S') and t.RepoItem=1 then 
							case when c.RepoPercentageCash>0 then (c.RepoPercentageCash+UpliftRate) else c.RepoPercentageCash end               -- #9782
				when AccType  not in ('C','S') and t.RepoItem=1 then 
							case when c.RepoPercentage>0 then (c.RepoPercentage+UpliftRate) else c.RepoPercentage end			-- #9782				
				End
        from #tempCommissions t,SalesCommissionRates c,stockitem i
        where c.CommissionType='PC'
        and t.commissiontype=' '
        and t.ItemId=i.ItemId				-- RI
        and t.stocklocn=i.stocklocn
        and cast(i.category as varchar)=c.itemText
        and t.CommissionDate between c.datefrom and c.dateto 
        
        SET @return = @@ERROR          
    End 

        -- Calculate Spiffs - Single - Normal - Specific Branch
        -- A Spiff for a specfic branch will take precedence over "All" branches
if @@Error=0
    Begin
        update #tempCommissions
        set spifftype=c.CommissionType,SpiffAmount=case			-- RI
			when c.percentage >0 and t.RepoItem=0 then cast((t.transValue-t.taxamt)*c.Percentage/100 as decimal(15,2))					
			when c.RepoPercentage >0 and t.RepoItem=1 then cast((t.transValue-t.taxamt)*c.RepoPercentage/100 as decimal(15,2))
			when c.value>0 and t.RepoItem=0 then cast((t.transValue/abs(t.transValue))*c.value as decimal(15,2))			--?? should taxamt be deducted??
            when c.RepoValue>0 and t.RepoItem=1 then cast((t.transValue/abs(t.transValue))*c.RepoValue as decimal(15,2))		--?? should taxamt be deducted??
            end,
            SpiffCommissionPcent= case				-- #9782
				when c.percentage >0 and t.RepoItem=0 then c.Percentage					
				when c.RepoPercentage >0 and t.RepoItem=1 then c.RepoPercentage				
				End
        from #tempCommissions t,SalesCommissionRates c
        where c.CommissionType='SP'
        and t.Spifftype=' '
        and t.ItemId=c.ItemId				-- RI
        and t.CommissionDate between c.datefrom and c.dateto
        and t.BranchNo=c.ComBranchNo		-- Spiff for specific branch   - CR1035
		and t.TransValue!=0 --IP - 04/09/08 - UAT5.1 - UAT(545) - prevent divide by zero
        
        SET @return = @@ERROR          
    End 

        -- Calculate Spiffs - Single - Normal - "All" branches
if @@Error=0
    Begin
        update #tempCommissions
        set spifftype=c.CommissionType,SpiffAmount=case			-- RI
			when c.percentage >0 and t.RepoItem=0 then cast((t.transValue-t.taxamt)*c.Percentage/100 as decimal(15,2))
			when c.RepoPercentage >0 and t.RepoItem=1 then cast((t.transValue-t.taxamt)*c.RepoPercentage/100 as decimal(15,2))
			when c.value>0 and t.RepoItem=0 then cast((t.transValue/abs(t.transValue))*c.value as decimal(15,2))			--?? should taxamt be deducted??
            when c.RepoValue>0 and t.RepoItem=1 then cast((t.transValue/abs(t.transValue))*c.RepoValue as decimal(15,2))		--?? should taxamt be deducted??
            end,
            SpiffCommissionPcent= case				-- #9782
				when c.percentage >0 and t.RepoItem=0 then c.Percentage					
				when c.RepoPercentage >0 and t.RepoItem=1 then c.RepoPercentage				
				End
        from #tempCommissions t,SalesCommissionRates c
        where c.CommissionType='SP'
        and t.Spifftype=' '
        and t.ItemId=c.ItemId				-- RI
        and t.CommissionDate between c.datefrom and c.dateto 
        and c.ComBranchNo='All'		-- Spiff for All branches   - CR1035
		and t.TransValue!=0 --IP - 04/09/08 - UAT5.1 - UAT(545) - prevent divide by zero
        
        SET @return = @@ERROR          
    End
    
        -- Calculate Spiffs - Single - TermsType - Specific Branch  - CR1035       
if @@Error=0
	Begin   
		update #tempCommissions
        set spifftype=c.CommissionType,SpiffAmount=case
            when c.percentage >0 then 
            cast((t.transValue-t.taxamt)*c.Percentage/100 as decimal(9,2))
            else
			cast((t.transValue/abs(t.transValue))*c.value as decimal(9,2)) -- 15/01/07 jec
            end,
            CommissionPcent= c.percentage				-- #9782
        from #tempCommissions t,SalesCommissionRates c, StockInfo s			-- RI
        where c.CommissionType='TS'
        and t.Spifftype=' '
        and t.ItemId=s.ID					-- RI
        and s.IUPC='DT'						-- RI 
        and t.termstype=c.itemText
        and t.CommissionDate between c.datefrom and c.dateto
        and t.BranchNo=c.ComBranchNo		-- Spiff for specific branch   - CR1035
		and t.TransValue!=0 --IP - 04/09/08 - UAT5.1 - UAT(545) - prevent divide by zero   
    
		SET @return = @@ERROR          
    End
  
          -- Calculate Spiffs - Single - TermsType - "All" Branches  - CR1035      
if @@Error=0
	Begin   
		update #tempCommissions
        set spifftype=c.CommissionType,SpiffAmount=case
            when c.percentage >0 then
            cast((t.transValue-t.taxamt)*c.Percentage/100 as decimal(9,2))
            else
			cast((t.transValue/abs(t.transValue))*c.value as decimal(9,2)) -- 15/01/07 jec
            end,
            CommissionPcent= c.percentage				-- #9782
        from #tempCommissions t,SalesCommissionRates c, StockInfo s			-- RI
        where c.CommissionType='TS'
        and t.Spifftype=' '
        and t.ItemId=s.ID					-- RI
        and s.IUPC='DT'						-- RI  
        and t.termstype=c.itemText
        and t.CommissionDate between c.datefrom and c.dateto
        and c.ComBranchNo='All'		-- Spiff for All branches   - CR1035
		and t.TransValue!=0 --IP - 04/09/08 - UAT5.1 - UAT(545) - prevent divide by zero   
    
		SET @return = @@ERROR          
    End   
        -- Calculate Spiffs - Multi	(Linked Spiffs) - Specific Branches
        -- A Linked Spiff for a specfic branch will take precedence over "All" branches
if @@Error=0
	Begin

-- select all accounts where order contains first multi-spiff product into temp table
	select acctno,agrmtno,percentage,value,item1,item2,item3,item4,item5,ItemId1,ItemId2,ItemId3,ItemId4,ItemId5			-- RI
		into #tempmultispiff
	from #tempCommissions t,salescommissionmultispiffrates m
		where t.ItemId = m.ItemId1					-- RI t.itemno = m.item1
			and commissiondate between m.datefrom and m.dateto
			and t.BranchNo=m.ComBranchNo		-- Spiff for specific branch   - CR1035
			 
-- delete rows from temp table where order does not contain 2nd multi-spiff product

	delete #tempmultispiff 
	from  #tempmultispiff m1
		where not exists (select t.* from #tempCommissions t, #tempmultispiff m2
				where t.ItemId = m2.ItemId2				-- RI t.itemno = m2.item2
					and t.acctno=m2.acctno
					and m1.acctno=m2.acctno)
					and m1.ItemId2 != 0			-- RI

-- delete rows from temp table where order does not contain 3rd multi-spiff product

	delete #tempmultispiff 
	from  #tempmultispiff m1
		where not exists (select t.* from #tempCommissions t, #tempmultispiff m2
				where t.ItemId = m2.ItemId3				-- RI t.itemno = m2.item3
					and t.acctno=m2.acctno
					and m1.acctno=m2.acctno)
					and m1.ItemId3 != 0			-- RI

-- delete rows from temp table where order does not contain 4th multi-spiff product

	delete #tempmultispiff 
	from  #tempmultispiff m1
		where not exists (select t.* from #tempCommissions t, #tempmultispiff m2
				where t.ItemId = m2.ItemId4				-- RI t.itemno = m2.item4
					and t.acctno=m2.acctno
					and m1.acctno=m2.acctno)
					and m1.ItemId4 != 0			-- RI

-- delete rows from temp table where order does not contain 5th multi-spiff product

	delete #tempmultispiff 
	from  #tempmultispiff m1
		where not exists (select t.* from #tempCommissions t, #tempmultispiff m2
				where t.ItemId = m2.ItemId5				-- RI t.itemno = m2.item5
					and t.acctno=m2.acctno
					and m1.acctno=m2.acctno)
					and m1.ItemId5 != 0			-- RI

	SET @return = @@ERROR  
	End

-- Now update tempcommissions table 
if @@Error=0
    Begin
        update #tempCommissions
        set spifftype='LS',SpiffAmount=case
            when m.percentage >0 then
            cast((t.transValue-t.taxamt)*m.Percentage/100 as decimal(15,2))
            else
			-- this is required for value to determine if the commission should be -ve i.e deducted (minus value /pos value) = -1
			cast((t.transValue/abs(t.transValue))*m.value as decimal(15,2)) -- 15/01/07 jec
            end,
            SpiffCommissionPcent= m.percentage			
        from #tempCommissions t,#tempmultispiff m
-- single spiff may be superceded by multi-product spiff
        where (t.Spifftype=' ' or t.Spifftype='SP')
			and t.acctno=m.acctno
			and t.agrmtno=m.agrmtno
			and (t.ItemId=m.ItemId1 or t.ItemId=m.ItemId2 or t.ItemId=m.ItemId3 or t.ItemId=m.ItemId4 or t.ItemId=m.ItemId5)		-- RI
			and t.TransValue!=0 --IP - 04/09/08 - UAT5.1 - UAT(545) - prevent divide by zero
                
        SET @return = @@ERROR          
    End 

       -- Calculate Spiffs - Multi	(Linked Spiffs) - "All" Branches	- CR1035
       -- A Linked Spiff for "All" branches may overide a Normal Spiff for a specific branch
if @@Error=0
	Begin
	
	truncate table #tempmultispiff
-- select all accounts where order contains first multi-spiff product into temp table
	Insert into #tempmultispiff
	select acctno,agrmtno,percentage,value,item1,item2,item3,item4,item5,ItemId1,ItemId2,ItemId3,ItemId4,ItemId5			-- RI		
	from #tempCommissions t,salescommissionmultispiffrates m
		where t.ItemId = m.ItemId1					-- RI t.itemno = m.item1
			and commissiondate between m.datefrom and m.dateto
			and m.ComBranchNo='All'		-- Spiff for "All" branches   - CR1035
			 
-- delete rows from temp table where order does not contain 2nd multi-spiff product

	delete #tempmultispiff 
	from  #tempmultispiff m1
		where not exists (select t.* from #tempCommissions t, #tempmultispiff m2
				where t.ItemId = m2.ItemId2					-- RI t.itemno = m2.item2
					and t.acctno=m2.acctno
					and m1.acctno=m2.acctno)
					and m1.ItemId2 != 0				-- RI

-- delete rows from temp table where order does not contain 3rd multi-spiff product

	delete #tempmultispiff 
	from  #tempmultispiff m1
		where not exists (select t.* from #tempCommissions t, #tempmultispiff m2
				where t.ItemId = m2.ItemId3					-- RI t.itemno = m2.item3
					and t.acctno=m2.acctno
					and m1.acctno=m2.acctno)
					and m1.ItemId3 != 0				-- RI

-- delete rows from temp table where order does not contain 4th multi-spiff product

	delete #tempmultispiff 
	from  #tempmultispiff m1
		where not exists (select t.* from #tempCommissions t, #tempmultispiff m2
				where t.ItemId = m2.ItemId4					-- RI t.itemno = m2.item4
					and t.acctno=m2.acctno
					and m1.acctno=m2.acctno)
					and m1.ItemId4 != 0				-- RI

-- delete rows from temp table where order does not contain 5th multi-spiff product

	delete #tempmultispiff 
	from  #tempmultispiff m1
		where not exists (select t.* from #tempCommissions t, #tempmultispiff m2
				where t.ItemId = m2.ItemId5					-- RI t.itemno = m2.item5
					and t.acctno=m2.acctno
					and m1.acctno=m2.acctno)
					and m1.ItemId5 != 0				-- RI

	SET @return = @@ERROR  
	End

-- Now update tempcommissions table 
if @@Error=0
    Begin
        update #tempCommissions
        set spifftype='LS',SpiffAmount=case
            when m.percentage >0 then
            cast((t.transValue-t.taxamt)*m.Percentage/100 as decimal(9,2))
            else
			-- this is required for value to determine if the commission should be -ve i.e deducted (minus value /pos value) = -1
			cast((t.transValue/abs(t.transValue))*m.value as decimal(9,2)) -- 15/01/07 jec
            end,
            SpiffCommissionPcent= m.percentage				
        from #tempCommissions t,#tempmultispiff m
-- single spiff may be superceded by multi-product spiff
        where (t.Spifftype=' ' or t.Spifftype='SP')
			and t.acctno=m.acctno
			and t.agrmtno=m.agrmtno
			and (t.ItemId=m.ItemId1 or t.ItemId=m.ItemId2 or t.ItemId=m.ItemId3 or t.ItemId=m.ItemId4 or t.ItemId=m.ItemId5)			-- RI
			and t.TransValue!=0 --IP - 04/09/08 - UAT5.1 - UAT(545) - prevent divide by zero
                
        SET @return = @@ERROR          
    End
     
-- Now select Extra Spiffs, these overwrite any calculated spiffs
if @@Error=0
    Begin
        update #tempCommissions
		set spifftype='ES',SpiffAmount=case		-- uat507 jec 04/08/08 
			when DelorColl='D' then e.SpiffAmount
			else e.SpiffAmount * -1.00		-- repo/collection make negative
			end,
            CommissionPcent= 0				-- #9782				
		from #tempCommissions t,SalesCommissionExtraSpiffs e
		where t.acctno=e.acctno
			and t.agrmtno=e.agrmtno
			and t.ItemId=e.ItemId			-- RI
			and t.stocklocn=e.stocklocn

    End 

Begin Transaction

        -- Insert commissions into SalesCommission table
        -- Commissions
		-- Added NetCommissionValue (actual value on which commission is calculated i.e excl. discounts/tax etc) 06/07/07
if @@Error=0
    Begin
        insert into SalesCommission (Employee,RunDate,Acctno,Agrmtno,CommissionType,Itemno,
					Stocklocn,CommissionAmount,BuffNo,ContractNo,UpliftRate,NetCommissionValue,ItemId,RepossessedItem,
					CommissionPcent)		-- #9782
        
        Select t.Employee,@CommRunDate,t.Acctno,t.Agrmtno,t.CommissionType,t.Itemno,
					t.StockLocn,sum(t.CommissionAmount),t.BuffNo,t.ContractNo,max(UpliftRate),sum(transValue)-sum(taxamt),t.ItemId,t.RepoItem,	-- RI
					t.CommissionPcent -- #9782
        from #tempCommissions t 
        where t.CommissionType!=' '
			and CommissionAmount!=0		-- exclude zero commissions ?????
		Group by t.Employee,t.Acctno,t.Agrmtno,t.CommissionType,t.Itemno,
					t.StockLocn,t.BuffNo,t.ContractNo,ItemId,RepoItem,CommissionPcent -- #9782 
       
        SET @return = @@ERROR          
    End 
        -- Spiffs
if @@Error=0
    Begin
        insert into SalesCommission (Employee,RunDate,Acctno,Agrmtno,CommissionType,Itemno,
					Stocklocn,CommissionAmount,BuffNo,ContractNo,UpliftRate,NetCommissionValue,ItemId,RepossessedItem,
					CommissionPcent)		-- #9782 
        
        Select t.Employee,@CommRunDate,t.Acctno,t.Agrmtno,t.SpiffType,t.Itemno,
					t.StockLocn,sum(t.SpiffAmount),t.BuffNo,t.ContractNo,0,sum(transValue)-sum(taxamt),t.ItemId,t.RepoItem,	-- RI
                    t.SpiffCommissionPcent 
        from #tempCommissions t
        where t.SpiffType!=' '
			and SpiffAmount!=0			-- exclude zero spiffs
		group by t.Employee,t.Acctno,t.Agrmtno,t.SpiffType,t.Itemno,
					t.StockLocn,t.BuffNo,t.ContractNo,ItemId,RepoItem,SpiffCommissionPcent -- #9782
        
                
    End     
SET @return = @@ERROR  

-- Update Collection Reason
update CollectionReason
	set DateCommissionCalculated=RunDate
	from collectionReason c,SalesCommission s
			where c.AcctNo=s.AcctNo
				and c.ItemID=s.ItemID
				and c.StockLocn=s.StockLocn
				and DateCommissionCalculated is null

	Commit Transaction

-- Remove Commission details older than xx months
if @@Error=0
    exec DN_SalesCommission_Delete @return
    
 SET @return = @@ERROR

      
GO
GRANT EXECUTE ON DN_EOD_Commissions_Calculation TO PUBLIC
GO
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO
-- end end end end end end end end end end end end end end end end end end end end end end end end end end end 


