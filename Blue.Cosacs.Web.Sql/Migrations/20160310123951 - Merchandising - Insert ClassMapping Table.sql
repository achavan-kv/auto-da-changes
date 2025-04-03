-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF EXISTS (SELECT 1 FROM sys.views WHERE object_id = OBJECT_ID(N'[Merchandising].[DepartmentMapping]'))
	DROP TABLE [Merchandising].[DepartmentMapping]
GO

IF NOT EXISTS (SELECT 1 FROM sys.views WHERE object_id = OBJECT_ID(N'[Merchandising].[ClassMapping]'))
	CREATE TABLE [Merchandising].[ClassMapping]
	(
		[Id] INT IDENTITY(1,1) NOT NULL,
		[ClassCode] VARCHAR(10) NOT NULL,
		[LegacyCode] VARCHAR(3) NOT NULL,
	 	
	 	CONSTRAINT [PK_Merchandising_ClassMapping] PRIMARY KEY CLUSTERED ( 
	 		[Id] ASC 
	 	)

	 	WITH (
	 		PAD_INDEX = OFF, 
	 		STATISTICS_NORECOMPUTE = OFF, 
	 		IGNORE_DUP_KEY = OFF, 
	 		ALLOW_ROW_LOCKS = ON, 
	 		ALLOW_PAGE_LOCKS = ON
	 	) ON [PRIMARY]
	) ON [PRIMARY]
GO