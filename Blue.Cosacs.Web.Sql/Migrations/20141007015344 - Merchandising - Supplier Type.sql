-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
CREATE TABLE [Merchandising].[SupplierType](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](100) NOT NULL,
	[Description] [varchar](100) NOT NULL,
 CONSTRAINT [pk_suppliertype] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

INSERT INTO Merchandising.SupplierType
	VALUES ('Local', 'Local')
INSERT INTO Merchandising.SupplierType
	VALUES ('Direct Import', 'Direct Import')
INSERT INTO Merchandising.SupplierType
	VALUES ('Broker Import (RWT)', 'Broker Import (RWT)')