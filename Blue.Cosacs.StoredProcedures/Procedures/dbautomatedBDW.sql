SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[dbautomatedBDW]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[dbautomatedBDW]
GO	

-- ============================================================================================
-- Author:		David Richardson
-- Create date: 05/05/2006
-- Modified :   11/12/2020
-- Version :  1.0
-- Description:	Procedure that automatically writes accounts off through the 'Automated Bad Debt Writeoff' End Of Day option.
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 22/12/09  IP  UAT5.2 (948) - Accounts written off should have their Acct.Arrears set to '0'.	
-- 17/09/10  IP  CR1107 - Write Off Review Screen Enhancements - For accounts generated for the manual rule code (MRA)
--			 these accounts should NOT  be automatically written off.
-- ============================================================================================

CREATE PROCEDURE [dbo].[dbautomatedBDW]
		@ContainsWarnings bit output,
         @return int OUTPUT
			
AS
    SET @ContainsWarnings = 0
    SET 	@return = 0			--initialise return code
	DECLARE	@acctno varchar(12), 
			@charges money, 
			@empeeno int,
			@transvalue money,
			@refno int,
			@bdwvalue money,
			@status integer, 
			@query_text varchar(1000),
			@interface varchar(10),
			@runno int,
			@branchno smallint,
			@dorun varchar(1),
			@dodefault varchar(1),
			@numprocessed int,
			@totalvalue money,
			@iCount int = 1,
			@TotalCount int =0,
			@SelectedAcctno varchar(12)


    SET @interface = 'AUTOBDW'
	SET 	@status = 0
    set @runno=0
	--SET	@interface = 'BDWFINAL'
	
