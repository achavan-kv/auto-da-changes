-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

select empeeno,Worklist,COUNT(*) from dbo.CMWorkListRights 
group by empeeno,Worklist having COUNT(*)>1

if @@ROWCOUNT>0
BEGIN
	
	select distinct Empeeno,worklist,MAX(empeetype) as empeetype,MAX(Empeenochange) as Empeenochange
	into #CMWorkListRights
	from CMWorkListRights 
	group by empeeno,Worklist
	
	truncate TABLE CMWorkListRights
	
	insert into CMWorkListRights	
	select * from #CMWorkListRights
	
END
