-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
-- Related to feature: #10902 - Warranty - Supplier Cost Matrix

IF NOT EXISTS(SELECT * FROM INFORMATION_SCHEMA.TABLES  WHERE TABLE_SCHEMA = 'Service' AND TABLE_NAME = 'SupplierCost')
BEGIN
	CREATE TABLE [Service].[SupplierCost]
	(
		Id int NOT NULL IDENTITY (1, 1),
		[Supplier] [varchar](30) NOT NULL,
		[Product] [varchar](30) NOT NULL,
		[Year] [smallint] NOT NULL,
		[PartType] [varchar](30) NOT NULL,
		[PartPercent] [money],
		[PartLimit] [money],
		[LabourPercent] [money],
		[LabourLimit] [money],
		[AdditionalPercent] [money],
		[AdditionalLimit] [money]
	)

	ALTER TABLE [Service].[SupplierCost] WITH CHECK ADD CONSTRAINT [Unq_SupplierCost] UNIQUE( [Supplier], [Product], [Year], [PartType])
	
	ALTER TABLE [Service].[SupplierCost] ADD  CONSTRAINT [pk_SupplierCost] PRIMARY KEY
	(	
		[Id] ASC
	)

END

IF NOT EXISTS(SELECT * FROM Config.picklist WHERE id = 'ServicePartMonth')
BEGIN
	
	INSERT INTO Config.PickList ( Id, Name )
	VALUES  ('ServicePartMonth', 'Service Part Month' )
	
	INSERT INTO Config.PickRow( ListId, [Order], String )
	VALUES ('ServicePartMonth', 1, '1 - 12'),
		   ('ServicePartMonth', 2, '13 - 24'),
		   ('ServicePartMonth', 3, '25 - 36'),
		   ('ServicePartMonth', 4, '37 - 48'),
		   ('ServicePartMonth', 5, '49 - 60')	

END

--Migrate existing Supplier Cost Matrix to the new table
INSERT INTO [service].SupplierCost
SELECT * FROM dbo.SR_PriceIndexMatrix p
WHERE NOT EXISTS(SELECT * FROM [service].SupplierCost s1
					WHERE s1.Supplier = p.Supplier
					AND s1.Product = p.Product
					AND s1.[Year] = p.[Year]
					AND s1.PartType = p.PartType)


