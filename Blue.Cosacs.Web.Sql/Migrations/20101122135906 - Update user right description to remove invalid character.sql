-- put your SQL code here

IF EXISTS (SELECT * FROM Task WHERE taskname = 'Immediate Delivery û Notify Immediate Delivery Failed')
update Task
SET taskname = 'Immediate Delivery - Notify Immediate Delivery Failed'
WHERE taskname = 'Immediate Delivery û Notify Immediate Delivery Failed'