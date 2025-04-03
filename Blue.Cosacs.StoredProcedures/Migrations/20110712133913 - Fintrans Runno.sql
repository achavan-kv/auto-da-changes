IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.columns 
		   WHERE table_name ='fintrans' 
		   AND column_name = 'runno' 
		   AND data_type = 'SMALLINT')
		   
BEGIN

SELECT origbr ,
        branchno ,
        acctno ,
        transrefno ,
        datetrans ,
        transtypecode ,
        empeeno ,
        transupdated ,
        transprinted ,
        transvalue ,
        bankcode ,
        bankacctno ,
        chequeno ,
        ftnotes ,
        paymethod ,
        CONVERT(INT,runno) AS runno ,
        source ,
        agrmtno ,
        ExportedToTallyman
        INTO fintrans_altered
        FROM fintrans
	
	IF (SELECT COUNT(*) 
	    FROM fintrans) = (SELECT COUNT(*) FROM fintrans_altered) 
	    BEGIN
			IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID('fk_fintrans_acctno') AND parent_object_id = OBJECT_ID(N'fintrans'))
			ALTER TABLE fintrans DROP CONSTRAINT fk_fintrans_acctno
			 
			IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID('fk_FinTransAccount_FinTrans') AND parent_object_id = OBJECT_ID('FinTransAccount'))
			ALTER TABLE FinTransAccount DROP CONSTRAINT fk_FinTransAccount_FinTrans
			
			IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID('fk_FinTransExchange_FinTrans') AND parent_object_id = OBJECT_ID('FinTransExchange'))
			ALTER TABLE FinTransExchange DROP CONSTRAINT fk_FinTransExchange_FinTrans 
			 			 
			 EXECUTE ('DROP TABLE fintrans')
			 EXEC sp_rename 'fintrans_altered','fintrans'
			 
			 ALTER TABLE [dbo].[fintrans] ADD  CONSTRAINT [pk_FinTrans] PRIMARY KEY CLUSTERED 
			(
				[acctno] ASC,
				[datetrans] ASC,
				[branchno] ASC,
				[transrefno] ASC
			)
			
			ALTER TABLE [dbo].[fintrans]  WITH NOCHECK ADD  CONSTRAINT [fk_fintrans_acctno] FOREIGN KEY([acctno])
			REFERENCES [dbo].[acct] ([acctno])

			ALTER TABLE [dbo].[fintrans] CHECK CONSTRAINT [fk_fintrans_acctno]
		
			ALTER TABLE [dbo].[fintrans]  WITH NOCHECK ADD  CONSTRAINT [chk_fintrans_datetrans] CHECK  (([datetrans] >= '1-jan-1980' and [datetrans] <= dateadd(day,10,getdate())))

			ALTER TABLE [dbo].[fintrans] CHECK CONSTRAINT [chk_fintrans_datetrans]

			ALTER TABLE [dbo].[fintrans]  WITH NOCHECK ADD  CONSTRAINT [chk_fintrans_transprinted] CHECK  (([transprinted] = '' or ([transprinted] = 'N' or [transprinted] = 'Y')))

			ALTER TABLE [dbo].[fintrans]  WITH NOCHECK ADD  CONSTRAINT [fintrans_source_check] CHECK  (([source] = 'COASTER' or [source] = 'COSACS'))

			ALTER TABLE [dbo].[fintrans] CHECK CONSTRAINT [fintrans_source_check]
			ALTER TABLE [dbo].[fintrans] CHECK CONSTRAINT [chk_fintrans_transprinted]
		
			CREATE NONCLUSTERED INDEX [ix_fintrans_branch] ON [dbo].[fintrans] 
			(
				[branchno] ASC,
				[transrefno] ASC,
				[acctno] ASC
			)
			
			CREATE NONCLUSTERED INDEX [ix_fintrans_branchno] ON [dbo].[fintrans] 
			(
				[branchno] ASC,
				[datetrans] ASC
			)
			
			CREATE NONCLUSTERED INDEX [ix_fintrans_cjspec2] ON [dbo].[fintrans] 
			(
				[datetrans] ASC,
				[branchno] ASC,
				[transrefno] ASC
			)
			
			CREATE NONCLUSTERED INDEX [ix_fintrans_empeeno] ON [dbo].[fintrans] 
			(
				[empeeno] ASC,
				[datetrans] ASC,
				[transtypecode] ASC
			)
			
			CREATE NONCLUSTERED INDEX [ix_fintrans_runno] ON [dbo].[fintrans] 
			(
				[runno] ASC,
				[transtypecode] ASC,
				[transvalue] ASC
			)
				
			CREATE NONCLUSTERED INDEX [ix_fintrans_transrefno] ON [dbo].[fintrans] 
			(
				[transrefno] ASC,
				[acctno] ASC
			)
	    END
	    
	    
	    ALTER TABLE [dbo].[FinTransAccount]  WITH NOCHECK ADD  CONSTRAINT [fk_FinTransAccount_FinTrans] FOREIGN KEY([AcctNo], [DateTrans], [BranchNo], [TransRefNo])
		REFERENCES [dbo].[fintrans] ([acctno], [datetrans], [branchno], [transrefno])

		ALTER TABLE [dbo].[FinTransAccount] CHECK CONSTRAINT [fk_FinTransAccount_FinTrans]

		ALTER TABLE [dbo].[FinTransExchange]  WITH NOCHECK ADD  CONSTRAINT [fk_FinTransExchange_FinTrans] FOREIGN KEY([AcctNo], [DateTrans], [BranchNo], [TransRefNo])
		REFERENCES [dbo].[fintrans] ([acctno], [datetrans], [branchno], [transrefno])
		ON DELETE CASCADE

		ALTER TABLE [dbo].[FinTransExchange] CHECK CONSTRAINT [fk_FinTransExchange_FinTrans]
	END
	
	