-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

declare @CYdate DATETIME, @LYdate DATETIME

set @CYdate= CONVERT(DATETIME ,CONVERT(CHAR(12),GETDATE(), 112))
set @LYdate= CONVERT(DATETIME ,CONVERT(CHAR(12),DATEADD(year,-1,GETDATE()), 112))

if not exists(select * from WTRDates)
insert into wtrdates
select @CYdate,@CYdate,@LYdate,@LYdate,0,'Report1',@CYdate,@CYdate,@LYdate,@LYdate,0,'Report2',
@CYdate,@CYdate,@LYdate,@LYdate,0,'Report3',@CYdate,@CYdate,@LYdate,@LYdate,0,'Report4',@CYdate,@CYdate,@LYdate,@LYdate,0,'Report5'


