
/****** Object:  StoredProcedure [dbo].[DN_TelephoneActionUpdateCustTelephone]    Script Date: 01/02/2009 07:31:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- ============================================================================================
-- Author:		Nasmi Mohamed
-- Create date: 30/12/2008
-- Description:	The procedure will update customers telephone numbers from the Telephone 
--				Action screen, or insert a new record if one does not exist.		
-- ============================================================================================
if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_TelephoneActionUpdateCustTelephone]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_TelephoneActionUpdateCustTelephone]
GO


CREATE PROCEDURE		[dbo].[DN_TelephoneActionUpdateCustTelephone]
			@custid varchar(20),
			@Htelno varchar(20),
		    @Hdialcode varchar(8),
			@HChanged bit,
			@Wtelno varchar(20),
		    @Wdialcode varchar(8),
			@WChanged bit,
			@Mtelno varchar(20),
		    @Mdialcode varchar(8),
			@MChanged bit,
			@Empeeno int,
			@return int OUTPUT
AS

	SET 	@return = 0			
	
	IF (@HChanged = 1)
	BEGIN 
		----Updating Home Telephone---------------------
		UPDATE	custtel
		SET		telno = @Htelno,
				DialCode = @Hdialcode,
				empeenochange = @Empeeno,
				datechange = getdate()
		WHERE	custid = @custid
		AND		tellocn = 'H'
		AND		datediscon is null

		IF(@@rowcount = 0) --If record does not exist
		----Inserting Home Telephone--------------------
		BEGIN
			INSERT
			INTO  custtel
					(custid, tellocn, dateteladd,telno, 
					 extnno, DialCode, empeenochange, datechange)
			VALUES	(@custid, 'H', GetDate(),@Htelno, 
					 '', @Hdialcode,@Empeeno, GetDate())
		END	
		------------------------------------------------		
	END

	
	IF (@WChanged = 1)
	BEGIN 
		----Updating Work Telephone---------------------
		UPDATE	custtel
		SET		telno = @Wtelno,
				DialCode = @Wdialcode,
				empeenochange = @Empeeno,
				datechange = getdate()
		WHERE	custid = @custid
		AND		tellocn = 'W'
		AND		datediscon is null

		IF(@@rowcount = 0) --If record does not exist
		----Inserting Work Telephone--------------------
		BEGIN
			INSERT
			INTO  custtel
					(custid, tellocn, dateteladd,telno, 
					 extnno, DialCode, empeenochange, datechange)
			VALUES	(@custid, 'W', GetDate(),@Wtelno, 
					 '', @Wdialcode,@Empeeno, GetDate())
		END	
		------------------------------------------------
	END

	IF (@MChanged = 1)
	BEGIN 
		----Updating Mobile Telephone---------------------
		UPDATE	custtel
		SET		telno = @Mtelno,
				DialCode = @Mdialcode,
				empeenochange = @Empeeno,
				datechange = getdate()
		WHERE	custid = @custid
		AND		tellocn = 'M'
		AND		datediscon is null

		IF(@@rowcount = 0) --If record does not exist
		----Inserting Mobile Telephone--------------------
		BEGIN			
			INSERT
			INTO  custtel
					(custid, tellocn, dateteladd,telno, 
					 extnno, DialCode, empeenochange, datechange)
			VALUES	(@custid, 'M', GetDate(),@Mtelno, 
					 '', @Mdialcode,@Empeeno, GetDate())
		END	
		------------------------------------------------	
	END


	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END






