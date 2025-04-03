IF EXISTS (SELECT * FROM SYS.OBJECTS AS o JOIN SYS.SCHEMAS S ON O.schema_id = S.schema_id WHERE O.NAME  = 'RemoveTechnican' AND S.NAME = 'Warehouse')
	DROP TRIGGER [Warehouse].[RemoveTechnican] 
GO
CREATE TRIGGER [Warehouse].[RemoveTechnican] on [Warehouse].[Booking]
FOR UPDATE 
AS
DECLARE @SRId INT,
		@AcctNo NVARCHAR(20),
		 @ItemNo NVARCHAR(18),
		 @StockBranch int


 SELECT @Acctno = AcctNo, @ItemNo = ItemNo, @StockBranch = StockBranch from INSERTED 

 SELECT @SRId = ID FROM [Service].[Request]
			WHERE Account = @Acctno and ItemNumber=@ItemNo and ItemStockLocation = @StockBranch
IF (UPDATE(DeliveryOrCollectionDate))
BEGIN
    IF EXISTS (SELECT * FROM [Service].TechnicianBooking WHERE RequestId=@SRId)
	BEGIN
		 DECLARE @TechincianId INT, @TechincianBookingId INT

		 SELECT @TechincianId = UserId, @TechincianBookingId =id FROM [Service].TechnicianBooking 
		 WHERE RequestId=@SRId

		 INSERT INTO [Service].[TechnicianBookingDelete] 
		 (id,RequestId, UserId, Date, TechincianId, Reason)
		 VALUES 
		 (@TechincianBookingId,@SRId, 99999, (SELECT GETDATE()),@TechincianId, 'Auto Remove')

		 DELETE [Service].TechnicianBooking 
		 WHERE RequestId=@SRId

		 UPDATE [Service].[Request]
		 SET [STATE] = 'Awaiting allocation',IsForceReIndexRequired =1
		 WHERE ID = @SRId
	END

END


GO


