IF EXISTS (SELECT * FROM sysobjects WHERE TYPE = 'P' AND NAME = 'DN_InstalPlanUpdateSP')
DROP PROCEDURE DN_InstalPlanUpdateSP
GO

CREATE PROCEDURE 	[dbo].[DN_InstalPlanUpdateSP]
			@origBr smallint,
			@acctNo varchar(12),
			@agreementNo int,
			@dateFirst datetime,
			@instalno int,
			@instalFreq char(1),
			@dateLast datetime,
			@instalAmount money,
			@finalInstal money,
			@instalTotal money,
			@monthIntFree smallint,
			@empeenochange int,
			@dueday smallint,
			@scoringband varchar(1),
			@autoda BIT = 0,
			@PrefInstalmentDay int,  --Added for 10.7 Feature CR new column for accepting the preference day for Instalment
			@return int OUTPUT

AS
	-- ================================================
	-- Project      : CoSACS .NET
	-- File Name    : DN_InstalPlanUpdateSP
	-- File Type    : MSSQL Server Stored Procedure Script
	-- Title        : DN_InstalPlanUpdateSP
	-- Author       : ??
	-- Date         : 
	-- Version		: 002
	-- 
	-- This procedure store the Instalment related information
	-- 
	-- Change Control
	-- --------------
	-- Date      By  Description
	-- ----      --  -----------
	-- 15/7/2020 Rahul Sonawane 10.7 CR - added new column(prefInstalmentDay) and stored the data. 
	-- ================================================		
	SET 	@return = 0			--initialise return code

	UPDATE	instalplan
	SET		origbr			=	@origBr ,		
			acctno			=	@acctNo ,
			agrmtno			=	@agreementNo ,	
			datefirst		=	@dateFirst ,	  
			instalno		=	@instalno ,	
			instalfreq		=	@instalFreq ,	
			datelast		=	@dateLast ,	 
			instalamount	=	@instalAmount ,	
			fininstalamt	=	@finalInstal ,	
			instaltot		=	@instalTotal ,	
			mthsintfree		=	@monthIntFree ,
			empeenochange	= 	@empeenochange,
			dueday			=	@dueday,
			scoringband     = ltrim(rtrim(@scoringband)),
			autoda  		= 	@autoda,
			PrefInstalmentDay= 	@PrefInstalmentDay  
	WHERE	acctno = @acctNo

	IF(@@rowcount = 0)	--new instal plan
	BEGIN
		INSERT
		INTO	instalplan 	(origbr,
					acctno,
					agrmtno,
					datefirst,
					instalno,
					instalfreq,
					datelast,
					instalamount,
					fininstalamt,
					instaltot,
					mthsintfree,
					empeenochange,
					dueday,
					scoringband,
					autoda,
					PrefInstalmentDay		
					)
		VALUES	(@origBr ,				
				@acctNo ,	
				@agreementNo ,	
				@dateFirst ,	
				@instalno ,	
				@instalFreq ,	
				@dateLast ,	
				@instalAmount ,	
				@finalInstal ,	
				@instalTotal ,	
				@monthIntFree,
				@empeenochange,
				@dueday,
				ltrim(rtrim(@scoringband)),
				@autoda,
				@PrefInstalmentDay  
				)
	END


	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
	
	--IF NOT EXISTS(select * from CLAmortizationSchedule where acctno=@acctNo)
	--begin
	--	exec DN_SaveAmortizedCLScheduleSP @acctNo,@instalAmount,@instalno,@scoringband,@return output
	--end



