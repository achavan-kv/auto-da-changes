-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

declare @parametercategory INT, @CtyFolder VARCHAR(8)

select @parametercategory=(select code from code where category='CMC' and codedescript='Interface')
select @CtyFolder=(select 'RI'+ 'rp' +'DB' from Country)


if not exists(select * from CountryMaintenance where codename='RISARpathRepo')
insert into CountryMaintenance (CountryCode,ParameterCategory,[Name],Value,[Type],[Precision],OptionCategory,
			OptionListName,[Description],CodeName) 
select CountryCode,@parametercategory,'RI outbound Repo SAR directory','D:\UserData\WMQFTE\V7\RICARIBE\OUT\' +@CtyFolder+'\SALES_RETN\','text',0,'',
			'','Output directory path for Repossession Sales & Returns (SAR) outbound file','RISARpathRepo' 
from Country

if not exists(select * from CountryMaintenance where codename='RIDTFpathRepo')
insert into CountryMaintenance (CountryCode,ParameterCategory,[Name],Value,[Type],[Precision],OptionCategory,
			OptionListName,[Description],CodeName) 
select CountryCode,@parametercategory,'RI outbound Repo DTF directory','D:\UserData\WMQFTE\V7\RICARIBE\OUT\' +@CtyFolder+'\DEL_TRAN\','text',0,'',
			'','Output directory path for Repossession Delivery transfer (DTF) outbound file','RIDTFpathRepo' 
from Country

--Inbound
if not exists(select * from CountryMaintenance where codename='RIABCpathRepo')
insert into CountryMaintenance (CountryCode,ParameterCategory,[Name],Value,[Type],[Precision],OptionCategory,
			OptionListName,[Description],CodeName) 
select CountryCode,@parametercategory,'RI inbound Repo ABC directory','D:\UserData\WMQFTE\V7\RICARIBE\IN\' +@CtyFolder+'\ABC_ITEMS\','text',0,'',
			'','Source directory path for Repossession Product information (new items, price changes, RTA) (ABC) inbound file','RIABCpathRepo' 
from Country

if not exists(select * from CountryMaintenance where codename='RIOHQYpathRepo')
insert into CountryMaintenance (CountryCode,ParameterCategory,[Name],Value,[Type],[Precision],OptionCategory,
			OptionListName,[Description],CodeName) 
select CountryCode,@parametercategory,'RI inbound Repo OHQY directory','D:\UserData\WMQFTE\V7\RICARIBE\IN\' +@CtyFolder+'\ONHAND_QTY\','text',0,'',
			'','Source directory path for Repossession On hand quantity (OHQY) inbound file','RIOHQYpathRepo' 
from Country

UPDATE CountryMaintenance
set name='RI inbound PODY directory',Codename='RIPODYpath'
where Codename='RIPODpath'