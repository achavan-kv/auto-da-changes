if exists (select * from dbo.sysobjects where id = object_id('[dbo].[VE_GetPayments]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
BEGIN
	DROP PROCEDURE [dbo].[VE_GetPayments]
END
GO

Create PROCEDURE [dbo].[VE_GetPayments]
	  @acctno varchar(12)
	, @Message varchar(MAX) output
	, @StatusCode varchar(5) output
AS
BEGIN
BEGIN TRY
	SET NOCOUNT ON;
	Declare @Accounttype varchar(10)
	SET @Message = ''
	SET @StatusCode = ''	
	IF NOT EXISTS (select 1 from custacct where acctno=@acctno)
	BEGIN
		SET @Message = 'Account not found'
		SET @StatusCode = '404'
		RETURN		
	END
	ELSE
	BEGIN	
	select @Accounttype=accttype from acct where acctno=@acctno
	
	IF(@Accounttype LIKE '%C%')
	BEGIN

	select   @acctno as 'ExternalPaymentId',
	'Order' as 'PaymentType',
	agr.dateauth as 'PaymentDate',
	'1' as 'EmployeeId',
	'Cash' as 'PaymentMethod' ,
	d.itemno as 'ItemNo',
	(l.ordval + l.taxamt ) as 'Amount',
	l.orderlineno as 'CheckOutId',
	l.orderno as 'OrderId',
	ft.ChequeNo as 'ChequeNumber',
	'' as AdjustmentType 
	from delivery d inner join lineitem l 
	ON d.acctno = l.acctno and d.itemno = l.itemno and d.stocklocn = l.stocklocn
	and d.itemid = l.itemid and d.parentitemid = l.parentitemid 
	inner join fintrans ft on ft.acctno=d.acctno
	inner join Agreement agr on agr.acctno=d.acctno
	where d.ACCTNO=@acctno and d.delorcoll='D' and d.itemno not in ('DT','STAX')
	and ft.transtypecode='PAY'
	
	END
	ELSE IF(@Accounttype LIKE '%R%')
	BEGIN
	
	select  @acctno as 'ExternalPaymentId', 
	'Order' as 'PaymentType',
	agr.dateauth as 'PaymentDate',
	'1' as 'EmployeeId',
	'Credit' as 'PaymentMethod' ,
	d.itemno as 'ItemNo',
	(l.ordval + l.taxamt ) as 'Amount',
	l.orderlineno as 'CheckOutId',
	l.orderno as 'OrderId',
	'' as 'ChequeNumber',
	'' as AdjustmentType 
	from delivery d inner join lineitem l 
	ON d.acctno = l.acctno and d.itemno = l.itemno and d.stocklocn = l.stocklocn
	and d.itemid = l.itemid and d.parentitemid = l.parentitemid 
	inner join Agreement agr on agr.acctno=d.acctno
	where d.ACCTNO=@acctno and d.delorcoll='D' and d.itemno not in ('DT','STAX')	
	END
	SET @Message = 'Payment Details found'
		SET @StatusCode = '200'
	END
	IF (@@error != 0)
              BEGIN
                     print'Transaction rolled back'
                  SET @StatusCode = 500;           
              END
              ELSE
              BEGIN
                     SET @StatusCode = 200;         
                     SET @Message = 'Payment Details found succesfully.'
                     print'Transaction committed'
              END
       END TRY 
              BEGIN CATCH 
                     IF (@@error > 0)
              SET @StatusCode = 500;         
              SET @Message = ERROR_Message()
      
       END CATCH
END
	print @Message