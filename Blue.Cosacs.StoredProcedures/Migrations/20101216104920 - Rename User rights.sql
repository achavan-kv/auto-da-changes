-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

update task set taskname='Non Stock Maintenance - View only'
where taskname='Non Stock Maintenance'

update task set taskname='Non Stock Maintenance'
where taskname='Non Stock Maintenance - Edit'

update task set taskname='Warranty Return Codes Maintenance - View only'
where taskname='Warranty Return Codes Maintenance'

update task set taskname='Warranty Return Codes Maintenance'
where taskname='Warranty Return Codes - Edit'

