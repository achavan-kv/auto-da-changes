-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
ALTER TABLE [Merchandising].[Product] DROP CONSTRAINT [FK_Product_ProductType]

alter table merchandising.product
drop column producttypeid

drop table merchandising.producttype

alter table merchandising.product
add [ProductType] varchar(100) null