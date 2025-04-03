-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

declare @parametercategory INT, @CtyFolder VARCHAR(8)

select @parametercategory=(select code from code where category='CMC' and codedescript='Interface')
select @CtyFolder=(select 'RI'+ ISOCountryCode +'DB' from Country)

if not exists(select * from CountryMaintenance where codename='RIKITpathRepo')
insert into CountryMaintenance (CountryCode,ParameterCategory,[Name],Value,[Type],[Precision],OptionCategory,
			OptionListName,[Description],CodeName) 
select CountryCode,@parametercategory,'RI inbound Repo KIT directory','D:\UserData\WMQFTE\V7\RICARIBE\IN\' +@CtyFolder+'\KIT\','text',0,'',
			'','Source directory path for Repossession KIT Product information (KIT) inbound file','RIKITpathRepo' 
from Country