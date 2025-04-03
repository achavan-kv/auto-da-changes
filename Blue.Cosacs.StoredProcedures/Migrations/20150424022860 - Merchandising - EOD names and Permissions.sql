-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here


-- change the names of the EOD jobs in winforms as they are no longer for FACT2000
update code
set codedescript = 'Merchandising Sales Export'
where code = 'COS FACT'

update code
set codedescript = 'Product Import'
where code = 'FACTCOS'

update code
set reference = 1 
where code in ('CoSACS2RI', 'ECOMMERCE', 'RI2CoSACS')

--Remove permissions

delete from [admin].rolepermission
where permissionid in
(372 --winform product associations
,394 --winform online products/magento
,351 --winform non stock view
,352) --winform non stock edit

delete from [admin].permission
where id in 
(372 --winform product associations
,394 --winform online products/magento
,351 --winform non stock view
,352) --winform non stock edit


--Remove access to product specific tax
update codecat
set usermaint = 'N'
where category = 'TXR'

