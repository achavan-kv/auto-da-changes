-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

if exists(select * from countrymaintenance where codename = 'IC_HighSettStat2Yr')
update countrymaintenance set description = 'The maximum status of any current account or settled account settled within the last 2 years. A status higher than this will prevent instant credit approval'
where codename = 'IC_HighSettStat2Yr'
