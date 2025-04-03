 IF EXISTS (SELECT  *  FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AshleyJob]')  AND type IN (N'P', N'PC'))
  DROP PROCEDURE [dbo].[AshleyJob]
GO
  --exec [dbo].[AshleyJob] 'AshleyCreateCSV'
CREATE  PROCEDURE [dbo].[AshleyJob] @name varchar(200)
AS
BEGIN
Declare @id varchar(50)
	set @id=(select Id from config.setting where Id='AshleyEnabled' and ValueBit=1 )
Declare @ex varchar(max), @jobStat varchar(50),@T varchar(50)
IF (@name = 'AshleyCreateCSV' and @id='AshleyEnabled')
begin
	EXEC msdb.dbo.sp_start_job N'AshleyCreateCSV'
SELECT    name AS [Job Name] 
         ,CONVERT(VARCHAR,DATEADD(S,(run_time/10000)*60*60 /* hours */  
          +((run_time - (run_time/10000) * 10000)/100) * 60 /* mins */  
          + (run_time - (run_time/100) * 100)  /* secs */
           ,CONVERT(DATETIME,RTRIM(run_date),102)),100) AS [Time Run]
         ,CASE WHEN enabled=1 THEN 'Enabled'  
               ELSE 'Disabled'  
          END [Job Status],message
         ,CASE WHEN SJH.run_status=0 THEN 'Failed'
                     WHEN SJH.run_status=1 THEN 'Succeeded'
                     WHEN SJH.run_status=2 THEN 'Retry'
                     WHEN SJH.run_status=3 THEN 'Cancelled'
               ELSE 'Unknown'  
          END [Job Outcome]
FROM   msdb.dbo.sysjobhistory SJH  
JOIN   msdb.dbo.sysjobs SJ  
ON     SJH.job_id=sj.job_id  
WHERE    name='AshleyCreateCSV'  --and step_name='CreateCSV'
 end 
--SELECT TOP 1  * FROM #T ORDER BY [Time Run] DESC
 
--SELECT TOP 1  @jobStat = [Job Outcome], @EX=[MESSAGE] FROM #T ORDER BY [Time Run] DESC
PRINT (@jobStat)
IF (@jobStat = 'Failed')
	BEGIN
		PRINT(@EX)
		RAISERROR(N'%s'
              ,16
              ,1
              ,@EX);
 end
IF (@name = 'AshleySaleOrder' and @id='AshleyEnabled')
begin
EXEC msdb.dbo.sp_start_job N'AshleySaleOrder'
SELECT name AS [Job Name] 
         ,CONVERT(VARCHAR,DATEADD(S,(run_time/10000)*60*60 /* hours */  
          +((run_time - (run_time/10000) * 10000)/100) * 60 /* mins */  
          + (run_time - (run_time/100) * 100)  /* secs */
           ,CONVERT(DATETIME,RTRIM(run_date),102)),100) AS [Time Run]
         ,CASE WHEN enabled=1 THEN 'Enabled'  
               ELSE 'Disabled'  
          END [Job Status],message
         ,CASE WHEN SJH.run_status=0 THEN 'Failed'
                     WHEN SJH.run_status=1 THEN 'Succeeded'
                     WHEN SJH.run_status=2 THEN 'Retry'
                     WHEN SJH.run_status=3 THEN 'Cancelled'
               ELSE 'Unknown'  
          END [Job Outcome] 
FROM   msdb.dbo.sysjobhistory SJH  
JOIN   msdb.dbo.sysjobs SJ  
ON     SJH.job_id=sj.job_id  
WHERE    name='AshleySaleOrder'  order by  [Time Run] DESC--and step_name='ReadCSVForSale'
--SELECT TOP 1  * FROM #T1 ORDER BY [Time Run] DESC
--SELECT TOP 1  @jobStat = [Job Outcome], @EX=[MESSAGE] FROM  #t ORDER BY [Time Run] DESC
--PRINT (@jobStat)
end
IF (@jobStat = 'Failed')
	BEGIN
		PRINT(@EX)
		RAISERROR(N'%s'
              ,16
              ,1
              ,@EX);
	END

 IF (@name = 'Export PO XML to FTP' and @id='AshleyEnabled')
 begin 
 EXEC msdb.dbo.sp_start_job N'AshleyCreateXML'
 SELECT  name AS [Job Name] 
         ,CONVERT(VARCHAR,DATEADD(S,(run_time/10000)*60*60 /* hours */  
          +((run_time - (run_time/10000) * 10000)/100) * 60 /* mins */  
          + (run_time - (run_time/100) * 100)  /* secs */
           ,CONVERT(DATETIME,RTRIM(run_date),102)),100) AS [Time Run]
         ,CASE WHEN enabled=1 THEN 'Enabled'  
               ELSE 'Disabled'  
          END [Job Status],message
         ,CASE WHEN SJH.run_status=0 THEN 'Failed'
                     WHEN SJH.run_status=1 THEN 'Succeeded'
                     WHEN SJH.run_status=2 THEN 'Retry'
                     WHEN SJH.run_status=3 THEN 'Cancelled'
               ELSE 'Unknown'  
          END [Job Outcome] 
FROM   msdb.dbo.sysjobhistory SJH  
JOIN   msdb.dbo.sysjobs SJ  
ON     SJH.job_id=sj.job_id  
WHERE    name='AshleyCreateXML'  --and step_name='XMLCreate'
--SELECT TOP 1  * FROM #T2 ORDER BY [Time Run] DESC
--SELECT TOP 1  @jobStat = [Job Outcome], @EX=[MESSAGE] FROM  #t ORDER BY [Time Run] DESC
PRINT (@jobStat)
end
IF (@jobStat = 'Failed')
	BEGIN
		PRINT(@EX)
		RAISERROR(N'%s'
              ,16
              ,1
              ,@EX);
	END
 IF (@name = 'ReadAsn' and @id='AshleyEnabled')
 begin
  EXEC msdb.dbo.sp_start_job N'AshleyReadXML'
 SELECT   name AS [Job Name] 
         ,CONVERT(VARCHAR,DATEADD(S,(run_time/10000)*60*60 /* hours */  
          +((run_time - (run_time/10000) * 10000)/100) * 60 /* mins */  
          + (run_time - (run_time/100) * 100)  /* secs */
           ,CONVERT(DATETIME,RTRIM(run_date),102)),100) AS [Time Run]
         ,CASE WHEN enabled=1 THEN 'Enabled'  
               ELSE 'Disabled'  
          END [Job Status],message
         ,CASE WHEN SJH.run_status=0 THEN 'Failed'
                     WHEN SJH.run_status=1 THEN 'Succeeded'
                     WHEN SJH.run_status=2 THEN 'Retry'
                     WHEN SJH.run_status=3 THEN 'Cancelled'
               ELSE 'Unknown'  
          END [Job Outcome] 
FROM   msdb.dbo.sysjobhistory SJH  
JOIN   msdb.dbo.sysjobs SJ  
ON     SJH.job_id=sj.job_id  
WHERE    name='AshleyReadXML'  --and step_name='XMLRead'
--SELECT TOP 1  * FROM #T3 ORDER BY [Time Run] DESC
--SELECT TOP 1  @jobStat = [Job Outcome], @EX=[MESSAGE] FROM  #t  ORDER BY [Time Run] DESC
PRINT (@jobStat)
end
IF (@jobStat = 'Failed')
	BEGIN
		PRINT(@EX)
		RAISERROR(N'%s'
              ,16
              ,1
              ,@EX);
 	END
 
 end

 
