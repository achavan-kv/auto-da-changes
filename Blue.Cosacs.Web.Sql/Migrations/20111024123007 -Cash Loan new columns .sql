-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
IF NOT EXISTS( SELECT * FROM INFORMATION_SCHEMA.columns WHERE column_name=  'EmpeenoDisburse' AND table_name ='CashLoan')
ALTER TABLE CashLoan ADD Termstype VARCHAR(2), EmpeenoAccept int, EmpeenoDisburse  INT 

