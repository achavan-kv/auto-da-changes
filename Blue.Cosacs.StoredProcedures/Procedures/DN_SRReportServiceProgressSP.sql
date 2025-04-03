/****** Object:  StoredProcedure [dbo].[DN_SRReportServiceProgress]    Script Date: 10/24/2006 15:06:02 ******/
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID('[dbo].[DN_SRReportServiceProgressSP]') AND OBJECTPROPERTY(id,'IsProcedure') = 1)
DROP PROCEDURE [dbo].[DN_SRReportServiceProgressSP]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Peter Chong
-- Create date: 24_Oct-2006
-- Description:	Procedure for service progress report
-- Modified by: Jez Hemans
-- Modified date: 12/11/2007
-- Modified Description: Requirement for Technician details to include name as well as ID
-- Modified by: Jez Hemans
-- Modified date: 05/10/2008
-- Modified Description: CR 949/958 Service Location and Delivery Date added. Additional parameter @branch to return results for service branch selected
-- Modified by: Ilyas Parker
-- Modified Date: 13/11/09 
-- Modified Description: CR1055 - Added columns 'Comments' and 'TechnicianReport' to be displayed as 'Soft Script Comments' and 'Technician Report'. (Merged from 5.1.8.1)
-- 22/07/11 jec CR1254 RI Changes
-- =============================================
CREATE PROCEDURE DN_SRReportServiceProgressSP
	-- Add the parameters for the stored procedure here
	@MinDateLogged datetime = null,
	@MaxDateLogged datetime = null,
	@ProductCode   varchar(18) = null,			-- RI
	@ProductGroup  smallint = null,  --This refers to category for now
	@Outstanding   bit,
	@Allocated	   bit,
	@Completed	   bit	,
	@RowCount	   int = 1000,	
	@fault		   CHAR(4),  -- UAT 453	
	@branch        SMALLINT,
	@return int out
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	SET @MaxDateLogged = convert(datetime, convert(char(10), @MaxDateLogged, 101), 101)
	SET @MinDateLogged = convert(datetime, convert(char(10), @MinDateLogged, 101), 101)
    
    DECLARE @branchNo VARCHAR(3)
    IF @branch = 0
    SET @branchNo = '%'
    ELSE
    SET @branchNo = CONVERT(VARCHAR(3),@branch)
    
	SET ROWCOUNT  @Rowcount
	-- Insert statements for procedure here
	IF @fault = 'ALL'
	BEGIN
		SELECT Distinct SR.[ServiceRequestNo]
	  , convert(varchar(4), SR.ServiceBranchNo) + convert(varchar(16), SR.ServiceRequestNo) [ServiceRequestNoStr]
	  ,[DateLogged]
	  ,[LoggedBy]
      ,SR.[AcctNo]
      ,[ServiceType]
	  , SR.Status
   -- , C.FirstName + ' ' + C.LastName [Name]
   -- , C.TelHome
   -- , C.TelWork
   -- , C.TelMobile,
	  ,C.FirstName + ' ' + C.Name [Name] --IP - 12/12/08 - (70591) - Use 'Customer' and 'Custtel' tables to retrieve customer details instead of SR_Customer.
	  ,CT.DialCode + ' ' + CT.Telno [TelHome] --IP - 12/12/08 - (70591)
	  ,CW.DialCode + ' ' + CW.Telno [TelWork] --IP - 12/12/08 - (70591)
	  ,CM.DialCode + ' ' + CM.Telno [TelMobile] --IP - 12/12/08 - (70591)
	  , Isnull(A.Arrears, 0) as Arrears--RM 25/08/09 pass 0 for null so no blank arrears values (70821)
      , SR.[PurchaseDate]
	  , SR.ProductCode
      , SR.Description
      , SR.ModelNo
	  , SR.SerialNo
	  , SR.ExtWarranty
	  , SR.ContractNo
	  , R.SoftScript
	  , SR.Comments --IP - 13/11/09 - CR1055 - Merged from (5.1.8.1)
	  --, SR.DepositPaid			-- UAT 341 field to be removed
	  , SR.RepairEstimate
	  , CONVERT(VARCHAR(6),AL.TechnicianId) + ' ' + T.FirstName + ' ' + T.LastName AS TechnicianId
	  , SR.TechnicianReport --IP - 13/11/09 - CR1055 - Merged from (5.1.8.1)
	  , AL.RepairDate
	  , AL.PartsDate	
	  , SP.PartNo
	  , SP.Quantity
	  , SI.itemdescr1  [PartDescription]
	  , R.Resolution
	  , Co.CodeDescript [ResolutionDescription] --Code maintenance
	  , R.DateClosed [ResolutionDate]
	  ,	R.Replacement
	  , R.FoodLoss
	  , R.ChargeTo
	  , SP.UnitPrice [Cost]
	  , R.LabourCost
	  , R.AdditionalCost
      , R.TotalCost
      , SR.ServiceBranchNo AS 'Service Location'
      , SR.PurchaseDate AS 'Delivery Date'
      , R.TransportCost   -- CR 1024 (NM 29/04/2009)
