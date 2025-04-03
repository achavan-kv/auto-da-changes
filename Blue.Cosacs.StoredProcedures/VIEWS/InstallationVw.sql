SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[InstallationVw]'))
	DROP VIEW [dbo].[InstallationVw]
GO

-- ================================================      
-- Project      : CoSACS .NET      
-- File Name    : InstallationVw.sql      
-- File Type    : MSSQL Server Stored Procedure Script      
-- Title        :   
-- Author       : ??      
-- Date         : ??      
--      
-- Change Control      
-- --------------      
-- Date      By  Description      
-- ----      --  -----------      
-- 19/07/11  IP  #4303 - TechnicianName was previously displayed in Account Details Installation tab.
--				 Changed view to return TechnicianId and TechnicianName
-- 19/07/11  IP  #4312 - Print Home and Work Telephone numbers on Installation Booking print
-- 02/10/12  IP  #10630 - LW75196 - For a new installation not yet created no rows were returned. If not
--				 in the Installation table then status should be 'New'.
-- =================================================================================   

CREATE VIEW [dbo].[InstallationVw] AS ( 
    SELECT 
		I.AcctNo, I.AgreementNo, 
		I.ItemId, I.ItemNo, I.CourtsCode,
		I.ProductDescription1, I.ProductDescription2,
		I.InstItemId, I.InstItemNo, I.InstCourtsCode,
		I.InstValue, I.Quantity, 
		I.DelQty, I.StockLocation,
		I.DateReqDel, I.DatePlanDel, I.DelNoteBranch,
		--LI.notes AS Notes, LI.isKit AS IsKit, LI.deliveryaddress AS DelAddressType, 
		I.ParentItemId, I.ParentLocation,
		I.DelProcess, --I.deliveryarea AS DelArea, LI.DeliveryPrinted AS DelPrinted, 
		--LI.assemblyrequired AS AssemblyRequired, LI.damaged AS Damaged, 
		AGR.dateagrmt AS DateAgreement, AGR.holdprop AS HoldProp, AGR.dateauth AS DateDelAuthorised, 
		CUST.custid AS CustId, CUST.name as CustLastName, CUST.firstname as CustFirstName, 
		ADDR.cusaddr1 AS DelAddress1, ADDR.cusaddr2 AS DelAddress2, ADDR.cusaddr3 AS DelAddress3, 
		ADDR.Notes AS DelAddressNote, ADDR.AddressID AS DelAddressId, ADDR.cuspocode AS DelPostCode,
		--'' AS HomeDialCode, '' AS HomeTelNo, '' AS HomeTelExt,--HTEL.DialCode AS HomeDialCode, HTEL.telno AS HomeTelNo, HTEL.extnno AS HomeTelExt,
		--'' AS WorkDialCode, '' AS WorkTelNo, '' AS WorkTelExt,--WTEL.DialCode AS WorkDialCode, WTEL.telno AS WorkTelNo, WTEL.extnno AS WorkTelExt,
		rtrim(HTEL.DialCode) +' '+ rtrim(HTEL.telno) + case when HTEL.extnno!='' then ' Ext: ' else ' ' end + rtrim(HTEL.extnno) AS HomeTelNo,			--IP - 19/07/11 - #4312	
		rtrim(WTEL.DialCode) +' '+ rtrim(WTEL.telno) + case when WTEL.extnno!='' then ' Ext: ' else ' ' end + rtrim(WTEL.extnno) AS WorkTelNo,			--IP - 19/07/11 - #4312	
		I.Supplier, I.SupplierCode,
		(SELECT MAX(DEL.datedel) FROM dbo.delivery DEL 
			WHERE DEL.delorcoll = 'D' AND DEL.acctno = I.acctno AND DEL.ItemID = I.ItemID AND del.stocklocn = I.StockLocation) AS DelDate,
		CASE
			WHEN EXISTS(SELECT 1 FROM dbo.delivery DEL WHERE DEL.delorcoll = 'D' AND DEL.acctno = I.acctno AND DEL.agrmtno = I.AgreementNo  AND DEL.ItemID = I.ItemID AND del.stocklocn = I.StockLocation) THEN 'DELIVERED'
			WHEN EXISTS(SELECT 1 FROM dbo.schedule SCH WHERE SCH.delorcoll = 'D' AND SCH.acctno = I.acctno AND SCH.agrmtno = I.AgreementNo AND SCH.ItemID = I.ItemID AND SCH.stocklocn = I.StockLocation) THEN 'AUTHORISED'			
		END AS DelStatus,
		CASE
			WHEN EXISTS(SELECT 1 
						FROM dbo.lineitem WAR
						INNER JOIN dbo.StockInfo SI_WAR ON WAR.ItemID = SI_WAR.ID 
						WHERE WAR.acctno = I.acctno AND 
							WAR.agrmtno = I.AgreementNo AND 
							WAR.quantity  > 0 AND
							WAR.ParentItemID = I.ItemID AND 
							WAR.parentlocation = I.StockLocation AND
							SI_WAR.category IN(SELECT code FROM dbo.code WHERE category = 'WAR')) 
			THEN CONVERT(BIT, 1)
			ELSE CONVERT(BIT, 0)
		END AS HasWarranty,
		I.StockCategory,
		--INST.InstallationNo as InstNo,
		ISNULL(INST.InstallationNo,0) as InstNo,	-- IP - 02/10/12 - #10630 - LW75196		
		INST.InstallationDate AS InstDate,
		--ISNULL(INST.[Status], 'Unknown') AS InstallationStatus,  -- Same as Blue.Cosacs.Shared.InstallationStatus enum
		ISNULL(INST.[Status], 'New') AS InstallationStatus,  -- IP - 02/10/12 - #10630 - LW75196
		ISNULL(ib.TechnicianId,0) AS TechnicianId,																		--IP - 19/07/11 - #4303
		ISNULL(st.FirstName + ' ' + st.LastName,'') as TechnicianName													--IP - 19/07/11 - #4303
	FROM InstallationItemVw I  --Installation Item
    --INNER JOIN dbo.lineitem LI ON LI_INST.ParentItemID = LI.ItemID AND LI_INST.parentlocation = LI.stocklocn AND 
				--					LI_INST.acctno = LI.acctno AND LI_INST.agrmtno = LI.agrmtno --AND LI.quantity > 0
    INNER JOIN dbo.agreement AGR ON AGR.acctno = I.AcctNo AND AGR.agrmtno = I.AgreementNo 
    --INNER JOIN dbo.StockInfo SI ON LI.ItemID = SI.ID	
    --INNER JOIN dbo.StockInfo SI_INST ON LI_INST.ItemID = SI_INST.ID
    --LEFT JOIN (SELECT category, code FROM dbo.code WHERE category IN ('PCW', 'PCE', 'PCF')) StockCode 
				--ON CONVERT(VARCHAR, SI.category) = StockCode.code
    INNER JOIN dbo.custacct CA ON CA.hldorjnt = 'H' AND CA.acctno = I.AcctNo
    INNER JOIN [dbo].[Customer] CUST ON CUST.[custid] = CA.[custid]
    LEFT OUTER JOIN [dbo].[CustAddress] AS ADDR ON ADDR.[custid] = CUST.[custid] AND I.[DelAddressType] = ADDR.[addtype] AND ADDR.[datemoved] IS NULL
    LEFT OUTER JOIN [dbo].[CustTel] AS HTEL ON CUST.[custid] = HTEL.[custid] AND HTEL.[datediscon] IS NULL AND HTEL.[tellocn] = 'H'					--IP - 19/07/11 - #4312 - Re-instated
    LEFT OUTER JOIN [dbo].[CustTel] AS WTEL ON CUST.[custid] = WTEL.[custid] AND WTEL.[datediscon] IS NULL AND WTEL.[tellocn] = 'W'					--IP - 19/07/11 - #4312 - Re-instated
    LEFT OUTER JOIN [dbo].[Installation] AS INST ON I.acctno = INST.AcctNo AND I.ItemID = INST.ItemId AND 
											I.AgreementNo = INST.AgreementNo AND I.StockLocation = INST.StockLocation
	LEFT JOIN InstallationBooking ib on ib.installationno = INST.InstallationNo AND ib.IsDeleted = 0					--IP - 19/07/11 - #4303
	LEFT JOIN SR_Technician st on ib.TechnicianId = st.TechnicianId														--IP - 19/07/11 - #4303
  --  WHERE LI_INST.quantity > 0 --AND SI_INST.IUPC IN (SELECT code FROM dbo.code WHERE category = 'INST')
		--and si_inst.category in (select code from code c2 where c2.category in('PCW', 'PCE', 'PCF') and c2.codedescript='Installation')
		--AND LI.quantity  > 0
)
GO
