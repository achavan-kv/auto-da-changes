UPDATE StoreCardPaymentDetails
SET StatementFrequency = NULL

ALTER TABLE StoreCardPaymentDetails
ALTER COLUMN StatementFrequency CHAR(1)