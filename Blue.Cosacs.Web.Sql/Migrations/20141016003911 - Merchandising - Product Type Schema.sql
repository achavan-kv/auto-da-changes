-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
CREATE TABLE [Merchandising].[ProductTypeSchema] (
	[Id] [int] NOT NULL IDENTITY(1,1),
	[ProductType] [varchar](100) NOT NULL,
	[FieldName] [varchar](100) NOT NULL,
	[DisplayName] [varchar](100) NOT NULL,
	CONSTRAINT [PK_ProductTypeSchema] PRIMARY KEY (Id ASC)
)