--•	Claim Amount
  FROM [SR_ServiceRequest] SR											LEFT OUTER JOIN
	StockItem SRI		ON SR.ItemId = SRI.ItemId AND SRI.StockLocn = SR.StockLocn				-- RI
	--StockItem SRI		ON SR.ProductCode = SRI.ItemNo AND SRI.StockLocn = SR.StockLocn LEFT OUTER JOIN
	--SR_Customer C		ON SR.ServiceRequestNo = C.ServiceRequestNo		LEFT OUTER JOIN --IP - 12/12/08 - (70591) 
	LEFT OUTER JOIN Customer C			ON SR.Custid = C.Custid							LEFT OUTER JOIN --IP - 12/12/08 - (70591)
	Custtel CT			ON SR.Custid = CT.Custid AND CT.tellocn = 'H' AND CT.datediscon is null	LEFT OUTER JOIN	--IP - 12/12/08 - (70591)
	Custtel CW			ON SR.Custid = CW.Custid AND CW.tellocn = 'W' AND CW.datediscon	is null LEFT OUTER JOIN	--IP - 12/12/08 - (70591)
	Custtel CM			ON SR.Custid = CM.Custid AND CM.tellocn = 'M' AND CM.datediscon	is null	LEFT OUTER JOIN	--IP - 12/12/08 - (70591)
	SR_ChargeAcct CA	ON SR.ServiceRequestNo = CA.ServiceRequestNo	LEFT OUTER JOIN
	acct A				ON CA.AcctNo = A.AcctNo							LEFT OUTER JOIN
	SR_Resolution	R   ON SR.ServiceRequestNo = R.ServiceRequestNo		LEFT OUTER JOIN
	SR_Allocation AL	ON SR.ServiceRequestNo = AL.ServiceRequestNo	LEFT OUTER JOIN
	SR_PartListResolved SP ON SR.ServiceRequestNo = SP.ServiceRequestNo LEFT OUTER JOIN
	--StockItem SI		ON SI.Itemno = SP.PartNo AND SI.StockLocn = SR.StockLocn LEFT OUTER JOIN
	StockItem SI		ON SI.ItemId = SP.PartId AND SI.StockLocn = SR.StockLocn				-- RI
	LEFT OUTER JOIN code Co				ON Co.Category = 'SRRESOLVE' AND R.Resolution = Co.Code  LEFT OUTER JOIN
	SR_Technician T     ON T.TechnicianId = AL.TechnicianId
 WHERE
	(@MinDateLogged IS NULL OR SR.DateLogged >= @MinDateLogged) AND
	(@MaxDateLogged IS NULL OR SR.DateLogged <=  dateadd(s, -1, dateadd(d, 1, @maxdatelogged))) AND
	(@ProductCode IS NULL OR SR.ProductCode LIKE @ProductCode + '%') AND 
	(@ProductGroup IS NULL OR SRI.Category = @ProductGroup  ) AND
	(SR.Status IN (	
						  (SELECT CASE WHEN @Outstanding = 1 THEN 'N' ELSE NULL END)
						, (SELECT CASE WHEN @Outstanding = 1 THEN 'D' ELSE NULL END)
						, (SELECT CASE WHEN @Outstanding = 1 THEN 'T' ELSE NULL END) -- To be allocated
						, (SELECT CASE WHEN @Allocated = 1 THEN 'A' ELSE NULL END)
						, (SELECT CASE WHEN @Allocated = 1 THEN 'H' ELSE NULL END)  -- Technician Allocated - CR 1024 (NM 28/04/2009) 
						, (SELECT CASE WHEN @Allocated = 1 THEN 'S' ELSE NULL END)  -- Allocated to Supplier - CR 1024 (NM 28/04/2009) 
						, (SELECT CASE WHEN @Completed = 1 THEN 'R' ELSE NULL END)
						, (SELECT CASE WHEN @Completed = 1 THEN 'C' ELSE NULL END) 
						) )
	AND SR.ServiceBranchNo LIKE @branchNo

	END
	ELSE
	BEGIN
		SELECT Distinct SR.[ServiceRequestNo]
	  , convert(varchar(4), SR.ServiceBranchNo) + convert(varchar(16), SR.ServiceRequestNo) [ServiceRequestNoStr]
	  ,[DateLogged]
	  ,[LoggedBy]
      ,SR.[AcctNo]
      ,[ServiceType]
	  , SR.Status