/* CR781 - code not required
	BEGIN TRAN

	SELECT	@dorun = donextrun,
			@dodefault = dodefault
	FROM		eodcontrol
	WHERE	interface = @interface

     	IF @dorun = 'Y'
     	BEGIN
		SET @query_text =N'create interface control record'
		EXEC DN_InterfaceAddSP	@interface = @interface,
						@runno = @runno OUTPUT,
						@return = @status OUTPUT
	END

	COMMIT TRAN
*/
    -- Get run number 
    set @runno=(Select max(runno) from interfacecontrol
                    where interface=@interface)
    
    If @runno = 0
        Begin
            SET @query_text = convert(varchar,@status) + ' ' + 'RunNo Error'
            raiserror (@query_text,1,1)

	    END

	BEGIN TRAN

	CREATE TABLE #bdwaccts(acctno varchar(12),
	                       		      charges money,
			       	      repovalue money,
			       	      bdwvalue money,
			       	      wopercentage float,
			       	      age int,
			       	      empeeno int,
	                       		      code varchar(5),
			       	      wodate datetime,
				      intcharges money,
				      admcharges money,
					  id int IDENTITY(1,1))

	SELECT	@branchno = hobranchno
	FROM 		country

	SET @status = @@error

     	IF @status = 0  -- AND @dorun = 'Y'
     	BEGIN
	    	SET @query_text =N'Stage 1 - Initial Select'
		INSERT INTO 	#bdwaccts
		SELECT	acctno,
			 	0,
				0,
				0,
				0,
				0,
				empeeno,
				code,
				getdate(),
				0,
				0
		FROM		BDWPending
		WHERE	runno = 0
	     	SET @status =@@error
	END
	
		
     	IF @status = 0   --  AND @dorun = 'Y'
     	BEGIN
        	SET 	@query_text =N'Stage 2 - Delete accounts not authorised for write off'
		DELETE FROM		#bdwaccts
		WHERE EXISTS	(SELECT acctno
				 	 FROM BDWPending
				 	 WHERE BDWPending.acctno = #bdwaccts.acctno
				  	 AND (BDWPending.empeeno = 0
				 	 AND BDWPending.empeenomanual != 0
				 	 OR(BDWPending.code='MRA' AND dbo.BDWPending.Empeeno = 0 AND BDWPending.empeenomanual=0))) --IP - 17/09/10 - CR1107  
     		SET @status =@@error
		END

     	IF @status = 0  --  AND @dorun = 'Y'
     	BEGIN
        	SET 	@query_text =N'Stage 3 - Calculate charges, bdw value'
		SELECT	f.acctno,
				ISNULL(SUM(f.transvalue), 0) as value
		INTO		#intreverse
		FROM 		#bdwaccts b, fintrans f
		WHERE	b.acctno = f.acctno
		AND		transtypecode = 'INT'
		GROUP BY 	f.acctno
		
		SELECT	f.acctno,
				ISNULL(SUM(f.transvalue), 0) as value
		INTO		#admreverse
		FROM 		#bdwaccts b, fintrans f
		WHERE	b.acctno = f.acctno
		AND		transtypecode = 'ADM'
		GROUP BY 	f.acctno

		UPDATE	#bdwaccts
		SET 		intcharges = i.value
		FROM		#intreverse i
		WHERE	#bdwaccts.acctno = i.acctno

		UPDATE	#bdwaccts
		SET 		admcharges = a.value
		FROM		#admreverse a
		WHERE	#bdwaccts.acctno = a.acctno

		UPDATE	#bdwaccts
		SET 		charges = intcharges + admcharges
		
		UPDATE	#bdwaccts
		SET 		bdwvalue = a.outstbal
		FROM		acct a
		WHERE	#bdwaccts.acctno = a.acctno

		UPDATE	#bdwaccts
		SET 		bdwvalue = bdwvalue - charges

     		SET @status =@@error
	END

     	IF @status = 0  -- AND @dorun = 'Y'
     	BEGIN
        	SET 	@query_text =N'Stage 4 - Set age of account'
		UPDATE	#bdwaccts
		SET 		age = DATEDIFF(month, a.dateacctopen, b.wodate)
		FROM		#bdwaccts b, acct a
		WHERE	b.acctno = a.acctno
     		SET @status =@@error
	END
     	
     	IF @status = 0  -- AND @dorun = 'Y'
     	BEGIN
        	SET 	@query_text =N'Set % of agreement write off'
		UPDATE	#bdwaccts
		SET 		wopercentage = bdwvalue / a.agrmttotal * 100
		FROM		#bdwaccts b, agreement a
		WHERE	b.acctno = a.acctno
		AND		a.agrmttotal > 0
     		SET @status =@@error
	END

   	IF @status = 0  -- AND @dorun = 'Y'
	BEGIN
        	SET 	@query_text =N'Set accounts to SC 6'
		UPDATE	acct
		SET		currstatus = '6',
				lastupdatedby = 99999
		FROM		acct a, #bdwaccts b
		WHERE	a.acctno = b.acctno
		AND		a.currstatus != '6'
	     	SET @status =@@error
	END

	IF @status = 0  -- AND @dorun = 'Y'
     	BEGIN
        	SET 	@query_text =N'Stage 5 - Write off accounts'
		/* ---11-Dec-2020
		UPDATE	acct
		SET 	currstatus = 'S',
				outstbal = 0,
				arrears = 0, --IP - 22/12/09 - UAT(948)
				bdwbalance = b.bdwvalue,
				bdwcharges = b.charges,
				lastupdatedby = 99999
		FROM		#bdwaccts b
		WHERE	acct.acctno = b.acctno
		*/
		--Loop through table and call stored procedure to update account---------------
		SELECT @TotalCount= count(id) FROM #bdwaccts b WHERE acctno = b.acctno
		WHILE (@iCount<= @TotalCount)
		BEGIN
		------Fetch one by one accounts and check if it is for -------------------
			SELECT @SelectedAcctno= acctno FROM #bdwaccts b WHERE acctno = b.acctno AND id = @iCount
			-- Check if account is of type AmortizedOutStandingBal.
			IF EXISTS (SELECT acctno FROM acct WHERE acctno = @SelectedAcctno AND IsAmortizedOutStandingBal = 1)
			BEGIN
				DECLARE	@return_value int,
				@Procreturn int
				EXEC	[dbo].[CLAmortizationWriteOffAccountBalance]
				@acctno = @SelectedAcctno,
				@Procreturn = @return OUTPUT
			END 
			SET @iCount = @iCount + 1
		END
     		SET @status =@@error
	END

     	IF @status = 0  -- AND @dorun = 'Y'
     	BEGIN
        	SET	@query_text =N'Stage 6 - Reverse charges'
		DECLARE admin_cursor CURSOR FOR 
		SELECT	acctno,
				admcharges,
				empeeno
		FROM		#bdwaccts	 
		
		OPEN admin_cursor
		
		FETCH NEXT FROM admin_cursor 
		INTO @acctno, @charges, @empeeno
		
		WHILE @@FETCH_STATUS = 0
		BEGIN
			EXECUTE @refno = nexttransrefno @branchno = @branchno, @numrefs= 1
		
			SET @transvalue = 0 - @charges
			
			INSERT INTO fintrans(branchno, acctno, transrefno, datetrans, transtypecode, empeeno,
		                             transupdated, transprinted, transvalue, runno, source)
		        	VALUES(@branchno, @acctno, @refno, getdate(), 'ADM', @empeeno, 'N', 'N',
		               		@transvalue, 0, 'COSACS');

			
			FETCH NEXT FROM admin_cursor 
		
			INTO @acctno, @charges, @empeeno
		END
		CLOSE admin_cursor
		DEALLOCATE admin_cursor

		SET @charges = 0
     		SET @status =@@error
	END	
	
     	IF @status = 0  -- AND @dorun = 'Y'
     	BEGIN
        	SET	@query_text =N'Stage 6 - Reverse charges'
		DECLARE interest_cursor CURSOR FOR 
		SELECT	acctno,
				intcharges,
				empeeno
		FROM		#bdwaccts	 
		
		OPEN interest_cursor
		
		FETCH NEXT FROM interest_cursor 
		INTO @acctno, @charges, @empeeno
		
		WHILE @@FETCH_STATUS = 0
		BEGIN
			EXECUTE @refno = nexttransrefno @branchno = @branchno, @numrefs= 1
		
			SET @transvalue = 0 - @charges
			
			INSERT INTO fintrans(branchno, acctno, transrefno, datetrans, transtypecode, empeeno,
		                             transupdated, transprinted, transvalue, runno, source)
		        	VALUES(@branchno, @acctno, @refno, getdate(), 'INT', @empeeno, 'N', 'N',
		               		@transvalue, 0, 'COSACS');

			
			FETCH NEXT FROM interest_cursor 
		
			INTO @acctno, @charges, @empeeno
		END
		CLOSE interest_cursor
		DEALLOCATE interest_cursor
     		SET @status =@@error
	END	

     	IF @status = 0  -- AND @dorun = 'Y'
     	BEGIN
        	SET 	@query_text =N'Stage 7 - Post bdw to fintrans'
		    DECLARE bdw_cursor CURSOR FOR 
		    SELECT	acctno,
				bdwvalue,
				empeeno
		    FROM		#bdwaccts	 
		
		    OPEN bdw_cursor
		
		    FETCH NEXT FROM bdw_cursor 
		    INTO @acctno, @bdwvalue, @empeeno
		
		    WHILE @@FETCH_STATUS = 0
		    BEGIN

			    EXECUTE @refno = nexttransrefno @branchno = @branchno, @numrefs= 1

                Declare @BduRebate money

                EXECUTE CalculateBduRebate @acctno, @BduRebate OUT, @return

                SET @BduRebate = 0 - ISNULL(@BduRebate,0)
		
			    SET @transvalue = (0 - @bdwvalue) - @BduRebate

			    INSERT INTO fintrans(branchno, acctno, transrefno, datetrans, transtypecode, empeeno,
		                                 transupdated, transprinted, transvalue, runno, source)
		        	    VALUES(@branchno, @acctno, @refno, getdate(), 'BDW', @empeeno, 'N', 'N',
		               		    @transvalue, 0, 'COSACS');

                EXECUTE @refno = nexttransrefno @branchno = @branchno, @numrefs= 1

                INSERT INTO fintrans(branchno, acctno, transrefno, datetrans, transtypecode, empeeno,
		                                 transupdated, transprinted, transvalue, runno, source)
		        	    VALUES(@branchno, @acctno, @refno, getdate(), 'BDU', @empeeno, 'N', 'N',
		               		    @BduRebate, 0, 'COSACS');

			    FETCH NEXT FROM bdw_cursor 
	
			    INTO @acctno, @bdwvalue, @empeeno
		    END
		    CLOSE bdw_cursor
		    DEALLOCATE bdw_cursor
     		    SET @status =@@error
	    END

    IF @status = 0  -- AND @dorun = 'Y'
     	BEGIN
        	SET 	@query_text =N'Stage 5 - Write off accounts'
		UPDATE	acct
		SET 	currstatus = 'S',
				outstbal = 0,
				arrears = 0, --IP - 22/12/09 - UAT(948)
				bdwbalance = b.bdwvalue,
				bdwcharges = b.charges,
				lastupdatedby = 99999
		FROM		#bdwaccts b
		WHERE	acct.acctno = b.acctno
		
				--Loop through table and call stored procedure to update account---------------
		SELECT @TotalCount= count(id) FROM #bdwaccts b WHERE acctno = b.acctno
		WHILE (@iCount<= @TotalCount)
		BEGIN
		------Fetch one by one accounts and check if it is for -------------------
			SELECT @SelectedAcctno= acctno FROM #bdwaccts b WHERE acctno = b.acctno AND id = @iCount
			-- Check if account is of type AmortizedOutStandingBal.
			IF EXISTS(SELECT acctno FROM acct WHERE acctno = @SelectedAcctno AND IsAmortizedOutStandingBal = 1)
			BEGIN
				--DECLARE	--@return_value int,
				--@return int
				EXEC	[dbo].[CLAmortizationWriteOffAccountBalance]
				@acctno = @SelectedAcctno,
				@Procreturn = @return OUTPUT
			END 
			SET @iCount = @iCount + 1
		END
     		SET @status =@@error
	END


     	IF @status = 0  -- AND @dorun = 'Y'
     	BEGIN
        	SET 	@query_text =N'Stage 8 - Insert processed accounts to audit table'

		UPDATE BDWAudit
		SET	addcharges = #bdwaccts.charges,
			repovalue = #bdwaccts.repovalue,
			bdwvalue = #bdwaccts.bdwvalue,
			agrmtwopercentage = #bdwaccts.wopercentage,
			ageofaccount = #bdwaccts.age,
			empeeno = #bdwaccts.empeeno,
			code = #bdwaccts.code,
			wodate = #bdwaccts.wodate
		FROM #bdwaccts
		WHERE BDWAUDIT.acctno = #bdwaccts.acctno

     		SET @status =@@error

		IF @status = 0
	     	BEGIN
			INSERT INTO BDWAudit
			SELECT	acctno,
		        	charges,
				repovalue,
				bdwvalue,
				wopercentage,
				age,
				empeeno,
				code,
				wodate
			FROM	#bdwaccts b
			WHERE	not exists (select acctno from bdwaudit ba where ba.acctno = b.acctno)

	     		SET @status =@@error
		END
	END

     	IF @status = 0  -- AND @dorun = 'Y'
     	BEGIN
        	SET 	@query_text =N'Stage 9 - Stamp run number'
		UPDATE	BDWPending
		SET	runno = @runno
		WHERE	(runno = 0 AND code!='MRA' OR(code = 'MRA' AND empeeno!=0)) --IP - do not set the runno for MRA code accounts not accepted
     		SET @status =@@error
	END

     	IF @status = 0  -- AND @dorun = 'Y'
     	BEGIN
        	SET 	@query_text =N'Stage 10 - Record values for processed accounts'
		SELECT	@numprocessed = COUNT(*),
		  	@totalvalue = SUM(bdwvalue)
		FROM	#bdwaccts
		
		-- lw69180 rdb 24/09/07
		if @totalValue is null
			begin
				set @totalValue = 0
				-- write an error to interfaceerror 'No accounts to be written off.'
				declare @retVal int , @dateTimeNow datetime
				set @dateTimeNow = getdate()
				print 'executing  DN_InterfaceErrorWriteSP ' + @interface
				exec DN_InterfaceErrorWriteSP 
					@interface= @interface, 
					@runno = @runno, 
					@errorDate = @dateTimeNow, 
					@errorText = 'No accounts to be written off.', 
					@severity = 'W', 
					@return = @retVal 

				set @ContainsWarnings = 1
			end
   IF @numprocessed >0 AND @totalvalue !=0
		EXEC DN_InterfaceValueAddSP	@interface = @interface,
						@runno = @runno,
						@counttype1 = 'PROCESSED',
						@counttype2 ='',
						@branchno = @branchno,
						@accttype = '',
						@countvalue = @numprocessed,
						@value = @totalvalue,
						@return = @status output
	END

