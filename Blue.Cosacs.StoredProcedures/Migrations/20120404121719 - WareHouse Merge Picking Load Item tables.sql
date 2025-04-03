-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

if not exists(select * from syscolumns where name='PickingId' and OBJECT_NAME(id)='Warehouse.Booking')
BEGIN
	alter TABLE Warehouse.Booking add AcctNo CHAR(12)
	alter TABLE Warehouse.Booking add OriginalId INT
	
	alter TABLE Warehouse.Booking add TruckId INT 
	alter TABLE Warehouse.Booking add PickingId INT 
	alter TABLE Warehouse.Booking add PickingAssignedBy INT
	alter TABLE Warehouse.Booking add PickQuantity INT  
	alter TABLE Warehouse.Booking add PickingItemComment VARCHAR(4000) 
	alter TABLE Warehouse.Booking add PickingStatus VARCHAR(50) 
	alter TABLE Warehouse.Booking add PickingRejected BIT
	 
	alter TABLE Warehouse.Booking add ScheduleId INT
	alter TABLE Warehouse.Booking add ScheduleComment VARCHAR(4000)
	alter TABLE Warehouse.Booking add ScheduleSequence INT



ALTER TABLE [Warehouse].[Booking]  WITH CHECK ADD  CONSTRAINT [FK_Booking_AssignedBy] FOREIGN KEY([PickingAssignedBy])
REFERENCES [dbo].[courtsperson] ([empeeno])


ALTER TABLE [Warehouse].[Booking]  WITH CHECK ADD  CONSTRAINT [FK_Booking_Truck] FOREIGN KEY([TruckId])
REFERENCES [Warehouse].[Truck] ([Id])


ALTER TABLE [Warehouse].[Booking]  WITH CHECK ADD  CONSTRAINT [FK_Booking_Load] FOREIGN KEY([ScheduleId])
REFERENCES [Warehouse].[Load] ([Id])

ALTER TABLE [Warehouse].[Booking]  WITH CHECK ADD  CONSTRAINT [FK_Booking_Picking] FOREIGN KEY([PickingId])
REFERENCES [Warehouse].[Picking] ([Id])

ALTER TABLE [Warehouse].[Booking]  WITH CHECK ADD  CONSTRAINT [FK_Booking_OriginalId] FOREIGN KEY([OriginalId])
REFERENCES [Warehouse].[Booking] ([Id])

END

go

