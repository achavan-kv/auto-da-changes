SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO
SET ANSI_WARNINGS ON
go

if exists (SELECT * FROM dbo.sysobjects WHERE id = object_id('[dbo].[PendingInstallationsGetSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[PendingInstallationsGetSP]
GO

CREATE PROCEDURE PendingInstallationsGetSP
-- ================================================  
-- Project      : CoSACS .NET  
-- File Name    : PendingInstallationsGetSP.prc  
-- File Type    : MSSQL Server Stored Procedure Script  
-- Title        : Installations - Get Pending  
-- Date         : 6 June 2011  
--  
-- This procedure will load the details of Installatiosn that have not been booked.  
--  
-- Change Control  
-- --------------  
-- Date      By  Description  
-- ----      --  -----------  
-- 20/07/11  IP  #4309 - Not all rows were displayed when Authorised selected for a date range. Added a day to @DAdateto.
-- 03/10/12  IP  #10630 - LW75196 - Prevent Closed Installations from being displayed in the Pending Installations screen.
-- ================================================  
 -- Add the parameters for the stored procedure here
		@acctno CHAR(12),
		@DAdatefrom DATETIME = null,
		@DAdateto DATETIME= null,
		@stocklocation INT,
		@status CHAR(1),
		@top INT  

as
	if @top is NULL
		set @top=9999
		 
	-- get installation items
	select l.acctno,l.agrmtno,itemid,stocklocn,ParentItemID,ParentLocation,si.iupc,ag.dateauth,l.ordval
	into #installations
	from lineitem l INNER JOIN stockinfo si on l.itemid=si.id
			INNER JOIN agreement ag on l.acctno=ag.acctno and l.agrmtno=ag.agrmtno
	where CAST(si.category as VARCHAR) in (select code from code c2 where c2.category in('PCW', 'PCE', 'PCF') and c2.codedescript='Installation')
		and l.ParentItemID!=0			-- installation pre installation functionality
		and (@DAdatefrom is null or dateauth >= CONVERT(DATETIME ,CONVERT(CHAR(12),@DAdatefrom, 112)) ) 
		--and (@DAdateto is null or dateauth <= CONVERT(DATETIME ,CONVERT(CHAR(12),@DAdateto, 112)))
		and (@DAdateto is null or dateauth <= dateadd(day, 1,CONVERT(DATETIME ,CONVERT(CHAR(12),@DAdateto, 112))))	--IP - 20/07/11 - #4309
		and (l.stocklocn=@stocklocation or @stocklocation=0)
		and (l.acctno=@acctno or @acctno='')
		and l.quantity>0
		and ag.holdprop='N'			-- Delivery authorised
		
	-- get other details
	SELECT top (@top)
		LI.acctno AS AcctNo, LI.agrmtno AS AgreementNo, 
		LI.ItemID AS ItemId, SI.IUPC AS ItemNo, SI.itemno AS CourtsCode,
		SI.itemdescr1 AS ProductDescription1, SI.itemdescr2 AS ProductDescription2,
		LI_INST.ItemID AS InstItemId, SI_INST.IUPC AS InstItemNo, SI_INST.itemno AS InstCourtsCode,
		LI_INST.OrdVal AS InstValue, LI.quantity AS Quantity, 
		LI.delqty AS DelQty, LI.stocklocn AS StockLocation,
		LI.datereqdel AS DateReqDel, LI.dateplandel AS DatePlanDel, LI.delnotebranch AS DelNoteBranch,
		LI.notes AS Notes, LI.isKit AS IsKit, LI.deliveryaddress AS DelAddressType, 
		LI.ParentItemID AS ParentItemId, LI.parentlocation AS ParentLocation,
		LI.deliveryprocess AS DelProcess, LI.deliveryarea AS DelArea, LI.DeliveryPrinted AS DelPrinted, 
		LI.assemblyrequired AS AssemblyRequired, LI.damaged AS Damaged, 
		AGR.dateagrmt AS DateAgreement, AGR.holdprop AS HoldProp, AGR.dateauth AS DateDelAuthorised, 
		CUST.custid AS CustId, CUST.name as CustLastName, CUST.firstname as CustFirstName, 
		ADDR.cusaddr1 AS DelAddress1, ADDR.cusaddr2 AS DelAddress2, ADDR.cusaddr3 AS DelAddress3, 
		ADDR.Notes AS DelAddressNote, ADDR.AddressID AS DelAddressId, ADDR.cuspocode AS DelPostCode,
		--HTEL.DialCode AS HomeDialCode, HTEL.telno AS HomeTelNo, HTEL.extnno AS HomeTelExt,
		--WTEL.DialCode AS WorkDialCode, WTEL.telno AS WorkTelNo, WTEL.extnno AS WorkTelExt,
		rtrim(HTEL.DialCode) +' '+ rtrim(HTEL.telno) + case when HTEL.extnno!='' then ' Ext: ' else ' ' end + rtrim(HTEL.extnno) AS HomeTelNo,		-- #4295 jec 18/07/11
		rtrim(WTEL.DialCode) +' '+ rtrim(WTEL.telno) + case when WTEL.extnno!='' then ' Ext: ' else ' ' end + rtrim(WTEL.extnno) AS WorkTelNo,		-- #4295 jec 18/07/11
		SI.Supplier, SI.suppliercode AS SupplierCode
		,(SELECT MAX(DEL.datedel) FROM dbo.delivery DEL 
			WHERE DEL.delorcoll = 'D' AND DEL.acctno = LI.acctno AND DEL.ItemID = LI.ItemID AND del.stocklocn = LI.stocklocn) AS DelDate,
		CASE
			WHEN EXISTS(SELECT 1 FROM dbo.delivery DEL WHERE DEL.delorcoll = 'D' AND DEL.acctno = LI.acctno AND DEL.agrmtno = LI.agrmtno  AND DEL.ItemID = LI.ItemID AND del.stocklocn = LI.stocklocn) THEN 'DELIVERED'
			WHEN EXISTS(SELECT 1 FROM dbo.schedule SCH WHERE SCH.delorcoll = 'D' AND SCH.acctno = LI.acctno AND SCH.agrmtno = LI.agrmtno AND SCH.ItemID = LI.ItemID AND SCH.stocklocn = LI.stocklocn) THEN 'AUTHORISED'			
		END AS DelStatus,
		CASE
			WHEN EXISTS(SELECT 1 
						FROM dbo.lineitem WAR
						INNER JOIN dbo.StockInfo SI_WAR ON WAR.ItemID = SI_WAR.ID 
						WHERE WAR.acctno = LI.acctno AND 
							WAR.agrmtno = LI.agrmtno AND 
							WAR.quantity  > 0 AND
							WAR.ParentItemID = LI.ItemID AND 
							WAR.parentlocation = LI.stocklocn AND
							SI_WAR.category IN(SELECT code FROM dbo.code WHERE category = 'WAR')) 
			THEN CONVERT(BIT, 1)
			ELSE CONVERT(BIT, 0)
		END AS HasWarranty,
		StockCode.category AS StockCategory,
		INST.InstallationNo as InstNo,
		INST.InstallationDate AS InstDate,
		ISNULL(INST.[Status], 'New') AS InstallationStatus  -- Same as Blue.Cosacs.Shared.InstallationStatus enum
	FROM #installations LI_INST  --Installation Item
    INNER JOIN dbo.lineitem LI ON LI_INST.ParentItemID = LI.ItemID AND LI_INST.parentlocation = LI.stocklocn AND 
									LI_INST.acctno = LI.acctno AND LI_INST.agrmtno = LI.agrmtno AND LI.quantity > 0
    INNER JOIN dbo.agreement AGR ON AGR.acctno = LI.acctno AND AGR.agrmtno = LI.agrmtno 
    INNER JOIN dbo.StockInfo SI ON LI.ItemID = SI.ID	
    INNER JOIN dbo.StockInfo SI_INST ON LI_INST.ItemID = SI_INST.ID
    LEFT JOIN (SELECT category, code FROM dbo.code WHERE category IN ('PCW', 'PCE', 'PCF')) StockCode 
				ON CONVERT(VARCHAR, SI.category) = StockCode.code
    INNER JOIN dbo.custacct CA ON CA.hldorjnt = 'H' AND CA.acctno = LI.acctno
    INNER JOIN [dbo].[Customer] CUST ON CUST.[custid] = CA.[custid]
    LEFT OUTER JOIN [dbo].[CustAddress] AS ADDR ON ADDR.[custid] = CUST.[custid] AND LI.[deliveryaddress] = ADDR.[addtype] AND ADDR.[datemoved] IS NULL
    LEFT OUTER JOIN [dbo].[CustTel] AS HTEL ON CUST.[custid] = HTEL.[custid] AND HTEL.[datediscon] IS NULL AND HTEL.[tellocn] = 'H'
    LEFT OUTER JOIN [dbo].[CustTel] AS WTEL ON CUST.[custid] = WTEL.[custid] AND WTEL.[datediscon] IS NULL AND WTEL.[tellocn] = 'W'
    LEFT OUTER JOIN [dbo].[Installation] AS INST ON LI.acctno = INST.AcctNo AND LI.ItemID = INST.ItemId AND 
											LI.agrmtno = INST.AgreementNo AND LI.stocklocn = INST.StockLocation
	where (@status=''
		or (@status='D' and EXISTS(SELECT 1 FROM dbo.delivery DEL WHERE DEL.delorcoll = 'D' AND DEL.acctno = LI.acctno AND DEL.agrmtno = LI.agrmtno  AND DEL.ItemID = LI.ItemID AND del.stocklocn = LI.stocklocn))
		or (@status='A' and EXISTS(SELECT 1 FROM dbo.schedule SCH WHERE SCH.delorcoll = 'D' AND SCH.acctno = LI.acctno AND SCH.agrmtno = LI.agrmtno AND SCH.ItemID = LI.ItemID AND SCH.stocklocn = LI.stocklocn))) --IP - 02/10/12 - #10630 - LW75196			
	AND ISNULL(INST.Status,'New') != 'Closed'		--IP - 02/10/12 - #10630 - LW75196	
 Go
  
 -- End End End End End End End End End End End End End End End End End End End End End End End End End End End End