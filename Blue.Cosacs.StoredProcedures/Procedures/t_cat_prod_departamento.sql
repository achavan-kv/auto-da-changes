SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].t_cat_prod_departamento') 
            and OBJECTPROPERTY(id, N'IsProcedure') = 1)
BEGIN
    DROP PROCEDURE dbo.t_cat_prod_departamento
END
GO

CREATE PROCEDURE dbo.t_cat_prod_departamento

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
		
SET @filename = 't_cat_prod_departamento.txt'		
		
select @directory = value from CountryMaintenance where CodeName = 'systemdrive'
set @directory += '\' + @filename

SELECT @bcpPath = VALUE FROM dbo.CountryMaintenance WHERE CodeName = 'BCPpath'
--Testing because our BCP directory is different
--SET @bcpPath = 'C:\Program Files\Microsoft SQL Server\100\Tools\binn'
SET @bcpPath += '\BCP'

SET @bcpCommand = '"' + @bcpPath + '" ..##tempExport out ' + @directory + ' -w -t^| -Usa -P'			-- #12859

SELECT * INTO ##tempExport FROM
(
	select 'pdp_Departamento' AS A, 'pdp_Descripcion' AS B, 'pdp_Descripcion_EN' AS C
	union ALL
	SELECT DISTINCT ISNULL(CAST(s.category AS VARCHAR), ''), c.codedescript + '_sp', c.codedescript + '_en'
	FROM dbo.StockInfo s 
    INNER JOIN dbo.code c
	ON s.category = c.code
	WHERE s.itemtype = 'S'
        AND c.category IN ('PCE', 'PCF', 'PCO', 'PCW', 'PCDIS')
) AS tmp

EXEC master..xp_cmdshell @bcpCommand

DROP TABLE ##tempExport

GO