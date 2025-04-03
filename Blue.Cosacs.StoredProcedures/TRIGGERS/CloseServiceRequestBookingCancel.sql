IF EXISTS (SELECT * FROM SYS.OBJECTS AS o JOIN SYS.SCHEMAS S ON O.schema_id = S.schema_id WHERE O.NAME  = 'CloseServiceRequest' AND S.NAME = 'Warehouse')
	DROP TRIGGER [Warehouse].CloseServiceRequest
GO
CREATE TRIGGER [Warehouse].CloseServiceRequest ON [Warehouse].[Cancellation] 
AFTER INSERT  
AS 

DECLARE @BookingId INT,
		@AcctNo NVARCHAR(20),
		@ItemNo NVARCHAR(18),
		@StockBranch int,
		@SrId int

 SELECT @BookingId =  id FROM INSERTED;

 SELECT @Acctno = AcctNo, @ItemNo = ItemNo, @StockBranch = StockBranch from [Warehouse].[Booking] WHERE id = @BookingId

 SELECT @SrID = ID 
 FROM [Service].[Request] 
 WHERE Account = @Acctno and ItemNumber=@ItemNo and ItemStockLocation = @StockBranch and [State] <> 'Closed'


IF @SrID IS NOT NULL
BEGIN
	IF EXISTS (SELECT * FROM [Service].[Request] WHERE ID = @SrId) 
	BEGIN
		UPDATE [Service].[Request]
		SET [State]='Closed' , LastUpdatedUser=99999, LastUpdatedOn=getdate(), IsClosed=1,IsForceReIndexRequired =1
		WHERE ID = @SrId
	END

	IF EXISTS (SELECT * FROM [Service].TechnicianBooking WHERE RequestId = @SRId)
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
	END
END