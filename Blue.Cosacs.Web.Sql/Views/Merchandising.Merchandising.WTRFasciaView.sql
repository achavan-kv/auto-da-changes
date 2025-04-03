IF EXISTS(SELECT * FROM sys.views where object_id = OBJECT_ID(N'[Merchandising].[Merchandising.WTRFasciaView]'))
    DROP VIEW [Merchandising].[Merchandising.WTRFasciaView]
GO

CREATE VIEW [Merchandising].[Merchandising.WTRFasciaView]
AS

    SELECT 'Courts' as FasciaName, 1 as FasciaId
    UNION
    SELECT 'Courts Optical' as FasciaName, 2 as FasciaId
    UNION
    SELECT 'Lucky Dollar' as FasciaName, 3 as FasciaId
    UNION
    SELECT 'Tropigas' as FasciaName, 4 as FasciaId
    UNION
    SELECT 'Radio Shack' as FasciaName, 5 as FasciaId
	UNION
	SELECT 'Ashley Home Store' as FasciaName, 6 as FasciaId
	UNION
	SELECT 'Ready Cash' as FasciaName, 7 as FasciaId
	UNION
	SELECT 'AMC Unicon' as FasciaName, 8 as FasciaId
	UNION
	SELECT 'OMNI' as FasciaName, 9 as FasciaId
GO