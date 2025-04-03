SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_EODSaveJobSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_EODSaveJobSP]
GO

-- =============================================
-- Modified By:	Jez Hemans
-- Modified On date: 08/02/2008
-- Description:	Directory for EOD.NET.exe no longer hard coded. Now passed as a parameter.
-- =============================================
-- ===============================================================================================
-- Version:		<001> 
-- ===============================================================================================
CREATE PROCEDURE  dbo.DN_EODSaveJobSP
   			@configuration varchar(12),
			@country char(1),
			@user varchar(10),
			@freqtype int,
			@startdate int,
			@starttime int,
			@url VARCHAR(100),
   			@return int OUTPUT
 
AS
 
 	SET  @return = 0   --initialise return code

	BEGIN TRANSACTION            
	  DECLARE @JobID BINARY(16)  
	  DECLARE @ReturnCode INT    
	  DECLARE @DBName VARCHAR(30)
	  DECLARE @EnableJob INT --IP - 04/03/08 Livewire (69582)
	  SELECT @ReturnCode = 0     
	IF (SELECT COUNT(*) FROM msdb.dbo.syscategories WHERE [name] = 'Database Maintenance') < 1 
	  EXECUTE msdb.dbo.sp_add_category @name = 'Database Maintenance'
	
	  -- Get current database name
	  SET @DBName = db_name()
	 
	  -- Delete the job with the same name (if it exists)
	  SELECT @JobID = job_id     
	  FROM   msdb.dbo.sysjobs    
	  WHERE ([name] = 'CoSACS EOD Run')       
	
	BEGIN 
	
	  -- Delete the step with the same name (if it exists)
	  EXECUTE @ReturnCode = msdb.dbo.sp_delete_jobstep @job_id = @JobID, @step_id = 1
	  IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback 

	  DECLARE @statement varchar(500)
	 
	  -- aviod the multipal call in Instanct credit sp
	  TRUNCATE TABLE [InstantCreditApprovalsCheckGen_Val]  
	 
	  SET @url = '"' + @url + 'bin\EOD.NET.exe"'
	  --changing to execute the command through command exec rather than xp_cmdshell
	  	SET @statement =  @url + ' ' + @configuration + ' ' + @user + ' ' + @country  
	  	
	  	
	  -- Add the job steps
	  EXECUTE @ReturnCode = msdb.dbo.sp_add_jobstep @job_id = @JobID, @step_id = 1, @step_name = 'Start EOD Run.', @command= @statement, @database_name = @DBName, @server = '', @database_user_name = '', 
	  @subsystem = 'CmdExec', @cmdexec_success_code = 0, @flags = 0, @retry_attempts = 0, @retry_interval = 1, @output_file_name = '', @on_success_step_id = 0, @on_success_action = 1, @on_fail_step_id = 0, @on_fail_action = 2
	  IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback 
	
	  EXECUTE @ReturnCode = msdb.dbo.sp_update_job @job_id = @JobID, @start_step_id = 1 

	  -- Add job schedule
	  
	  --IP - 04/03/08 - Livewire (69582) - if the 'Frequency' selected is 'Manual'
      --then disable the job, else enable the job.
	  IF (@freqtype = 8)
		BEGIN
	  		SET @freqtype = 1
			SET @EnableJob = 0	
		END
	  ELSE
		SET @EnableJob = 1
	  
  	  IF (EXISTS (SELECT  * 
	              FROM    msdb.dbo.sysjobschedules 
	              WHERE   (job_id = @JobID))) 
	  BEGIN
		EXECUTE @ReturnCode = msdb.dbo.sp_delete_jobschedule @job_id = @JobID, @name = 'EOD Schedule'
	  	IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback 
	  END

  	   EXECUTE @ReturnCode = msdb.dbo.sp_add_jobschedule @job_id = @JobID, @name = 'EOD Schedule', @enabled = @EnableJob, @freq_type = @freqtype,
 								@freq_interval = 1,@active_start_date = @startdate, @active_start_time = @starttime
	  IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback 
	
	END
	
	COMMIT TRANSACTION          
	GOTO   EndSave              
	QuitWithRollback:
	  IF (@@TRANCOUNT > 0) ROLLBACK TRANSACTION 
	EndSave: 

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

 
