-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

--Create new table to hold working hours ber branch
CREATE TABLE [Admin].[BranchOpeningHours]
(
    branchNo SMALLINT NOT NULL PRIMARY KEY,
    MondayOpen TIME NULL,
    MondayClose TIME NULL,
    TuesdayOpen TIME NULL,
    TuesdayClose TIME NULL,
    WednesdayOpen TIME NULL,
    WednesdayClose TIME NULL,
    ThursdayOpen TIME NULL,
    ThursdayClose TIME NULL,
    FridayOpen TIME NULL,
    FridayClose TIME NULL,
    SaturdayOpen TIME NULL,
    SaturdayClose TIME NULL,
    SundayOpen TIME NULL,
    SundayClose TIME  NULL
);
GO

--Add foreign key to the branch table 
ALTER TABLE [Admin].[BranchOpeningHours]
ADD CONSTRAINT FK_BranchOpeningHours_Branchno FOREIGN KEY (branchNo)
    REFERENCES dbo.branch(branchno);
GO

--Populate branch numbers
INSERT INTO [Admin].[BranchOpeningHours](branchNo)
SELECT branchno FROM branch
GO