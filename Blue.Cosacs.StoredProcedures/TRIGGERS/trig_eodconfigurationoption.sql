if  exists (select * from sysobjects  where name =  'trig_eodconfigurationoption' )
drop trigger trig_eodconfigurationoption
go
/* trigger to audit changes in the eodconfiguratoinoptions table.*/
create trigger trig_eodconfigurationoption
-- =============================================
-- Author:		??
-- Create date: ??
-- Description:	trig_eodconfigurationoption
--
--	This trigger writes the configuration changes to the audit table
-- 
-- Change Control
-----------------
-- 27/11/09 jec UAT693 correct the update to audit table
-- 05/04/11 jec RI Add Rerunno to Audit
-- =============================================
	-- Add the parameters for the stored procedure here
on eodconfigurationoption 
for insert, update,delete
as
declare  @newconfigurationname varchar(16),@oldconfigurationname varchar(16),
 @lasteditby int,@datelastedit datetime,@newoptioncode varchar(16),@oldoptioncode varchar(16),
@newsortorder smallint,@oldsortorder smallint,@action varchar(10)

     insert into eodconfigurationoptionaudit
    (configurationname,     lasteditby,
     datelastedit,     optioncode,
     newsortorder,     oldsortorder,
     ConfigAction,ReRunNo)			-- jec 05/04/11
     select 
	 configurationname,     lasteditby,
     GETDATE(),     optioncode,
     0,     sortorder,
     'Deleted',ReRunNo		-- jec 05/04/11
     from deleted

-- This bit of code doesn't seem necessary  - 
     --update e
     -- set configaction ='Changed',
     -- newsortorder = sortorder
     -- from inserted i,eodconfigurationoptionaudit e
     -- where i.optioncode=e.optioncode
     -- and i.configurationname=e.configurationname
     -- and i.lasteditby = e.lasteditby
     -- and i.sortorder !=e.oldsortorder
     -- and getdate() >dateadd(second,3,e.datelastedit )
 
	  --delete
      update e
      set configaction ='Changed',
      newsortorder = sortorder
      from inserted i,eodconfigurationoptionaudit e
      where i.optioncode=e.optioncode
      and i.configurationname=e.configurationname
      and i.lasteditby = e.lasteditby
      and i.sortorder !=e.oldsortorder
      and getdate() >dateadd(second,-3,e.datelastedit )
      -- ensures only changing rows from last save
      and e.datelastedit !< dateadd(second,-5,getdate())
 
       -- marking those as the same where no changes have taken place
      update e
      set configaction ='same',
      newsortorder = sortorder
      from inserted i,eodconfigurationoptionaudit e
      where i.optioncode=e.optioncode
      and i.configurationname=e.configurationname
      and i.lasteditby = e.lasteditby
      and i.sortorder =e.oldsortorder
      and getdate() >dateadd(second,-3,e.datelastedit )
      -- ensures only changing rows from last save 
      and e.datelastedit !< dateadd(second,-5,getdate())
    
      
     insert into eodconfigurationoptionaudit
    (configurationname,     lasteditby,
     datelastedit,     optioncode,
     newsortorder,     oldsortorder,
     ConfigAction,ReRunNo)		-- jec 05/04/11
     select 
	 configurationname,     lasteditby,
     GETDATE(),     optioncode,
     sortorder,		0,
     'Added' , ReRunNo			-- jec 05/04/11
     from inserted i
	 where not exists (select * from eodconfigurationoptionaudit e
      where i.optioncode=e.optioncode
      and i.configurationname=e.configurationname
      and i.lasteditby = e.lasteditby
      and e.datelastedit >dateadd(second,-3,getdate()) )
      
     -- don't want any where no change made
	  delete from eodconfigurationoptionaudit where configaction ='same'
go

-- End End End End End End End End End End End End End End End End End End End End End End
