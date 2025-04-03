SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS OFF
GO

IF EXISTS (SELECT * FROM SysObjects
           WHERE Name = 'SalesCommissionBranchEnquiryGetDetails' AND Type = 'P')
BEGIN
    DROP PROCEDURE SalesCommissionBranchEnquiryGetDetails
END
GO


CREATE PROCEDURE SalesCommissionBranchEnquiryGetDetails

------------------------------------------------------------------------------

 ---- Project      : CoSACS .NET
 ---- File Name    : SalesCommissionBranchEnquiryGetDetails.sql
 ---- File Type    : MSSQL Server Stored Procedure Script
 ---- Title        : Sales Commission Branch summary
 ---- Author       : Ilyas Parker
 ---- Date         : 17 October 2014


 ---- Change Control
 --------------
 ---- Date      By  Description
      ----      --  -----------

------------------------------------------------------------------------------
    -- Parameters
	@branchNo		  int = null,		
    @dateFrom         datetime,
    @dateTo           datetime


AS  --DECLARE
--    -- Local variables

		;WITH ProductAndTTSalesCommission AS
		(
			SELECT
				Employee, 
				SUM(TermsTypeCommission) AS TermsTypeCommission, 
				SUM(ProductCommission) AS ProductCommission
				--SUM(NetCommissionValue) as NetCommissionValue
			FROM
			(
				select 
					s.Employee, 
					s.CommissionType,
					CASE 
						WHEN s.CommissionType = 'TT' THEN sum(s.CommissionAmount) 
						ELSE NULL
					END as TermsTypeCommission, 
					CASE 
						WHEN s.CommissionType != 'TT' THEN sum(s.CommissionAmount) 
						ELSE NULL
					END as ProductCommission,
					sum(s.NetCommissionValue) as NetCommissionValue
				from 
					SalesCommission s
				inner join 
					delivery d on s.AcctNo=d.AcctNo 
					and s.ItemId=d.ItemId 
					and	s.AgrmtNo=d.AgrmtNo 
					and s.Buffno=d.BuffNo 
					and s.StockLocn=d.StockLocn
				inner join 
					stockinfo si on s.ItemId = si.id
				where 
					si.category not in (12, 82)								--Exclude commission on warranties
					and cast(d.datedel as date) >= cast(@dateFrom as date)
					and cast(d.datedel as date) <= cast(@dateTo as date)
					and (s.stocklocn = @branchno or @branchno is null)
					and ((d.transvalue>0 and s.CommissionAmount>0) or (d.transvalue<=0 and s.CommissionAmount<0))	-- jec 28/01/10
				group by 
					s.Employee,
					s.CommissionType	
			) AS Commissions
			GROUP BY 
				Employee
		),

		WarrantySalesCommission AS
		(
			select s.Employee, 
				   sum(s.CommissionAmount) as WarrantyCommission--,
				   --sum(s.NetCommissionValue) as NetCommissionValue
			from 
				SalesCommission s
			inner join 
				delivery d on s.AcctNo=d.AcctNo 
				and s.ItemId=d.ItemId 
				and	s.AgrmtNo=d.AgrmtNo 
				and s.Buffno=d.BuffNo 
				and s.StockLocn=d.StockLocn
			inner join 
				stockinfo si on s.ItemId = si.id
			where si.category in (12, 82)							
				and cast(d.datedel as date) >= cast(@dateFrom as date)
				and cast(d.datedel as date) <= cast(@dateTo as date)	
				and (s.stocklocn = @branchno or @branchno is null)	
				and ((d.transvalue>0 and s.CommissionAmount>0) or (d.transvalue<=0 and s.CommissionAmount<0))	-- jec 28/01/10
			group by 
				s.Employee		
		),
		NetCommissionableValue AS			--Only count NetCommissionValue on an item that my have more then one commission type on it e.g. SP and PC
		(
			select sum(CommissionableValue) as CommissionableValue, Employee
			from 
			(
					select distinct s.Acctno, s.ItemId, s.Buffno, s.StockLocn, s.Employee, s.NetCommissionValue as CommissionableValue
					from 
						SalesCommission s
					inner join 
						delivery d on s.AcctNo=d.AcctNo 
						and s.ItemId=d.ItemId 
						and	s.AgrmtNo=d.AgrmtNo 
						and s.Buffno=d.BuffNo 
						and s.StockLocn=d.StockLocn
						and s.contractno = d.contractno
					where
						cast(d.datedel as date) >= cast(@dateFrom as date)
						and cast(d.datedel as date) <= cast(@dateTo as date)	
						and (s.stocklocn = @branchno or @branchno is null)	
						and ((d.transvalue>0 and s.CommissionAmount>0) or (d.transvalue<=0 and s.CommissionAmount<0))
					group by
						s.Acctno, s.ItemId, s.Buffno, s.StockLocn, s.NetCommissionValue, s.Employee
				) as commissionable
			group by Employee

		)

		select e.FullName as [Employee], 
			   cast(e.id as varchar(10)) + ' : ' + e.FullName + ' ' + cast(e.id as varchar(10)) as [EmployeeIdAndName],
			   --cast(isnull(p.NetCommissionValue,0) + isnull(w.NetCommissionValue,0) as decimal (12,2)) as [Commission Valuable],
			   cast(isnull(n.CommissionableValue,0) as decimal (12,2)) as [Commission Valuable],
			   isnull(cast(p.ProductCommission as decimal (12,3)),0) as [Product Commission], 
			   isnull(cast(p.TermsTypeCommission as decimal (12,3)),0) as [Terms Type Commission], 
			   isnull(cast(w.WarrantyCommission as decimal(12,3)),0) as [Warranty Commission],
			   cast(isnull(p.ProductCommission,0) + isnull(p.TermsTypeCommission,0) + isnull(w.WarrantyCommission,0) as decimal(12,2)) as [Total Commission]

		from 
			(
				select distinct 
					s.Employee,
					u.FullName,
					u.Id
				from 
					SalesCommission s
				inner join 
					delivery d on s.AcctNo = d.acctno
					and s.ItemId=d.ItemId 
					and	S.AgrmtNo=d.AgrmtNo 
					and S.Buffno=d.BuffNo 
					and s.StockLocn=d.StockLocn,
					Admin.[User] u
				where 
					cast(d.datedel as date) >= cast(@dateFrom as date)
					and cast(d.datedel as date) <= cast(@dateTo as date)
					and (s.stocklocn = @branchno or @branchno is null)
					and u.id = s.Employee
					and ((d.transvalue>0 and s.CommissionAmount>0) or (d.transvalue<=0 and s.CommissionAmount<0))	-- jec 28/01/10
			) e
			LEFT JOIN ProductAndTTSalesCommission p on p.Employee = e.Employee
			LEFT JOIN WarrantySalesCommission w on w.Employee = e.Employee
			LEFT JOIN NetCommissionableValue n on n.Employee = e.Employee
		order by e.FullName asc

GO
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO
-- end end end end end end end end end end end end end end end end end end end end end end end 
