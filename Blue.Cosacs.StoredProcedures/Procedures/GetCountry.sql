if exists (select * from sysobjects where name = 'GetCountry')
drop procedure getCountry
go 

create procedure getCountry as 

select CountryCode from Country

GO 