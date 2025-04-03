CREATE NONCLUSTERED INDEX IX_WarrantySale_CustomerAccount_ItemId_WarrantyContractNo ON Warranty.WarrantySale
(
	CustomerAccount,
	ItemId,
	WarrantyContractNo
) 
 WITH
 ( 
     STATISTICS_NORECOMPUTE = OFF, 
     IGNORE_DUP_KEY = OFF, 
     ALLOW_ROW_LOCKS = ON, 
     ALLOW_PAGE_LOCKS = ON
) ON [PRIMARY]