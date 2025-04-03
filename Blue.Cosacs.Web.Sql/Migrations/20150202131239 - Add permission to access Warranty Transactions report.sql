-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF NOT EXISTS(select * from admin.[permission] where id = 2024)
BEGIN

    insert into 
        admin.[permission]
    select
        2024, 'Report - Broker Warranty Transactions', 20, 'Broker Warranty Transactions Report'

END