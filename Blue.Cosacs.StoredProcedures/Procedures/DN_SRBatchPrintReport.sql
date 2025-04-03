
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID('[dbo].[DN_SRBatchPrintReport]') AND OBJECTPROPERTY(id,'IsProcedure') = 1)
DROP PROCEDURE [dbo].[DN_SRBatchPrintReport]

GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[DN_SRBatchPrintReport]
/*#############################################################################  
Procedure:  DN_SRBatchPrintReport  
  
Script filename: DN_SRBatchPrintReport.sql  
  
Description: Returns all the required fields for Service Request Batch Printing  
  
  
Created by: J.Hemans  
Created date: 12/12/2006  
Modified by: J.Hemans  
Modified date: 03/01/2008  
Modified Description: Code change required so that date comparisons work for @MinDateLogged = @MaxDateLogged UAT 340  
Modified by: J.Hemans  
Modified date: 18/04/2008  
Modified Description: Code change required to correctly bring back customer details and deposit paid UAT 403  
Modified by: J.Hemans  
Modified date: 05/10/2008  
Modified Description: CR 949/958 Action Required and Print Location added. Additional parameter @print to return results for branch logged in at  
17/02/09 jec copy 70524 changes from current into 5.1.6.6 
Modified by: I.Parker 
Modified date: 22/07/2011  
Modified Description: RI - #4380 - System Integration changes.
Modified by: I.Parker 
Modified date: 23/09/2011  
Modified Description: RI - #8238 - CR8201 - Service Batch printout -  description needs to be: descr+brand+vendor style long
 
-- 13/09/11  jec #3445 Not all SR's are returned when searched with All option from Technicain dropdown

#############################################################################*/
   
  @MinSRNo INT = NULL,  
  @MaxSRNo INT = NULL,  
  @MinDateLogged SMALLDATETIME = NULL,  
  @MaxDateLogged SMALLDATETIME = NULL,  
  @tech INT = NULL,  
  @printLocn  SMALLINT,  
  @reprintOnly BIT,  --CR 1024 (NM 23/04/2009)  
  @showAll BIT, --CR 1056
  @return INT = 0 OUTPUT  
    
AS  
 
SET NOCOUNT ON 
  
SELECT  code,codedescript INTO #actionCodes FROM code WHERE category = 'SRSERVACT'  
SELECT code, codedescript INTO #serviceLocations FROM code WHERE category = 'SRSERVLCN' --IP - 06/05/09 - UAT(656) --IP - 29/06/09 - Checked UAT(656) merged from 5.1  
  
DECLARE @techmin int, @techmax INT, @IsRepairCentre BIT, @printLocnIncludeNulls BIT
set @techmin=isnull(@tech,0)
set @techmax=isnull(@tech,999999)

SELECT @printLocn AS 'BranchNo' INTO #AllowedPrintLocns 

IF EXISTS(SELECT 1 FROM Branch WHERE BranchNo = @printLocn AND ServiceRepairCentre = 1) -- If it's a repair centre
BEGIN
	INSERT INTO #AllowedPrintLocns SELECT BranchNo FROM Branch WHERE ServiceRepairCentre = 1 --Include all the repiar centres
	SET @IsRepairCentre = 1
	SET @printLocnIncludeNulls = 0
END
ELSE
BEGIN	
	SET @IsRepairCentre = 0
	SET @printLocnIncludeNulls = 1
END



