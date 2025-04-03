-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

if not exists( select * from Code where Code='RI2CoSACS' and CATEGORY='EDC')
Begin
insert into dbo.code (
	origbr,
	category,
	code,
	codedescript,
	statusflag,
	sortorder,
	reference,
	additional
) VALUES ( 
	/* origbr - smallint */ 0,
	/* category - varchar(12) */ 'EDC',
	/* code - varchar(12) */ 'RI2CoSACS',
	/* codedescript - nvarchar(64) */ N'RI to CoSACS Interface Import',
	/* statusflag - char(1) */ 'L',
	/* sortorder - smallint */ 8,
	/* reference - varchar(12) */ '0',
	/* additional - varchar(15) */ '1' )	-- CanReRun

End

if not exists( select * from Code where Code='CoSACS2RI' and CATEGORY='EDC')
Begin
insert into dbo.code (
	origbr,
	category,
	code,
	codedescript,
	statusflag,
	sortorder,
	reference,
	additional
) VALUES ( 
	/* origbr - smallint */ 0,
	/* category - varchar(12) */ 'EDC',
	/* code - varchar(12) */ 'CoSACS2RI',
	/* codedescript - nvarchar(64) */ N'CoSACS to RI Interface Export',
	/* statusflag - char(1) */ 'L',
	/* sortorder - smallint */ 11,
	/* reference - varchar(12) */ '0',
	/* additional - varchar(15) */ '1' )	-- CanReRun

End

declare @parametercategory INT, @CtyFolder VARCHAR(8)

select @parametercategory=(select code from code where category='CMC' and codedescript='Interface')
select @CtyFolder=(select 'RI'+ ISOCountryCode +'DB' from Country)

if not exists(select * from CountryMaintenance where codename='RIQTYpath')
insert into CountryMaintenance (CountryCode,ParameterCategory,[Name],Value,[Type],[Precision],OptionCategory,
			OptionListName,[Description],CodeName) 
select CountryCode,@parametercategory,'RI outbound QTY directory','D:\UserData\WMQFTE\V7\RICARIBE\OUT\' +@CtyFolder+'\UPD_CO_QTY\','text',0,'',
			'','Output directory path for Committed Stock (QTY) outbound file','RIQTYpath' 
from Country

if not exists(select * from CountryMaintenance where codename='RISARpath')
insert into CountryMaintenance (CountryCode,ParameterCategory,[Name],Value,[Type],[Precision],OptionCategory,
			OptionListName,[Description],CodeName) 
select CountryCode,@parametercategory,'RI outbound SAR directory','D:\UserData\WMQFTE\V7\RICARIBE\OUT\' +@CtyFolder+'\SALES_RETN\','text',0,'',
			'','Output directory path for Sales & Returns (SAR) outbound file','RISARpath' 
from Country

if not exists(select * from CountryMaintenance where codename='RIDTFpath')
insert into CountryMaintenance (CountryCode,ParameterCategory,[Name],Value,[Type],[Precision],OptionCategory,
			OptionListName,[Description],CodeName) 
select CountryCode,@parametercategory,'RI outbound DTF directory','D:\UserData\WMQFTE\V7\RICARIBE\OUT\' +@CtyFolder+'\DEL_TRAN\','text',0,'',
			'','Output directory path for Delivery transfer (DTF) outbound file','RIDTFpath' 
from Country

if not exists(select * from CountryMaintenance where codename='RIRPOpath')
insert into CountryMaintenance (CountryCode,ParameterCategory,[Name],Value,[Type],[Precision],OptionCategory,
			OptionListName,[Description],CodeName) 
select CountryCode,@parametercategory,'RI outbound RPO directory','D:\UserData\WMQFTE\V7\RICARIBE\OUT\' +@CtyFolder+'\REPO_RECV\','text',0,'',
			'','Output directory path for Repossessions (RPO) outbound file','RIRPOpath' 
from Country

--Inbound
if not exists(select * from CountryMaintenance where codename='RIABCpath')
insert into CountryMaintenance (CountryCode,ParameterCategory,[Name],Value,[Type],[Precision],OptionCategory,
			OptionListName,[Description],CodeName) 
select CountryCode,@parametercategory,'RI inbound ABC directory','D:\UserData\WMQFTE\V7\RICARIBE\IN\' +@CtyFolder+'\ABC_ITEMS\','text',0,'',
			'','Source directory path for Product information (new items, price changes, RTA) (ABC) inbound file','RIABCpath' 
from Country

if not exists(select * from CountryMaintenance where codename='RIKITpath')
insert into CountryMaintenance (CountryCode,ParameterCategory,[Name],Value,[Type],[Precision],OptionCategory,
			OptionListName,[Description],CodeName) 
select CountryCode,@parametercategory,'RI inbound KIT directory','D:\UserData\WMQFTE\V7\RICARIBE\IN\' +@CtyFolder+'\KIT\','text',0,'',
			'','Source directory path for KIT Product information (KIT) inbound file','RIKITpath' 
from Country

if not exists(select * from CountryMaintenance where codename='RIPODpath')
insert into CountryMaintenance (CountryCode,ParameterCategory,[Name],Value,[Type],[Precision],OptionCategory,
			OptionListName,[Description],CodeName) 
select CountryCode,@parametercategory,'RI inbound POD directory','D:\UserData\WMQFTE\V7\RICARIBE\IN\' +@CtyFolder+'\PO_ARR_DAT\','text',0,'',
			'','Source directory path for Purchase Order Details (POD) inbound file','RIPODpath' 
from Country

if not exists(select * from CountryMaintenance where codename='RIOHQYpath')
insert into CountryMaintenance (CountryCode,ParameterCategory,[Name],Value,[Type],[Precision],OptionCategory,
			OptionListName,[Description],CodeName) 
select CountryCode,@parametercategory,'RI inbound OHQY directory','D:\UserData\WMQFTE\V7\RICARIBE\IN\' +@CtyFolder+'\ONHAND_QTY\','text',0,'',
			'','Source directory path for On hand quantity (OHQY) inbound file','RIOHQYpath' 
from Country

IF NOT EXISTS (SELECT * FROM syscolumns
			   WHERE name = 'CanReRun'
               AND OBJECT_NAME(id) = 'EodConfigurationOption')
BEGIN
  ALTER TABLE EodConfigurationOption ADD CanReRun CHAR(1)
END

IF NOT EXISTS (SELECT * FROM syscolumns
			   WHERE name = 'CanReRun'
               AND OBJECT_NAME(id) = 'EodConfigurationOptionAudit')
BEGIN
  ALTER TABLE EodConfigurationOptionAudit ADD CanReRun CHAR(1)
END
