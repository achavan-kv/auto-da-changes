
/****** Object:  Trigger [dbo].[trig_agr_authins]    Script Date: 01/03/2019 2:39:41 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



ALTER TRIGGER [dbo].[trig_agr_authins]
ON  [dbo].[agreement] 
FOR insert
AS
   DECLARE @new_holdprop char(1)
   DECLARE @new_acctno   char(12)

   SELECT @new_holdprop = holdprop,
          @new_acctno   = acctno
   FROM   inserted

   IF @new_holdprop = 'Y'
   BEGIN
      EXECUTE dbnewauth @acctno = @new_acctno
   END


   
   	-- Code added for Invoice CR---
  Declare @branchno varchar(15),  @InvoiceNumber varchar(15),@new_agrmtno int

   SELECT @new_agrmtno = agrmtno	        
   FROM   inserted
  --Fetch Branchno From account table
	SELECT  @branchno= branchno FROM acct 	WHERE acctno=@new_acctno 
	--If agreement number is greater than 1 then fetch invoice number from Sales.order table.
	if (@new_agrmtno > 1)
	BEGIN
		SELECT @InvoiceNumber = AgreementInvoiceNumber FROM sales.[order] WHERE id = @new_agrmtno
		IF(@InvoiceNumber ='')
		BEGIN 
		   SELECT @InvoiceNumber = AgreementInvoiceNumber FROM agreement WHERE acctno = @new_acctno
		END
		
	END
	Else
	Begin
		-- Generate new Invoice Number based on Branch Number.
		exec  GenerateInvoiceNumber @branchno, @InvoiceNumber OUTPUT,0
	End



  --Update new invoice number in agreement table.
  UPDATE agreement
  SET AgreementInvoiceNumber=@InvoiceNumber
  WHERE acctno=@new_AcctNo AND agrmtno=@new_agrmtno
  
