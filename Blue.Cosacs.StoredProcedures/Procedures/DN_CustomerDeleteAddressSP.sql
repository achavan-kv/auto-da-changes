SET QUOTED_IDENTIFIER OFF
GO

SET ANSI_NULLS OFF
GO

IF EXISTS (
		SELECT 1
		FROM dbo.sysobjects
		WHERE id = object_id('[dbo].[DN_CustomerDeleteAddressSP]')
			AND OBJECTPROPERTY(id, 'IsProcedure') = 1
		)
	DROP PROCEDURE [dbo].[DN_CustomerDeleteAddressSP]
GO

-- ================================================
-- Project      : CoSACS .NET
-- File Name    : DN_CustomerDeleteAddressSP.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Soft Delete Customer Addresses and Phone numbers for Category
-- Change Control
-- --------------
-- Version	Date      By  Description
-- ----      --  -----------
-- 1.0			
-- 2.0		07/20/2020	Sachin Wandhare		Included additional update for Category CA1 and CT1 to soft delete address Phone for 'CA1' and for mobile 'CT1' category. 
-- ================================================
CREATE PROCEDURE dbo.DN_CustomerDeleteAddressSP @custid VARCHAR(20)
	,@addressType CHAR(3)
	,@category VARCHAR(12)
	,@return INT OUTPUT
AS
BEGIN
	SET NOCOUNT ON
	SET @return = 0 --initialise return code

	IF (@category = 'CA1')
	BEGIN
		UPDATE custaddress
		SET datemoved = getdate()
		WHERE custid = @custid
			AND addtype = @addressType
			AND ISNULL(datemoved, '') = ''

		UPDATE custtel -- Address Standardization CR 25
		SET datediscon = getdate(),
			dateteladd = DATEADD(SECOND,DATEDIFF(SECOND, dateteladd, GETDATE()),dateteladd) -- Address Standardization CR 25
		WHERE custid = @custid
			AND tellocn = @addressType
			AND ISNULL(datediscon, '') = ''
	END

	IF (@category = 'CT1')
	BEGIN
		UPDATE custtel
		SET datediscon = getdate(),
			dateteladd = DATEADD(SECOND,DATEDIFF(SECOND, dateteladd, GETDATE()),dateteladd) -- Address Standardization CR 25
		WHERE custid = @custid
			AND tellocn = @addressType
			AND ISNULL(datediscon, '') = ''
	END

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
END
