SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_CheckReceiptNotIssuedSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_CheckReceiptNotIssuedSP]
GO

CREATE PROCEDURE 	dbo.DN_CheckReceiptNotIssuedSP
			@firstreceiptno int,
			@lastreceiptno int ,
			@checkoption int,
			@issuedcount int out,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code
	IF (@checkoption = 1)
		BEGIN
			SELECT  @issuedcount= count(*) 
			  FROM  tempreceipt  
			 WHERE  receiptno >= @firstreceiptno 
			   AND  receiptno <= @lastreceiptno
		END
	ELSE 
		BEGIN
	
			SELECT  @issuedcount= count(*) 
			  FROM  tempreceipt  
			 WHERE  receiptno >= @firstreceiptno 
			   AND  receiptno <= @lastreceiptno
			   AND  acctno <> '000000000000'
		           AND  amount <> 0
			   AND  datepresent <> '01/01/1900'
		END
	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END 


GO

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO
