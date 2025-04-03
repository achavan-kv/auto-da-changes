-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

CREATE TABLE [Merchandising].[Location] (
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[LocationId] [varchar](100) NOT NULL,
	[Name] [varchar](100) NOT NULL,
	[Fascia] [varchar](100) NOT NULL,
	[StoreType] [varchar](100) NOT NULL,
	[Warehouse] [bit] NOT NULL,
	[AddressLine1] [varchar](100) NOT NULL,
	[AddressLine2] [varchar](100) NULL,
	[City] [varchar](100) NOT NULL,
	[PostCode] [varchar](100) NOT NULL,
	[Contacts] [varchar](max) NULL,
 CONSTRAINT [PK_Merchandising.Location] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]