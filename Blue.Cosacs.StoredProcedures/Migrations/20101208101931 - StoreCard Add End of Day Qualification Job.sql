-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

if not exists (select * from code where category ='EDC' and code ='STCARDQUAL')
begin
   insert into code (origbr,category,code,codedescript,statusflag,sortorder,reference)
   values (0,'EDC','STCARDQUAL','Store Card Qualification','L',13,0)
end
go 
