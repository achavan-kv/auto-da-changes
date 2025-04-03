-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF EXISTS(select * from countrymaintenance where CodeName = 'MQEnabled')
BEGIN
	update countrymaintenance set Name = 'Broker MQ Enabled',
								  [Description] = 'Determines whether the Broker MQ Link is enabled'
	where CodeName = 'MQEnabled'
END