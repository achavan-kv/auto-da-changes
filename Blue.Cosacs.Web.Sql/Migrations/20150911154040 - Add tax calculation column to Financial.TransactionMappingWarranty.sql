-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF NOT EXISTS ( SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
                WHERE TABLE_SCHEMA='Financial' AND Table_Name='TransactionMappingWarranty' AND Column_Name='CalculateTax')
    ALTER TABLE Financial.TransactionMappingWarranty
    ADD CalculateTax BIT NULL
