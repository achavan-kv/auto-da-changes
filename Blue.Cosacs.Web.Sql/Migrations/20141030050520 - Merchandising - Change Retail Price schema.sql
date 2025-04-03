-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
alter table [merchandising].[retailprice] drop column includestax
alter table [merchandising].[retailprice] add Fascia varchar(100) NULL
alter table [merchandising].[retailprice] alter column LocationId int NULL