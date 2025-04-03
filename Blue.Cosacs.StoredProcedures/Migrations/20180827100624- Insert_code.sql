
-- Script Comment : Insert equifax code
-- Script Name : Insert_code.sql
-- Created For	: BB/BZ/TT
-- Created By	: Nilesh
-- Created On	: CR
-- Modified On	Modified By	Comment
--------------------------------------------------------------------------------------------------------------------------
IF NOT EXISTS (SELECT * FROM code WHERE category = 'SCT' AND codedescript LIKE 'Equifax Applicant Scoring')
BEGIN
	INSERT INTO dbo.code (	origbr,	category,	code,	codedescript,	statusflag,	sortorder,	reference,	additional) 
	VALUES ( 0,	'SCT',	'C',	N'Equifax Applicant Scoring',	'L',	2,	'0',	NULL ) 
END

IF NOT EXISTS (SELECT * FROM code WHERE category = 'SCT' AND codedescript LIKE 'Equifax Behaviourial Scoring')
BEGIN
	INSERT INTO dbo.code (	origbr,	category,	code,	codedescript,	statusflag,	sortorder,	reference,	additional) 
	VALUES ( 0, 'SCT', 'D', N'Equifax Behaviourial Scoring','L',3,'0', NULL )
END
-------------------------------------------------------------------------------------------------------------------------

IF NOT EXISTS (SELECT * FROM [code] WHERE [category]='CST' and [code]='N')
BEGIN
    Insert into dbo.code (category,	code,	codedescript,	statusflag,	sortorder,reference) 
	values	('CST',	'N','New Customer','L',1,'0')
END

IF NOT EXISTS (SELECT * FROM [code] WHERE [category]='CST' and [code]='E')
BEGIN
   Insert into dbo.code (category,	code,	codedescript,	statusflag,	sortorder,reference)
   values	('CST',	'E','Existing Without His Data','L',2,'0')
END	
-------------------------------------------------------------------------------------------------------------------------	