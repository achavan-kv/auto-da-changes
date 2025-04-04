if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_GetInvoicePaymentDetails]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
DROP PROCEDURE [dbo].[DN_GetInvoicePaymentDetails]
GO

/****** Object:  StoredProcedure [dbo].[DN_GetInvoicePaymentDetails]    Script Date: 12/24/2018 4:20:30 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO

CREATE PROCEDURE  [dbo].[DN_GetInvoicePaymentDetails]
   			@acctNo VARCHAR(15),
			@agrmtno VARCHAR(10),
			@AgreementInvoiceNumber Varchar(15),
   			@return int OUTPUT
 
AS
 
 	SET  @return = 0   --initialise return code
	--Suvidha Added CR 2018-13
	Declare @amount money, @payMethod Varchar(20)--, @acctno Varchar(20)
	Declare @saleOrderID  Varchar(10)
	set @amount = 0;

	CREATE TABLE #invoicePayment (payMethod varchar(20), amount money)

	if(@AgreementInvoiceNumber = "")
	Begin		

		--select @agrmtno = agrmtno from agreement where acctno = @acctNo
		select @saleOrderID = @agrmtno
	End
	Else
	Begin
		select @saleOrderID = id 
		from [Sales].[Order] where AgreementInvoiceNumber = @AgreementInvoiceNumber	
		
		
		select @acctno = acctno from Agreement where agreementinvoicenumber = @AgreementInvoiceNumber	

		--select @soldByName = FirstName + ' ' + LastName from [admin].[user]  where id = @soldByID
		--select @createdByName = FirstName + ' ' + LastName from [admin].[user]  where id  = @createdByID
	End

	if(@agrmtno = "1")
	Begin
		
		insert into #invoicePayment
		select b.codedescript, sum(a.transvalue) 
		from fintrans a 
		Left join code b on a.payMethod = b.code and b.category = 'FPM'
		where a.transtypecode = 'PAY' and a.acctno =@acctno
		group by b.codedescript

	End	
	Else
	Begin

		insert into #invoicePayment
		select b.[Description], a.Amount from [Sales].[OrderPayment] a
		Left join [Payments].[PaymentMethod] b on a.PaymentMethodId = b.Id
		where orderid = @saleOrderID
	End

	select * from #invoicePayment	
 
 	IF (@@error != 0)
 	BEGIN
  		SET @return = @@error
 	END

