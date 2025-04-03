SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].t_cat_productos') 
            and OBJECTPROPERTY(id, N'IsProcedure') = 1)
BEGIN
    DROP PROCEDURE dbo.t_cat_productos
END
GO

CREATE PROCEDURE dbo.t_cat_productos

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
		
SET @filename = 't_cat_productos.txt'		
		
select @directory = value from CountryMaintenance where CodeName = 'systemdrive'
set @directory += '\' + @filename

SELECT @bcpPath = VALUE FROM dbo.CountryMaintenance WHERE CodeName = 'BCPpath'
--Testing because our BCP directory is different
--SET @bcpPath = 'C:\Program Files\Microsoft SQL Server\100\Tools\binn'
SET @bcpPath += '\BCP'

SET @bcpCommand = '"' + @bcpPath + '" ..##tempExport out ' + @directory + ' -w -t^| -Usa -P'	

--create temp table with the top 3 levels ordered 
create table #levels
(
    id int,
    levelID int,
    name varchar(100)
)
insert into #levels
select 1, wl.Id, wl.Name
from Warranty.[Level] wl
where wl.Id = ISNULL(
                        (select id from Warranty.[Level] where name = 'Department'), 
                        (select min(id) from Warranty.[Level])
                    )
if exists (select 'a' from #levels where name = 'Category')
begin
    insert into #levels
    select 2, wl.Id, wl.Name
    from Warranty.[Level] wl
    where wl.Id = (select min(id) from Warranty.[Level] 
                                    where id not in (select levelId from #levels))
end 
else
begin
    insert into #levels
    select 2, wl.Id, wl.Name
    from Warranty.[Level] wl
    where wl.Id = ISNULL(
                            (select id from Warranty.[Level] where name = 'Category'), 
                            (select min(id) from Warranty.[Level] 
                                            where id not in (select levelId from #levels))
                        )
end
if exists (select 'a' from #levels where name = 'Class')
begin
    insert into #levels
    select 3, wl.Id, wl.Name
    from Warranty.[Level] wl
    where wl.Id = (select min(id) from Warranty.[Level] 
                                    where id not in (select levelId from #levels))
end 
else
begin
    insert into #levels
    select 3, wl.Id, wl.Name
    from Warranty.[Level] wl
    where wl.Id = ISNULL(
                            (select id from Warranty.[Level] where name = 'Class'), 
                            (select min(id) from Warranty.[Level] 
                                            where id not in (select levelId from #levels))
                        )
end

--Get data for the export
SELECT * INTO ##tempExport FROM
(
	select 'pdi_Division' AS A, 'pdp_Departamento' AS B, 'clp_ClaseProducto' AS C
	union ALL
	SELECT DISTINCT  ISNULL(CAST(c.category AS VARCHAR), ''), ISNULL(CAST(s.category AS VARCHAR), ''), ISNULL(s.Class, '')
	FROM dbo.code c INNER JOIN dbo.StockInfo s 
	ON c.code = s.category
	    AND c.category IN ('PCE', 'PCF', 'PCO', 'PCDIS')
        AND c.code not in (12, 82)
    UNION ALL
    SELECT 'Warranty ' + ISNULL(wt1.Name, wl1.Name),
           ISNULL(wt2.Name, wl2.Name),
           ISNULL(wt3.Name, wl3.Name)
    FROM Warranty.[Level] wl1
    LEFT JOIN Warranty.Tag wt1
    ON wl1.Id = wt1.LevelId
    ,Warranty.[Level] wl2
    LEFT JOIN Warranty.Tag wt2
    ON wl2.Id = wt2.LevelId
    ,Warranty.[Level] wl3
    LEFT JOIN Warranty.Tag wt3
    ON wl3.Id = wt3.LevelId
    WHERE wl1.Id = (select LevelId from #levels where id = 1)
        AND wl2.Id = (select LevelId from #levels where id = 2)
        AND wl3.Id = (select LevelId from #levels where id = 3)
) AS tmp

EXEC master..xp_cmdshell @bcpCommand

DROP TABLE ##tempExport
DROP TABLE #levels

GO