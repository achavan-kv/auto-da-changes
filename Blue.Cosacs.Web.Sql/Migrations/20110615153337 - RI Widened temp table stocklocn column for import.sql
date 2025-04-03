-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

ALTER TABLE RItemp_RawPOload
ALTER COLUMN StockLocn VARCHAR(5) NOT NULL

ALTER TABLE RItemp_RawProductload
ALTER COLUMN BranchNo VARCHAR(5) NULL

ALTER TABLE RItemp_RawProductLoadRepo
ALTER COLUMN BranchNo VARCHAR(5) NULL

ALTER TABLE RItemp_RawStkQtyload
ALTER COLUMN StockLocn VARCHAR(5) NOT NULL 

ALTER TABLE RItemp_RawStkQtyloadRepo
ALTER COLUMN StockLocn VARCHAR(5) NOT NULL
