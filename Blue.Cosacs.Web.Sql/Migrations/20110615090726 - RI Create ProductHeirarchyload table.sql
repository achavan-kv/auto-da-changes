-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF  NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID('RItemp_RawProductHeirarchy') AND type in (N'U'))
BEGIN	
	CREATE TABLE RItemp_RawProductHeirarchy
	(
		RISource			CHAR(2)		default '' not null,
	    CatalogType			CHAR(2)		default '' not null,
	    PrimaryCode			VARCHAR(10)	default '' not null,
	    CodeDescription		VARCHAR(60)	,
	    ParentCode			VARCHAR(10)	default '' not null,
	    CodeStatus			CHAR(1)		default '' not null
	)
END


declare @parametercategory INT, @CtyFolder VARCHAR(8)

select @parametercategory=(select code from code where category='CMC' and codedescript='Interface')
select @CtyFolder=(select 'RI'+ 'rp' +'DB' from Country)


--Inbound
if not exists(select * from CountryMaintenance where codename='RICTXpath')
insert into CountryMaintenance (CountryCode,ParameterCategory,[Name],Value,[Type],[Precision],OptionCategory,
			OptionListName,[Description],CodeName) 
select CountryCode,@parametercategory,'RI inbound CTX directory','D:\UserData\WMQFTE\V7\RICARIBE\IN\' +@CtyFolder+'\CATALOG\','text',0,'',
			'','Source directory path for Product Heirarchy information (CTX) inbound file','RICTXpath' 
from Country
