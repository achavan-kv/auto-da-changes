IF NOT EXiSTS (SELECT *
FROM sys.indexes 
WHERE name='IDX_Financial_Transaction_MessageId')
BEGIN
CREATE NONCLUSTERED INDEX IDX_Financial_Transaction_MessageId
ON [Financial].[Transaction] ([MessageId])
INCLUDE ([Id])
END
GO

IF NOT EXiSTS (SELECT *
FROM sys.indexes 
WHERE name='IDX_Financial_WarrantyMessage')
CREATE NONCLUSTERED INDEX IDX_Financial_WarrantyMessage
ON [Financial].[WarrantyMessage] ([ContractNumber])
INCLUDE ([Id],[DeliveredOn],[AccountType],[Department],[SalePrice],[CostPrice],[BranchNo],[WarrantyNo],[WarrantyLength])
GO