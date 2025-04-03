-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
alter table merchandising.goodsreceipt drop column status
alter table merchandising.goodsreceipt drop column processedby
alter table merchandising.goodsreceipt drop column processedbyid