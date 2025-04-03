-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF NOT EXISTS (SELECT * FROM code WHERE category = 'edc'
AND code = 'STInterest' )
begin
   insert into code (origbr,category,code,codedescript,statusflag,sortorder,reference)
   values (0,'EDC','STInterest','Store Card Interest Calculation','L',13,0)
END


IF NOT EXISTS (SELECT * FROM code WHERE category = 'edc'
AND code = 'STStatements' )
begin
   insert into code (origbr,category,code,codedescript,statusflag,sortorder,reference)
   values (0,'EDC','STStatements','Store Card Statements','L',13,0)
END

