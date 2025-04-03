-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here


CREATE TABLE [Warehouse].[Driver](
	[Id] INT IDENTITY(1,1) NOT NULL,
	[Name] VARCHAR(50) NOT NULL,
	[PhoneNumber] VARCHAR(30) NOT NULL
 CONSTRAINT [PK_Driver] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [Warehouse].[Truck] ADD DriverId INT NULL
GO

ALTER TABLE [Warehouse].[Truck]  WITH CHECK ADD  CONSTRAINT [FK_Truck_Driver] FOREIGN KEY([DriverId])
REFERENCES [Warehouse].[Driver] ([Id])
GO

ALTER TABLE [Warehouse].[Truck] CHECK CONSTRAINT [FK_Truck_Driver]
GO
