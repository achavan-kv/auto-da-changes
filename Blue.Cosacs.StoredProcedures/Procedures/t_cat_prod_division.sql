SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].t_cat_prod_division') 
            and OBJECTPROPERTY(id, N'IsProcedure') = 1)
BEGIN
    DROP PROCEDURE dbo.t_cat_prod_division
END
GO

CREATE PROCEDURE dbo.t_cat_prod_division

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
		
SET @filename = 't_cat_prod_division.txt'		
		
select @directory = value from CountryMaintenance where CodeName = 'systemdrive'
set @directory += '\' + @filename

SELECT @bcpPath = VALUE FROM dbo.CountryMaintenance WHERE CodeName = 'BCPpath'
--Testing because our BCP directory is different
--SET @bcpPath = 'C:\Program Files\Microsoft SQL Server\100\Tools\binn'
SET @bcpPath += '\BCP'

SET @bcpCommand = '"' + @bcpPath + '" ..##tempExport out ' + @directory + ' -w -t^| -Usa -P'			-- #12859

SELECT * INTO ##tempExport FROM
(
	select 'pdi_Division' AS A, 'pdi_Descripcion' AS B, 'pdi_Descripcion_EN' AS C
	union ALL
	SELECT DISTINCT ISNULL(category, ''), catdescript + '_sp', catdescript + '_en' 
	FROM dbo.codecat WHERE category IN ('PCE', 'PCF', 'PCO', 'PCDIS')
    UNION ALL
    SELECT 'Warranty ' + ISNULL(wt.Name, wl.Name),
           ISNULL(wt.Name, wl.Name) + '_sp',
           ISNULL(wt.Name, wl.Name) + '_en'
    FROM Warranty.[Level] wl
    LEFT JOIN Warranty.Tag wt
    ON wl.Id = wt.LevelId
    WHERE wl.Id = ISNULL(
                      (select id from Warranty.[Level] where name = 'Department'), 
                      (select min(id) from Warranty.[Level])
                     )
) AS tmp

EXEC master..xp_cmdshell @bcpCommand

DROP TABLE ##tempExport

GO