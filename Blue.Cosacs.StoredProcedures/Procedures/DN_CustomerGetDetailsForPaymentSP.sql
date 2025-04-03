-- =============================================
-- CREATED BY:	   Snehal Devadhe
-- CREATE DATE:	   17/07/2020
-- SCRIPT Name:    DN_CustomerGetDetailsForPaymentSP.sql
-- SCRIPT COMMENT: Added Latitude, Longitude columns to retrive data 
-- Discription:   Address Standardization CR 
-- =============================================
SET QUOTED_IDENTIFIER OFF
GO

SET ANSI_NULLS OFF
GO

IF EXISTS (
		SELECT 1
		FROM dbo.sysobjects
		WHERE id = object_id('[dbo].[DN_CustomerGetDetailsForPaymentSP]')
			AND OBJECTPROPERTY(id, 'IsProcedure') = 1
		)
	DROP PROCEDURE [dbo].[DN_CustomerGetDetailsForPaymentSP]
GO
-- =============================================
-- CREATED BY:	   Snehal Devadhe
-- CREATE DATE:	   17/07/2020
-- SCRIPT Name:    DN_CustomerGetDetailsForPaymentSP.sql
-- SCRIPT COMMENT: Added Latitude, Longitude columns to retrive data 
-- Discription:   Address Standardization CR 
-- Version	Date      By  Description
-- ----      --  -----------
-- 1.0			
-- 2.0		17/07/2020 Snehal Devdha	Included Latitude, Longitude columns of resultset to show on Payment screen.
-- =============================================
CREATE PROCEDURE dbo.DN_CustomerGetDetailsForPaymentSP @custid VARCHAR(20)
	,@return INT OUTPUT
AS
DECLARE @privClub INT
	,@privClubCode VARCHAR(4)
	,@privClubDesc VARCHAR(64)

BEGIN
	SET @return = 0
	SET @privClub = 0;
	SET @privClubCode = NULL
	SET @privClubDesc = ""

	EXEC DN_CustomerIsPrivilegeClubMemberSP @custid
		,@privClubCode OUT
		,@privClubDesc OUT
		,@return OUT

	IF (@privClubCode IS NOT NULL)
		SET @privClub = 1

	-- Retrieve Customer preferably with Home Address otherwise Postal Address
	-- but allow for no address record at all
	SELECT TOP 1 isnull(CA.cusaddr1, '') AS cusaddr1
		,isnull(CA.cusaddr2, '') AS cusaddr2
		,isnull(CA.cusaddr3, '') AS cusaddr3
		,isnull(CA.cuspocode, '') AS cuspocode
		,C.title AS 'Title'
		,C.firstname AS 'FirstName'
		,C.name AS 'LastName'
		,@privClub AS 'PrivilegeClub'
		,@privClubDesc AS 'PrivilegeClubDesc'
		,0 AS 'SundryCredit'
		,CA.Latitude AS 'Latitude'
		,CA.Longitude AS 'Longitude'
	FROM customer C
	LEFT OUTER JOIN custaddress CA ON CA.custid = C.custid
		AND CA.addtype IN (
			'H'
			,'P'
			)
		AND ca.datemoved IS NULL
	WHERE C.custid = @custid
	ORDER BY CA.addtype

	SET @return = @@error
END
GO

SET QUOTED_IDENTIFIER OFF
GO

SET ANSI_NULLS ON
GO


