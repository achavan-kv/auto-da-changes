-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF NOT EXISTS (SELECT * FROM syscolumns
			   WHERE name = 'PayMethod'
               AND OBJECT_NAME(id) = 'CashAndGoReceipt')
BEGIN
  ALTER TABLE CashAndGoReceipt ADD PayMethod INT not null default 0
END