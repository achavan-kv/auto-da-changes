SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO
SET ANSI_WARNINGS ON
go

if exists (SELECT * FROM dbo.sysobjects WHERE id = object_id('[dbo].[DN_EODAssocProductDataLoadSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_EODAssocProductDataLoadSP]
GO

CREATE PROCEDURE DN_EODAssocProductDataLoadSP
        @return int OUTPUT

AS

    SET 	@return = 0			--initialise return code
       
	IF EXISTS (SELECT Table_Name FROM INFORMATION_SCHEMA.tables
			   WHERE  Table_Name = 'temp_associatedload')
	BEGIN
	    DROP TABLE temp_associatedload
	END    
	
    CREATE TABLE temp_associatedload
	(
		productcategory 	varchar(10) default '' not null,
		associatedcategory	varchar(10) default '' not null
	)

    BULK INSERT temp_associatedload
    FROM 'd:\users\default\bmsfaprd.DAT'
    WITH (
    DATAFILETYPE = 'char',
    FORMATFILE = 'd:\users\default\bcp_bmsfcaprd.fmt' )
    

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO
SET ANSI_WARNINGS Off
