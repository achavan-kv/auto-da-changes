SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_WarrantyBandUpdateSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_WarrantyBandUpdateSP]
GO

CREATE PROCEDURE 	dbo.DN_WarrantyBandUpdateSP
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : DN_WarrantyBandUpdateSP.prc
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Warranty Band Update
-- Author       : ??
-- Date         : ??
--
-- This procedure will update existing Warranty Bands or insert new Warranty Bands if they do not exist
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 31/08/11  IP  RI - #4690 - RI System Integration
-- ================================================
			--@itemno varchar(10),
			@itemno varchar(18),							--IP - 31/08/11 - RI - #4690
			@refcode varchar(3),
			@minprice money,
			@maxprice money,
			@length float,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

    UPDATE	warrantyband 
	SET		waritemno = @itemno,
    		refcode = @refcode,
    		minprice = @minprice,
    		maxprice = @maxprice,
    		warrantylength = @length
    WHERE	waritemno = @itemno
    AND refcode = @refcode
    --71862 RM 9/12/09 need to check refcode as well

	IF(@@rowcount = 0)
	BEGIN
		--INSERT INTO warrantyband(waritemno, refcode, minprice, maxprice, warrantylength)
		--VALUES(@itemno, @refcode, @minprice, @maxprice, @length)
		
		--IP - 31/08/11 - RI - #4690 - Below sql replaces the above.
		INSERT INTO warrantyband(waritemno, refcode, minprice, maxprice, warrantylength,ItemID)
		SELECT @itemno, @refcode, @minprice, @maxprice, @length,StockInfo.ID
		FROM StockInfo 
		WHERE StockInfo.itemno = @itemno
		AND StockInfo.RepossessedItem = 0
		
	END

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

