GO
/****** Object:  StoredProcedure [dbo].[DN_SRGetServiceRequestSummaryForAccount]    Script Date: 11/15/2006 14:53:54 ******/
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID('[dbo].[DN_SRGetServiceRequestSummaryForAccount]') AND OBJECTPROPERTY(id,'IsProcedure') = 1)
DROP PROCEDURE [dbo].[DN_SRGetServiceRequestSummaryForAccount]


SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Peter Chong
-- Create date: 15-11-2006
-- Description:	Gets a service request summary for an account
-- Modified by: Jez Hemans
-- Modified date: 08/10/2008
-- Modified Description: CR 949/958 Labour Cost, Additional Labour, Total Cost, Amount Paid, Date Paid, Outstanding Balance and Charge-To Account added to the summary details.

-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 22/07/11  IP  RI - #4374 - Service - Description column is blank in service summary tab in account details screen. Join using ItemID
-- 26/11/12  IP  #10912 - Service now moved to web therefore only show limited details in Account Details
-- =============================================
CREATE PROCEDURE DN_SRGetServiceRequestSummaryForAccount
(
	@AcctNo char(12),
	@return int out
)
AS
BEGIN
	SET NOCOUNT ON;

    
	--SELECT 
	--SR.ServiceRequestNo
	--, dbo.fn_SRGetServiceRequestNo(SR.ServiceRequestNo) [ServiceRequestNoStr]
	--, DateLogged
	--, ProductCode
	--, S.itemdescr1 + ' ' + S.itemdescr2 [Description] 
	--, SE.DateClosed
	--, ISNULL(SE.ReplacementStatus, '') [ReplacementStatus]
	--, ISNULL(C.Codedescript, '') [Deliverer]
	--, SE.LabourCost [Labour Cost]
	--, SE.AdditionalCost [Additional Labour] -- CR 1024 (NM 29/04/2009)
	--, SE.TransportCost [Transport Cost]
	--, SE.TotalCost [Total Cost]
	--, -1*(SUM(ISNULL(CASE WHEN transtypecode = 'PAY' THEN F.transvalue END,0))) [Amount Paid]
	--, ISNULL(A.datelastpaid,'') [Date Paid]
	--, A.outstbal [Outstanding Balance]
	--, ISNULL(SC.AcctNo,'') [Charge-To Account]
	--FROM SR_ServiceRequest SR LEFT OUTER JOIN
	--	--StockItem S ON SR.ProductCode = S.ItemNo AND SR.StockLocn = S.StockLocn LEFT OUTER JOIN
	--	StockItem S ON SR.ItemID = S.ID AND SR.StockLocn = S.StockLocn LEFT OUTER JOIN				--IP - 22/07/11 - RI - #4374
	--	SR_Resolution SE ON SR.ServiceRequestNo = SE.ServiceRequestNo LEFT OUTER JOIN
	--	Code C ON  SE.Deliverer = C.Code AND C.Category = 'SRDELIVERER' LEFT OUTER JOIN
	--	SR_ChargeAcct SC ON SR.ServiceRequestNo = SC.ServiceRequestNo LEFT OUTER JOIN
	--	acct A ON SC.AcctNo = A.acctno LEFT OUTER JOIN
	--	fintrans F ON SC.acctno = F.acctno
	
	--WHERE SR.AcctNo = @AcctNo 
	----AND (ChargeType = 'C' or ChargeType is NULL) -- CR 1024 (NM 28/04/2009)
	---- RM 05/08/09 LW71513 service request should display regardless of charge to
	--GROUP BY SR.ServiceRequestNo,DateLogged,ProductCode,itemdescr1,itemdescr2,SE.DateClosed,SE.ReplacementStatus,Codedescript
	--,SE.LabourCost, SE.AdditionalCost, SE.TransportCost, SE.TotalCost,datelastpaid,outstbal,SC.AcctNo
	
	
	SELECT 
	convert(varchar(4), SR.Branch) + convert(varchar(16), SR.ServiceRequestNo) AS 'Service Request No', 
	SR.DateLogged,
	S.iupc AS ProductCode,
	S.itemdescr1 + ' ' + S.itemdescr2 [Description],
	SR.DateClosed
	FROM dbo.SR_Summary SR INNER JOIN
		StockItem S ON SR.ItemId = S.ID AND SR.StockLocn = S.StockLocn 		
	
	WHERE SR.AcctNo = @AcctNo


	SET @return = @@error
	
END
GO
