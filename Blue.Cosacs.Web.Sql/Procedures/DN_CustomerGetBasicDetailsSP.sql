-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_CustomerGetBasicDetailsSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
	drop procedure [dbo].[DN_CustomerGetBasicDetailsSP]
GO

-- =======================================================================================
-- Project			: CoSaCS.NET
-- Procedure Name   : [dbo].[DN_CustomerGetBasicDetailsSP]
-- Author			: 
-- Create Date		: 
-- Description		: 
-- Version			: 002					  

-- Change Control
-- --------------
-- Ver	Date			By				Description
-- ---	----			--				-----------
-- 002  05 Feb 2021		Amit (Zensar)	Populating IsSpouseWorking, dependant to calculate Disposible income.
-- =======================================================================================

CREATE PROCEDURE 	[dbo].[DN_CustomerGetBasicDetailsSP]
			@custID varchar(20),
			@acctNo varchar(12),
			@relationship varchar(2),
			@title varchar(25) OUT,
			@firstName varchar(30) OUT,
			@lastName varchar(60) OUT,	
			@alias varchar(25) OUT,
			@budgetCard int OUT,		
			@custout varchar(20) OUT,
			@RFLimit money OUT,
			@idnumber varchar(30) OUT,
			@dateborn datetime OUT,
			@RFAvailable money OUT,
			@maidenname varchar(30) OUT,
			@sex char(1) OUT,
			@morerewardsno varchar(16) OUT,
			@rfcardseqno tinyint OUT,
			@oldrfcreditlimit money OUT,
			@LimitType char(1) OUT,
			@ScoringBand char(1) OUT,
			@StoreType varchar(2) OUT,
			@LoanQualified bit OUT,
			@dependants int OUT,
			@maritalStat char(1) OUT,
			@nationality char(4) OUT,
			@blacklisted bit OUT, --IP - 28/08/09 - 5.2 UAT(823)
			@StoreCardLimit MONEY OUT,
			@StoreCardAvailable MONEY OUT,
			@StoreCardApproved BIT OUT ,
			@cashLoanBlocked varchar(1) OUT,	--IP - 03/11/11 - CR1232 - Cash Loans
			@ResieveSms Bit OUT,
			@DELTitleC varchar(25) OUT,
			@DELFirstName varchar(30) OUT,
			@DELLastName varchar(60) OUT,
			@IsSpouseWorking Bit OUT,
			@dependantsFromProposal int OUT,
			@return int OUTPUT
AS

	SET @return = 0			--initialise return code
	DECLARE @count int
	SET	@count = 1

	--set nocount on
	
	IF(isnull(@acctNo,'') != '' )
	BEGIN
		SELECT	@custID = custid
		FROM		custacct
		WHERE	acctno = @acctNo
		AND		hldorjnt = @relationship
		SET		@count = @@rowcount
	END
	ELSE
	BEGIN
		SELECT	@custID = linked
		FROM		customerlinks
		WHERE	relationship = @relationship
		AND		holder = @custID
		SET		@count = @@rowcount

		IF(@count=0 and @relationship = 'H')
			SET	@count = 1		
	END	
	
	IF(@count>0)
	BEGIN
		IF EXISTS	(SELECT	1
				FROM		custcatcode
				WHERE	custid = @custID
				AND		code = 'BCC')
		BEGIN
			SET	@budgetCard = 1
		END
		ELSE
		BEGIN
			SET	@budgetCard = 0
		END

		IF EXISTS (SELECT 1 
		           FROM custcatcode
				   WHERE custid = @custid AND code = 'R' AND datedeleted is null)
			SET @blacklisted = 1
		ELSE
			SET @blacklisted = 0

		EXEC DN_CustomerGetRFLimitSP @custID, '', @RFLimit OUT, @RFAvailable OUT, @return OUT

		---Add Bhupesh
		if @acctNo = ''
		Begin
			IF EXISTS(SELECT 1 FROM [dbo].[proposal] WHERE custid = @custID)
				BEGIN
					SELECT	TOP 1 @IsSpouseWorking = ISNULL(IsSpouseWorking, 0)
							, @dependantsFromProposal = ISNULL(dependants, 0) 
					FROM	[dbo].[proposal] 
					WHERE	custid = @custID 
							--AND acctno = @acctNo
					ORDER BY DateProp DESC 
				END
				ELSE 
				BEGIN
					SET @IsSpouseWorking = 0
					SET @dependantsFromProposal = 0
				END
		END
		ELSE
		Begin

			IF EXISTS(SELECT 1 FROM [dbo].[proposal] WHERE custid = @custID AND acctno = @acctNo)
			BEGIN
				SELECT	TOP 1 @IsSpouseWorking = ISNULL(IsSpouseWorking, 0)
						, @dependantsFromProposal = ISNULL(dependants, 0) 
				FROM	[dbo].[proposal] 
				WHERE	custid = @custID 
						AND acctno = @acctNo
				ORDER BY DateProp DESC 
			END
			ELSE 
			BEGIN
				SET @IsSpouseWorking = 0
				SET @dependantsFromProposal = 0
			END
		END
		--Ended By Bhupesh

		SELECT	@custout = c.custid,
				@title = c.title,
				@firstName = c.firstname,
				@lastName = name,
				@DELTitleC = cadd.DELTitleC,
				@DELFirstName = cadd.DELFirstName,
				@DELLastName = cadd.DELLastName,
				@alias = alias,
				@RFLimit = RFCreditLimit,
				@idnumber = idnumber,  
				@dateborn = dateborn,
				@RFAvailable = @RFAvailable,
				@maidenname = maidenname,
				@sex = sex,
				@morerewardsno = morerewardsno,
				@rfcardseqno = rfcardseqno,
				@oldrfcreditlimit = oldrfcreditlimit,
				@LimitType = LimitType,
				@ScoringBand = isnull(ScoringBand,''),
				@StoreType = StoreType,
				@LoanQualified = LoanQualified,
				@dependants = dependants ,
				@maritalStat = maritalStat, 
				@nationality = nationality,
				@StoreCardLimit = isnull(StoreCardLimit,0),
				@StoreCardAvailable =  CASE WHEN c.AvailableSpend - ISNULL(C.StoreCardAvailable,0) >0  THEN
				CASE WHEN  C.StoreCardAvailable >0 THEN  C.StoreCardAvailable ELSE 0 END ELSE  
				CASE WHEN  C.AvailableSpend >0 THEN c.AvailableSpend ELSE 0 END END,
				@StoreCardApproved=isnull(StoreCardApproved,0),
				@cashLoanBlocked = ISNULL(CashLoanBlocked,''),
				@ResieveSms = ISNULL(ResieveSms, 1)
		FROM	customer c 
				LEFT JOIN Service.Request r ON r.CustomerId=c.custid AND r.Account=@acctNo
				LEFT JOIN custaddress cadd ON cadd.custid=r.CustomerId AND r.addtype=cadd.addtype
		WHERE	c.custid = @custID


		IF(@@rowcount = 0)
			SET	@return = -1
	END
	ELSE
		SET	@return = -1

	IF (@@error != 0)
		SET @return = @@error

