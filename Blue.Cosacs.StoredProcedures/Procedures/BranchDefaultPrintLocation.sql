IF EXISTS (select * from dbo.sysobjects where id = object_id(N'[dbo].[BranchDefaultPrintLocation]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[BranchDefaultPrintLocation]
GO

GO
CREATE PROCEDURE [dbo].[BranchDefaultPrintLocation]
AS
SELECT branchno, defaultprintlocation FROM branch

GO