SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_SRCustomerUpdateForCashGoSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_SRCustomerUpdateForCashGoSP]
GO

-- =============================================
-- Author:		Jez Hemans
-- Create date: 26-Sept-2008
-- Description:	Updates/inserts customer details for a Cash & Go SR where the customer has no invoice
-- =============================================


CREATE PROCEDURE 	dbo.DN_SRCustomerUpdateForCashGoSP
            @custid varchar (20),
			@lastname varchar(60) ,
			@firstname varchar(30) ,
			@title varchar(25) ,
			@address1       varchar(50),
            @address2       varchar(50),
            @address3       varchar(50),
            @postcode       varchar(10),
            @directions     NVARCHAR(1000),
            @hometel        VARCHAR(20),
            @worktel        VARCHAR(20),
            @mobile         VARCHAR(20),
            @branchNo       INT,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	UPDATE	customer
	SET		name		=	@lastname,
			firstname	=	@firstname,
			title		=	@title
	WHERE	custid = @custid

	IF(@@rowcount=0 and @@error = 0)
	BEGIN
		INSERT
		INTO	customer 	(
					custid,
					name,
					firstname,
					title,
					branchnohdle,
					morerewardsno,
					IdNumber,
					IdType,
					creditblocked,
					RFCreditLimit,
					RFCardSeqNo,
					RFCardPrinted,
					empeenochange,
					OldRFCreditLimit,
					LimitType,
					AvailableSpend,
					dependants,
					maritalstat,
					Nationality,
					InstantCredit,
					StoreType,
					LoanQualified
					)
		VALUES		(
					@custid ,
					@lastname,
					@firstname,
					@title,
					@branchNo,
					'',
					'',
					'',
					0,
					0,
					0,
					'N',
					0,
					0,
					'',
					0,
					0,
					'',
					'',
					'N',
					'C',
					0)
	END	


    UPDATE	custaddress
	SET		cusaddr1	=	@address1,
			cusaddr2	=	@address2,
			cusaddr3	=	@address3,
			cuspocode   =   @postcode,
			Notes       =   @directions
	WHERE	custid = @custid
	
	IF(@@rowcount=0 and @@error = 0)
	BEGIN
		INSERT INTO custaddress (
		    custid,
			cusaddr1,
			cusaddr2,
			cusaddr3,
			cuspocode,
			Notes,
			addtype,
			datein,
			hasstring,
			Email,
			PropType,
			empeenochange
		) VALUES ( 
			@custid,@address1,@address2,@address3,@postcode,@directions,'H',' ',0,'','',0) 
	END
		
	UPDATE custtel
	SET telno = @hometel
	WHERE custid = @custid AND tellocn = 'H'
			
	IF(@@rowcount=0 and @@error = 0 AND @hometel <> '')
	BEGIN
		INSERT INTO custtel (
		    custid,
			tellocn,
			telno,
			dateteladd,
			DialCode
		) VALUES ( 
			@custid,'H',@hometel,' ','' ) 
	END
			
	UPDATE custtel
	SET telno = @worktel
	WHERE custid = @custid AND tellocn = 'W'
			
	IF(@@rowcount=0 and @@error = 0 AND @worktel <> '')
	BEGIN
		INSERT INTO custtel (
		    custid,
			tellocn,
			telno,
			dateteladd,
			DialCode
		) VALUES ( 
			@custid,'W',@worktel,' ','' ) 
	END
	
	UPDATE custtel
	SET telno = @mobile
	WHERE custid = @custid AND tellocn = 'M'
			
	IF(@@rowcount=0 and @@error = 0 AND @mobile <> '')
	BEGIN
		INSERT INTO custtel (
		    custid,
			tellocn,
			telno,
			dateteladd,
			DialCode
		) VALUES ( 
			@custid,'M',@mobile,' ','' ) 
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