--	  , C.FirstName + ' ' + C.LastName [Name]
--	  , C.TelHome
--	  , C.TelWork
--	  , C.TelMobile
	  ,C.FirstName + ' ' + C.Name [Name] --IP - 12/12/08 - (70591) - Use 'Customer' and 'Custtel' tables to retrieve customer details instead of SR_Customer.
	  ,CT.DialCode + ' ' + CT.Telno [TelHome] --IP - 12/12/08 - (70591)
	  ,CW.DialCode + ' ' + CW.Telno [TelWork] --IP - 12/12/08 - (70591)
	  ,CM.DialCode + ' ' + CM.Telno [TelMobile] --IP - 12/12/08 - (70591)
	  , ISNULL(A.Arrears, 0) as Arrears --RM 25/08/09 pass 0 for null so no blank arrears values (70821)
      , SR.[PurchaseDate]
	  , SR.ProductCode
      , SR.Description
      , SR.ModelNo
	  , SR.SerialNo
	  , SR.ExtWarranty
	  , SR.ContractNo
	  , R.SoftScript
	  , SR.Comments --IP - 13/11/09 - CR1055 - Merged from (5.1.8.1)
	  --, SR.DepositPaid			-- UAT 341 field to be removed
	  , SR.RepairEstimate
	  , CONVERT(VARCHAR(6),AL.TechnicianId) + ' ' + T.FirstName + ' ' + T.LastName AS TechnicianId
	  , SR.TechnicianReport --IP - 13/11/09 - CR1055 - Merged from (5.1.8.1)
	  , AL.RepairDate
	  , AL.PartsDate	
	  , SP.PartNo
	  , SP.Quantity
	  , SI.itemdescr1  [PartDescription]
	  , R.Resolution
	  , Co.CodeDescript [ResolutionDescription] --Code maintenance
	  , R.DateClosed [ResolutionDate]
	  ,	R.Replacement
	  , R.FoodLoss
	  , R.ChargeTo
	  , SP.UnitPrice [Cost]
	  , R.LabourCost
	  , R.AdditionalCost
      , R.TotalCost
      , SR.ServiceBranchNo AS 'Service Location'
      , SR.PurchaseDate AS 'Delivery Date'
      , R.TransportCost  -- CR 1024 (NM 29/04/2009)
