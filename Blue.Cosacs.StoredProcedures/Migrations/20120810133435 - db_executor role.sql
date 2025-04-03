-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
/* CREATE A NEW ROLE */
CREATE ROLE db_executor;
go

/* GRANT EXECUTE TO THE ROLE */
GRANT EXECUTE TO db_executor