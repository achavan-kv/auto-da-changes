SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_AgreementPopulateSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_AgreementPopulateSP]
GO


CREATE PROCEDURE 	dbo.DN_AgreementPopulateSP
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : DN_AgreementPopulateSP
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        :  
-- Author       : ??
-- Date         : ??
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 11/10/11 ip  #3921 - CR1232 - populate AdminFee and InsCharge
-- ================================================
			@origBr smallint OUT,	
			@acctNo varchar(12),	
			@agreementNo int,
			@agreementDate datetime OUT,	
			@salesPerson int OUT,	
			@depChqClears datetime OUT,	
			@holdMerch char(1) OUT,	
			@holdProp char(1) OUT,	
			@dateDel datetime OUT,	
			@dateNextDue datetime OUT,	
			@oldAgreementBal money OUT,	
			@cashPrice money OUT,	
			@discount money OUT,	
			@pxallowed money OUT,	
			@serviceCharge money OUT,	
			@sundryChargeTotal money OUT,	
			@agreementTotal money OUT,	
			@deposit money OUT,	
			@codFlag char(1) OUT,	
			@soa varchar(4) OUT,	
			@paymethod varchar(1) OUT,	
			@unpaidFlag varchar(1) OUT, 	
			@deliveryFlag varchar(1) OUT,	
			@fullDelFlag varchar(1) OUT,	
			@PaymentMethod char(1) OUT,	
			@employeeNumAuth int OUT,	
			@dateAuth datetime OUT,	
			@employeeNumChange int OUT,	
			@dateChange datetime OUT,	
			@createdby int OUT,
			@paymentholidays smallint OUT,
			@source varchar(32) out,
			@adminFee money OUT,								--IP - #3921 - CR1232
			@insCharge money OUT,								--IP - #3921 - CR1232
            @taxFree bit OUT,
			@return int OUTPUT	
AS

	SET 	@return = 0			--initialise return code

	SELECT	@origBr 		=	origbr,
			@acctNo 		=	acctno,
			@agreementNo 		=	agrmtno,
			@agreementDate	=	dateagrmt,
			@salesPerson 		=	empeenosale,
			@depChqClears 	=	datedepchqclr,
			@holdMerch 		=	holdmerch,
			@holdProp 		=	holdprop,
			@dateDel 		=	datedel,
			@dateNextDue 		=	datenextdue,
			@oldAgreementBal 	=	oldagrmtbal,
			@cashPrice 		=	cashprice,
			@discount 		=	discount,
			@pxallowed 		=	pxallowed,
			@serviceCharge 	=	servicechg,
			@sundryChargeTotal 	=	sdrychgtot,
			@agreementTotal 	=	agrmttotal,
			@deposit 		=	deposit,
			@codFlag 		=	codflag,
			@soa 			=	soa,
			@paymethod 		=	paymethod,
			@unpaidFlag  		=	unpaidflag,
			@deliveryFlag 		=	deliveryflag,
			@fullDelFlag 		=	fulldelflag,
			@PaymentMethod 	=	PaymentMethod,
			@employeeNumAuth 	=	empeenoauth,
			@dateAuth 		=	dateauth,
			@employeeNumChange 	=	empeenochange,
			@dateChange		=	datechange,
			@createdby		=	createdby,
			@paymentholidays = paymentholidays,
			@source = source,
			@adminFee = AdminFee,								--IP - #3921 - CR1232
			@insCharge = InsCharge,								--IP - #3921 - CR1232
            @taxFree = TaxFree
	FROM		agreement
	WHERE	acctno = @acctNo
	AND		agrmtno = @agreementNo

	IF(@@rowcount = 0)
	BEGIN
		SET @return = -1
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

