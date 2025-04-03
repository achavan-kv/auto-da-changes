-- **********************************************************************
-- Title: trig_cmstrategy
-- Developer: Alex Ayscough
-- Date: April 2007
-- Purpose:Create Trigger for sort order for Strategies
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 17/11/09 jec  UAT643 Sort Order for New Strategies.
-- 08/02/12 jec #9521 CR9417 - duplication of existing strategies
-- **********************************************************************

if exists (select * from sysobjects where type ='tr' and name = 'trig_cmstrategy')
drop trigger trig_cmstrategy
go 
-- allows you to determine in which order the strategies will appear
create trigger trig_cmstrategy on cmstrategy for insert ,update,delete
as declare
	@strategy varchar(7), -- #9521
	@description varchar(132)
select @strategy = strategy,
@description = description from inserted where isactive = 1
if @@rowcount >0
begin
   if not exists (select * from code where category ='SS1' and code =@strategy)
   begin
      insert into code (origbr,category,code,codedescript,statusflag,sortorder,reference,additional) -- include new column
      select 0,'SS1',@strategy,@description,'L',ISNULL(MAX(sortorder),0)+5,0,''
			from code c where c.category='SS1'			-- UAT643 set sort order to max +5
   end
   else
   begin -- just make sure the descriptions are the same.
      update code set codedescript = @description where category='SS1' AND code = @strategy
   end
end
-- removing the strategy from code maintenance if being deleted.
if exists (select * from deleted) and not exists (select * from inserted)
begin
   delete from code where category='SS1' AND code = @strategy
end
go

-- End  End End End End End End End End End End End End End End End End End End End
