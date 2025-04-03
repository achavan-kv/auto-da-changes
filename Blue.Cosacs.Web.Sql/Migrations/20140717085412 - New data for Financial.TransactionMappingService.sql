-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
TRUNCATE TABLE Financial.TransactionMappingService

INSERT INTO Financial.TransactionMappingService
(ChargeType, Label, ServiceType, IsExternal, Replacement, CashNotCredit, Department, Account, Percentage, UseValueColumn, TransactionType)
VALUES 
	('Supplier', '*', '*', NULL, NULL, NULL, '*', '1392', 1, 1, 'SRS'),
	('Supplier', '*', '*', NULL, NULL, NULL, '*', '9020S', -1, 1, 'SRS'),
	('FYW', '*', '*', NULL, NULL, NULL, '*', '2930', 1, 1, 'SRY'),
	('FYW', '*', '*', NULL, NULL, NULL, '*', '9020I', -1, 1, 'SRY'),
	('EW', '*', '*', NULL, NULL, NULL, '*', '2910', 1, 1, 'SRW'),
	('EW', '*', '*', NULL, NULL, NULL, '*', '9020E', -1, 1, 'SRW'),
	('Internal', '*', '*', NULL, NULL, NULL, '*', '9010', 1, 1, 'SRI'),
	('Internal', '*', '*', NULL, NULL, NULL, '*', '9020B', -1, 1, 'SRI'),
	('Customer', '*', 'II', NULL, NULL, 1, 'Electrical', '1301', 1, 1, 'PAY'),
	('Customer', '*', 'II', NULL, NULL, 1, 'Electrical', '5193', -1, 1, 'PAY'),
	('Customer', '*', 'II', NULL, NULL, 1, 'Furniture', '1301', 1, 1, 'PAY'),
	('Customer', '*', 'II', NULL, NULL, 1, 'Furniture', '5192', -1, 1, 'PAY'),
	('Customer', '*', 'II', NULL, NULL, 0, 'Electrical', '1301', 1, 1, 'PAY'),
	('Customer', '*', 'II', NULL, NULL, 0, 'Electrical', '5293', -1, 1, 'PAY'),
	('Customer', '*', 'II', NULL, NULL, 0, 'Furniture', '1301', 1, 1, 'PAY'),
	('Customer', '*', 'II', NULL, NULL, 0, 'Furniture', '5292', -1, 1, 'PAY'),
	('Customer', '*', 'IE', NULL, NULL, 1, 'Electrical', '1301', 1, 1, 'PAY'),
	('Customer', '*', 'IE', NULL, NULL, 1, 'Electrical', '5193', -1, 1, 'PAY'),
	('Customer', '*', 'IE', NULL, NULL, 1, 'Furniture', '1301', 1, 1, 'PAY'),
	('Customer', '*', 'IE', NULL, NULL, 1, 'Furniture', '5192', -1, 1, 'PAY'),
	('Customer', '*', 'IE', NULL, NULL, 0, 'Electrical', '1301', 1, 1, 'PAY'),
	('Customer', '*', 'IE', NULL, NULL, 0, 'Electrical', '5293', -1, 1, 'PAY'),
	('Customer', '*', 'IE', NULL, NULL, 0, 'Furniture', '1301', 1, 1, 'PAY'),
	('Customer', '*', 'IE', NULL, NULL, 0, 'Furniture', '5292', -1, 1, 'PAY'),
	('Customer', '*', 'SI', NULL, NULL, NULL, '*', '1301', 1, 1, 'PAY'),
	('Customer', '*', 'SI', NULL, NULL, NULL, '*', '9020C', -1, 1, 'PAY'),
	('Customer', '*', 'SE', NULL, NULL, NULL, '*', '1301', 1, 1, 'PAY'),
	('Customer', '*', 'SE', NULL, NULL, NULL, '*', '9020C', -1, 1, 'PAY'),
	('Installation Charge Electrical', '*', '*', NULL, NULL, 0, '*', '6093', 1, 1, 'INE'),
	('Installation Charge Electrical', '*', '*', NULL, NULL, 0, '*', '6293', -1, 1, 'INE'),
	('Installation Charge Electrical', '*', '*', NULL, NULL, 1, '*', '6093', 1, 1, 'INE'),
	('Installation Charge Electrical', '*', '*', NULL, NULL, 1, '*', '6193', -1, 1, 'INE'),
	('Installation Charge Furniture', '*', '*', NULL, NULL, 0, '*', '6092', 1, 1, 'INF'),
	('Installation Charge Furniture', '*', '*', NULL, NULL, 0, '*', '6292', -1, 1, 'INF'),
	('Installation Charge Furniture', '*', '*', NULL, NULL, 1, '*', '6092', 1, 1, 'INF'),
	('Installation Charge Furniture', '*', '*', NULL, NULL, 1, '*', '6192', -1, 1, 'INF'),
	('Deliverer', '*', '*', NULL, NULL, NULL, '*', '1393', 1, 1, 'PAY'),
	('Deliverer', '*', '*', NULL, NULL, NULL, '*', '9020D', -1, 1, 'PAY')
