INSERT INTO Service.ServiceSupplier
        ( Supplier, Account )
SELECT codedescript,reference FROM dbo.code
WHERE category = 'SRSUPPLIER'

delete FROM Config.PickRow
WHERE ListId = 'ServiceSupplier'

delete FROM config.Picklist
WHERE id = 'ServiceSupplier'

