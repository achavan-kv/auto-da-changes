IF EXISTS (SELECT * FROM sysobjects WHERE NAME = 'VE_TriggerCashRefund' AND type = 'TR')
	BEGIN
		DROP Trigger [dbo].[VE_TriggerCashRefund]
	END
GO

CREATE TRIGGER [dbo].[VE_TriggerCashRefund]
ON [dbo].[fintrans]
AFTER INSERT
AS	
	DECLARE @AcctNo VARCHAR(50)
	DECLARE @ItemNo VARCHAR(50)
	DECLARE @Quantity VARCHAR(10)
	DECLARE @sqlCommand VARCHAR(max)
	DECLARE @DocType VARCHAR(5)
	DECLARE @DocType1 VARCHAR(5)
	DECLARE @AcctType VARCHAR(10)
	DECLARE @TransRef VARCHAR(10)
	DECLARE @PaymentType VARCHAR(5)
	SELECT	
		@DocType1=T0.transtypecode
    FROM inserted T0 

	IF(@DocType1='REF' OR @DocType1='XFR')
	BEGIN
	    SELECT	TOP 1
			 @AcctNo=T0.Acctno
			,@ItemNo=T00.ItemId
			,@Quantity=T00.quantity
			,@DocType=T00.delorcoll
			,@TransRef=T0.transrefno
			,@PaymentType=(CASE	WHEN T2.acctnoxfr!='' THEN 'XFR' ELSE T0.transtypecode END)
    FROM inserted T0
		INNER JOIN delivery T00 ON T0.Acctno=T00.AcctNo AND T00.delorcoll='C'
		INNER JOIN VE_LineItem T1 ON T0.AcctNo=T1.AcctNo AND T00.ItemId=T1.ItemId 
		LEFT OUTER JOIN finxfr T2 on T0.Acctno=T2.AcctNo AND T0.transrefno=T2.transrefno AND T0.chequeno=T2.acctnoxfr
	WHERE ISNULL(T1.IsReturn,'false')='true'
		AND ISNULL(T1.IsSync,'false')='true' 
		AND ISNULL(T1.PaymentType,'') NOT IN ('REF','XFR')
		AND T00.itemno!='DON'
	ORDER by T00.datetrans desc

	SELECT @AcctType=accttype from acct where acctno=@AcctNo

	IF @AcctNo!='' AND @ItemNo!='' AND @Quantity<1 AND @DocType='C' AND @AcctType='C'
	BEGIN

	SET @Quantity=(@Quantity*(-1))
	print @Quantity
	
	Declare @Chkout VARCHAR(20)
	set @Chkout=(SELECT TOP 1 ISNULL(oldcheckoutid,CheckoutID) from VE_lineItem VE
												 WHERE VE.Orderno IN (
																		SELECT TOP 1 Orderno 
																		FROM VE_LineItem 
																		WHERE Acctno=@AcctNo 
																		AND ItemId=@ItemNo 
																		AND ISNULL(IsReturn,'false')='true'
																		AND ISNULL(IsSync,'false')='true'  
																		AND ISNULL(oldcheckoutid,CheckoutID) NOT IN 
																			(
																				--SELECT T1.CheckOutID 
																				--FROM VE_taskschedular T0
																				--INNER JOIN VE_LineItem T1 ON ISNULL(T1.Oldcheckoutid,T1.CheckoutID)=T0.CheckoutId 
																				--	AND T0.COde=T1.AcctNo
																				--WHERE Code=@AcctNo
																				
																				--START-Done changes for resolve the issue for 2nd order no of Same Itemcode in same order payment not Sync with MVE 
																				SELECT DISTINCT T0.CheckOutID 
																				FROM VE_taskschedular T0
																				INNER JOIN VE_LineItem T1 ON T0.CheckoutId IN (
																						SELECT DISTINCT CheckoutId FROM VE_LineItem WHERE Acctno=@AcctNo
																						UNION ALL
																						SELECT DISTINCT oldCheckoutId FROM VE_LineItem WHERE Acctno=@AcctNo
																				 )
																				WHERE Code=@AcctNo
																				--END-Done chnages for resolve the issue for 2nd order no of Same Itemcode in same order payment not Sync with MVE 
																			)
																	 )
												)
		 IF NOT EXISTS (SELECT ServiceCode FROM VE_taskschedular WHERE Code=@AcctNo AND ServiceCode='delc' AND Status='0' AND CheckOutID=@Chkout)
		 BEGIN		
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
											)
		 END
			SET @sqlCommand = '
			UPDATE VE_lineItem SET IsSync=''false'',PaymentType='''+@PaymentType+'''
			WHERE Acctno='''+@AcctNo+''' AND OrderNo IN
			(
				SELECT TOP '+@Quantity+' OrderNo FROM VE_LineItem
				WHERE Acctno='''+@AcctNo+''' AND ItemId='''+@ItemNo+''' AND ISNULL(IsDeliver,''false'')=''true'' 
				AND  ISNULL(IsSync,''false'')=''true''
				--AND  ISNULL(IsSync,''false'')=''false''
				AND ISNULL(oldcheckoutid,CheckoutID)='''+@Chkout+'''
			)'
			print @sqlCommand
			EXEC (@sqlCommand);					
	END
	END