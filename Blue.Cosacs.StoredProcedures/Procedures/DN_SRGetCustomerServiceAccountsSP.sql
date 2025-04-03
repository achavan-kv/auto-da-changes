IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID('[dbo].[DN_SRGetCustomerServiceAccountsSP]') AND OBJECTPROPERTY(id,'IsProcedure') = 1)
DROP PROCEDURE [dbo].[DN_SRGetCustomerServiceAccountsSP]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================================================================================
-- Author:		Peter Chong
-- Create date: 20-Oct-2006
-- Description:	Gets a list of accounts for a service customer
-- Changes: IP - 05/06/09 - Credit Collection Walkthrough Changes - returned the strategy the account may be in
-- =============================================================================================================
CREATE PROCEDURE DN_SRGetCustomerServiceAccountsSP
	@CustId varchar(20),
	@return int output
AS
BEGIN
	SET NOCOUNT ON;


	SELECT  isnull(dbo.fn_SRGetServiceRequestNo(max(S.ServiceRequestNo)), '') [ServiceRequestNo]
	, A.acctno
	, dateacctopen
	, accttype
	, agrmttotal
	, outstbal
	, arrears
	, hldorjnt
	, currstatus
	, isnull(cms.strategy, '') AS strategy
	--, max(CI.Date)
	FROM customer C INNER JOIN 
		custacct CA					ON  CA.custid = C.custid INNER JOIN
		acct A						ON CA.Acctno = A.Acctno LEFT OUTER JOIN 
		SR_ServiceRequest S			ON S.AcctNo = CA.Acctno LEFT OUTER JOIN
		cmstrategyacct cms			ON CA.Acctno = cms.Acctno
    AND cms.dateto is null
	
		/*(	SELECT top 1 CI2.ServiceRequestNo, CI2.CustomerId 
			FROM SR_CustomerInteraction CI2 JOIN 
				(	SELECT CustomerID, Max([Date]) [Date] 
					FROM SR_CustomerInteraction 
					GROUP BY CustomerID) CI1 
			ON CI2.CustomerId = CI1.CustomerID AND CI2.[Date] = CI1.[Date]
		) CI ON CI.CustomerID = C.CustID */
	WHERE 
		C.CustId = @CustId
	GROUP BY
		 A.acctno
		, dateacctopen
		, accttype
		, agrmttotal
		, outstbal
		, arrears
		, hldorjnt
		, currstatus
		, cms.strategy
  ORDER BY A.acctno 
  
	SET @return = @@error
END
GO


