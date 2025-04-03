SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO
SET ANSI_WARNINGS ON
go

if exists (SELECT * FROM dbo.sysobjects WHERE id = object_id('[dbo].[DN_EODAssociatedProductDataLoadSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_EODAssociatedProductDataLoadSP]
GO

CREATE PROCEDURE DN_EODAssociatedProductDataLoadSP
        @source varchar(20),
        @return int OUTPUT

AS

	DECLARE @statement SQLText, @drive varchar(100)
	select @drive = value from CountryMaintenance where codename = 'systemdrive'
	SET 	@return = 0			--initialise return code

    IF(@source = 'NonStocks')
    BEGIN
        IF EXISTS (SELECT Table_Name FROM INFORMATION_SCHEMA.tables
			   WHERE  Table_Name = 'temp_nonstockassociatedload')
	    BEGIN
	        DROP TABLE temp_nonstockassociatedload
	    END    
	
        CREATE TABLE temp_nonstockassociatedload
	    (
		    productgroup 	    varchar(5)  default '' not null,
		    category	        varchar(5)  default '' not null,
            class	            varchar(5)  default '' not null,
            subclass	        varchar(5)  default '' not null,
            associtemid	        varchar(18) default '' not null
	    )

        SET @statement =
        'BULK INSERT temp_nonstockassociatedload
        FROM '''+@drive+'\nonstocks_prodAssoc.DAT''
        WITH (
        DATAFILETYPE = ''char'',
        FORMATFILE = '''+@drive+'\nonstocks_prodAssoc.fmt'') '	

        EXEC sp_executesql @statement
		SET @return = @@error
    END

	IF(@source = 'Merchandising')
    BEGIN
        IF EXISTS (SELECT Table_Name FROM INFORMATION_SCHEMA.tables
			   WHERE  Table_Name = 'temp_MerchandisingAssociatedLoad')
	    BEGIN
	        DROP TABLE temp_MerchandisingAssociatedLoad
	    END    
	
        CREATE TABLE temp_MerchandisingAssociatedLoad
	    (
		    division 	    varchar(100),
		    department	        varchar(5) ,
            class	            varchar(5),
            subclass	        varchar(5),
            sku	        varchar(18) not null
	    )


		SET @statement = 
		'BULK INSERT temp_MerchandisingAssociatedLoad
		FROM '''+@drive+'\bmsfaprd.dat''
		WITH (
		DATAFILETYPE = ''char'',
		FORMATFILE = '''+@drive+'\bcp_bmsfaprd.fmt'') '
		
		EXEC sp_executesql @statement
		SET @return = @@error
    END

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO
SET ANSI_WARNINGS Off
