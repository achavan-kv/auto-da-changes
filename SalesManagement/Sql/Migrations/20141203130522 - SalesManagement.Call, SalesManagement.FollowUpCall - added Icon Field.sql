-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here


ALTER TABLE SalesManagement.Call
ADD Icon varchar(32) SPARSE NULL 

Go 

ALTER TABLE SalesManagement.FollowUpCall
ADD Icon varchar(32) SPARSE NULL 
