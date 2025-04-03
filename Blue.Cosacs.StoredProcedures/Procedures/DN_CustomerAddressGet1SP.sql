IF EXISTS (
		SELECT 1
		FROM sys.objects
		WHERE object_id = OBJECT_ID(N'[dbo].[DN_CustomerAddressGet1SP]')
			AND type IN (
				N'P'
				,N'PC'
				)
		)
	DROP PROCEDURE [dbo].[DN_CustomerAddressGet1SP]
GO
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : DN_CustomerAddressGet1SP.prc
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Get Customer Addresses and Phone numbers
-- Author       : ??
-- Date         : ??
--
--
-- Change Control
-- --------------
-- Version	Date      By  Description
-- ----      --  -----------
-- 1.0		20/08/10  jec CR1084 Return address Zone
-- 2.0		11/06/18   km CR	    Displaying delivery Persons Title,FirstName,LastName in address   
-- 3.0		6/3/2020				Address Standardization CR Latitude and Longitude columns added for Address Standardization CR2019 - 025
-- 4.0		06/23/2020  Snehalata Tilekar - Add DataLegnth and DataType instead of Empty AddressType In select Query for Address Standardization CR2019 - 025
-- 5.0		07/20/2020	Sachin Wandhare		Included correct Category column value instead of Hard code CT1 to allow Address Type removal from system. 
-- ================================================
CREATE PROCEDURE [dbo].[DN_CustomerAddressGet1SP]
	@custid VARCHAR(20)
	,@return INT OUTPUT
AS
BEGIN
	SET NOCOUNT ON
	SET @return = 0 --initialise return code  

	SELECT custid AS 'CustomerID'
		,CAST(addtype AS CHAR(3)) AS 'AddressType'
		,-- Address Standardization CR2019 - 025
		cusaddr1 AS 'Address1'
		,cusaddr2 AS 'Address2'
		,cusaddr3 AS 'Address3'
		,cuspocode AS 'PostCode'
		,ISNULL(DeliveryArea, '') AS DeliveryArea
		,email AS 'Email'
		,datein AS 'Date In'
		,CONVERT(VARCHAR(8), '') AS 'DialCode'
		,--dial code  
		CONVERT(VARCHAR(20), '') AS 'Phone'
		,--telno  
		CONVERT(VARCHAR(6), '') AS 'Ext'
		,--ext  
		CONVERT(VARCHAR(300), notes) AS 'Notes'
		,codedescript AS 'AddressDescription'
		,code.category
		,ISNULL(zone, '') AS Zone
		,--CR1084 jec
		DELTitleC AS 'DELTitleC'
		,DELFirstName AS 'DELFirstName'
		,DELLastname AS 'DELLastname'
	INTO #address
	FROM custaddress
	LEFT JOIN code ON custaddress.addtype = code.code
		AND category = 'CA1'
	WHERE custid = @custid
		AND datemoved IS NULL --Get current addresses only
		AND addtype != '' --LW71247 must have an address type 

	INSERT INTO #address
	SELECT custid AS 'CustomerID'
		,CAST(tellocn AS CHAR(3)) AS 'AddressType'
		,-- Address Standardization CR2019 - 025
		'' AS 'Address1'
		,'' AS 'Address2'
		,'' AS 'Address3'
		,'' AS 'PostCode'
		,'' AS 'DeliveryArea'
		,'' AS 'Email'
		,dateteladd AS 'Date In'
		,--will always be null for a record from the custtel table  
		DialCode
		,--dial code  
		ISNULL(telno, '') AS 'Phone'
		,--telno  
		extnno AS 'Ext'
		,--ext  
		'' AS 'Notes'
		,codedescript AS 'AddressDescription'
		,code.category
		,'' AS zone
		,--CR1084 jec 
		'' AS 'DELTitleC'
		,'' AS 'DELFirstName'
		,'' AS 'DELLastname'
	FROM custtel
	LEFT JOIN code ON custtel.tellocn = code.code
		AND category = 'CT1'
	WHERE custid = @custid
		AND datediscon IS NULL --Get current telephone numbers only 
		AND tellocn != '' -- LW71247 must have an address type  
	ORDER BY 'AddressType'

	SELECT C.CustomerID
		,C.AddressType
		,C.Address1
		,C.Address2
		,C.Address3
		,C.PostCode
		,C.DeliveryArea
		,C.Email
		,C.[Date In]
		,C.DialCode
		,C.Phone
		,C.Ext
		,C.Notes
		,C.AddressDescription
		,C.Category
		,C.Zone
		,--CR1084 jec 
		C.DELTitleC
		,C.DELFirstName
		,C.DELLastname
		,A.Latitude
		,A.Longitude
	FROM (
		SELECT CustomerID
			,MAX(AddressType) AS AddressType
			,MAX(Address1) AS Address1
			,MAX(Address2) AS Address2
			,MAX(Address3) AS Address3
			,MAX(PostCode) AS PostCode
			,MAX(DeliveryArea) AS DeliveryArea
			,MAX(Email) AS Email
			,MAX([Date In]) AS 'Date In'
			,MAX(DialCode) AS DialCode
			,MAX(Phone) AS Phone
			,MAX(Ext) AS Ext
			,MAX(Notes) AS Notes
			,MAX(AddressDescription) AS AddressDescription
			,MIN(category) AS Category
			,MAX(Zone) AS Zone
			,--CR1084 jec 
			MAX(DELTitleC) AS DELTitleC
			,MAX(DELFirstName) AS DELFirstName
			,MAX(DELLastname) AS DELLastname
		FROM #address
		GROUP BY CustomerID
			,AddressType
		) C
	LEFT JOIN custaddress A ON C.CustomerID = A.custid
		AND C.AddressType = A.addtype AND A.datemoved IS NULL

	IF (@@ERROR != 0)
	BEGIN
		SET @return = @@ERROR
	END
END
GO