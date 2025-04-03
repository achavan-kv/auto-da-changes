-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

INSERT INTO Financial.TransactionMappingWarranty
 (AccountType, Department, Percentage, Cancelation, Repossession, CalculateAmount, CostOrSale, Account, TransactionType)
VALUES
    ('Credit', 'PCE', 1, 0, 0, 1, 'Sale', '1301', 'BHW'),
    ('Credit', 'PCE', -1, 0, 0, 1, 'Sale', '5112', 'BHW'),
    ('*', 'PCE', 1, 0, 0, 1, 'Cost', '6012', 'COW'),
    ('*', 'PCE', -1, 0, 0, 1, 'Cost', '2910', 'COW'),
    ('Cash', 'PCE', 1, 0, 0, 1, 'Sale', '1301', 'BCW'),
    ('Cash', 'PCE', -1, 0, 0, 1, 'Sale', '5212', 'BCW'),
    ('Credit', 'PCF', 1, 0, 0, 1, 'Sale', '1301', 'BHW'),
    ('Credit', 'PCF', -1, 0, 0, 1, 'Sale', '5182', 'BHW'),
    ('*', 'PCF', 1, 0, 0, 1, 'Cost', '6082', 'COW'),
    ('*', 'PCF', -1, 0, 0, 1, 'Cost', '2910', 'COW'),
    ('Cash', 'PCF', 1, 0, 0, 1, 'Sale', '1301', 'BCW'),
    ('Cash', 'PCF', -1, 0, 0, 1, 'Sale', '5282', 'BCW'),
    ('Credit', 'PCE', 1, 1, 0, 1, 'Sale', '5112', 'BHW'),
    ('Credit', 'PCE', -1, 1, 0, 1, 'Sale', '1301', 'BHW'),
    ('*', 'PCE', 1, 1, 0, 1, 'Cost', '2910', 'COW'),
    ('*', 'PCE', -1, 1, 0, 1, 'Cost', '6012', 'COW'),
    ('Cash', 'PCE', 1, 1, 0, 1, 'Sale', '5212', 'BCW'),
    ('Cash', 'PCE', -1, 1, 0, 1, 'Sale', '1301', 'BCW'),
    ('Credit', 'PCF', 1, 1, 0, 1, 'Sale', '5182', 'BHW'),
    ('Credit', 'PCF', -1, 1, 0, 1, 'Sale', '1301', 'BHW'),
    ('*', 'PCF', 1, 1, 0, 1, 'Cost', '2910', 'COW'),
    ('*', 'PCF', -1, 1, 0, 1, 'Cost', '6082', 'COW'),
    ('Cash', 'PCF', 1, 1, 0, 1, 'Sale', '5282', 'BCW'),
    ('Cash', 'PCF', -1, 1, 0, 1, 'Sale', '1301', 'BCW'),
    ('Credit', 'PCE', 1, 1, 1, 0, 'Sale', '5112', 'REP'),
    ('Credit', 'PCE', -1, 1, 1, 0, 'Sale', '1301', 'REP'),
    ('*', 'PCE', 1, 1, 1, 1, 'Cost', '2910', 'COW'),
    ('*', 'PCE', -1, 1, 1, 1, 'Cost', '6012', 'COW'),
    ('Credit', 'PCE', 1, 1, 1, 1, 'Sale', '1301', 'CRE'),
    ('Credit', 'PCE', -1, 1, 1, 1, 'Sale', '5112', 'CRE'),
    ('Cash', 'PCE', 1, 1, 1, 1, 'Sale', '5212', 'REP'),
    ('Cash', 'PCE', -1, 1, 1, 1, 'Sale', '1301', 'REP'),
    ('Cash', 'PCE', 1, 1, 1, 1, 'Sale', '1301', 'CRE'),
    ('Cash', 'PCE', -1, 1, 1, 1, 'Sale', '5212', 'CRE'),
    ('Credit', 'PCF', 1, 1, 1, 0, 'Sale', '5182', 'REP'),
    ('Credit', 'PCF', -1, 1, 1, 0, 'Sale', '1301', 'REP'),
    ('*', 'PCF', 1, 1, 1, 1, 'Cost', '2910', 'COW'),
    ('*', 'PCF', -1, 1, 1, 1, 'Cost', '6082', 'COW'),
    ('Credit', 'PCF', 1, 1, 1, 1, 'Sale', '1301', 'CRF'),
    ('Credit', 'PCF', -1, 1, 1, 1, 'Sale', '5182', 'CRF'),
    ('Cash', 'PCF', 1, 1, 1, 1, 'Sale', '5282', 'REP'),
    ('Cash', 'PCF', -1, 1, 1, 1, 'Sale', '1301', 'REP'),
    ('Cash', 'PCF', 1, 1, 1, 1, 'Sale', '1301', 'CRF'),
    ('Cash', 'PCF', -1, 1, 1, 1, 'Sale', '5282', 'CRF')