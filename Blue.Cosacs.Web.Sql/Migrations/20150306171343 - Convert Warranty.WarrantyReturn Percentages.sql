-- transaction: true

-- get all old warranty return percentages
select
    WarrantyMonths AS WarrantyLength,
    MonthSinceDelivery AS ElapsedMonths,
    refundpercentfromAIG AS PercentageReturn,
    CASE
        WHEN ProductType='E' THEN 'PCE'
        WHEN ProductType='F' THEN 'PCF'
        ELSE ProductType
    END Level_1,
    ManufacturerMonths AS FreePeriod
into #OldWarrantyReturnCodes
from WarrantyReturnCodes
order by ProductType, WarrantyMonths, MonthSinceDelivery


-- update WarrantyLength
update Warranty.WarrantyReturn
set WarrantyLength = c.WarrantyLength - c.FreePeriod
from Warranty.WarrantyReturn wr
inner join #OldWarrantyReturnCodes c
    on wr.WarrantyLength = c.WarrantyLength
    and wr.ElapsedMonths = c.ElapsedMonths
    and wr.PercentageReturn = c.PercentageReturn
    and wr.Level_1 = c.Level_1


-- drop temp table
drop table #OldWarrantyReturnCodes
