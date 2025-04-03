-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
ALTER TABLE dbo.proposal ALTER COLUMN TransportType VARCHAR(4)
ALTER TABLE dbo.proposal ALTER COLUMN EducationLevel VARCHAR(4)
ALTER TABLE dbo.proposal ALTER COLUMN Industry VARCHAR(4)
ALTER TABLE dbo.proposal ALTER COLUMN Jobtitle VARCHAR(4)
ALTER TABLE dbo.proposal ALTER COLUMN Organisation VARCHAR(4)
