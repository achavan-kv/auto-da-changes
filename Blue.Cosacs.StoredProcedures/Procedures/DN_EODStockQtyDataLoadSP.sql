SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO
SET ANSI_WARNINGS ON
go

if exists (SELECT * FROM dbo.sysobjects WHERE id = object_id('[dbo].[DN_EODStockQtyDataLoadSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_EODStockQtyDataLoadSP]
GO

CREATE PROCEDURE DN_EODStockQtyDataLoadSP
        @return int OUTPUT

AS

    SET 	@return = 0			--initialise return code
    
	DECLARE @statement SQLText, @drive varchar(100)
      
	select @drive = value from CountryMaintenance where codename = 'systemdrive'

	IF EXISTS (SELECT Table_Name FROM INFORMATION_SCHEMA.tables
			   WHERE  Table_Name = 'temp_stockload')
	BEGIN
		DROP TABLE temp_stockload
	END	

	CREATE TABLE temp_stockload
	(
		warehouseno	varchar(4) default '' not null,
	    itemno varchar(10) default '' not null ,
	    stockfactavailable varchar(10) default '' not null,
	    stockactual varchar(10) default '' not null,
	    stockonorder varchar(10) default '' not null,
	    stocklastplanneddate varchar(22) default '' not null,
	    stockdamage varchar(10) default '' not null,
	    planneddate datetime default '' not null,
        branchno smallint default 0 not null,
        stockprocessed smallint default 0 not null
	)

	SET @statement = 
	'BULK INSERT temp_stockload
	FROM '''+@drive+'\bmsfpstk.dat''
	WITH (
	DATAFILETYPE = ''char'',
	FORMATFILE = '''+@drive+'\bcp_bmsfpstkin.fmt'') '

	print @statement
	exec sp_executesql @statement

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO
SET ANSI_WARNINGS Off
