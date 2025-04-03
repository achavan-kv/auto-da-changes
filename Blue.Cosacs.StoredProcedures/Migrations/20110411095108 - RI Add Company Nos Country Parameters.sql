-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

declare @parametercategory INT, @Company smallint,@RepoCompanyNo SMALLINT, @countryCode VARCHAR(2)

select @parametercategory=(select code from code where category='CMC' and codedescript='Interface')
select @countryCode=CountryCode from dbo.country
 set @Company = case 
				when @countryCode='A' then 01	--Guyana
				when @countryCode='B' then 22	--Barbados
				when @countryCode='D' then 03	--Dominica
				when @countryCode='G' then 04	--Grenada
				when @countryCode='J' then 21	--Jamaica
				when @countryCode='K' then 05	--St Kitts
				when @countryCode='L' then 06	--St Lucia
				when @countryCode='N' then 07	--Antigua
				when @countryCode='T' then 08	--Trinidad
				when @countryCode='V' then 09	--St Vincent
				when @countryCode='Z' then 10	--Belize
				end
set @RepoCompanyNo = case 
				when @countryCode='A' then 01	--Guyana
				when @countryCode='B' then 82	--Barbados
				when @countryCode='D' then 03	--Dominica
				when @countryCode='G' then 04	--Grenada
				when @countryCode='J' then 81	--Jamaica
				when @countryCode='K' then 05	--St Kitts
				when @countryCode='L' then 06	--St Lucia
				when @countryCode='N' then 07	--Antigua
				when @countryCode='T' then 08	--Trinidad
				when @countryCode='V' then 09	--St Vincent
				when @countryCode='Z' then 10	--Belize
				end

if not exists(select * from CountryMaintenance where codename='RICompanyNo')
insert into CountryMaintenance (CountryCode,ParameterCategory,[Name],Value,[Type],[Precision],OptionCategory,
			OptionListName,[Description],CodeName) 
select CountryCode,@parametercategory,'RI Company Number - Regular Stock',@Company,'numeric',0,'',
			'','RI Company Number for regular stock items','RICompanyNo' 
from Country

if not exists(select * from CountryMaintenance where codename='RICompanyNoRepo')
insert into CountryMaintenance (CountryCode,ParameterCategory,[Name],Value,[Type],[Precision],OptionCategory,
			OptionListName,[Description],CodeName) 
select CountryCode,@parametercategory,'RI Company Number - Repossession Stock',@RepoCompanyNo,'numeric',0,'',
			'','RI Company Number for repossession stock items','RICompanyNoRepo' 
from Country

