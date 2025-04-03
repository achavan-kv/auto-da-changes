SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO
SET ANSI_WARNINGS ON
go

if exists (SELECT * FROM dbo.sysobjects WHERE id = object_id('[dbo].[DN_EODPromoDataLoadSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_EODPromoDataLoadSP]
GO

CREATE PROCEDURE DN_EODPromoDataLoadSP
        @return int OUTPUT

AS

    SET 	@return = 0			--initialise return code
       
	IF EXISTS (SELECT Table_Name FROM INFORMATION_SCHEMA.tables
			   WHERE  Table_Name = 'temp_rawpromoload')
	BEGIN
		DROP TABLE temp_rawpromoload
	END	
	
	CREATE TABLE temp_rawpromoload
	(
	    itemno          varchar(10)    default ''  not null,
	    warehouseno     varchar(4)     default ''  not null,
	    pricehp1        varchar(17)    default ''  not null,
	    datefromhp1     varchar(22)    default ''  not null,
	    datetohp1       varchar(22)    default ''  not null,
	    pricehp2        varchar(17)    default ''  not null,
	    datefromhp2     varchar(22)    default ''  not null,
	    datetohp2       varchar(22)    default ''  not null,
	    pricehp3        varchar(17)    default ''  not null,
	    datefromhp3     varchar(22)    default ''  not null,
	    datetohp3       varchar(22)    default ''  not null,
	    pricecash1      varchar(17)    default ''  not null,
	    datefromcash1   varchar(22)    default ''  not null,
	    datetocash1     varchar(22)    default ''  not null,
	    pricecash2      varchar(17)    default ''  not null,
	    datefromcash2   varchar(22)    default ''  not null,
	    datetocash2     varchar(22)    default ''  not null,
	    pricecash3      varchar(17)    default ''  not null,
	    datefromcash3   varchar(22)    default ''  not null,
	    datetocash3     varchar(22)    default ''  not null,
		PromotionId     varchar(20)     default ''  not null,
		branchno        smallint       default 0   not null
	
	)

	DECLARE @statement SQLText, @drive varchar(100)
      
	select @drive = value from CountryMaintenance where codename = 'systemdrive'

	SET @statement = 
	'BULK INSERT temp_rawpromoload
	FROM '''+@drive+'\bmsfcprm.dat''
	WITH (
	DATAFILETYPE = ''char'',
	FORMATFILE = '''+@drive+'\bcp_bmsfcprm.fmt'') '

   EXEC sp_executesql @statement

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO
SET ANSI_WARNINGS Off
