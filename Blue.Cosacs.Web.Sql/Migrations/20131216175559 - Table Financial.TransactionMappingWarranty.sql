-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
IF OBJECT_ID('Financial.TransactionMappingWarranty') IS NOT NULL
	DROP TABLE Financial.TransactionMappingWarranty
GO

CREATE TABLE Financial.TransactionMappingWarranty
(
	Id				Int IDENTITY (1, 1) NOT NULL,
	AccountType		VarChar(6)			NOT NULL,
	Department		Char(3)				NOT NULL,
	Percentage		Decimal(3,2)		NOT NULL,
	Cancelation		Bit					NOT NULL,
	Account			VarChar(35)			NOT NULL,
	TransactionType	Char(3)				NOT NULL 
)
GO

ALTER TABLE Financial.TransactionMappingWarranty ADD  CONSTRAINT PK_TransactionMappingWarranty PRIMARY KEY CLUSTERED 
(
	Id ASC
)WITH 
	(
		PAD_INDEX = OFF, 
		STATISTICS_NORECOMPUTE = OFF, 
		IGNORE_DUP_KEY = OFF, 
		ALLOW_ROW_LOCKS = ON, 
		ALLOW_PAGE_LOCKS = ON
) 
GO

INSERT INTO Financial.TransactionMappingWarranty
	(AccountType, Department, Percentage, Cancelation, Account, TransactionType)
VALUES
	('BHW', 'PCE', 1, 0, '1301', 'BHW'),
	('BHW', 'PCE', -1, 0, '5112', 'BHW'),
	('BHW', 'PCE', 1, 0, '6012', 'COW'),
	('BHW', 'PCE', -1, 0, '2910', 'COW'),
	('BCW', 'PCE', 1, 0, '1301', 'BCW'),
	('BCW', 'PCE', -1, 0, '5212', 'BCW'),
	('BCW', 'PCE', 1, 0, '6012', 'COW'),
	('BCW', 'PCE', -1, 0, '2910', 'COW'),
	('BCW', 'PCF', 1, 0, '1301', 'BCW'),
	('BCW', 'PCF', -1, 0, '5282 ', 'BCW'),
	('BCW', 'PCF', 1, 0, '6082', 'COW'),
	('BCW', 'PCF', -1, 0, '2910', 'COW'),
	('BHW', 'PCF', 1, 0, '1301', 'BHW'),
	('BHW', 'PCF', -1, 0, '5182', 'BHW'),
	('BHW', 'PCF', 1, 0, '6082', 'COW'),
	('BHW', 'PCF', -1, 0, '2910', 'COW'),
	('BHW', 'PCE', -1, 1, '1301', 'CRE'),
	('BHW', 'PCE', 1, 1, '5112', 'CRE'),
	('BHW', 'PCE', -1, 1, '6012', 'COW'),
	('BHW', 'PCE', 1, 1, '2910', 'COW'),
	('BCW', 'PCE', -1, 1, '1301', 'CRE'),
	('BCW', 'PCE', 1, 1, '5212', 'CRE'),
	('BCW', 'PCE', -1, 1, '6012', 'COW'),
	('BCW', 'PCE', 1, 1, '2910', 'COW'),
	('BCW', 'PCF', -1, 1, '1301', 'CRF'),
	('BCW', 'PCF', 1, 1, '5282 ', 'CRF'),
	('BCW', 'PCF', -1, 1, '6082', 'COW'),
	('BCW', 'PCF', 1, 1, '2910', 'COW'),
	('BHW', 'PCF', -1, 1, '1301', 'CRF'),
	('BHW', 'PCF', 1, 1, '5182', 'CRF'),
	('BHW', 'PCF', -1, 1, '6082', 'COW'),
	('BHW', 'PCF', 1, 1, '2910', 'COW')
