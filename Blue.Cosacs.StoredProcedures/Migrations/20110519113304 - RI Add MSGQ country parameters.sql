-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here


declare @parametercategory INT, @CtyFolder VARCHAR(8)

select @parametercategory=(select code from code where category='CMC' and codedescript='Interface')
select @CtyFolder=(select 'RI'+ ISOCountryCode +'DB' from Country)

if not exists(select * from CountryMaintenance where codename='RIQTYpathMSGQ')
insert into CountryMaintenance (CountryCode,ParameterCategory,[Name],Value,[Type],[Precision],OptionCategory,
			OptionListName,[Description],CodeName) 
select CountryCode,@parametercategory,'RI outbound QTY MSGQ directory','/QIBM/UserData/WMQFTE/V7/RICARIBE/IN/' +@CtyFolder+'/UPD_CO_QTY/','text',0,'',
			'','Output MSGQ path for Committed Stock (QTY) outbound file','RIQTYpathMSGQ' 
from Country

if not exists(select * from CountryMaintenance where codename='RISARpathMSGQ')
insert into CountryMaintenance (CountryCode,ParameterCategory,[Name],Value,[Type],[Precision],OptionCategory,
			OptionListName,[Description],CodeName) 
select CountryCode,@parametercategory,'RI outbound SAR MSGQ directory','/QIBM/UserData/WMQFTE/V7/RICARIBE/IN/' +@CtyFolder+'/SALES_RETN/','text',0,'',
			'','Output MSGQ path for Sales & Returns (SAR) outbound file','RISARpathMSGQ' 
from Country

if not exists(select * from CountryMaintenance where codename='RISARpathRepoMSGQ')
insert into CountryMaintenance (CountryCode,ParameterCategory,[Name],Value,[Type],[Precision],OptionCategory,
			OptionListName,[Description],CodeName) 
select CountryCode,@parametercategory,'RI outbound Repo SAR MSGQ directory','/QIBM/UserData/WMQFTE/V7/RICARIBE/IN/' +@CtyFolder+'/SALES_RETN/','text',0,'',
			'','Output MSGQ path for Sales & Returns Repossession (SAR) outbound file','RISARpathRepoMSGQ' 
from Country

if not exists(select * from CountryMaintenance where codename='RIDTFpathMSGQ')
insert into CountryMaintenance (CountryCode,ParameterCategory,[Name],Value,[Type],[Precision],OptionCategory,
			OptionListName,[Description],CodeName) 
select CountryCode,@parametercategory,'RI outbound DTF MSGQ directory','/QIBM/UserData/WMQFTE/V7/RICARIBE/IN/' +@CtyFolder+'/DEL_TRAN/','text',0,'',
			'','Output MSGQ path for Delivery transfer (DTF) outbound file','RIDTFpathMSGQ' 
from Country

if not exists(select * from CountryMaintenance where codename='RIDTFpathRepoMSGQ')
insert into CountryMaintenance (CountryCode,ParameterCategory,[Name],Value,[Type],[Precision],OptionCategory,
			OptionListName,[Description],CodeName) 
select CountryCode,@parametercategory,'RI outbound Repo DTF MSGQ directory','/QIBM/UserData/WMQFTE/V7/RICARIBE/IN/' +@CtyFolder+'/DEL_TRAN/','text',0,'',
			'','Output MSGQ path for Delivery transfer Repossession (DTF) outbound file','RIDTFpathRepoMSGQ' 
from Country

if not exists(select * from CountryMaintenance where codename='RIRPOpathMSGQ')
insert into CountryMaintenance (CountryCode,ParameterCategory,[Name],Value,[Type],[Precision],OptionCategory,
			OptionListName,[Description],CodeName) 
select CountryCode,@parametercategory,'RI outbound RPO MSGQ directory','/QIBM/UserData/WMQFTE/V7/RICARIBE/IN/' +@CtyFolder+'/REPO_RECV/','text',0,'',
			'','Output MSGQ path for Repossessions (RPO) outbound file','RIRPOpathMSGQ' 
from Country

--delete CountryMaintenance where codename like 'RI%MSGQ'