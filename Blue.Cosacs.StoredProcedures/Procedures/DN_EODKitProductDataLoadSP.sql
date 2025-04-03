SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO
SET ANSI_WARNINGS ON
go

if exists (SELECT * FROM dbo.sysobjects WHERE id = object_id('[dbo].[DN_EODKitProductDataLoadSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_EODKitProductDataLoadSP]
GO

CREATE PROCEDURE DN_EODKitProductDataLoadSP
        @return int OUTPUT
AS
BEGIN
    SET 	@return = 0			--initialise return code
   DECLARE @statement SQLText, @drive varchar(100)
      
	IF EXISTS (SELECT Table_Name FROM INFORMATION_SCHEMA.tables
			   WHERE  Table_Name = 'temp_rawkitload')
	BEGIN
		DROP TABLE temp_rawkitload
	END	
	
	CREATE TABLE temp_rawkitload
	(
		itemno	 	 	varchar(10) default '' not null,
		componentno 	varchar(10) default '' 	not null,
		componentqty 	varchar(5) default '' 	not null
	)

	select @drive = value from CountryMaintenance where codename = 'systemdrive'

	SET @statement = 
	'BULK INSERT temp_rawkitload
	FROM '''+@drive+'\bmsfckit.dat''
	WITH (
	DATAFILETYPE = ''char'',
	FORMATFILE = '''+@drive+'\bcp_bmsfckitin.fmt'') '

   EXEC sp_executesql @statement

	IF EXISTS (SELECT Table_Name FROM INFORMATION_SCHEMA.tables
			   WHERE  Table_Name = 'temp_kitload')
	BEGIN
		DROP TABLE temp_kitload
	END	
	
	SELECT DISTINCT itemno,
    				componentno,
    				componentqty,
				    convert(smallint,0) as itexists,
				    convert(smallint,0) as cpexists,
				    convert(smallint,0) as rowprocessed
	INTO	temp_kitload
    FROM  	temp_rawkitload
END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO
SET ANSI_WARNINGS Off

GO
