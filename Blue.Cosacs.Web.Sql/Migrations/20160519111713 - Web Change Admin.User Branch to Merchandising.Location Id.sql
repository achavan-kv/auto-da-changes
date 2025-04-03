-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF NOT EXISTS(SELECT 1 FROM sys.columns WHERE name = 'BranchId' and OBJECT_ID = OBJECT_ID('Admin.User'))
    ALTER TABLE Admin.[User]
    ADD BranchId INT NULL
GO 

IF NOT EXISTS(SELECT TOP 1 'A' FROM Admin.[User] WHERE BranchId IS NOT NULL)
BEGIN
    IF OBJECT_ID('Merchandising.Location') IS NOT NULL AND EXISTS(SELECT 1 FROM Merchandising.Location)
    BEGIN
            UPDATE u
            SET BranchNo = ISNULL(l.SalesId, 0)
            FROM Admin.[User] u
            INNER JOIN Merchandising.Location l on u.BranchNo = l.Id

            UPDATE u
            SET BranchId = ISNULL(l.Id, 0)
            FROM Admin.[User] u
            INNER JOIN Merchandising.Location l on u.BranchNo = l.SalesId
    END
    ELSE
        UPDATE Admin.[User] SET BranchId = BranchNo

    ALTER TABLE Admin.[User]
    ALTER COLUMN BranchId INT NOT NULL
END