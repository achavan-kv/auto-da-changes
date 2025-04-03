alter table Warranty.WarrantySale
add StockLocation smallint

alter table Warranty.WarrantySale
add CustomerNotes varchar(4000)

alter table Warranty.WarrantySale
add [ItemCostPrice] [money] NULL

alter table Warranty.WarrantySale
add [ItemDeliveredOn] [smalldatetime] NULL
