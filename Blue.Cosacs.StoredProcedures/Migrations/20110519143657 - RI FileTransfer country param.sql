-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
if not exists(select * from CountryMaintenance where codename='RIFTEBatchScriptPath')
insert into CountryMaintenance (CountryCode,ParameterCategory,[Name],Value,[Type],[Precision],OptionCategory,
			OptionListName,[Description],CodeName) 
select CountryCode,
	  (select code from code where category='CMC' and codedescript='Interface'),'RI FTE Batch Script Path', 'FTESender.bat', 'text' ,0,'',
			'','RI FTE Message Queue Batch Script Path','RIFTEBatchScriptPath' 
from Country