-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
TRUNCATE TABLE Financial.TransactionMappingService

INSERT INTO Financial.TransactionMappingService
(ChargeType, Label, ServiceType, IsExternal, Replacement, CashNotCredit, Department, Account, Percentage, UseValueColumn, TransactionType)
VALUES 
	('Supplier', 'Labour and Additional', '*', 0, NULL, NULL, '*', '1392', 1, 1, 'SRS'),
	('Supplier', 'Labour and Additional', '*', 0, NULL, NULL, '*', '9020S', -1, 1, 'SRS'),
	('Supplier', 'Labour', '*', 0, NULL, NULL, '*', '1392', 1, 1, 'SRS'),
	('Supplier', 'Labour', '*', 0, NULL, NULL, '*', '9020S', -1, 1, 'SRS'),
	('Supplier', 'Additional', '*', 0, NULL, NULL, '*', '1392', 1, 1, 'SRS'),
	('Supplier', 'Additional', '*', 0, NULL, NULL, '*', '9020S', -1, 1, 'SRS'),
	('Supplier', 'Labour and Additional', '*', 1, NULL, NULL, '*', '1392', 1, 0, 'SRS'),
	('Supplier', 'Labour and Additional', '*', 1, NULL, NULL, '*', '9020S', -1, 0, 'SRS'),
	('Supplier', 'Labour', '*', 1, NULL, NULL, '*', '1392', 1, 0, 'SRS'),
	('Supplier', 'Labour', '*', 1, NULL, NULL, '*', '9020S', -1, 0, 'SRS'),
	('Supplier', 'Additional', '*', 1, NULL, NULL, '*', '1392', 1, 0, 'SRS'),
	('Supplier', 'Additional', '*', 1, NULL, NULL, '*', '9020S', -1, 0, 'SRS'),
	('FYW', 'Labour and Additional', '*', 1, NULL, NULL, '*', '2930', 1, 1, 'SRY'),
	('FYW', 'Labour and Additional', '*', 1, NULL, NULL, '*', '9020I', -1, 1, 'SRY'),
	('FYW', 'Labour', '*', 1, NULL, NULL, '*', '2930', 1, 1, 'SRY'),
	('FYW', 'Labour', '*', 1, NULL, NULL, '*', '9020I', -1, 1, 'SRY'),
	('FYW', 'Additional', '*', 1, NULL, NULL, '*', '2930', 1, 1, 'SRY'),
	('FYW', 'Additional', '*', 1, NULL, NULL, '*', '9020I', -1, 1, 'SRY'),
	('FYW', 'Labour and Additional', '*', 0, NULL, NULL, '*', '2930', 1, 1, 'SRY'),
	('FYW', 'Labour and Additional', '*', 0, NULL, NULL, '*', '9020I', -1, 1, 'SRY'),
	('FYW', 'Labour', '*', 0, NULL, NULL, '*', '2930', 1, 1, 'SRY'),
	('FYW', 'Labour', '*', 0, NULL, NULL, '*', '9020I', -1, 1, 'SRY'),
	('FYW', 'Additional', '*', 0, NULL, NULL, '*', '2930', 1, 1, 'SRY'),
	('FYW', 'Additional', '*', 0, NULL, NULL, '*', '9020I', -1, 1, 'SRY'),
	('EW', 'Labour and Additional', '*', 0, NULL, NULL, '*', '2910', 1, 1, 'SRW'),
	('EW', 'Labour and Additional', '*', 0, NULL, NULL, '*', '9020E', -1, 1, 'SRW'),
	('EW', 'Labour', '*', 0, NULL, NULL, '*', '2910', 1, 1, 'SRW'),
	('EW', 'Labour', '*', 0, NULL, NULL, '*', '9020E', -1, 1, 'SRW'),
	('EW', 'Additional', '*', 0, NULL, NULL, '*', '2910', 1, 1, 'SRW'),
	('EW', 'Additional', '*', 0, NULL, NULL, '*', '9020E', -1, 1, 'SRW'),
	('EW', 'Labour and Additional', '*', 1, NULL, NULL, '*', '2910', 1, 1, 'SRW'),
	('EW', 'Labour and Additional', '*', 1, NULL, NULL, '*', '9020E', -1, 1, 'SRW'),
	('EW', 'Labour', '*', 1, NULL, NULL, '*', '2910', 1, 1, 'SRW'),
	('EW', 'Labour', '*', 1, NULL, NULL, '*', '9020E', -1, 1, 'SRW'),
	('EW', 'Additional', '*', 1, NULL, NULL, '*', '2910', 1, 1, 'SRW'),
	('EW', 'Additional', '*', 1, NULL, NULL, '*', '9020E', -1, 1, 'SRW'),
	('Internal', 'Labour and Additional', '*', NULL, NULL, NULL, '*', '9010', 1, 1, 'SRI'),
	('Internal', 'Labour and Additional', '*', NULL, NULL, NULL, '*', '9020B', -1, 1, 'SRI'),
	('Internal', 'Labour', '*', NULL, NULL, NULL, '*', '9010', 1, 1, 'SRI'),
	('Internal', 'Labour', '*', NULL, NULL, NULL, '*', '9020B', -1, 1, 'SRI'),
	('Internal', 'Additional', '*', NULL, NULL, NULL, '*', '9010', 1, 1, 'SRI'),
	('Internal', 'Additional', '*', NULL, NULL, NULL, '*', '9020B', -1, 1, 'SRI'),
	('Customer', 'Labour and Additional', 'IE', NULL, NULL, NULL, 'Electrical', '6093', 1, 1, 'INE'),
	('Customer', 'Labour and Additional', 'IE', NULL, NULL, NULL, 'Electrical', '5293', -1, 1, 'INE'),
	('Customer', 'Labour', 'IE', NULL, NULL, NULL, 'Electrical', '6093', 1, 1, 'INE'),
	('Customer', 'Labour', 'IE', NULL, NULL, NULL, 'Electrical', '5293', -1, 1, 'INE'),
	('Customer', 'Additional', 'IE', NULL, NULL, NULL, 'Electrical', '6093', 1, 1, 'INE'),
	('Customer', 'Additional', 'IE', NULL, NULL, NULL, 'Electrical', '5293', -1, 1, 'INE'),
	('Customer', 'Labour and Additional', 'IE', NULL, NULL, NULL, 'Furniture', '6093', 1, 1, 'INF'),
	('Customer', 'Labour and Additional', 'IE', NULL, NULL, NULL, 'Furniture', '5292', -1, 1, 'INF'),
	('Customer', 'Labour', 'IE', NULL, NULL, NULL, 'Furniture', '6093', 1, 1, 'INF'),
	('Customer', 'Labour', 'IE', NULL, NULL, NULL, 'Furniture', '5292', -1, 1, 'INF'),
	('Customer', 'Additional', 'IE', NULL, NULL, NULL, 'Furniture', '6093', 1, 1, 'INF'),
	('Customer', 'Additional', 'IE', NULL, NULL, NULL, 'Furniture', '5292', -1, 1, 'INF'),
	('Deliverer', 'Labour and Additional', '*', NULL, NULL, NULL, '*', '9010', 1, 1, 'SRI'),
	('Deliverer', 'Labour and Additional', '*', NULL, NULL, NULL, '*', '9020D', -1, 1, 'SRI'),
	('Deliverer', 'Labour', '*', NULL, NULL, NULL, '*', '9010', 1, 1, 'SRI'),
	('Deliverer', 'Labour', '*', NULL, NULL, NULL, '*', '9020D', -1, 1, 'SRI'),
	('Deliverer', 'Additional', '*', NULL, NULL, NULL, '*', '9010', 1, 1, 'SRI'),
	('Deliverer', 'Additional', '*', NULL, NULL, NULL, '*', '9020D', -1, 1, 'SRI')
