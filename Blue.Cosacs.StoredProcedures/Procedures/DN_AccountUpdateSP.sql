if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_AccountUpdateSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
DROP PROCEDURE [dbo].[DN_AccountUpdateSP]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


-- =============================================
-- Author:		
-- Create date: 
-- Description:	
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
--15/12/18 added a column InvoiceVersion as a part of CR#
--21/06/19 KD	Removed is amortized flag and added in SaveCashLoan SP
-- =============================================
CREATE PROCEDURE 	dbo.DN_AccountUpdateSP
			@origBr smallint,
			@acctNo varchar(12),
			@acctType char(1),
			@dateAcctOpen datetime,
			@creditDays smallint,
			@agreementTotal money,
			@dateLastPaid datetime,
			@as400bal money,
			@outstandingBal money,
			@arrears money,
			@highestStatus char(1),
			@currentStatus char(1),
			@highStatusDays smallint,
			@branchNo smallint,	
			@paidPcent smallint,
			@termsType varchar(2),
			@repossArrears money,
			@repossValue money,
			@dateIntoArrears datetime,
			@country varchar(2),
			@lastupdatedby int, 
			@bdwbalance money,
			@bdwcharges money,
            @securitised varchar(2),
            @haslineitems bit,
			--@isAmortized bit,			
			@return int OUTPUT

AS

	/*Prevent errors if agreement total < 0 */
	DECLARE @InvoiceNumber nvarchar(14)
	if @agreementTotal <0
           set @agreementTotal =0


	SET 	@return = 0			--initialise return code
            if @accttype= 'C' and (@termstype = '' or @termstype is null)
            begin
             select @termstype =min(termstype) from termstype
            end
	/* convert the accountType in the right type */
	SELECT	@acctType 	=	genaccttype
	FROM		accttype
	WHERE	accttype	=	@accttype
	AND		countrycode	=	@country

	UPDATE	acct
	SET		origbr		    =	@origBr ,				
			acctno		    =	@acctNo ,	
			accttype	    =	@acctType ,	
/*			dateacctopen	=	@dateAcctOpen ,	AA- should never update only insert dateacctopen*/
			creditdays	    =	@creditDays ,	
			agrmttotal	    =	@agreementTotal ,	
			datelastpaid	=	@dateLastPaid ,	
			as400bal	    =	@as400bal ,	
			outstbal		=	@outstandingBal ,	
			arrears		    =	@arrears ,	
			highststatus	=	@highestStatus ,	
			currstatus	    =	@currentStatus ,	
			histatusdays	=	@highStatusDays ,	
			branchno	    =	@branchNo ,	
			paidpcent	    =	@paidPcent ,	
			termstype	    =	@termsType ,	
			repossarrears	=	@repossArrears ,	
			repossvalue	    =	@repossValue ,	
			dateintoarrears	=	@dateIntoArrears,
			lastupdatedby 	=	@lastupdatedby,
			bdwbalance	    =	@bdwbalance,
			bdwcharges	    =	@bdwcharges,
            securitised     =       @securitised,
            hasstocklineitems =	@haslineitems  
	WHERE	acctno = @acctNo

	IF(@@rowcount = 0)		--new account
	BEGIN
		INSERT
		INTO	acct	(origbr,
				acctno,
				accttype,
				dateacctopen,
				creditdays,
				agrmttotal,
				datelastpaid,
				as400bal,
				outstbal,
				arrears,
				highststatus,
				currstatus,
				histatusdays,
				branchno,
				paidpcent,
				termstype,
				repossarrears,
				repossvalue,
				dateintoarrears,
				lastupdatedby,
				bdwbalance,
				bdwcharges,
                securitised,
                hasstocklineitems)
				--isAmortized)
		VALUES	(@origBr ,				
				@acctNo ,	
				@acctType ,	
				@dateAcctOpen ,	
				@creditDays ,	
				@agreementTotal ,	
				@dateLastPaid ,	
				@as400bal ,	
				@outstandingBal ,	
				@arrears ,	
				@highestStatus ,	
				@currentStatus ,	
				@highStatusDays ,	
				@branchNo ,	
				@paidPcent ,	
				@termsType ,	
				@repossArrears ,	
				@repossValue ,	
				@dateIntoArrears,
				@lastupdatedby,
				@bdwbalance,
				@bdwcharges,
                @securitised,
                @haslineitems)
				--@isAmortized)
	END


	--68185 11/05/06 Updating highest status to current status if current status is higher then the higest status
	IF @currentStatus > @highestStatus 
	BEGIN

		UPDATE	Acct
		SET	highststatus = @currentStatus
		WHERE	acctno = @acctNo
		AND	@currentStatus between '2' and '9'
		AND	@currentStatus > @highestStatus 
	
	END

	--Step 1-- Check if account has agreement number in agreement table
	
   Select @InvoiceNumber =   AgreementInvoiceNumber from agreement where acctno=@acctNo and agrmtno=1
   if( @InvoiceNumber IS NULL)
	BEGIN
	
	declare @p2 varchar(14)
	set @p2=NULL
	declare @p3 int
	set @p3=NULL
	--Step 2--If No then call generateinvoicenumber SP and update that account in Agreement table.	
		exec GenerateInvoiceNumber @BranchNumber= @origBr,@InvoiceNumber=@p2  output,@return=@p3 output		
		update agreement set agreementinvoiceNumber=@p2 where acctno=@acctNo
		
		END	
	 	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END


GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO


	