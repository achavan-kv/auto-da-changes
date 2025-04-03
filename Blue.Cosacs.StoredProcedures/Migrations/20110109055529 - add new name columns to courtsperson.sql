-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here


alter table courtsperson
add firstname varchar(50)
go

alter table courtsperson
add lastname varchar(50)
GO



update courtsperson 
set firstname = 
ltrim(rtrim(substring(empeename, 1, charindex(' ', empeename)))),  
lastname = 
ltrim(rtrim(substring(empeename, charindex(' ', empeename), len(empeename))))
GO

alter table courtsperson
drop column empeename 
go

SET QUOTED_IDENTIFIER ON

alter table courtsperson
add empeename AS (firstname + ' ' + lastname)
GO

SET QUOTED_IDENTIFIER OFF