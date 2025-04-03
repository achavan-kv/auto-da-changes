IF EXISTS (SELECT * FROM SYS.OBJECTS AS o JOIN SYS.SCHEMAS S ON O.schema_id = S.schema_id WHERE O.NAME  = 'CloseServiceRequest' AND S.NAME = 'DBO')
	DROP TRIGGER [DBO].[CloseServiceRequest] 
GO

CREATE TRIGGER [DBO].[CloseServiceRequest] on Lineitem
FOR UPDATE 
AS DECLARE 
		   @Acctno NVARCHAR(20),
		   @ParentItemID INT,
		   @StockBranch int,
		   @qty int,
		   @ordval decimal(18,3),
		   @ItemNumber varchar(18),
		   @SrID int

SELECT @StockBranch = stocklocn,@Acctno = acctno, @ParentItemID = ParentItemID, @ItemNumber = itemno,@qty = quantity FROM INSERTED

SELECT @SrID = ID 
FROM [Service].[Request]
WHERE Account = @Acctno and ItemNumber=@ItemNumber and ItemStockLocation = @StockBranch and [State] <> 'Closed'


IF @SrID IS NOT NULL
BEGIN

		IF (UPDATE(quantity) AND @qty = 0)
		BEGIN
				UPDATE [Service].[Request]
				SET [State]='Closed' , LastUpdatedUser=99999, LastUpdatedOn=getdate(), IsClosed=1,IsForceReIndexRequired =1
				WHERE id = @SrID 

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
				END
		END
END
IF (UPDATE(quantity) AND @qty >0 AND @ParentItemID <>0)
BEGIN
	SELECT @ordval = ordval FROM INSERTED
	UPDATE [Service].[Request]
	SET ItemAmount = @ordval , LastUpdatedOn=getdate(),IsForceReIndexRequired =1
	WHERE Account = @Acctno and ItemID=@ParentItemID and ItemStockLocation = @StockBranch
END

GO


