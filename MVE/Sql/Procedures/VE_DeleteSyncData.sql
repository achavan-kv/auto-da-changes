if exists (select * from dbo.sysobjects where id = object_id('[dbo].[VE_DeleteSyncData]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
BEGIN
	DROP PROCEDURE [dbo].[VE_DeleteSyncData]
END
GO

Create PROCEDURE [dbo].[VE_DeleteSyncData]
		@ServiceCode VARCHAR(20) = N'',
		@Code VARCHAR(20) = N'',
		@IsInsertRecord VARCHAR(20) = N'',
	    @IsEODRecords VARCHAR(20) = N'',
	    @Message varchar(1000) =N'',
		@OrderId varchar(1000) =N'',
		@ID  VARCHAR(20) = N''
AS
BEGIN
	SET NOCOUNT ON;
	declare @stri varchar(max)
	declare @OrderType varchar(max)

	SET @OrderType= (SELECT Distinct BillType 
					FROM VE_LineItem T0
					INNER JOIN VE_TaskSchedular T1 ON ISNULL(T0.oldcheckoutid,T0.CheckoutId)=T1.CheckoutId
					WHERE T1.Id=@Id)
	IF @Message='success'
		BEGIN
			IF @ServiceCode!='EOD'
			BEGIN
				UPDATE VE_TaskSchedular
				SET 
					Status='true',
					Message=@Message
				WHERE 
					ServiceCode=@ServiceCode
					AND Code=@Code
					AND IsInsertRecord=@IsInsertRecord
					AND IsEODRecords=@IsEODRecords
					AND ID=@ID
			END
			IF @ServiceCode='EOD'
			BEGIN
				UPDATE VE_TaskSchedular
				SET 
					Status='true',
					Message=@Message,
					CheckOutID=@OrderId
				WHERE 
					ServiceCode=@ServiceCode
					AND ID=@ID
			END
					IF @ServiceCode='pyt'
					BEGIN
					declare @string varchar(max)
					set @string='UPDATE VE_lineitem SET DeliveryPercent=''true'' where OrderNo IN('+@OrderId+') AND acctno='+@Code
						print @string
					EXEC (@string);
					END		

					IF @ServiceCode='delc' AND @OrderType!='IdenticalEx'
					BEGIN
					set @stri='UPDATE VE_lineitem SET IsSync=''true'', SyncDate=getdate()  where OrderNo IN('+@OrderId+') AND DeliveryPercent=''1'' AND acctno='+@Code
						print @stri
					EXEC (@stri);
				    END

					IF @ServiceCode='delc' AND @OrderType='IdenticalEx'
					BEGIN
					
					set @stri='UPDATE VE_lineitem SET IsSync=''true'', SyncDate=getdate()  where OrderNo IN('+@OrderId+') AND acctno='+@Code
						print @stri
					EXEC (@stri);
				    END															
		END
	ELSE
		BEGIN
			UPDATE VE_TaskSchedular
			SET 
				Status='false',
				Message=@Message
			WHERE 
				ServiceCode=@ServiceCode
				AND Code=@Code
				AND IsInsertRecord=@IsInsertRecord
				AND IsEODRecords=@IsEODRecords
				AND isnull(Status,'0')!='1'
				AND ID=@ID
		END
END