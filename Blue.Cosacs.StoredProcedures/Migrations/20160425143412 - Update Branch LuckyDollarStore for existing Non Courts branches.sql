-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here


update 
    branch
set
    LuckyDollarStore = 1
where
    StoreType = 'N'
    and branchname NOT LIKE '%Ashley%'


update 
    branch
set 
    AshleyStore = 1
where
    StoreType = 'N'
    and branchname LIKE '%Ashley%'






