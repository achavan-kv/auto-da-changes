-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here


IF NOT EXISTS(SELECT * FROM sysobjects WHERE NAME ='temp_Summary1_MR')
   SELECT * INTO temp_Summary1_MR
   FROM Summary1_MR WHERE acctno = 'bob'
GO
  
if (not exists
(select * from sys.columns a inner join sys.objects b
 on  a.object_id=b.object_id
 where a.name like 'Repoamt' and b.name='temp_Summary1_MR'))
 begin
 alter table temp_Summary1_MR add repoamt money
 end
GO

IF NOT EXISTS(SELECT * FROM sysobjects WHERE NAME ='temp_Summary1_MR_acctno')
   ALTER TABLE temp_Summary1_MR ADD CONSTRAINT temp_Summary1_MR_acctno PRIMARY KEY  CLUSTERED ( acctno)
GO

if (not exists
(select * from sys.columns a inner join sys.objects b
 on  a.object_id=b.object_id
 where a.name like 'Repoamt' and b.name='Summary1_MR'))
 begin
 alter table Summary1_MR add repoamt money
 end
GO