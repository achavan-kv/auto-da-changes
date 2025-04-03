SET QUOTED_IDENTIFIER OFF
GO

SET ANSI_NULLS OFF
GO

IF EXISTS (
		SELECT 1
		FROM dbo.sysobjects
		WHERE id = object_id('[dbo].[DN_CustomerAddressGetHomeSP]')
			AND OBJECTPROPERTY(id, 'IsProcedure') = 1
		)
	DROP PROCEDURE [dbo].[DN_CustomerAddressGetHomeSP]
GO
-- =============================================
-- Modified BY:	     Snehal Devadhe
-- Modified DATE:	 21/07/2020
-- SCRIPT Name:      DN_CustomerAddressGetHomeSP.sql
-- SCRIPT COMMENT:   Added Latitude, Longitude columns to retrive data 
-- Discription:      Address Standardization CR 
-- Change Control
-- --------------
-- Version	Date      By  Description
-- ----      --  -----------
-- 1.0
-- 2.0		20/08/10	Snehal Devadhe	Added Latitude, Longitude columns to retrive data 
-- =============================================
CREATE PROCEDURE dbo.DN_CustomerAddressGetHomeSP
	@custid VARCHAR(20)
	,@datein DATETIME OUT
	,@dateout DATETIME OUT
	,@resstatus CHAR(1) OUT
	,@proptype CHAR(4) OUT
	,@address1 CHAR(50) OUT
	,@address2 CHAR(50) OUT
	,@address3 CHAR(50) OUT
	,@postcode CHAR(10) OUT
	,@DeliveryArea VARCHAR(8) OUT
	,@pdatein DATETIME OUT
	,@pdateout DATETIME OUT
	,@presstatus CHAR(1) OUT
	,@pproptype CHAR(4) OUT
	,@mthlyrent FLOAT OUT
	,@paddress1 CHAR(50) OUT
	,@paddress2 CHAR(50) OUT
	,@paddress3 CHAR(50) OUT
	,@ppostcode CHAR(10) OUT
	,@pDeliveryArea VARCHAR(8) OUT
	,@Latitude FLOAT OUT
	,@Longitude FLOAT OUT
	,@return INT OUTPUT
AS
BEGIN
	SET NOCOUNT ON
	SET @return = 0 --initialise return code

	SELECT @datein = datein
		,@dateout = datemoved
		,@resstatus = resstatus
		,@proptype = proptype
		,@mthlyrent = mthlyrent
		,@address1 = cusaddr1
		,@address2 = cusaddr2
		,@address3 = cusaddr3
		,@postcode = cuspocode
		,@DeliveryArea = isnull(DeliveryArea, '')
		,@Latitude = Latitude
		,@Longitude = Longitude
	FROM custaddress
	WHERE custid = @custid
		AND addtype = 'H'
		AND datemoved IS NULL

	SELECT TOP 1 @pdatein = datein
		,@pdateout = datemoved
		,@presstatus = resstatus
		,@pproptype = proptype
		,@paddress1 = cusaddr1
		,@paddress2 = cusaddr2
		,@paddress3 = cusaddr3
		,@ppostcode = cuspocode
		,@pDeliveryArea = isnull(DeliveryArea, '')
		,@Latitude = Latitude
		,@Longitude = Longitude
	FROM custaddress
	WHERE custid = @custid
		AND addtype = 'H'
		AND datemoved IS NOT NULL
	ORDER BY datein DESC

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
END
GO

SET QUOTED_IDENTIFIER OFF
GO

SET ANSI_NULLS ON
GO


