IF  EXISTS (SELECT * FROM sysobjects WHERE name = 'CustTelSave' AND type in ('P', 'PC'))
DROP PROCEDURE [dbo].[CustTelSave]
GO

/****** Object:  StoredProcedure [dbo].[CustTelSave]    Script Date: 07/07/2008 11:25:36 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : CustTelSave.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Customer Telephone Save  
-- Author       : ??
-- Date         : ??
--
-- This procedure will save the customer telephone details
-- Change Control
-- --------------
-- Version	Date      By  Description
-- ----      --  -----------
-- 1.0		18/02/10 jec CR1072 Malaysia v4 merge
-- 2.0		10/6/2020 Snehalata Tilekar Address Standardization CR2019 - 025 -Added Multiple delivery mobile to keep dilacode value empty. 
-- ================================================
CREATE PROCEDURE [dbo].[CustTelSave]
	@custid			VARCHAR(20),
	@tellocn		CHAR(3),
	@dateteladd		DATETIME,
	@telno			VARCHAR(20),
	@extnno			VARCHAR(6),
	@DialCode		CHAR(8),
	@empeenochange	INT,
	@newRecord		BIT,
	@return			INT out
AS
	SET @return = 0
	IF @newRecord = 1

BEGIN
	-- CR1072
	SET NOCOUNT ON
	IF @dateteladd='1900-01-01' 
	BEGIN
		set @dateteladd=GETDATE()
	END	
	UPDATE custtel
		SET datediscon = @dateteladd,
			datechange = GETDATE(),
			empeenochange = @empeenochange
			WHERE custid = @custid AND
					tellocn = @tellocn AND 
					ISNULL(datediscon,'')=''

            -- Insert the amended address
            INSERT INTO custtel
                (origbr,
                 custid,
                 tellocn,
                 dateteladd,
                 datediscon,
                 telno,
                 extnno,
                 DialCode,
                 empeenochange,
                 datechange)
            VALUES
                (null,
                 @custID,
                 @tellocn,
                 @dateteladd,
                 null,
                 @telno,
                 @extnno,
                 CASE
					WHEN @tellocn IN ('M','DM','D1M','D2M','D3M') THEN ' ' --Address Standardization CR2019 - 025					
					ELSE @DialCode END, -- IP - 26/10/09 - UAT(113)
                 @empeenochange,
                 GETDATE())
END
ELSE
BEGIN
	-- update, if no record count insert
    -- Insert the amended address
    update custtel
		SET dateteladd = @dateteladd,
			telno = @telno,
			extnno = @extnno,
			DialCode= CASE
				WHEN @tellocn IN ('M','DM','D1M','D2M','D3M') THEN ' '	--Address Standardization CR2019 - 025			
				ELSE @DialCode END, --IP - 26/10/09 - UAT(113)
			empeenochange = @empeenochange,
			datechange =  getdate()
	WHERE custid = @custid AND 
			tellocn = @tellocn AND 
			ISNULL(datediscon,'')=''

	IF @@ROWCOUNT = 0
	BEGIN
	            -- Insert the amended address
                        INSERT INTO custtel
                (origbr,
                 custid,
                 tellocn,
                 dateteladd,
                 datediscon,
                 telno,
                 extnno,
                 DialCode,
                 empeenochange,
                 datechange)
            VALUES
                (null,
                 @custID,
                 @tellocn,
                 @dateteladd,
                 null,
                 @telno,
                 @extnno,
                 CASE
					WHEN @tellocn IN ('M','DM','D1M','D2M','D3M')  THEN ' ' --Address Standardization CR2019 - 025
					
				ELSE @DialCode END, -- IP - 26/10/09 - UAT(113) previously was attempting to insert null.
                 @empeenochange,
                 GETDATE())
	END
END

	IF (@@error != 0)
    BEGIN
        SET @return = @@error
    END	



GO
-- End End End End End End End End End End End End End End End End End End End End End End End End End End