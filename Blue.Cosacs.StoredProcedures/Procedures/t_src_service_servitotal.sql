SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].t_src_service_servitotal') 
            and OBJECTPROPERTY(id, N'IsProcedure') = 1)
BEGIN
    DROP PROCEDURE dbo.t_src_service_servitotal
END
GO

-- =============================================
-- Author:		Iliya Zanev
-- Create date: 17 August 2012
-- Description:	Hyperion Extract files
--
-- 12/04/13 jec #12859 - UAT12602 
-- =============================================

CREATE PROCEDURE dbo.t_src_service_servitotal
(
	@dateFrom DATETIME
	, @dateTo DATETIME
)

AS

DECLARE @directory VARCHAR(100),
		@filename VARCHAR(50),
		@bcpCommand VARCHAR(5000),
		@bcpPath VARCHAR(100)
		
SET @filename = 't_src_service_servitotal.txt'		
		
select @directory = value from CountryMaintenance where CodeName = 'systemdrive'
set @directory += '\' + @filename

SELECT @bcpPath = VALUE FROM dbo.CountryMaintenance WHERE CodeName = 'BCPpath'
--Testing because our BCP directory is different
--SET @bcpPath = 'C:\Program Files\Microsoft SQL Server\110\Tools\Binn'
SET @bcpPath += '\BCP'

SET @bcpCommand = '"' + @bcpPath + '" ..##tempExport out ' + @directory + ' -w -t^| -Usa -P'			-- #12859

SELECT * INTO ##tempExport FROM
(
	SELECT 'ent_Entidad' AS ent_Entidad,
           'cda_CDA' AS cda_CDA, 
           'svc_CDC' AS svc_CDC, 
           'svc_NumOrden' AS svc_NumOrden, 
           'svc_Articulo' AS svc_Articulo, 
           'svc_CalAnio' AS svc_CalAnio, 
           'svc_Periodo' AS svc_Periodo, 
           'tos_TipoOrdenService' AS tos_TipoOrdenService, 
           'svc_Taller' AS svc_Taller, 
		   'svc_Tipo' AS svc_Tipo, 
           'svc_Marca' AS svc_Marca, 
           'svc_Modelo' AS svc_Modelo, 
           'svc_Costo_ManoObra' AS svc_Costo_ManoObra,
		   'svc_Costo_Repuesto' AS svc_Costo_Repuesto, 
           'svc_Costo_Transporte' AS svc_Costo_Transporte, 
           'svc_Costo_Miscelaneo' AS svc_Costo_Miscelaneo, 
           'svc_Costo_Total' AS svc_Costo_Total, 
           'svc_FechaActualizacion' AS svc_FechaActualizacion 
	UNION ALL
	SELECT ISNULL(LTRIM(RTRIM(c.ISOCountryCode)) + LTRIM(RTRIM(b.StoreType)), ' ') AS ent_Entidad
		   , CAST(r.Branch AS VARCHAR) as 'cda_CDA'
		   , '0000' as 'svc_CDC'
		   , CAST(r.Id AS VARCHAR) as 'svc_NumOrden'
		   , ISNULL(s.itemdescr1, 'External Item') as 'svc_Articulo'
		   , CAST(ISNULL(DATEPART(YEAR, r.ResolutionDate), '0') AS VARCHAR) as 'svc_CalAnio'
		   , CAST(ISNULL(LEFT(DATENAME(MONTH, r.ResolutionDate), 3), '0') AS VARCHAR) as 'svc_Periodo'
		   , r.ResolutionPrimaryCharge as 'tos_TipoOrdenService'
		   , CAST(r.Branch AS VARCHAR) 'svc_Taller'
		   , ' ' as 'svc_Tipo'
		   , ISNULL(Case patIndex ('%[ /-]%', LTrim (s.itemdescr1))
			            When 0 Then LTrim (s.itemdescr1)
			            Else substring (LTrim (s.itemdescr1), 1, patIndex ('%[ /-]%', LTrim (s.itemdescr1)) - 1) 
                    END, 'External Item') as 'svc_Marca'
		   , ' ' as 'svc_Modelo'
		   , CAST(r.ResolutionLabourCost AS VARCHAR) as 'svc_Costo_ManoObra'
		   , CAST((SELECT SUM(Quantity * Price) FROM [Service].[RequestPart] WHERE RequestId = r.Id) AS VARCHAR) as 'svc_Costo_Repuesto'
		   , CAST(r.ResolutionTransportCost AS VARCHAR) as 'svc_Costo_Transporte'
		   , CAST(r.ResolutionAdditionalCost AS VARCHAR) as 'svc_Costo_Miscelaneo'
		   , CAST((SELECT SUM(Value) FROM [Service].[Charge] WHERE RequestId = r.Id) AS VARCHAR) as 'svc_Costo_Total'
		   , CONVERT(varchar, getdate(), 112) as 'svc_Costo_Totalsvc_FechaActualizacion'
	FROM country c,
         [Service].[Request] r 
         LEFT OUTER JOIN dbo.StockInfo s 
	     ON r.ItemID = s.id
         INNER JOIN branch b
         ON r.Branch = b.branchno
    WHERE r.LastUpdatedOn >= @dateFrom
        AND r.LastUpdatedOn < @dateTo
        AND r.Resolution IS NOT NULL
        AND r.[State] = 'Closed'
        AND r.[Type] in ('SI', 'SE', 'S')
) AS tmp

EXEC master..xp_cmdshell @bcpCommand

DROP TABLE ##tempExport
GO		  
	