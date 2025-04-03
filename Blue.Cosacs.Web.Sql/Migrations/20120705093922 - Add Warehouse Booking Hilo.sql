-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here


IF NOT EXISTS(select * from HiLo where sequence = 'warehouse.booking')
BEGIN
 INSERT INTO HiLo
 SELECT 'warehouse.booking', 1, 100
END