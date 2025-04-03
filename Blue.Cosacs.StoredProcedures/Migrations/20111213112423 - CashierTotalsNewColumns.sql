-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
IF EXISTS (SELECT * FROM sysobjects WHERE NAME= 'PK_CashierTotals')
ALTER TABLE CashierTotals DROP CONSTRAINT PK_CashierTotals

ALTER TABLE CashierTotals
ADD CONSTRAINT PK_cashiertotals PRIMARY KEY  (ID) 
GO 
IF NOT EXISTS (SELECT * FROM sysobjects WHERE NAME='fk_cashiertotalsId')
ALTER TABLE CashierTotalsBreakdown
ADD CONSTRAINT fk_cashiertotalsId FOREIGN KEY  (cashiertotalid) 
REFERENCES CashierTotals(id) 

GO 

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE table_name ='CashierTotalsBreakdown'
AND column_name = 'Payments' ) 
ALTER TABLE CashierTotalsBreakdown ADD Payments MONEY,

Corrections MONEY,
Refunds MONEY,
PettyCash MONEY,
Remittances MONEY,
Allocations MONEY,
Disbursements MONEY
GO 
 