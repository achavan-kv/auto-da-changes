-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
DROP TABLE Financial.TransactionMappingWarranty
GO 

CREATE TABLE Financial.TransactionMappingWarranty
(
	Id					int IDENTITY(1,1)	NOT NULL,
	AccountType			varchar(6)			NOT NULL,
	Department			char(3)				NOT NULL,
	Percentage			decimal(3, 2)		NOT NULL,
	Cancelation			bit					NOT NULL,
	Repossession		Bit					NOT NULL,
	CostOrSale			varchar(4)			NOT NULL,
	Account				varchar(4)			NOT NULL,
	TransactionType		char(3)				NOT NULL,

	CONSTRAINT PK_TransactionMappingWarranty PRIMARY KEY CLUSTERED 
	(
		Id ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
) 
GO
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
	('*', 'PCE', 1, 1, 0, 'Cost', '2910', 'COW'),
	('*', 'PCE', -1, 1, 0, 'Cost', '6012', 'COW'),
	('Credit', 'PCF', 1, 1, 0, 'Sale', '5182', 'BHW'),
	('Credit', 'PCF', -1, 1, 0, 'Sale', '1301', 'BHW'),
	('*', 'PCF', 1, 1, 0, 'Cost', '2910', 'COW'),
	('*', 'PCF', -1, 1, 0, 'Cost', '6082', 'COW'),
	('Cash', 'PCF', 1, 1, 0, 'Sale', '5282', 'BCW'),
	('Cash', 'PCF', -1, 1, 0, 'Sale', '1301', 'BCW'),
	('*', 'PCF', 1, 1, 0, 'Cost', '2910', 'COW'),
	('*', 'PCF', -1, 1, 0, 'Cost', '6082', 'COW'),
	('Credit', 'PCE', 1, 0, 1, 'Sale', '5112', 'REP'),
	('Credit', 'PCE', -1, 0, 1, 'Sale', '1301', 'REP'),
	('*', 'PCE', 1, 0, 1, 'Cost', '2910', 'COW'),
	('*', 'PCE', -1, 0, 1, 'Cost', '6012', 'COW'),
	('Credit', 'PCE', 1, 0, 1, 'Sale', '1301', 'CRE'),
	('Credit', 'PCE', -1, 0, 1, 'Sale', '5112', 'CRE'),
	('Cash', 'PCE', 1, 0, 1, 'Sale', '5212', 'REP'),
	('Cash', 'PCE', -1, 0, 1, 'Sale', '1301', 'REP'),
	('*', 'PCE', 1, 0, 1, 'Cost', '2910', 'COW'),
	('*', 'PCE', -1, 0, 1, 'Cost', '6012', 'COW'),
	('Cash', 'PCE', 1, 0, 1, 'Sale', '1301', 'CRE'),
	('Cash', 'PCE', -1, 0, 1, 'Sale', '5212', 'CRE'),
	('Credit', 'PCF', 1, 0, 1, 'Sale', '5182', 'REP'),
	('Credit', 'PCF', -1, 0, 1, 'Sale', '1301', 'REP'),
	('*', 'PCF', 1, 0, 1, 'Cost', '2910', 'COW'),
	('*', 'PCF', -1, 0, 1, 'Cost', '6082', 'COW'),
	('Credit', 'PCF', 1, 0, 1, 'Sale', '1301', 'CRF'),
	('Credit', 'PCF', -1, 0, 1, 'Sale', '5182', 'CRF'),
	('Cash', 'PCF', 1, 0, 1, 'Sale', '5282', 'REP'),
	('Cash', 'PCF', -1, 0, 1, 'Sale', '1301', 'REP'),
	('*', 'PCF', 1, 0, 1, 'Cost', '2910', 'COW'),
	('*', 'PCF', -1, 0, 1, 'Cost', '6082', 'COW'),
	('Cash', 'PCF', 1, 0, 1, 'Sale', '1301', 'CRF'),
	('Cash', 'PCF', -1, 0, 1, 'Sale', '5282', 'CRF')