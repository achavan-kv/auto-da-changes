-- Add caller Commissions Parameter

declare @cat INT
select @cat = (select code from code where category='CMC' and codedescript='Sales Commissions')

insert into CountryMaintenance ( CountryCode, ParameterCategory, Name, Value, Type, Precision, OptionCategory, OptionListName, Description, CodeName)
select top 1  countrycode, @cat, 'Max Days for Caller Commissions',14, 'numeric',0,'','','Actions will only be considered for commission after this number of days has passed','MaxDaysCallerComm'
from CountryMaintenance
where not exists (select 'x' from CountryMaintenance where codename='MaxDaysCallerComm')
group by countrycode

