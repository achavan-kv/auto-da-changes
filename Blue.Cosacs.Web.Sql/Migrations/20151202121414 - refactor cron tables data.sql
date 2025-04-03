-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
SELECT 
	Id, EndpointId, DailyAt, EveryFewMinutes, CronExpression, SendEmailOnStartAndSuccess
	INTO #Cronjob 
FROM 
	Cron.job 

SELECT 
	r.Id, r.JobId, r.StartRun, r.EndRun, r.LastException 
	INTO #CronRun
FROM 
	Cron.Run r

DELETE Cron.Run
DELETE Cron.Job
DELETE Cron.Endpoint

INSERT INTO Cron.Endpoint
	(Id, Name, Url, Module)
VALUES 
	(100,'Allocate Customers to CSR','/SalesManagement/api/AllocateCustomersToCSR','Sales Management'),
	(101,'Customer Instalment Ending','/SalesManagement/api/CustomerInstalmentEnding','Sales Management'),
	(102,'Inactive Customers','/SalesManagement/api/InactiveCustomers','Sales Management'),
	(103,'Flush Unmade Calls','/SalesManagement/api/FlushUnmadeCalls','Sales Management'),
	(104,'Reload Dashboard','/SalesManagement/api/DashBoard','Sales Management'),
	(105,'Sales KPI','/Courts.NET.WS/SalesManagement/GetSalesKPI','Sales Management'),
	(106,'Customer re-index','/Customer/api/Reindex/ForceIndex','Customer'),
	(107,'Send Sales Management emails','/SalesManagement/api/PublishEmails','Sales Management'),
	(108,'Load e-mail unsubscriptions','/Communication/api/LoadRejectionLists','Communication'),
	(109,'Send Sales Management Sms','/SalesManagement/api/PublishSms','Sales Management'),
	(110,'Import Sales Summary Data','/SalesManagement/api/SummaryTableData','Sales Management'),
	(200,'Auto-expire Purchase Orders','/cosacs/Merchandising/Purchase/AutoExpire','Merchandising'),
	(201,'Auto-close Stock Counts','/cosacs/Merchandising/StockCount/AutoClose','Merchandising'),
	(202,'Export Winform Files','/cosacs/Merchandising/Export/Export','Merchandising'),
	(203,'Migrate Product Status','/cosacs/Merchandising/ProductStatus/Progress','Merchandising'),
	(204,'Export Hyperion','/cosacs/Merchandising/Export/ExportHyperion','Merchandising'),
	(205,'Export Magento','/cosacs/Merchandising/Export/ExportMagento','Merchandising'),
	(206,'Index Products','/cosacs/Merchandising/Products/IndexProducts','Merchandising'),
	(207,'Merchandising Report Summary Data','/cosacs/Merchandising/Report/SummariseData','Merchandising'),
	(208,'Abbreviated Stock Export','/cosacs/Merchandising/Export/ExportAbbreviatedStock','Merchandising'),
	(209,'RP3 Export','/cosacs/Merchandising/rp3export/export','Merchandising'),
	(250,'Index Non-Stocks','/NonStocks/Api/Indexing/ForceIndex','Non Stocks'),
	(300,'Customer Reindex','/credit/api/CustomerReindex','Credit'),
	(800,'Winform Stock','/Cosacs/Catalogue/ForceIndex','Sales')

	/*create a new id so it will generate unic values to insert*/
	DECLARE @NewId Int = ISNULL((SELECT MAX(Id) FROM #CronJob), 0)

	SET IDENTITY_INSERT Cron.Job ON
	INSERT INTO Cron.Job
		(Id, EndpointId, DailyAt, EveryFewMinutes, CronExpression, SendEmailOnStartAndSuccess)
	SELECT Id, EndpointId, DailyAt, EveryFewMinutes, CronExpression, SendEmailOnStartAndSuccess
	FROM  
	(
		SELECT Id, EndpointId, DailyAt, EveryFewMinutes, CronExpression, SendEmailOnStartAndSuccess FROM #CronJob WHERE EndpointId IN (SELECT id FROM Cron.Endpoint)  UNION ALL
		SELECT @NewId + 1, 100, CONVERT(DATETIME, '19700101 07:00'), null, null, null WHERE NOT EXISTS(SELECT 1 FROM #CronJob WHERE EndpointId = 100) UNION ALL
		SELECT @NewId + 2, 101, CONVERT(DATETIME, '19700101 07:15'), null, null, null WHERE NOT EXISTS(SELECT 1 FROM #CronJob WHERE EndpointId = 101)  UNION ALL
		SELECT @NewId + 3, 102, CONVERT(DATETIME, '19700101 07:30'), null, null, null WHERE NOT EXISTS(SELECT 1 FROM #CronJob WHERE EndpointId = 102)  UNION ALL
		SELECT @NewId + 4, 103, CONVERT(DATETIME, '19700101 07:45'), null, null, null WHERE NOT EXISTS(SELECT 1 FROM #CronJob WHERE EndpointId = 103)  UNION ALL
		SELECT @NewId + 5, 104, null, 15, null, null WHERE NOT EXISTS(SELECT 1 FROM #CronJob WHERE EndpointId = 104)  UNION ALL
		SELECT @NewId + 6, 105, null, 15, null, null WHERE NOT EXISTS(SELECT 1 FROM #CronJob WHERE EndpointId = 105)  UNION ALL
		SELECT @NewId + 7, 106, CONVERT(DATETIME, '19700101 06:45'), null, null, null WHERE NOT EXISTS(SELECT 1 FROM #CronJob WHERE EndpointId = 106)  UNION ALL
		SELECT @NewId + 8, 108, CONVERT(DATETIME, '19700101 06:00'), NULL, NULL, 0 WHERE NOT EXISTS(SELECT 1 FROM #CronJob WHERE EndpointId = 108)  UNION ALL
		SELECT @NewId + 9, 109, CONVERT(DATETIME, '19700101 08:00'), NULL, NULL, 0 WHERE NOT EXISTS(SELECT 1 FROM #CronJob WHERE EndpointId = 109)  UNION ALL
		SELECT @NewId + 10, 250, CONVERT(DATETIME, '19700101 06:00'), null, null, null WHERE NOT EXISTS(SELECT 1 FROM #CronJob WHERE EndpointId = 250)  UNION ALL
		SELECT @NewId + 11, 300, CONVERT(DATETIME, '19700101 07:30'), null, null, 0 WHERE NOT EXISTS(SELECT 1 FROM #CronJob WHERE EndpointId = 300)  UNION ALL
		SELECT @NewId + 12, 800, CONVERT(DATETIME, '19700101 05:00'),  null,  null, null WHERE NOT EXISTS(SELECT 1 FROM #CronJob WHERE EndpointId = 800)  
	) NewData
	SET IDENTITY_INSERT Cron.Job OFF

	SET IDENTITY_INSERT Cron.Run ON
	INSERT INTO Cron.Run
		(Id, JobId, StartRun, EndRun, LastException)
	SELECT 
		Id, JobId, StartRun, EndRun, LastException 
	FROM 
		#CronRun
	WHERE
		JobId IN (SELECT id FROM Cron.Job)
	SET IDENTITY_INSERT Cron.Run OFF

DROP TABLE #CronRun
DROP TABLE #Cronjob