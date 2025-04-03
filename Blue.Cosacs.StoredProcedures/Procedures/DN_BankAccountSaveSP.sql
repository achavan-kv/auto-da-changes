SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_BankAccountSaveSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_BankAccountSaveSP]
GO

CREATE PROCEDURE 	dbo.DN_BankAccountSaveSP
			@custid varchar(20),
			@bankacctno varchar(20) ,
			@bankcode varchar(6) ,
			@dateopened datetime ,
			@code char(1) ,
			@ismandate smallint,
			@duedayid int,
			@acctname varchar(40),
			@acctno varchar(12),		--customer account number
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	--if it's a mandate update 
		--insert a mandate record if it does not already exist
	IF(@ismandate = 1)
	BEGIN
		-- Cannot change a mandate record without telling the bank clearing house
		IF NOT EXISTS (SELECT 1 FROM DDMandate WHERE acctno = @acctno AND status = 'C'
        			AND (EndDate IS NULL OR (DATEDIFF(Day, GETDATE(), EndDate) > 0 AND DATEDIFF(Day, StartDate, EndDate ) > 0)))
		BEGIN
                        UPDATE DDMandate 
                        SET    status = 'H'
                        WHERE  acctno = @acctno AND status = 'C'

			INSERT 
			INTO	DDMandate
				(AcctNo, DueDayId, BankCode, BankBranchNo, BankAcctNo, BankAcctName)
                        -- The bank imposes its own limits on field sizes
			VALUES (@acctno, @duedayid, LEFT(LTRIM(RTRIM(@bankcode)),4), '', LEFT(LTRIM(RTRIM(@bankacctno)),11), LEFT(LTRIM(RTRIM(@acctname)),20))
		END
	END
	
	--whether or not it's a mandate record perform the updates/insert into the 
	--bankacct table	
  
   delete from bankacct where custid =@custid
   /*UPDATE	bankacct we are removing this in case the customer has already two accounts we were getting a duplicate key on update
   just delete all existing accounts insert the new record
   SET		dateopened = @dateopened,
   		code = @code,
          bankacctno = @bankacctno,
          bankcode = @bankcode
   WHERE	custid = @custid*/
   
   	--create a new record
   	INSERT 
   	INTO	bankacct (origbr, custid,  bankacctno, bankcode, dateopened, code)
   	VALUES	(0, @custid, @bankacctno, @bankcode, @dateopened, @code)

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

