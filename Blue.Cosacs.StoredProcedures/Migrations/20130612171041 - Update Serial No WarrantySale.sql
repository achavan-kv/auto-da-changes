alter table warranty.warrantysale
add ItemSerialNumber varchar(50) null
GO

UPDATE w 
SET w.ItemSerialNumber = r.ItemSerialNumber
FROM warranty.Warrantysale w
inner join service.Request r on r.WarrantyContractId = w.WarrantyId 
                                and w.WarrantyContractNo = r.WarrantyContractNo 
								and w.SaleBranch = r.ItemStockLocation
where r.[ItemSerialNumber] is not null 
and len(ltrim(rtrim(r.[ItemSerialNumber]))) > 0

