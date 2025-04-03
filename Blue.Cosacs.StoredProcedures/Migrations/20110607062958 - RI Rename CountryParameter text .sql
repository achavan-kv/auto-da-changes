-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

UPDATE CountryMaintenance
	set name= REPLACE(name,'PODY','POD')
Where codename like 'RIPODYpath'

UPDATE CountryMaintenance
	set name= REPLACE(name,'OHQY','OHQ'),[Description]=REPLACE([Description],'OHQY','OHQ')
Where codename like 'RIOHQYpath%'


