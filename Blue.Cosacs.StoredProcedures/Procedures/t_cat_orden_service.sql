SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].t_cat_orden_service') 
            and OBJECTPROPERTY(id, N'IsProcedure') = 1)
BEGIN
    DROP PROCEDURE dbo.t_cat_orden_service
END
GO

CREATE PROCEDURE dbo.t_cat_orden_service

-- =============================================
-- Author:		Iliya Zanev
-- Create date: 17 August 2012
-- Description:	Hyperion Extract files
--
-- 12/04/13 jec #12859 - UAT12602 
-- =============================================

AS

DECLARE @directory VARCHAR(100),
		@filename VARCHAR(50),
		@bcpCommand VARCHAR(5000),
		@bcpPath VARCHAR(100)
		
SET @filename = 't_cat_orden_service.txt'		
		
select @directory = value from CountryMaintenance where CodeName = 'systemdrive'
set @directory += '\' + @filename

SELECT @bcpPath = VALUE FROM dbo.CountryMaintenance WHERE CodeName = 'BCPpath'
--Testing because our BCP directory is different
--SET @bcpPath = 'C:\Program Files\Microsoft SQL Server\100\Tools\binn'
SET @bcpPath += '\BCP'

SET @bcpCommand = '"' + @bcpPath + '" ..##tempExport out ' + @directory + ' -w -t^| -Usa -P'			-- #12859

--Create temporary table to get all the current Charge To types
create table #temp2
(
    remainderText varchar(max),
    id int identity(1,1)
)

--insert the initial record holding the complete string replacing the new line chanracters
insert into #temp2
select REPLACE(REPLACE(cast(valuetext as varchar(max)), CHAR(13),'¬'), CHAR(10), '¬')
from Config.Setting
where [Namespace] = 'Blue.Cosacs.Service'
    AND [Id] = 'ServiceChargeTo' 

--loop through to extract all single charge to types
while (1 = 1)
begin 

    if (select CHARINDEX('¬', remainderText) from #temp2 where id = (select max(id) from #temp2)) != 0

        insert into #temp2
        select right(remainderText, len(remainderText) - CHARINDEX('¬', remainderText)) as remainderText
        from #temp2
        where id = (select max(id) from #temp2)
    else 
        break

end

create table #tempExp
(
    tos_TipoOrdenService varchar(max),
    tos_Descripcion varchar(max),
    id int identity(1,1)
)

insert into #tempExp
	select 'tos_TipoOrdenService' AS A, 'tos_Descripcion' AS B

insert into #tempExp
	select distinct [type], [type] 
    from [Service].[Charge]
    UNION
    select 
        case 
            when id < (select max(id) from #temp2) THEN LTRIM(RTRIM(SUBSTRING(remainderText, 0, CHARINDEX('¬', remainderText))))
            ELSE LTRIM(RTRIM(remainderText))
        END,
        case 
            when id < (select max(id) from #temp2) THEN LTRIM(RTRIM(SUBSTRING(remainderText, 0, CHARINDEX('¬', remainderText))))
            ELSE LTRIM(RTRIM(remainderText))
        END
from #temp2 

--#tempExp because of ordering
SELECT * INTO ##tempExport FROM
(
	select top 100 percent tos_TipoOrdenService, tos_Descripcion 
    from #tempExp
    order by id
) AS tmp

EXEC master..xp_cmdshell @bcpCommand

DROP TABLE ##tempExport
drop table #temp2
drop table #tempExp
GO
