

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID('[dbo].[DN_SalesCommissionReportHeaderSP]') AND OBJECTPROPERTY(id,'IsProcedure') = 1)
DROP PROCEDURE [dbo].[DN_SalesCommissionReportHeaderSP]

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Peter Chong
-- Create date: 2006-Dec-19
-- Description:	Get commission report header information
-- Modification:
--				2007-Jan-22: Negative valued DT items are cancellations [PC]
--				2007-Feb-14: Employee parameter for user rights		(jec)
--				4th July 2007: Restrict details to selected branch	(jec)
--              3rd July 2008: Product total net of taxes UAT443   (jec)
-- 29/07/09 jec UAT680 add isnull to lineitem values
-- =============================================
CREATE PROCEDURE DN_SalesCommissionReportHeaderSP
(	-- Add the parameters for the stored procedure here
	@piBranchNo int,
	@Empeeno int,				-- 0 = view all commissions
	@FromDate datetime,
	@ToDate datetime,
	@ShowCommission bit,
	@ShowSPIFF bit,
	@return int output
)
AS
BEGIN
	SET NOCOUNT ON;
	

	SELECT u.id
		, u.FullName AS EmployeeName
		-- Commission restrict by 'D' to prevent duplicates
		, Sum(/*case when D.delorcoll = 'D'	then */ isnull(S.CommissionAmount, 0)	/*else 0 end*/) CommissionTotal
		--RB is the code for rebates
		, Sum(case when S.Itemno = 'RB' AND D.delorcoll = 'D' then CommissionAmount	else 0 end) RebateTotal
		--Look for negative commission delorcoll = 'R' for repossession
		, Sum(case when S.ItemNo <> 'RB' AND S.CommissionAmount < 0 AND  D.delorcoll = 'R' then CommissionAmount else 0 end ) RepossessionTotal
		
		--Look for negative commission where delorcol = 'C' for cancellation
		, Sum(case when D.Delorcoll = 'C' OR (D.ItemNo = 'DT' AND S.CommissionAmount < 0) then S.CommissionAmount else 0 end) [CancellationTotal] 
		, Sum(case when S.CommissionType IN ('SP', 'LS', 'ES') then S.CommissionAmount else 0 end) SPIFFTotal
		, Sum(D.TransValue-isnull(l.taxamt,0)) [DeliveryTotal]			-- jec 03/07/09 UAT443 net of taxes 
		, isnull(ROUND(Sum(isnull(S.CommissionAmount, 0)	) / nullif(Sum(D.TransValue-isnull(l.taxamt,0)),0),3),0)  [commnpercent] -- jec 03/07/09 UAT443 net of taxes 
	FROM 
	Delivery D INNER JOIN
	Agreement A       ON D.AcctNo = A.AcctNo AND D.AgrmtNo = A.AgrmtNo 
	INNER JOIN Admin.[User] u    ON A.Empeenosale = u.id 
	INNER JOIN
	SalesCommission S ON 
		S.Employee = A.Empeenosale   
		AND S.Acctno =  A.Acctno   
		AND S.AgrmtNo = A.AgrmtNo 
		AND S.ItemNo = D.Itemno
		AND S.StockLocn = D.StockLocn
		AND S.BUffNo = D.BUffNo
	--inner join lineItem l			-- required for taxamt jec 03/07/08
	left outer join lineItem l			-- UAT680 jec 29/07/09 -- required for taxamt jec 03/07/08
					on l.AcctNo=d.AcctNo and 
						l.itemno=d.ItemNo and 
						l.AgrmtNo=d.AgrmtNo and 					
						l.StockLocn=d.StockLocn
		
	WHERE D.DateTrans BETWEEN  @FromDate AND @ToDate
			and (S.Employee = @Empeeno or @Empeeno = 0)			-- jec 14/02/07
			and @piBranchNo=u.branchno				-- jec 04/07/07 restrict details to selected branch
	
	GROUP BY u.id
		, u.FullName
	HAVING 
		(@ShowCommission = 1 AND Sum(isnull(S.CommissionAmount, 0))	 <> 0 )
	OR  (@ShowSPIFF = 1 AND  Sum(case when S.CommissionType IN ('SP', 'LS', 'ES') then S.CommissionAmount else 0 end)  > 0 )
	
	SET @return = @@error
	
END

go

-- End End End End End End End End End End End End End End End End End End End End End End End End End End End End

/* Testing
DN_SalesCommissionReportHeaderSP '2007-01-12', '2007-01-13', 1, 1, 0
DN_SalesCommissionReportDetailSP  2004, '2007-01-12', '2007-01-13', 1 ,1 , 0
*/

