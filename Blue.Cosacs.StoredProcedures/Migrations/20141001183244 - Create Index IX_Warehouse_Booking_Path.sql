-- transaction: true
IF EXISTS (SELECT * FROM sys.indexes 
           WHERE name='IX_Warehouse_Booking_Path' AND OBJECT_ID = OBJECT_ID('Warehouse.Booking'))
BEGIN
    DROP INDEX [IX_Warehouse_Booking_Path] ON [Warehouse].[Booking]
END

CREATE NONCLUSTERED INDEX [IX_Warehouse_Booking_Path] ON [Warehouse].[Booking] 
(
	[Path] ASC
) ON [PRIMARY]
