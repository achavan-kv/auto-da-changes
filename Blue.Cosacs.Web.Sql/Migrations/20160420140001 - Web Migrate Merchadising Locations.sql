insert into Merchandising.Location (LocationId,
										Name,
										Fascia,
										StoreType,
										Warehouse,
										AddressLine1,
										AddressLine2,
										City,
										PostCode,
										Contacts,
										SalesId,
										Active,
										VirtualWarehouse)
    SELECT LocationId,
        Name,
        Fascia,
        StoreType,
        Warehouse,
        AddressLine1,
        AddressLine2,
        City,
        PostCode,
        Contacts,
        SalesId,
        Active,
        VirtualWarehouse
    FROM ( select
        b.branchno LocationId
        , b.branchname Name
        , 'Courts'   as Fascia
        , b.StoreType 
        , 0          as Warehouse
        , b.branchaddr1 AddressLine1
        , b.branchaddr2 AddressLine2
        , b.branchaddr3 City
        , b.branchpocode PostCode
        , null       as Contacts
        , b.branchno as SalesId
        , 1          as Active
        , 0          as VirtualWarehouse
    from dbo.branch b) AS BASE
    WHERE NOT EXISTS (select 1 from merchandising.location l2 where l2.salesid != base.salesid)