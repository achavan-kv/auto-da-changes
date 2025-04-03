IF NOT EXISTS (SELECT * FROM sys.columns WHERE  object_id = OBJECT_ID(N'[InvoiceDetails]') AND name = 'contractno')
BEGIN
	/*Add Column in InvoiceDetails Table */
	ALTER TABLE InvoiceDetails
	ADD contractno [varchar](10) NULL
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE  object_id = OBJECT_ID(N'[InvoiceDetails]') AND name = 'returnquantity')
BEGIN
	ALTER TABLE InvoiceDetails
	ADD returnquantity [int] NULL
END
	
IF NOT EXISTS (SELECT * FROM sys.columns WHERE  object_id = OBJECT_ID(N'[InvoiceDetails]') AND name = 'RetItemNo')
BEGIN
	ALTER TABLE InvoiceDetails
	ADD [RetItemNo] [varchar](18) NULL
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE  object_id = OBJECT_ID(N'[InvoiceDetails]') AND name = 'RetVal')
BEGIN
	ALTER TABLE InvoiceDetails
	ADD [RetVal] [float]  NULL
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE  object_id = OBJECT_ID(N'[InvoiceDetails]') AND name = 'LineItemID')
BEGIN
	ALTER TABLE InvoiceDetails
	ADD [LineItemID] [int]  NULL
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE  object_id = OBJECT_ID(N'[InvoiceDetails]') AND name = 'OrdVal')
BEGIN
	ALTER TABLE InvoiceDetails
	ADD [OrdVal] [money] NOT NULL DEFAULT(0)
END
