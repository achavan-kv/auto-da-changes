IF EXISTS (SELECT * FROM sysobjects WHERE NAME = 'VE_Triggerdelivery' AND type = 'TR')
	BEGIN
		DROP Trigger [dbo].[VE_Triggerdelivery]
	END
GO

CREATE TRIGGER [dbo].[VE_Triggerdelivery]
ON [dbo].[delivery]
AFTER INSERT
AS	
	DECLARE @AcctNo VARCHAR(50)
	DECLARE @ItemNo VARCHAR(50)
	DECLARE @Quantity VARCHAR(10)
	DECLARE @sqlCommand VARCHAR(max)
	DECLARE @DocType VARCHAR(5)
	DECLARE @AcctType VARCHAR(10)
	DECLARE @ItemName VARCHAR(10)
	DECLARE @result NVARCHAR(MAX)
	Declare @Chkout int

    SELECT 
		 @AcctNo=T0.Acctno
		,@ItemNo=T0.ItemId
		,@Quantity=T0.quantity
		,@DocType=T0.delorcoll
		,@ItemName=T0.itemno
    FROM INSERTED T0 INNER JOIN VE_LineItem T1 ON T0.AcctNo=T1.AcctNo 		

	SELECT @AcctType=accttype from acct where acctno=@AcctNo

	--Start Delivery
	IF @AcctNo!='' AND @ItemNo!='' AND @Quantity!=-1 AND @ItemName!='DT' AND @DocType='D'
	BEGIN	
	set @Chkout=(SELECT TOP 1 ISNULL(oldcheckoutid,CheckoutID) from VE_lineItem VE
												 WHERE VE.Orderno IN (
																		SELECT TOP 1 Orderno FROM VE_LineItem
																		WHERE Acctno=@AcctNo AND ItemId=@ItemNo AND ISNULL(IsDeliver,'false')!='true' AND  ISNULL(IsSync,'false')!='true'
																	 )
												)
				INSERT INTO VE_taskschedular(ServiceCode,Code,IsInsertRecord,IsEODRecords,Status,CheckOutID)
											(SELECT Distinct
												'delc'
												,@AcctNo,
												1,
												0,
												0,

												(SELECT TOP 1 ISNULL(oldcheckoutid,CheckoutID) from VE_lineItem VE
												 WHERE VE.Orderno IN (
																		SELECT TOP 1 Orderno FROM VE_LineItem
																		WHERE Acctno=@AcctNo AND ItemId=@ItemNo AND ISNULL(IsDeliver,'false')!='true' AND  ISNULL(IsSync,'false')!='true'
																	 )
												)

											FROM VE_lineItem T0
											WHERE T0.AcctNo=@AcctNo
											AND ItemID=@ItemNo
											AND @ItemName!='DT'
											)

	SET @sqlCommand = '
		UPDATE VE_lineItem SET IsDeliver=''true''
		WHERE Acctno='''+@AcctNo+''' AND OrderNo IN
		(
			SELECT TOP '+@Quantity+' OrderNo FROM VE_LineItem
			WHERE Acctno='''+@AcctNo+''' AND ItemId='''+@ItemNo+''' AND ISNULL(IsDeliver,''false'')!=''true'' AND  ISNULL(IsSync,''false'')!=''true''
		)'  
		print @sqlCommand
		EXEC (@sqlCommand);
				
	END
	--End Delievry

	--Start Exchange Delivery for RF Account

	IF @AcctNo!='' AND @ItemNo!='' AND @Quantity<1 AND @AcctType='R' AND @ItemName!='DT' AND @DocType='D' 
	BEGIN

	SET @Quantity=(@Quantity*(-1))
	print @Quantity
	
	set @Chkout=(SELECT TOP 1 ISNULL(oldcheckoutid,CheckoutID) from VE_lineItem VE
					WHERE VE.Orderno IN (
										SELECT TOP 1 Orderno FROM VE_LineItem
										WHERE Acctno=@AcctNo AND ItemId=@ItemNo 
										AND ISNULL(IsDeliver,'false')='true' 
										AND  ISNULL(IsSync,'false')='true'
										)
				)
	
			INSERT INTO VE_taskschedular(ServiceCode,Code,IsInsertRecord,IsEODRecords,Status,CheckOutID)
											(SELECT Distinct
												'delc'
												,@AcctNo,
												1,
												0,
												0,
												@Chkout

												FROM VE_lineItem T0
												WHERE T0.AcctNo=@AcctNo
												AND ItemID=@ItemNo
												AND @ItemName!='DT'
											) 
			--Update delqty after return or exchnage
			update lineitem set delqty = delqty- @Quantity where acctno=@AcctNo and ItemId=@ItemNo
			delete LineItemBookingSchedule where ItemId=@ItemNo and DelOrColl='D' and Quantity=-1

			SET @sqlCommand = '
			UPDATE VE_lineItem SET IsSync=''false''
			WHERE Acctno='''+@AcctNo+''' AND OrderNo IN
			(
				SELECT TOP '+@Quantity+' OrderNo FROM VE_LineItem
				WHERE Acctno='''+@AcctNo+''' AND ItemId='''+@ItemNo+''' AND ISNULL(IsDeliver,''false'')=''true'' AND  ISNULL(IsSync,''false'')=''true''
			)'
			print @sqlCommand
			EXEC (@sqlCommand);					
	END
	 --End Collection for RF Account

	 --Start Reurn Collection for RF Account

	IF @AcctNo!='' AND @ItemNo!='' AND @Quantity<1 AND @AcctType='R' AND @ItemName!='DT' AND @DocType='C' 
	BEGIN

	SET @Quantity=(@Quantity*(-1))
	print @Quantity
	
	set @Chkout=(SELECT TOP 1 ISNULL(oldcheckoutid,CheckoutID) from VE_lineItem VE
					WHERE VE.Orderno IN (
										SELECT TOP 1 Orderno FROM VE_LineItem
										WHERE Acctno=@AcctNo AND ItemId=@ItemNo 
										AND ISNULL(IsDeliver,'false')='true' 
										AND  ISNULL(IsSync,'false')='true'
										AND ISNULL(Isreturn,'0')='1'  
										AND itemno!='ADMIN'
										)
				)
	IF(@Chkout!=0)
	BEGIN
			--Update delqty after return or exchnage
			update lineitem set delqty = delqty- @Quantity where acctno=@AcctNo and ItemId=@ItemNo
			delete LineItemBookingSchedule where ItemId=@ItemNo and DelOrColl='D' and Quantity=-1

			SET @sqlCommand = '
			UPDATE VE_lineItem SET IsSync=''false''
			WHERE Acctno='''+@AcctNo+''' AND OrderNo IN
			(
				SELECT TOP '+@Quantity+' OrderNo FROM VE_LineItem
				WHERE Acctno='''+@AcctNo+''' AND ItemId='''+@ItemNo+''' AND ISNULL(IsDeliver,''false'')=''true'' AND  ISNULL(IsSync,''false'')=''true''
			)'
			print @sqlCommand
			EXEC (@sqlCommand);
	END				
	END
	 --End Collection for RF Account

	 --Start Collection for Cash Account

	IF @AcctNo!='' AND @ItemNo!='' AND @Quantity<1 AND @DocType='C' AND @AcctType='C'
	BEGIN

	SET @Quantity=(@Quantity*(-1))

	UPDATE lineitem set delqty = delqty- @Quantity where acctno=@AcctNo and ItemId=@ItemNo
	END
	--End Collection for Cash Account
