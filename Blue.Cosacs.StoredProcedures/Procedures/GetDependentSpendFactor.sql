

 GO

IF EXISTS(
            SELECT 1
            FROM   sys.procedures WITH (NOLOCK)
            WHERE  NAME = 'GetDependentSpendFactor'
                        AND type = 'P'
        )
BEGIN
              DROP PROCEDURE [dbo].[GetDependentSpendFactor]
END
GO


SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


-- =======================================================================================
-- Project			: CoSaCS.NET
-- PROCEDURE Name   : GetDependentSpendFactor
-- Author			: SHUBHAM GAIKWAD
-- Create Date		: 28 July 2020
-- Description		: This procedure is used to get dependent spend factor. 

-- Change Control
-- --------------
-- Date			By			Description
-- ----			--			-----------
-- 
-- =======================================================================================

CREATE PROCEDURE [dbo].[GetDependentSpendFactor] 
	@return INT OUT
AS
BEGIN
SET @return=0;
DECLARE @CNTOfDep varchar='0'


SELECT @CNTOfDep = count(distinct NumOfDependents) from [DependentSpendFactor] where ISActive=1


SELECT DISTINCT TOP  (cast(@CNTOfDep AS INT)) id, NumOfDependents, DependnetSpendFactorInPercent 
	INTO ##DependentSpendFactorTemp 
	FROM [DependentSpendFactor] WHERE ISActive=1 ORDER BY id DESC


ALTER TABLE ##DependentSpendFactorTemp ADD ISBASEOFNEXT BIT
ALTER TABLE ##DependentSpendFactorTemp ADD OrgNumOfDep varchar(3)

UPDATE T
SET T.ISBASEOFNEXT = D.ISBASEOFNEXT,
	T.OrgNumOfDep =D.OrgNumOfDep
	from ##DependentSpendFactorTemp T 
	INNER JOIN DependentSpendFactor D
	ON T.Id = D.id

SELECT id, NumOfDependents, DependnetSpendFactorInPercent from ##DependentSpendFactorTemp where ISBASEOFNEXT=0 
UNION 
Select id, NumOfDependents, DependnetSpendFactorInPercent from ##DependentSpendFactorTemp where 
OrgNumOfDep in (Select Min(distinct OrgNumOfDep) from DependentSpendFactor where IsBAseofNExt=1 and ISActive=1)

IF OBJECT_ID('tempdb..##DependentSpendFactorTemp') IS NOT NULL
		DROP TABLE ##DependentSpendFactorTemp

END 

GO