SELECT  c.Title + ' '+ c.FirstName + ' '+ c.LastName AS Name,   
   isnull(c.Address1,'') as address1, isnull(c.Address2,'') as address2, isnull(c.Address3,'') as address3,   
   c.Directions, c.TelHome, c.TelWork, c.TelMobile,   
      c.AddressPC, SR_TechnicianDiary.SlotDate,   
   CASE WHEN SR_TechnicianDiary.SlotNo < 5 THEN 'AM' ELSE 'PM' END AS Slot,   
   SR_PartListResolved.PartNo, SR_PartListResolved.Quantity,   
   SR_PartListResolved.PartType,  
   convert(varchar(4), SR.ServiceBranchNo) + convert(varchar(16), SR.ServiceRequestNo) AS ServiceRequestNo,   
   sr.AcctNo, sr.DateLogged,   
   sr.PurchaseDate, sr.ProductCode,isnull(s.itemdescr1,sr.[Description]) AS ProdDescription, s.itemdescr2 AS ProdDescription2, --70524  
   CASE WHEN sr.ExtWarranty = 'Y' THEN 'Yes' ELSE 'No' END AS ExtWarranty,   
   CASE WHEN DATEADD(year, 1, PurchaseDate) > DateLogged THEN 'Yes' ELSE 'No' END AS FYW,   
      -- SUM(CASE WHEN fintrans.transtypecode = 'PAY' THEN fintrans.transvalue ELSE 0 END)* -1 AS Deposit,  
      cast(0 as money) as Deposit,
   CASE WHEN ISNULL(a.OutstBal,0) < 0 THEN 0 ELSE ISNULL(a.OutstBal,0) END AS Balance,  
   sr.Comments,  
   CASE WHEN SR_Resolution.ChargeTo = 'CUS' AND a.OutstBal > 0 THEN 'Yes' ELSE 'No' END AS ChargeToCustomer,  
   s.warrantable, sra.instructions, --IP - 05/08/08 - UAT5.1 - UAT(516)  
   cd.codedescript AS ActionRequired,SR.PrintLocn,   
   SR.ServiceBranchNo, sl.codedescript AS ServiceLocn, SR.BatchPrintFlag, --CR 1024 (NM 23/04/2009)  --IP - 06/05/09 - UAT(656) - Changed service location to display the description.  
   SR.RepairCentrePrintFlag, -- CR 1056
   SR.SerialNo, -- LW 72308
   SR.ServiceRequestNo as ServiceRequestNo_Short,
   SR.ItemID,												--IP - 22/07/11 - RI - #4380
   rtrim(ltrim(isnull(s.VendorLongStyle,''))) as Style,		--IP - 23/09/11 - RI - #8238 - CR8201
   rtrim(ltrim(isnull(s.Brand,''))) as Brand				--IP - 23/09/11 - RI - #8238 - CR8201
   into #sr_batch  
   FROM  SR_Customer AS c INNER JOIN SR_ServiceRequest AS sr ON c.CustId = sr.CustId AND c.ServiceRequestNo = sr.ServiceRequestNo  
      LEFT OUTER JOIN SR_PartListResolved ON sr.ServiceRequestNo = SR_PartListResolved.ServiceRequestNo   
      LEFT OUTER JOIN SR_TechnicianDiary ON sr.ServiceRequestNo = SR_TechnicianDiary.ServiceRequestNo   
      LEFT OUTER JOIN SR_Resolution ON sr.ServiceRequestNo = SR_Resolution.ServiceRequestNo   
      --LEFT OUTER JOIN stockitem s ON SR.ProductCode = s.itemno AND SR.ServiceBranchNo = s.stocklocn   
      LEFT OUTER JOIN stockitem s ON SR.ItemID = s.ID AND SR.ServiceBranchNo = s.stocklocn			--IP - 22/07/11 - RI - #4380  
      LEFT OUTER JOIN SR_ChargeAcct src ON src.ServiceRequestNo = sr.ServiceRequestNo   
      --LEFT OUTER JOIN fintrans ON src.AcctNo = fintrans.acctno and fintrans.transtypecode = 'PAY'  
      LEFT OUTER JOIN acct a ON a.acctno = src.AcctNo  
      LEFT OUTER JOIN SR_Allocation sra ON sr.ServiceRequestNo = sra.ServiceRequestNo --IP - 05/08/08 - UAT5.1 - UAT(516)  
      LEFT OUTER JOIN #actionCodes cd ON SR.ActionRequired = cd.code  
      LEFT OUTER JOIN #serviceLocations sl ON SR.ServiceLocn = sl.code --IP - 06/05/09 - UAT(656)  
   WHERE       isnull(SR_TechnicianDiary.TechnicianId,0) between @techmin and @techmax AND   
      (src.ChargeType IS NULL OR   
       (src.ChargeType ='C' AND SR_Resolution.ChargeTo <> 'DEL')    
       OR src.ChargeType in ('D','S','I','W'))	-- 30/09/11 #3445
       and (@MinDateLogged IS NULL OR CONVERT(SMALLDATETIME,CONVERT(VARCHAR,SR.DateLogged,103),103) >= @MinDateLogged) AND  
      (@MaxDateLogged IS NULL OR CONVERT(SMALLDATETIME,CONVERT(VARCHAR,SR.DateLogged,103),103) <= @MaxDateLogged) AND  
      (@MinSRNo IS NULL OR SR.ServiceRequestNo >= @MinSRNo) AND  
      (@MaxSRNo IS NULL OR SR.ServiceRequestNo <= @MaxSRNo) AND 
      ((@showAll = 1 OR SR.PrintLocn IN (SELECT BranchNo FROM #AllowedPrintLocns)) OR (@printLocnIncludeNulls = 1 AND SR.PrintLocn IS NULL)) 
   /*GROUP BY c.Title,c.FirstName, c.LastName, c.Address1, c.Address2, c.Address3, c.Directions, c.TelHome, c.TelWork, c.TelMobile,   
      c.AddressPC, SR_TechnicianDiary.SlotDate, SR_TechnicianDiary.SlotNo,SR_TechnicianDiary.TechnicianId,  
      SR_PartListResolved.PartNo, SR_PartListResolved.Quantity, SR_PartListResolved.PartType,SR.ServiceBranchNo, sr.ServiceRequestNo, sr.AcctNo, sr.DateLogged,   
      sr.PurchaseDate, sr.ProductCode,isnull(s.itemdescr1,sr.[Description]),  s.itemdescr2,sr.ExtWarranty, sr.Comments,SR_Resolution.ChargeTo,s.warrantable,a.outstbal, sra.instructions, --IP - 05/08/08 - UAT5.1 - UAT(516)  
      cd.codedescript,SR.PrintLocn, sl.codedescript, SR.BatchPrintFlag, SR.RepairCentrePrintFlag, --CR 1024 (NM 23/04/2009)  --IP - 06/05/09 - UAT(656) - added 'sl.codedescript'
	  sr.StockLocn, -- UAT(5.1) - 752
	  SR.SerialNo */ -- LW 72308
   /*HAVING  (@MinDateLogged IS NULL OR CONVERT(SMALLDATETIME,CONVERT(VARCHAR,SR.DateLogged,103),103) >= @MinDateLogged) AND  
      (@MaxDateLogged IS NULL OR CONVERT(SMALLDATETIME,CONVERT(VARCHAR,SR.DateLogged,103),103) <= @MaxDateLogged) AND  
      (@MinSRNo IS NULL OR SR.ServiceRequestNo >= @MinSRNo) AND  
      (@MaxSRNo IS NULL OR SR.ServiceRequestNo <= @MaxSRNo) AND 
      ((@showAll = 1 OR SR.PrintLocn IN (SELECT BranchNo FROM #AllowedPrintLocns)) OR (@printLocnIncludeNulls = 1 AND SR.PrintLocn IS NULL)) */
        
  
   insert into #sr_batch  
    SELECT  c.Title + ' '+ c.FirstName + ' '+ c.Name AS Name, isnull(ca.cusaddr1,'') AS Address1, isnull(ca.cusaddr2,'') AS Address2, isnull(ca.cusaddr3,'') AS Address3, isnull(ca.Notes,'') AS Directions,  
       ct1.DialCode + ' ' + ct1.TelNo AS TelHome,ct2.DialCode + ' ' + ct2.TelNo AS TelWork,ct3.DialCode + ' ' + ct3.TelNo AS TelMobile,   
       isnull(ca.CusPoCode,'') AS AddressPC, SR_TechnicianDiary.SlotDate, CASE WHEN SR_TechnicianDiary.SlotNo < 5 THEN 'AM' ELSE 'PM' END AS Slot,   
       SR_PartListResolved.PartNo, SR_PartListResolved.Quantity, SR_PartListResolved.PartType,  
       convert(varchar(4), SR.ServiceBranchNo) + convert(varchar(16), SR.ServiceRequestNo) AS ServiceRequestNo, sr.AcctNo, sr.DateLogged,   
       sr.PurchaseDate, sr.ProductCode,isnull(s.itemdescr1,sr.[Description]) AS ProdDescription,  s.itemdescr2 AS ProdDescription2,  --70524  
       CASE WHEN sr.ExtWarranty = 'Y' THEN 'Yes' ELSE 'No' END AS ExtWarranty, CASE WHEN DATEADD(year, 1, PurchaseDate) > DateLogged THEN 'Yes' ELSE 'No' END AS FYW,   
                   --SUM(CASE WHEN fintrans.transtypecode = 'PAY' THEN fintrans.transvalue ELSE 0 END)* -1 AS Deposit,  
                   cast (0 as money) as Deposit,
       CASE WHEN ISNULL(a.OutstBal,0) < 0 THEN 0 ELSE ISNULL(a.OutstBal,0) END AS Balance,sr.Comments,  
       CASE WHEN SR_Resolution.ChargeTo = 'CUS' AND a.OutstBal > 0 THEN 'Yes' ELSE 'No' END AS ChargeToCustomer,s.warrantable, sra.instructions, --IP - 05/08/08 - UAT5.1 - UAT(516)  
       cd.codedescript AS ActionRequired,SR.PrintLocn,   
       SR.ServiceBranchNo, sl.codedescript AS ServiceLocn, SR.BatchPrintFlag, --CR 1024 (NM 23/04/2009)   --IP 06/05/09 - UAT(656)  
       SR.RepairCentrePrintFlag, -- CR 1056
	   SR.SerialNo, -- LW 72308
	   SR.ServiceRequestNo as ServiceRequestNo_Short,
	   SR.ItemID,											--IP - 22/07/11 - RI - #4380
	   rtrim(ltrim(isnull(s.VendorLongStyle,''))) as Style,		--IP - 23/09/11 - RI - #8238 - CR8201
	   rtrim(ltrim(isnull(s.Brand,''))) as Brand				--IP - 23/09/11 - RI - #8238 - CR8201
    FROM  Customer AS c LEFT OUTER JOIN CustAddress AS ca  
       ON   ca.CustId = c.custid AND ca.AddType = 'H' AND ISNULL(ca.DateMoved,'') = ''  
       LEFT OUTER JOIN CustTel ct1  
       ON   ct1.CustId = c.custid AND ct1.TelLocn = 'H' AND ISNULL(ct1.DateDiscon,'') = ''  
       LEFT OUTER JOIN CustTel ct2  
       ON   ct2.CustId = c.custid AND ct2.TelLocn = 'W' AND ISNULL(ct2.DateDiscon,'') = ''  
       LEFT OUTER JOIN CustTel ct3  
       ON   ct3.CustId = c.custid AND ct3.TelLocn = 'M' AND ISNULL(ct3.DateDiscon,'') = ''  
       INNER JOIN  
       SR_ServiceRequest AS sr ON c.CustId = sr.CustId LEFT OUTER JOIN  
       SR_PartListResolved ON sr.ServiceRequestNo = SR_PartListResolved.ServiceRequestNo LEFT OUTER JOIN  
       SR_TechnicianDiary ON sr.ServiceRequestNo = SR_TechnicianDiary.ServiceRequestNo LEFT OUTER JOIN  
       SR_Resolution ON sr.ServiceRequestNo = SR_Resolution.ServiceRequestNo LEFT OUTER JOIN  
       --stockitem s ON SR.ProductCode = s.itemno AND SR.ServiceBranchNo = s.stocklocn LEFT OUTER JOIN  
       stockitem s ON SR.ItemID = s.ID AND SR.ServiceBranchNo = s.stocklocn LEFT OUTER JOIN					--IP - 22/07/11 - RI - #4380
       SR_ChargeAcct src ON src.ServiceRequestNo = sr.ServiceRequestNo LEFT OUTER JOIN acct a ON a.acctno = src.AcctNo 
       --LEFT OUTER JOIN  fintrans ON src.AcctNo = fintrans.acctno and fintrans.transtypecode = 'PAY'
       LEFT OUTER JOIN SR_Allocation sra ON sr.ServiceRequestNo = sra.ServiceRequestNo --IP - 05/08/08 - UAT5.1 - UAT(516)  
       LEFT OUTER JOIN #actionCodes cd ON SR.ActionRequired = cd.code  
       LEFT OUTER JOIN #serviceLocations sl ON SR.ServiceLocn = sl.code--IP - 06/05/09 - UAT(656)  
    WHERE       isnull(SR_TechnicianDiary.TechnicianId,0) between @techmin and @techmax  
       and SR.ServiceRequestNo NOT IN (SELECT sc.ServiceRequestNo FROM SR_Customer sc INNER JOIN SR_ServiceRequest sr ON sc.CustId = sr.CustId AND sc.ServiceRequestNo = sr.ServiceRequestNo 
						WHERE (status = 'C' OR status = 'R') AND (@MinSRNo IS NULL OR SR.ServiceRequestNo >= @MinSRNo)AND (@MaxSRNo IS NULL OR SR.ServiceRequestNo <= @MaxSRNo))   
       AND (src.ChargeType IS NULL OR (src.ChargeType ='C' AND SR_Resolution.ChargeTo <> 'DEL') 
									OR src.ChargeType in ('D','S','I','W'))  -- 30/09/11 #3445
       and (@MinDateLogged IS NULL OR CONVERT(SMALLDATETIME,CONVERT(VARCHAR,SR.DateLogged,103),103) >= @MinDateLogged) AND  
      (@MaxDateLogged IS NULL OR CONVERT(SMALLDATETIME,CONVERT(VARCHAR,SR.DateLogged,103),103) <= @MaxDateLogged) AND  
       (@MinSRNo IS NULL OR SR.ServiceRequestNo >= @MinSRNo) AND  
       (@MaxSRNo IS NULL OR SR.ServiceRequestNo <= @MaxSRNo) and
       ((@showAll = 1 OR SR.PrintLocn IN (SELECT BranchNo FROM #AllowedPrintLocns)) OR (@printLocnIncludeNulls = 1 AND SR.PrintLocn IS NULL))
    /*GROUP BY c.Title,c.FirstName,c.Name, ca.cusaddr1, ca.cusaddr2 , ca.cusaddr3, ca.Notes, ct1.DialCode,ct1.TelNo, ct2.DialCode,ct2.TelNo, ct3.DialCode,ct3.TelNo,   
       ca.CusPoCode, SR_TechnicianDiary.SlotDate, SR_TechnicianDiary.SlotNo, SR_TechnicianDiary.TechnicianId,  
       SR_PartListResolved.PartNo, SR_PartListResolved.Quantity, SR_PartListResolved.PartType,SR.ServiceBranchNo, sr.ServiceRequestNo, sr.AcctNo, sr.DateLogged,   
       sr.PurchaseDate, sr.ProductCode,isnull(s.itemdescr1,sr.[Description]),  s.itemdescr2,sr.ExtWarranty, sr.Comments,SR_Resolution.ChargeTo,s.warrantable,a.outstbal, sra.instructions, --IP - 05/08/08 - UAT5.1 - UAT(516)  
       cd.codedescript,SR.PrintLocn, sl.codedescript, SR.BatchPrintFlag, SR.RepairCentrePrintFlag, --CR 1024 (NM 23/04/2009)    --IP 06/05/09 - UAT(656)
	   sr.StockLocn, -- UAT(5.1) - 752
	   SR.SerialNo*/ -- LW 72308
    /*HAVING  (@MinDateLogged IS NULL OR CONVERT(SMALLDATETIME,CONVERT(VARCHAR,SR.DateLogged,103),103) >= @MinDateLogged) AND  
       (@MaxDateLogged IS NULL OR CONVERT(SMALLDATETIME,CONVERT(VARCHAR,SR.DateLogged,103),103) <= @MaxDateLogged) AND  
       (@MinSRNo IS NULL OR SR.ServiceRequestNo >= @MinSRNo) AND  
       (@MaxSRNo IS NULL OR SR.ServiceRequestNo <= @MaxSRNo) AND 
       ((@showAll = 1 OR SR.PrintLocn IN (SELECT BranchNo FROM #AllowedPrintLocns)) OR (@printLocnIncludeNulls = 1 AND SR.PrintLocn IS NULL))*/
   
-- calculate the deposit
update #sr_batch 
set Deposit = B.Deposit * -1 
from #sr_batch A inner join (select src.ServiceRequestNo, sum(fintrans.transvalue) as Deposit
							  from SR_ChargeAcct src 
							  join fintrans
							    on src.AcctNo = fintrans.acctno and fintrans.transtypecode = 'PAY'
							  left join SR_Resolution 
							    on src.ServiceRequestNo = SR_Resolution.ServiceRequestNo   
							   AND (src.ChargeType IS NULL OR (src.ChargeType ='C' AND SR_Resolution.ChargeTo <> 'DEL') OR src.ChargeType in ('D','S','I','W'))  -- 30/09/11 #3445
							  group by src.ServiceRequestNo) B
on A.ServiceRequestNo_Short = B.ServiceRequestNo

alter table #sr_batch drop column ServiceRequestNo_Short;
  
SELECT DISTINCT   
SR.Name, SR.AcctNo, SR.ServiceRequestNo, SR.SlotDate,   
SR.ActionRequired, SR.PrintLocn, SR.ServiceLocn,  
Case   --CR 1024 (NM 23/04/2009)   
 When SI.SupplierCode is NOT NULL and SI.SupplierCode != '' Then SI.SupplierCode  
 When SI.Supplier is NOT NULL and SI.Supplier != '' Then SI.Supplier  
 Else ''  
End as Supplier   
FROM #sr_batch SR  
--LEFT JOIN StockItem SI on SI.ItemNo = SR.ProductCode and SI.StockLocn = SR.ServiceBranchNo  
LEFT JOIN StockItem SI on SI.ID = SR.ItemID and SI.StockLocn = SR.ServiceBranchNo				--IP - 22/07/11 - RI - #4380
WHERE (@IsRepairCentre = 0 AND ((@reprintOnly = 0 and BatchPrintFlag = '') or (@reprintOnly = 1 and BatchPrintFlag != ''))) --CR1056
	OR (@IsRepairCentre = 1 AND ((@reprintOnly = 0 and RepairCentrePrintFlag = '') or (@reprintOnly = 1 and RepairCentrePrintFlag != ''))) --CR1056
--WHERE (@reprintOnly = 0 and BatchPrintFlag = '') or (@reprintOnly = 1 and BatchPrintFlag != '') -- UAT 699
   
  
SELECT   
DISTINCT SR.*, Case  --CR 1024 (NM 23/04/2009)   
     When SI.SupplierCode is NOT NULL and SI.SupplierCode != '' Then SI.SupplierCode  
     When SI.Supplier is NOT NULL and SI.Supplier != '' Then SI.Supplier       
     Else ''  
      End as Supplier  
FROM #sr_batch SR  
--LEFT JOIN StockItem SI on SI.ItemNo = SR.ProductCode and SI.StockLocn = SR.ServiceBranchNo 
LEFT JOIN StockItem SI on SI.ID = SR.ItemID and SI.StockLocn = SR.ServiceBranchNo					--IP - 22/07/11 - RI - #4380
WHERE (@IsRepairCentre = 0 AND ((@reprintOnly = 0 and BatchPrintFlag = '') or (@reprintOnly = 1 and BatchPrintFlag != ''))) --CR1056
	OR (@IsRepairCentre = 1 AND ((@reprintOnly = 0 and RepairCentrePrintFlag = '') or (@reprintOnly = 1 and RepairCentrePrintFlag != ''))) --CR1056
--WHERE (@reprintOnly = 0 and BatchPrintFlag = '') or (@reprintOnly = 1 and BatchPrintFlag != '') -- UAT 699  
  
  
SET @Return = @@error  
GO

SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO

-- End End End End End End End End End End End End End End End End End End End End End End End End End End End End End End End End End End End