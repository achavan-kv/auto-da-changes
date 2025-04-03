
-- =============================================
-- Author:      NILESH KUMAR CHOUBISA
-- Create date: 4 JULY 2019
-- Description:	Dispatch Track Interface Report -CR 6157161
-- =============================================
IF EXISTS (SELECT * FROM sysobjects WHERE TYPE = 'P' AND NAME = 'usp_DispatchTrackInterfaceReport')
DROP PROCEDURE usp_DispatchTrackInterfaceReport
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- ========================================================================
-- Version:		<001> 
-- ========================================================================
CREATE PROCEDURE usp_DispatchTrackInterfaceReport   --'11/01/2018','7/04/2019'
@Branch	   SMALLINT
AS	
BEGIN TRY
	Declare @StartDate DATE
	Declare @EndDate   DATE 	
	SET @StartDate =   DATEADD(d,-1,getdate())
	SET @EndDate   =  DATEADD(d,1,getdate())

	IF OBJECT_ID('tempdb..##DispatchTrackReport') IS NOT NULL
	BEGIN 
		DROP TABLE ##DispatchTrackReport
	END
	
	DECLARE @PATH VARCHAR(1000) = 'd:\users\default'
	DECLARE @Country VARCHAR(20)
	DECLARE @countryCode Char(1)
	SELECT @Country=countryname,@countryCode= countrycode FROM country; 
	
	With DispatchTrack_Report(ID,Truck,Driver,ShipCity,ITEMNO,ACCTNO,SalesPerson,QUANTITY,ZipCode,CustomerName,DeliveryBranch,DELIVERYORCOLLECTIONDATE,
	ShipAddress1,ShipAddress2,ShipAddress3,ADDRESSNOTES,Itemdescription,HomePhone,WorkPhone,MobilePhone,DeliveryType)
	AS	
	(SELECT DISTINCT
			WB.ID																				AS	'ID',
			WT.NAME																					AS	'Truck',
			WD.NAME																				AS	'Driver',
			@Country																			AS	'ShipCity',    			
			WB.ITEMNO																			AS	'Itemno',
			WB.ACCTNO																			AS	'Acctno',
			UV.FULLNAME																			AS	'SalesPerson',
			WB.QUANTITY																			AS	'Quantity',
			REPLACE(WB.POSTCODE,',',' ')														AS  'ZipCode',
			WB.CUSTOMERNAME																		AS	'CustomerName',
			WB.DELIVERYBRANCH																	AS	'DeliveryBranch',
			FORMAT(WB.DELIVERYORCOLLECTIONDATE, 'MM/dd/yyyy')									AS	'DeliveryORCollectionDate',		
			REPLACE((REPLACE((REPLACE(WB.ADDRESSLINE1, CHAR(9), ' ')),'  ','')),',',' ')		AS	'ShipAddress1',
			REPLACE((REPLACE((REPLACE(WB.ADDRESSLINE2, CHAR(9), ' ')),'  ','')),',',' ')		AS	'ShipAddress2',
			REPLACE((REPLACE((REPLACE(WB.ADDRESSLINE3, CHAR(9), ' ')),'  ','')),',',' ')		AS	'ShipAddress3',
			--REPLACE((REPLACE((REPLACE(WB.ADDRESSNOTES, CHAR(9), ' ')),'  ','')),',',' ')		AS	'ADDRESSNOTES',			
			--REPLACE((REPLACE((REPLACE(WB.PRODUCTDESCRIPTION, CHAR(9), ' ')),'  ','')),',',' ')	AS	'Itemdescription',
			REPLACE(REPLACE(REPLACE(REPLACE(WB.ADDRESSNOTES, CHAR(13), ''), CHAR(10), ''), '  ', ' '),',',' ')		AS	'AddressNotes',
			REPLACE(REPLACE(REPLACE(REPLACE(WB.PRODUCTDESCRIPTION, CHAR(13), ''), CHAR(10), ''), '  ', ' '),',',' ') AS	'Itemdescription',		
			CASE 
				WHEN TV.HOMETEL='' THEN ''
				ELSE	Case 
							When  @countryCode = 'T' Then
								CONCAT('868-',TV.HOMETEL)
							 When  @countryCode = 'J' Then
								CONCAT('876-',TV.HOMETEL)
							 When  @countryCode = 'B' Then
								CONCAT('246-',TV.HOMETEL)
							 When  @countryCode = 'A' Then
								CONCAT('592-',TV.HOMETEL)
							 When  @countryCode = 'O' Then
								CONCAT('170-',TV.HOMETEL)
							 When  @countryCode = 'X' Then
								CONCAT('721-',TV.HOMETEL)
							 When  @countryCode = 'C' Then
								CONCAT('599-',TV.HOMETEL)
							 When  @countryCode = 'N' Then
								CONCAT('268-',TV.HOMETEL)
							 When  @countryCode = 'V' Then
								CONCAT('784-',TV.HOMETEL)
							 When  @countryCode = 'L' Then
								CONCAT('758-',TV.HOMETEL)
							 When  @countryCode = 'Z' Then
								CONCAT('501-',TV.HOMETEL)
							 When  @countryCode = 'G' Then
								CONCAT('473-',TV.HOMETEL)
							 When  @countryCode = 'D' Then
								CONCAT('767-',TV.HOMETEL)
							 When  @countryCode = 'K' Then
								CONCAT('869-',TV.HOMETEL)
						End
				END																				AS	'HomePhone',
			CASE 
				WHEN TV.WORKTEL='' THEN ''
					ELSE 
						Case 
							When  @countryCode = 'T' Then
								CONCAT('868-',TV.WORKTEL)
							 When  @countryCode = 'J' Then
								CONCAT('876-',TV.WORKTEL)
							 When  @countryCode = 'B' Then
								CONCAT('246-',TV.WORKTEL)
							 When  @countryCode = 'A' Then
								CONCAT('592-',TV.WORKTEL)
							 When  @countryCode = 'O' Then
								CONCAT('170-',TV.WORKTEL)
							 When  @countryCode = 'X' Then
								CONCAT('721-',TV.WORKTEL)
							 When  @countryCode = 'C' Then
								CONCAT('599-',TV.WORKTEL)
							 When  @countryCode = 'N' Then
								CONCAT('268-',TV.WORKTEL)
							 When  @countryCode = 'V' Then
								CONCAT('784-',TV.WORKTEL)
							 When  @countryCode = 'L' Then
								CONCAT('758-',TV.WORKTEL)
							 When  @countryCode = 'Z' Then
								CONCAT('501-',TV.WORKTEL)
							 When  @countryCode = 'G' Then
								CONCAT('473-',TV.WORKTEL)
							 When  @countryCode = 'D' Then
								CONCAT('767-',TV.WORKTEL)
							 When  @countryCode = 'K' Then
								CONCAT('869-',TV.WORKTEL)
						End
					
				END																				AS	'WorkPhone',
			CASE 
				WHEN TV.MOBILETEL='' THEN ''
					ELSE 
						Case 
							When  @countryCode = 'T' Then
								CONCAT('868-',TV.MOBILETEL)
							 When  @countryCode = 'J' Then
								CONCAT('876-',TV.MOBILETEL)
							 When  @countryCode = 'B' Then
								CONCAT('246-',TV.MOBILETEL)
							 When  @countryCode = 'A' Then
								CONCAT('592-',TV.MOBILETEL)
							 When  @countryCode = 'O' Then
								CONCAT('170-',TV.MOBILETEL)
							 When  @countryCode = 'X' Then
								CONCAT('721-',TV.MOBILETEL)
							 When  @countryCode = 'C' Then
								CONCAT('599-',TV.MOBILETEL)
							 When  @countryCode = 'N' Then
								CONCAT('268-',TV.MOBILETEL)
							 When  @countryCode = 'V' Then
								CONCAT('784-',TV.MOBILETEL)
							 When  @countryCode = 'L' Then
								CONCAT('758-',TV.MOBILETEL)
							 When  @countryCode = 'Z' Then
								CONCAT('501-',TV.MOBILETEL)
							 When  @countryCode = 'G' Then
								CONCAT('473-',TV.MOBILETEL)
							 When  @countryCode = 'D' Then
								CONCAT('767-',TV.MOBILETEL)
							 When  @countryCode = 'K' Then
								CONCAT('869-',TV.MOBILETEL)
						End
				
				END																				AS	'MobilePhone',

				WB.DELIVERYORCOLLECTION AS 'DeliveryType'
			FROM WAREHOUSE.BOOKING WB
				LEFT JOIN TELVIEW TV ON WB.ACCTNO = TV.ACCTNO
				INNER JOIN WAREHOUSE.TRUCK	WT ON WB.TRUCKID=WT.ID  
				INNER JOIN WAREHOUSE.DRIVER WD ON WT.DRIVERID=WD.ID
				INNER JOIN USERROLEVIEW UV ON WB.BookedBy=UV.ID --AND UV.NAME='Sales Person' 
				WHERE WB.DELIVERYORCOLLECTIONDATE BETWEEN @STARTDATE AND @ENDDATE
				--AND WB.TRUCKID IS NULL 
				--And WB.ScheduleQuantity  IS null
				AND WB.DeliveryConfirmedBy IS NULL 
				--AND WB.ScheduleRejected IS NULL 
				AND WB.DeliveryConfirmedOnDate IS NULL 
				--AND WB.DeliveryRejected IS NULL 
				--AND WB.PickingRejected IS NULL
				AND WB.DeliveryBranch = @Branch
				AND WB.Scheduleid IS NULL
				--AND WB.Quantity > 0 
				AND WB.DELIVERYORCOLLECTION IN ('C','D')
		 )

	SELECT * INTO ##DispatchTrackReport FROM DispatchTrack_Report WHERE DeliveryType IS NOT NULL
	------------------------------------------------------------------------------------
	DECLARE @statement sqltext, @Header NVARCHAR(MAX),  @bcppath VARCHAR(100), @filename VARCHAR(1000)  
	  
	SELECT @bcppath = (SELECT value FROM CountryMaintenance WHERE CodeName = 'BCPpath')
	
	SELECT @filename = @path+ '\DispatchTrackInterfaceReport' +  CONVERT(VARCHAR, GETDATE(), 112) +'.csv'  --yyyymmdd

	SET @statement =db_name()
	SET @statement =  'bcp " select ''Shipment Number'',''Customer Name'',''Ship Address 1'',''Ship Address 2'',''Ship Address 3'',''ZipCode'',''Ship City'',''Delivery Branch'',''Delivery Type'',''Item Number'',''Item description'',''Home Phone'',''Work Phone'',''Mobile Phone'',''Account Number'',''Truck'',''Driver'',''Sales Person'',''Delivery or Collection Date'',''Address Notes'',''Number of units of the item'' union all select CAST(ID as varchar(10)),CustomerName, ShipAddress1,ShipAddress2,ShipAddress3,ZipCode,ShipCity,CAST(DeliveryBranch as varchar(5)),DeliveryType,Itemno,Itemdescription,HomePhone,WorkPhone,MobilePhone,Acctno,Truck,Driver,SalesPerson,DeliveryORCollectionDate,AddressNotes,CAST(QUANTITY as varchar(10))  from '
			+ @statement + '.dbo.' + '##DispatchTrackReport" queryout ' +  @filename + ' -t, -c -q -T ';	
	
	EXECUTE MASTER.DBO.XP_CMDSHELL @statement
	DROP TABLE ##DispatchTrackReport 
END TRY

BEGIN CATCH
    DECLARE @err_msg VARCHAR(MAX)                                
	SELECT @err_msg =                               
			   'Procedure ' + CONVERT(VARCHAR(50),ERROR_PROCEDURE()) +                                
			   ', Error ' + CONVERT(VARCHAR(50), ERROR_NUMBER()) +                                
			   ', Severity ' + CONVERT(VARCHAR(5), ERROR_SEVERITY()) +                                
			   ', State ' + CONVERT(VARCHAR(5), ERROR_STATE()) +                                 
			   ', Line ' + CONVERT(VARCHAR(5), ERROR_LINE()) +                                 
			   ', ErrorMessage ' +  CONVERT(VARCHAR(8000), ERROR_MESSAGE()) 
	RAISERROR (@err_msg, 16, 1); 
END CATCH

