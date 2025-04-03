
-- Script Comment : Insert codecat
-- Script Name : Insert_codecat.sql
-- Created For	: BB/BZ/TT
-- Created By	: Nilesh
-- Created On	: CR
-- Modified On	Modified By	Comment

IF NOT EXISTS (SELECT * FROM [codecat] WHERE [category]='CST')
BEGIN
   Insert into codecat (category,catdescript,	codelgth,	forcenum,forcenumdesc,	usermaint)
 values ('CST',	'Flag Customer Status',3,'N','N','Y')
END

IF NOT EXISTS (SELECT * FROM [codecat] WHERE [category]='MBN')
BEGIN
  Insert into codecat (category,catdescript,	codelgth,	forcenum,forcenumdesc,	usermaint)
 values 
('MBN',	'Mobile number availability',3,'N','N','Y')
END
