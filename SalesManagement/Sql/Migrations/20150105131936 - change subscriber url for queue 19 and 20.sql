-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

UPDATE Hub.Queue
SET SubscriberHttpUrl = '/SalesManagement/api/Jobs/CustomerFollowUpCalls'
WHERE Id = 19

UPDATE Hub.Queue
SET SubscriberHttpUrl = '/SalesManagement/api/Jobs/DashBoard'
WHERE Id = 20