--•	Claim Amount
  FROM [SR_ServiceRequest] SR											LEFT OUTER JOIN
	--StockItem SRI		ON SR.ProductCode = SRI.ItemNo AND SRI.StockLocn = SR.StockLocn LEFT OUTER JOIN
	StockItem SRI		ON SR.ItemId = SRI.ItemId AND SRI.StockLocn = SR.StockLocn				-- RI
	--SR_Customer C		ON SR.ServiceRequestNo = C.ServiceRequestNo		LEFT OUTER JOIN
	LEFT OUTER JOIN Customer C			ON SR.Custid = C.Custid							LEFT OUTER JOIN --IP - 12/12/08 - (70591)
	Custtel CT			ON SR.Custid = CT.Custid AND CT.tellocn = 'H' AND CT.datediscon	is null	LEFT OUTER JOIN	--IP - 12/12/08 - (70591)
	Custtel CW			ON SR.Custid = CW.Custid AND CW.tellocn = 'W' AND CW.datediscon	is null	LEFT OUTER JOIN	--IP - 12/12/08 - (70591)
	Custtel CM			ON SR.Custid = CM.Custid AND CM.tellocn = 'M' AND CM.datediscon	is null	LEFT OUTER JOIN	--IP - 12/12/08 - (70591)
	SR_ChargeAcct CA	ON SR.ServiceRequestNo = CA.ServiceRequestNo	LEFT OUTER JOIN
	acct A				ON CA.AcctNo = A.AcctNo							LEFT OUTER JOIN
	SR_Resolution	R   ON SR.ServiceRequestNo = R.ServiceRequestNo		LEFT OUTER JOIN
	SR_Allocation AL	ON SR.ServiceRequestNo = AL.ServiceRequestNo	LEFT OUTER JOIN
	SR_PartListResolved SP ON SR.ServiceRequestNo = SP.ServiceRequestNo LEFT OUTER JOIN
	--StockItem SI		ON SI.Itemno = SP.PartNo AND SI.StockLocn = SR.StockLocn LEFT OUTER JOIN
	StockItem SI		ON SI.ItemId = SP.PartId AND SI.StockLocn = SR.StockLocn				-- RI
	LEFT OUTER JOIN code Co				ON Co.Category = 'SRRESOLVE' AND R.Resolution = Co.Code  LEFT OUTER JOIN
	SR_Technician T     ON T.TechnicianId = AL.TechnicianId
 WHERE
	(@MinDateLogged IS NULL OR SR.DateLogged >= @MinDateLogged) AND
	(@MaxDateLogged IS NULL OR SR.DateLogged <=  dateadd(s, -1, dateadd(d, 1, @maxdatelogged))) AND
	(@ProductCode IS NULL OR SR.ProductCode LIKE @ProductCode + '%') AND 
	(@ProductGroup IS NULL OR SRI.Category = @ProductGroup  ) AND 
	(SR.Status IN (	
						 (SELECT CASE WHEN @Outstanding = 1 THEN 'N' ELSE NULL END)
						, (SELECT CASE WHEN @Outstanding = 1 THEN 'D' ELSE NULL END)
						, (SELECT CASE WHEN @Outstanding = 1 THEN 'T' ELSE NULL END) -- To be allocated
						, (SELECT CASE WHEN @Allocated = 1 THEN 'A' ELSE NULL END)
						, (SELECT CASE WHEN @Allocated = 1 THEN 'H' ELSE NULL END)  -- Technician Allocated - CR 1024 (NM 28/04/2009) 
						, (SELECT CASE WHEN @Allocated = 1 THEN 'S' ELSE NULL END)  -- Allocated to Supplier - CR 1024 (NM 28/04/2009) 
						, (SELECT CASE WHEN @Completed = 1 THEN 'R' ELSE NULL END)
						, (SELECT CASE WHEN @Completed = 1 THEN 'C' ELSE NULL END)  
						) )
						AND R.Fault = @fault
						AND SR.ServiceBranchNo LIKE @branchNo

	END
	
	SET @return = @@error	

	
END
GO


-- End End End End End End End End End End End End End End End End End End End End End End End End End End End End
