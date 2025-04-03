-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
DELETE Financial.TransactionMappingWarranty

INSERT INTO Financial.TransactionMappingWarranty
	(AccountType, Department, Percentage, Cancelation, Repossession, CostOrSale, Account, TransactionType)
VALUES
	('Credit', 'PCE', 1, 0, 0, 'Sale', '1301', 'BHW'),
	('Credit', 'PCE', -1, 0, 0, 'Sale', '5112', 'BHW'),
	('*', 'PCE', 1, 0, 0, 'Cost', '6012', 'COW'),
	('*', 'PCE', -1, 0, 0, 'Cost', '2910', 'COW'),
	('Cash', 'PCE', 1, 0, 0, 'Sale', '1301', 'BCW'),
	('Cash', 'PCE', -1, 0, 0, 'Sale', '5212', 'BCW'),
	('Credit', 'PCF', 1, 0, 0, 'Sale', '1301', 'BHW'),
	('Credit', 'PCF', -1, 0, 0, 'Sale', '5182', 'BHW'),
	('*', 'PCF', 1, 0, 0, 'Cost', '6082', 'COW'),
	('*', 'PCF', -1, 0, 0, 'Cost', '2910', 'COW'),
	('Cash', 'PCF', 1, 0, 0, 'Sale', '1301', 'BCW'),
	('Cash', 'PCF', -1, 0, 0, 'Sale', '5282', 'BCW'),
	('Credit', 'PCE', 1, 1, 0, 'Sale', '5112', 'BHW'),
	('Credit', 'PCE', -1, 1, 0, 'Sale', '1301', 'BHW'),
	('*', 'PCE', 1, 1, 0, 'Cost', '2910', 'COW'),
	('*', 'PCE', -1, 1, 0, 'Cost', '6012', 'COW'),
	('Cash', 'PCE', 1, 1, 0, 'Sale', '5212', 'BCW'),
	('Cash', 'PCE', -1, 1, 0, 'Sale', '1301', 'BCW'),
	('Credit', 'PCF', 1, 1, 0, 'Sale', '5182', 'BHW'),
	('Credit', 'PCF', -1, 1, 0, 'Sale', '1301', 'BHW'),
	('*', 'PCF', 1, 1, 0, 'Cost', '2910', 'COW'),
	('*', 'PCF', -1, 1, 0, 'Cost', '6082', 'COW'),
	('Cash', 'PCF', 1, 1, 0, 'Sale', '5282', 'BCW'),
	('Cash', 'PCF', -1, 1, 0, 'Sale', '1301', 'BCW')