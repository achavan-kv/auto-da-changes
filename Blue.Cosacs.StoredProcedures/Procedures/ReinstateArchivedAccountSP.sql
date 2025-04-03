-- ============================================================================================
-- Author:		Alex Ayscough
-- Modified by: Ilyas Parker
-- Modified date: 03/02/2009
-- Description:			
-- ============================================================================================


IF EXISTS (SELECT * FROM sysobjects WHERE NAME = 'ReinstateArchivedAccountSP')
DROP PROCEDURE ReinstateArchivedAccountSP 
GO
-- This gives the ability for the user to reinstate the archived account
CREATE PROCEDURE ReinstateArchivedAccountSP 
				 @acctno CHAR(12),
				 @archivedAcct bit, --IP - CR971
				 @unsettleAcct bit, --IP - CR971
				 @return INT OUTPUT

AS 

BEGIN
	SET @return =0
			
	DECLARE @maxSettledDate datetime,
			@maxBeforeSettledDate datetime,
			@statusPriorSettled varchar,
			@count int,
			@noPrevStatus bit

	--IP - CR971 - If option 'Unarchive' selected on an archived
	--account then unarchive BUT DO NOT Unsettle.
	--and if option 'Unsettle' selected, then 'Unarchive' and 'Unsettle'.
	IF((@archivedAcct = 1 AND @unsettleAcct = 0 ) 
			OR (@archivedAcct = 1 AND @unsettleAcct = 1))
	BEGIN
		DECLARE @statement SQLText,
				@archive_table VARCHAR(32)

		-- what we are doing here is to load up the archive tables and then move across back to the original tables
		-- so first retrieve all archive tables from the archive database
		DECLARE acct_cursor CURSOR FAST_FORWARD READ_ONLY FOR
--		SELECT archivetable 
--		FROM cosacs_archive.information_schema.columns WHERE table_name LIKE '%_archive' AND
--		column_name = 'acctno'
--		and table_name not in ('vw_CashandGo_Archive', 'CashandGo_Archive', 'acct_archive')
		select archivetable from UnarchiveOrder order by sort
		OPEN acct_cursor
		FETCH NEXT FROM acct_cursor INTO @archive_table
		WHILE @@FETCH_STATUS = 0
		
		BEGIN
			EXEC ReinstateArchiveAccounttableSP @archive_table = @archive_table,@acctno=@acctno
			FETCH NEXT FROM acct_cursor INTO @archive_table
		END

		CLOSE acct_cursor
		DEALLOCATE acct_cursor

		--Delete the 'Acct_archive' table from Live as the account is no longer archived.
		DELETE FROM Acct_Archive WHERE acctno =@acctno
	END

	--If the option 'Unsettle' selected on an archived account
	--then unarchive and unsettle.
	--If selected on an account not archived, just unsettle the account and restore status to previous
	--non 'U', '0' or 'S' status if exists.
	IF((@archivedAcct = 1 AND @unsettleAcct = 1)
			OR (@archivedAcct = 0 AND @unsettleAcct = 1))

	BEGIN --IF((@archivedAcct = 1 AND @unsettleAcct = 1) OR (@archivedAcct = 0 AND @unsettleAcct = 1))
		
		IF EXISTS(SELECT * FROM status 
					WHERE acctno = @acctno)
		BEGIN --IF EXISTS(SELECT * FROM status WHERE acctno = @acctno)
		
			set @count = 0
			set @noPrevStatus = 0 

			--IP - Could maybe use the following to replace the below if the 
			--code to un-settle an account does not work.
--			declare @status char(1)
--			select @status=statuscode from dbo.status s
--			where s.acctno='720004196501' 
--			and datestatchge=(select max(datestatchge) from status s2 
--			where s2.statuscode not in('U','0','S') and s2.acctno=s.acctno)
--			select @status
--			if @status is null
--			select @status=statuscode from dbo.status s
--			where s.acctno='720004196501' 
--			and datestatchge=(select max(datestatchge) from status s2 
--			where s2.statuscode not in('S') and s2.acctno=s.acctno)
--			select @status
--			set @status =coalesce(@status,'1') --@status
--			select @status

			--When un-settling an account to its previous status prior to being settled
			--continue to loop through until a non '0', 'U' or 'S' status is found
			--and set the status to this.
			--If the only prior status's are '0' or 'U' then set the status to this.
			--If there are no prior status's to 'S' when un-settling, set the status to '1'.
			WHILE ((@statusPriorSettled in ('U','0','S')
			and exists 
				(SELECT count(*) from status 
					where acctno = @acctno 
					AND datestatchge < @maxBeforeSettledDate having count(*) > 0) OR @count = 0)
				)

				BEGIN --While begin
					
					SET @count = @count + 1
					
					--First time entered this loop select the date settled.
					IF (@count = 1)
					BEGIN
						SELECT @maxSettledDate = (SELECT max(s.datestatchge)
												  FROM status s
												  WHERE s.acctno = @acctno
												  AND s.statuscode = 's')
					END
					--The status prior to being settled was either 'U' or '0'
					ELSE
					BEGIN

						SELECT @maxSettledDate = @maxBeforeSettledDate

					END
					
					--select the max date before the settled date
					SELECT @maxBeforeSettledDate = (SELECT max(s.datestatchge)
														FROM status s
														WHERE s.datestatchge < @maxSettledDate
														AND s.acctno = @acctno)
					--if the date before settled is null then there were no prior status's to the account being settled.
					--therefore update the accounts status to '1' when re-opening.
					IF(@maxBeforeSettledDate is null)
					BEGIN
						UPDATE acct SET currstatus = '1'
						WHERE ACCTNO = @acctno
						SET @noPrevStatus = 1
					END
					ELSE
					BEGIN
						SELECT @statusPriorSettled  = (SELECT distinct statuscode
														  FROM status
														  WHERE datestatchge = @maxBeforeSettledDate
														  AND acctno = @acctno) 
					END
				END ----While end
		
			--Only process this update if there were prior status records for the account.
			IF(@noPrevStatus = 0)
			BEGIN
				UPDATE acct
				SET acct.currstatus = @statusPriorSettled
				WHERE acct.acctno = @acctno
			END
		END ----IF EXISTS(SELECT * FROM status WHERE acctno = @acctno)
		ELSE
		PRINT 'Cannot unsettle as no status records found'		
	END ----IF((@archivedAcct = 1 AND @unsettleAcct = 1) OR (@archivedAcct = 0 AND @unsettleAcct = 1))
END

GO
