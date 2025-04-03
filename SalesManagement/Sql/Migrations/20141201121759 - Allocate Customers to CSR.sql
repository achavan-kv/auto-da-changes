-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

DELETE SalesManagement.Call
WHERE CustomerId NOT IN (SELECT CustomerId FROM SalesManagement.CustomerSalesPerson)

IF NOT EXISTS(SELECT 1 FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_NAME ='FK_Call_CustomerSalesPerson')
BEGIN 
	ALTER TABLE [SalesManagement].[Call]  WITH CHECK ADD  CONSTRAINT [FK_Call_CustomerSalesPerson] FOREIGN KEY([CustomerId])
	REFERENCES [SalesManagement].[CustomerSalesPerson] ([CustomerId])

	ALTER TABLE [SalesManagement].[Call] CHECK CONSTRAINT [FK_Call_CustomerSalesPerson]
END

IF EXISTS(SELECT 1 FROM sys.tables t WHERE t.name = 'CustomerContact')
	DROP TABLE CustomerContact
	
IF EXISTS(SELECT 1 FROM sys.tables t WHERE t.name = 'CustomerAccount')
	DROP TABLE CustomerAccount 
	
IF EXISTS(SELECT 1 FROM sys.tables t WHERE t.name = 'LineItemAuditTmp')
	DROP TABLE LineItemAuditTmp 