-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
CREATE TABLE [Merchandising].[RepossessedConditions] (
	Id [int] NOT NULL IDENTITY(1,1),
	Name varchar(100) NOT NULL,
	SKUSuffix varchar(10) NOT NULL,
	CONSTRAINT [PK_RepossessedConditions] PRIMARY KEY (Id ASC))