-- Script for Address Standardization CR2019 - 025
--Script Description : To Change Char(2) to char(3) Datalength for custel table to save and edit and get purpose delivery mobile funtionality

IF OBJECT_ID('[dbo].[custtelAudit_bk]', 'U') IS NOT NULL 
  DROP TABLE [dbo].[custtelAudit_bk]; 
	ELSE	
SELECT custid,datechange,tellocn,dateteladd,datediscon,telno,extnno,DialCode ,empeenochange,ChangeType INTO [dbo].[custtelAudit_bk] from custtelAudit

ALTER TABLE [dbo].[custtelAudit] ALTER COLUMN [tellocn] CHAR(3);
GO
IF OBJECT_ID('[dbo].[custtelAudit_bk]', 'U') IS NOT NULL 
  DROP TABLE [dbo].[custtelAudit_bk]; 