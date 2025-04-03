-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- Asked for two -- give her two!!!
-- Put your SQL code here

ALTER TABLE storecard ADD MonthlyAmount DECIMAL,
 PaymentMethod VARCHAR(4), PaymentOption VARCHAR(4)
