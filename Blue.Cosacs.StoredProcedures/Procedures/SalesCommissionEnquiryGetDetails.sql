SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS OFF
GO

IF EXISTS (SELECT * FROM SysObjects
           WHERE Name = 'SalesCommissionEnquiryGetDetails' AND Type = 'P')
BEGIN
    DROP PROCEDURE SalesCommissionEnquiryGetDetails
END
GO


CREATE PROCEDURE SalesCommissionEnquiryGetDetails

----------------------------------------------------------------------------------
----
---- Project      : CoSACS .NET
---- File Name    : SalesCommissionEnquiryGetDetails.sql
---- File Type    : MSSQL Server Stored Procedure Script
---- Title        : Sales Commissions/Spiffs get details for enquiry
---- Author       : Ilyas Parker
---- Date         : 15 October 2014
----
----
---- Change Control
---- --------------
---- Date      By  Description
---- ----      --  -----------

----------------------------------------------------------------------------------

    -- Parameters
	@branchNo		  int = null,		
    @empeeNo          int = null,
    @dateFrom         datetime,
    @dateTo           datetime

AS  --DECLARE
    -- Local variables

Declare
		@CatOrDept BIT,				-- RI	
		@datestart datetime,
		@datefinish datetime,
		@agreementTaxType char(1)

select @CatOrDept = value from CountryMaintenance where codename='RIDispCatAsDept'		-- RI 

set @agreementTaxType = (SELECT value FROM CountryMaintenance WHERE CodeName = 'agrmttaxtype')

    Select Distinct 
			u.fullname as [Employee], 
			s.AcctNo as [Account Number],	--- For excel CR1035
			s.AgrmtNo as [Invoice Number],
			i.IUPC as [Item Number],			-- RI
			-- delamount net of tax jec 03/07/08  UAT443
			CAST(s.NetCommissionValue as DECIMAL(12,2)) as [Product Value (excluding VAT)],		-- #9783
			CommissionAmount as [Total Commission Value],					-- #9783
			CommissionPcent as [Total Commission %],		-- #9783 
			d.datedel as [Item Delivery Date], 
			i.ItemDescr1 as [Item Description],
			i.ItemDescr2 as [Item Description 2],
			case 
					when s.CommissionType ='P' 
						then 'Product'		
					when s.CommissionType ='PL' 
						then 'Product Class'							-- RI
					when s.CommissionType ='PS' 
						then 'Product SubClass'							-- RI
					when s.CommissionType ='PC' and @CatOrDept = 0 
						then 'Product Category'		-- RI
					when s.CommissionType ='PC' and @CatOrDept = 1 
						then 'Product Department'	-- RI  
					when s.CommissionType ='SP' 
						then 'SPIFF'
					when s.CommissionType ='LS' 
						then 'Linked SPIFF'
					when s.CommissionType ='TT' 
						then 'Terms Type'
					when s.CommissionType ='TS' 
						then 'Terms Type SPIFF'
					when s.CommissionType ='ES' 
						then 'Extra SPIFF'
					else s.CommissionType
			End as [Commission Type],
			d.delorcoll as [Delivery Type],
			isnull(co.codedescript,'Not Categorised') as [Product Category], 	-- CR1035
			d.contractno as [Contract Number],
			d.buffno as [Buff No]
	from 
		Delivery d 
	left outer join 
		SalesCommission s on s.AcctNo=d.AcctNo 
		and s.ItemId=d.ItemId 
		and	S.AgrmtNo=d.AgrmtNo 
		and S.Buffno=d.BuffNo 
		and s.StockLocn=d.StockLocn
		and s.contractno=d.contractno
	left outer join							-- required for taxamt jec 03/07/08
		lineItem l on l.AcctNo=d.AcctNo 
		and l.ItemId=d.ItemId		-- RI  
		and	l.AgrmtNo=d.AgrmtNo 
		and l.StockLocn=d.StockLocn,
		StockItem i 
	left outer join 
		code co on CAST(i.category as varchar(4)) = co.code 
		and co.category like 'PC%', -- RI 
		Admin.[User] u 
    where 
		cast(d.datedel as date) >= cast(@dateFrom as date)
		and cast(d.datedel as date) <= cast(@dateTo as date)
        and s.ItemId=i.ItemId				-- RI
        and s.StockLocn=i.stocklocn
		and s.Employee=u.id
		and ((d.transvalue>0 and s.CommissionAmount>0) or (d.transvalue<=0 and s.CommissionAmount<0))	-- jec 28/01/10
		and ((@branchNo=s.stocklocn) or @branchNo is null)	-- #9783
		and ((Employee=@empeeNo) or @empeeNo is null) --change 9/11/2006 sum for all employees
	order by 
			d.datedel asc, s.AcctNo, i.IUPC
    
GO
----GRANT EXECUTE ON SalesCommissionEnquiryGetDetails TO PUBLIC
----GO
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO
-- end end end end end end end end end end end end end end end end end end end end end end end 
