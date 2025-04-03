SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_AgreementUpdateSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_AgreementUpdateSP]
GO






CREATE PROCEDURE 	dbo.DN_AgreementUpdateSP
			@origBr smallint,
			@acctNo varchar(12),
			@agreementNo int,
			@agreementDate datetime,
			@salesPerson int,
			@depChqClears datetime,
			@holdMerch char(1),
			@holdProp char(1),
			@dateDel datetime,
			@dateNextDue datetime,
			@oldAgreementBal money,
			@cashPrice money,
			@discount money,
			@pxallowed money,
			@serviceCharge money,
			@sundryChargeTotal money,
			@agreementTotal money,
			@deposit money,
			@codFlag char(1),
			@soa varchar(4),
			@paymethod varchar(1),
			@unpaidFlag varchar(1), 
			@deliveryFlag varchar(1),
			@fullDelFlag varchar(1),
			@PaymentMethod char(1),
			@employeeNumAuth int,
			@dateAuth datetime,
			@employeeNumChange int,
			@dateChange datetime,
			@createdby int,
			@paymentholidays smallint,
			@source varchar(15),
            @taxFree bit,
			@return int OUTPUT

AS

/* 68353 Prevent errors if agreement total or deposit < 0 */
	if @agreementTotal <0
           set @agreementTotal =0

	if @deposit < 0
           set @deposit = 0


    SET 	@return = 0			--initialise return code
   --allowing update of salesperson now that application works correctly again.
/*   declare @oldempeeno integer
   select @oldempeeno = isnull (empeenosale,0) from agreement where acctno =@acctno and agrmtno =@agreementno
   if @oldempeeno > 0
       set @salesPerson =@oldempeeno */

	UPDATE 	agreement
	SET		origbr		=	@origBr ,				
			acctno		=	@acctNo ,	
			agrmtno		=	@agreementNo ,	
			dateagrmt	=	@agreementDate ,	
			empeenosale	=	@salesPerson ,	
			datedepchqclr	=	@depChqClears ,	
			holdmerch	=	@holdMerch ,	
			holdprop	=	@holdProp ,	
			datedel		=	@dateDel ,	
			datenextdue	=	@dateNextDue ,	
			oldagrmtbal	=	@oldAgreementBal ,	
			cashprice	=	@cashPrice ,	
			discount	=	@discount ,	
			pxallowed	=	@pxallowed ,	
			servicechg	=	@serviceCharge ,	
			sdrychgtot	=	@sundryChargeTotal ,	
			agrmttotal	=	@agreementTotal ,	
			deposit		=	@deposit ,	
			codflag		=	@codFlag ,	
			soa		=	@soa ,	
			paymethod	=	@paymethod ,	
			unpaidflag	=	@unpaidFlag , 	
			deliveryflag	=	@deliveryFlag ,	
			fulldelflag	=	@fullDelFlag ,	
			PaymentMethod	=	@PaymentMethod ,	
			empeenoauth	=	@employeeNumAuth ,	
			dateauth	=	@dateAuth ,	
			empeenochange=	@employeeNumChange ,	
			datechange	=	@dateChange,
			createdby	=	@createdby,
			paymentholidays = 	@paymentholidays,
			source		= @source,
            TaxFree = @taxFree
	WHERE	acctno = @acctNo
	AND		agrmtno = @agreementNo

	IF(@@rowcount=0)		--new agreement
	BEGIN
		INSERT 	
		INTO	agreement	(origbr,
					acctno,
					agrmtno,
					dateagrmt,
					empeenosale,
					datedepchqclr,
					holdmerch,
					holdprop,
					datedel,
					datenextdue,
					oldagrmtbal,
					cashprice,
					discount,
					pxallowed,
					servicechg,
					sdrychgtot,
					agrmttotal,
					deposit,
					codflag,
					soa,
					paymethod,
					unpaidflag,
					deliveryflag,
					fulldelflag,
					PaymentMethod,
					empeenoauth,
					dateauth,
					empeenochange,
					datechange,
					createdby,
					paymentholidays,
					source,
                    TaxFree)
		VALUES		(@origBr ,
					@acctNo ,
					@agreementNo ,
					@agreementDate ,
					@salesPerson ,
					@depChqClears ,
					@holdMerch ,
					@holdProp ,
					@dateDel ,
					@dateNextDue ,
					@oldAgreementBal ,
					@cashPrice ,
					@discount ,
					@pxallowed ,
					@serviceCharge ,
					@sundryChargeTotal ,
					@agreementTotal ,
					@deposit ,
					@codFlag,
					@soa ,
					@paymethod ,
					@unpaidFlag , 
					@deliveryFlag ,
					@fullDelFlag ,
					@PaymentMethod ,
					@employeeNumAuth ,
					@dateAuth ,
					@employeeNumChange ,
					@dateChange,
					@createdby,
					@paymentholidays,
					@source,
                    @taxFree)
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

