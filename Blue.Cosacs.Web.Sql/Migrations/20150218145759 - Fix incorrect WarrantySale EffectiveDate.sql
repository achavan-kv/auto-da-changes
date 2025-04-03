-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here


select wsf.CustomerAccount, wsf.ItemId, wsf.WarrantyLength as 'FreeLength', wsf.EffectiveDate as 'FreeEffectiveDate',
        wse.WarrantyLength as 'ExtendedLength', wse.EffectiveDate as 'ExtendedEffectiveDate', wse.Id, dateadd(m, wsf.WarrantyLength, wsf.EffectiveDate) as 'CorrectEffectiveDate'
into #WarrantiesToFix
from 
    warranty.WarrantySale wsf
inner join 
    warranty.WarrantySale wse
    on wsf.CustomerAccount = wse.CustomerAccount
    and wsf.AgreementNumber = wse.AgreementNumber
    and wsf.ItemId = wse.ItemId
    and wsf.StockLocation = wse.StockLocation
    and wsf.WarrantyGroupId = wse.WarrantyGroupId
    and wsf.EffectiveDate = wse.EffectiveDate
where
    wsf.WarrantyType != wse.WarrantyType
    and wsf.WarrantyType = 'F'

update
    Warranty.WarrantySale 
set
    EffectiveDate = w.CorrectEffectiveDate
from
    Warranty.WarrantySale ws
inner join 
    #WarrantiesToFix w on ws.Id = w.Id


drop table #WarrantiesToFix
  
