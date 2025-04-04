if exists (select * from dbo.sysobjects where id = object_id('[dbo].[VE_StockTransferSave]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
BEGIN
	DROP PROCEDURE [dbo].[VE_StockTransferSave]
END
GO

Create PROCEDURE [dbo].[VE_StockTransferSave]
	@StockTransferxml XML
	,@Message VARCHAR(MAX) OUTPUT
	,@StatusCode INT OUTPUT    
	
AS
BEGIN
BEGIN TRY
	SET NOCOUNT ON;   
	SET @Message = '';
	SET @StatusCode = 0

	DECLARE @SendingLocation INT = N'',
	@ReceivingLocation INT = N'',
	@vialocation INT,
	@vialocationSTR VARCHAR(20),
	@documentReferenceNo VARCHAR(20),
	@Comments VARCHAR(20),
	@CreatedById INT = N'',
	@Productid INT = N'',
	@Quantity INT = N'',
	@StockTransferId INT =N'',
	@AverageWeightedCost DECIMAL(19,4),
	@cnt INT, 
	@totalcount INT,
	@attName VARCHAR(30),
	@attValue VARCHAR(30)                     

	BEGIN TRANSACTION

	SELECT @SendingLocation = T.c.value('SendingLocation[1]','INT'),
	@ReceivingLocation = T.c.value('ReceivingLocation[1]','INT'),
	@vialocationSTR =T.c.value('vialocation[1]','[varchar](20)'),
	@documentReferenceNo = T.c.value('documentReferenceNo[1]','[varchar](20)'),
	@Comments = T.c.value('Comments[1]','[varchar](20)'),
	@CreatedById = '100000'
	FROM  @StockTransferxml.nodes('/StockTransferModel') T(c)
	
		select @SendingLocation=id from  [Merchandising].Location where SalesId= @SendingLocation
		select @ReceivingLocation=id from  [Merchandising].Location where SalesId= @ReceivingLocation
		select @vialocationSTR=id from  [Merchandising].Location where SalesId= @vialocationSTR	

	set @vialocation=(case when isnull(@vialocationSTR,'')='' or UPPER(@vialocationSTR)='NULL' THEN null else convert(int , @vialocationSTR) end)
	
	INSERT INTO [Merchandising].[StockTransfer]([SendingLocationId], [ReceivingLocationId], [ViaLocationId], [ReferenceNumber], [Comments],
	[OriginalPrint], [CreatedDate], [CreatedById])
	VALUES (@SendingLocation, @ReceivingLocation, @vialocation, @documentReferenceNo, @Comments, GETDATE(), GETDATE(), @CreatedById)

	SELECT @StockTransferId = IDENT_CURRENT('merchandising.StockTransfer')
	
	SELECT 
	@cnt = 1,
	@totalcount = @StockTransferxml.value('count(/StockTransferModel/Products/Product)','INT')
	PRINT @totalcount;
	
	WHILE @cnt <= @totalcount BEGIN

	SELECT @Productid = T.c.value('Productid[1]','INT'),
	@Quantity = T.c.value('Quantity[1]','INT')
	FROM  @StockTransferxml.nodes('/StockTransferModel/Products/Product[position()=sql:variable("@cnt")]') T(c)

	SELECT top 1 @AverageWeightedCost=AverageWeightedCost FROM Merchandising.CostPrice WHERE ProductId=@Productid 
	order by AverageWeightedCostUpdated desc

	INSERT INTO [Merchandising].[StockTransferProduct]([StockTransferId], [ProductId], [Quantity], [Comments], [AverageWeightedCost], [BookingId],[QuantityCancelled], [QuantityReceived], [CompletedOn])
    VALUES (@StockTransferId, @Productid, @Quantity, @Comments, @AverageWeightedCost, (select max([BookingId])+1 from [Merchandising].[StockTransferProduct]),0, 0, GETDATE())

	if(@vialocation is not null)
	begin
	
	UPDATE [Merchandising].[ProductStockLevel]
	SET [StockOnHand] = [StockOnHand] - @Quantity, [StockAvailable] =[StockAvailable] - @Quantity
	WHERE productid=@Productid and LocationId=@SendingLocation

	UPDATE [Merchandising].[ProductStockLevel]
	SET [StockOnHand] = [StockOnHand] + @Quantity, [StockAvailable] =[StockAvailable] + @Quantity
	WHERE productid=@Productid and LocationId=@vialocation

	end
	
	else
	begin
	
	UPDATE [Merchandising].[ProductStockLevel]
	SET [StockOnHand] = [StockOnHand] - @Quantity, [StockAvailable] =[StockAvailable] - @Quantity
	WHERE productid=@Productid and LocationId=@SendingLocation

	UPDATE [Merchandising].[ProductStockLevel]
	SET [StockOnHand] = [StockOnHand] + @Quantity, [StockAvailable] =[StockAvailable] + @Quantity
	WHERE productid=@Productid and LocationId=@ReceivingLocation
	
	end

	SET @cnt = @cnt + 1
    END

	IF (@@error != 0)
              BEGIN
                     ROLLBACK
                     print'Transaction rolled back'
                 SET @StatusCode = 500;           
              END
              ELSE
              BEGIN
                   SET @StatusCode = 200;         
                    SET @Message = CAST(@StockTransferId as VARCHAR(MAX))	
                     COMMIT
                     print'Transaction committed'
              END
       END TRY 
              BEGIN CATCH 
                     IF (@@error > 0)
              SET @StatusCode = 500;         
              SET @Message = ERROR_Message()
       ROLLBACK TRAN 
       END CATCH
END