/* CR781 - code not required
     	IF @status = 0 AND @dorun = 'Y'
     	BEGIN
        	SET 	@query_text =N'Stage 11 - Set date finish for interface run'
		EXEC DN_InterfaceSetDateFinishSP	@interface = @interface,
							@runno = @runno,
							@return = @status output
	END

     	IF @status = 0 AND @dorun = 'Y'
     	BEGIN
        	SET 	@query_text =N'Stage 12 - Set result for interface run'
		EXEC DN_InterfaceSetResultSP	@result = 'P',
						@interface = @interface,
						@runno = @runno,
						@return = @status output

	END

*/
/* jec CR781
	UPDATE	eodcontrol 
	SET	donextrun = dodefault
	WHERE	interface = @interface
*/
	IF @status != 0
	BEGIN
	   	ROLLBACK TRAN
		SET @query_text = convert(varchar,@status) + ' ' + @query_text
/* JEC CR781
   		INSERT INTO Interfaceerror(	interface,
						runno,
						errordate,
						severity,
						errortext)   
		   VALUES(	@interface,
				0,
				getdate(),
				'E',
				@query_text)   

		EXEC DN_InterfaceSetResultSP	@result = 'F',
						@interface = @interface,
						@runno = @runno,
						@return = @status output
*/
-- pass error back. .net will handle pass/fail etc  
    raiserror (@query_text,1,1)
	END

	COMMIT TRAN

--	RETURN @status

IF @@error != 0
	BEGIN
		SET @return = @@error
	END

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO
