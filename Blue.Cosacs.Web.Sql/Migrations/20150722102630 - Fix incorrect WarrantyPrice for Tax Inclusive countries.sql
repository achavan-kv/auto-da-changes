-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here


     declare @taxrate float, @stockTaxType char(1)

     select @taxrate = value from CountryMaintenance where CodeName = 'TaxRate'
     select @stockTaxType = value from CountryMaintenance where CodeName = 'taxtype'

     IF(@stockTaxType = 'I')
     BEGIN

        UPDATE
            Warranty.WarrantyPrice
        SET
            RetailPrice = FixPrice.CorrectPrice
        FROM
            Warranty.WarrantyPrice wp
        INNER JOIN 
        (
            SELECT 
                wp.Id, si.id as StockId, w.Number, w.Id as WarrantyId, wp.RetailPrice, Round((wp.RetailPrice * (100 + @taxrate) / 100),2) as CorrectPrice
            FROM 
                warranty.warrantyprice wp
            INNER JOIN 
                warranty.warranty w on wp.warrantyid = w.id
            INNER JOIN
                stockinfo si on w.Number = si.itemno
            WHERE EXISTS(select * from StockPrice sp
                            where sp.ID = si.Id
                            and sp.CashPrice != wp.RetailPrice)
                AND w.taxrate = 0

        ) FixPrice on FixPrice.Id = wp.Id  

     END
