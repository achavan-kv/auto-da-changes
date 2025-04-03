SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO
SET ANSI_WARNINGS ON
go

if exists (SELECT * FROM dbo.sysobjects WHERE id = object_id('[dbo].[DN_EODAssocProductImportSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_EODAssocProductImportSP]
GO

CREATE PROCEDURE DN_EODAssocProductImportSP
        @return int OUTPUT

AS

    SET 	@return = 0			--initialise return code
       
    DELETE FROM StockItemAssociated

	SET @return = @@error

	IF(@return = 0)
	BEGIN
	    INSERT INTO StockItemAssociated
	    (
	     	AssociatedItemCategory,
			ProductCategory    
	    )
	    SELECT AssociatedCategory,
	           ProductCategory
	    FROM   temp_associatedload

		SET @return = @@error
	END


	--remove carriage return, line feed, and tab 
   UPDATE dbo.stockitemassociated
   SET associateditemcategory=REPLACE(REPLACE(REPLACE(associateditemcategory, CHAR(10), ''), CHAR(13), ''), CHAR(9), '')


GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO
SET ANSI_WARNINGS Off
