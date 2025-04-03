-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
--SERVICE SCHEMA
--HOLIDAY TABLE
--Drop Index
DROP INDEX [IX_HolidayDates] ON [Service].[Holiday]
GO

-- Change column datatype
ALTER TABLE [Service].[Holiday] 
ALTER COLUMN EndDate date not null

ALTER TABLE [Service].[Holiday] 
ALTER COLUMN StartDate date not null

--Enable Index
CREATE NONCLUSTERED INDEX [IX_HolidayDates] ON [Service].[Holiday]
(
	[UserId] ASC
)
INCLUDE ( 	[StartDate],
	[EndDate]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO


--REQUEST TABLE
ALTER TABLE [Service].[Request] 
ALTER COLUMN AllocationItemReceivedOn date null

ALTER TABLE [Service].[Request] 
ALTER COLUMN AllocationPartExpectOn date null

ALTER TABLE [Service].[Request] 
ALTER COLUMN EstimateReceived date null

ALTER TABLE [Service].[Request] 
ALTER COLUMN FinaliseReturnDate date null

ALTER TABLE [Service].[Request] 
ALTER COLUMN ResolutionDate date null

--TECHNICIANBOOKING TABLE
DROP INDEX [IX_UserId] ON [Service].[TechnicianBooking]
GO

ALTER TABLE [Service].[TechnicianBooking] 
ALTER COLUMN CompletedDate date null

ALTER TABLE [Service].[TechnicianBooking] 
ALTER COLUMN [Date] date not null

CREATE NONCLUSTERED INDEX [IX_UserId] ON [Service].[TechnicianBooking]
(
	[UserId] ASC
)
INCLUDE ( 	[Date]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO


--WAREHOUSE SCHEMA
--BOOKING TABLE
ALTER TABLE [Warehouse].[Booking] 
ALTER COLUMN DeliveryConfirmedDate date null

ALTER TABLE [Warehouse].[Booking] 
ALTER COLUMN DeliveryOrCollectionDate date not null

--PICKING TABLE
ALTER TABLE [Warehouse].[Picking] 
ALTER COLUMN [PickedOn] date null


--WARRANTY SCHEMA
--WARRANTYPROMOTION TABLE
ALTER TABLE [Warranty].[WarrantyPromotion] 
ALTER COLUMN [EndDate] date not null

ALTER TABLE [Warranty].[WarrantyPromotion] 
ALTER COLUMN [StartDate] date not null

--WARRANTYSALE TABLE
ALTER TABLE [Warranty].[WarrantySale] 
ALTER COLUMN [ItemDeliveredOn] date null

ALTER TABLE [Warranty].[WarrantySale] 
ALTER COLUMN [SoldOn] date null