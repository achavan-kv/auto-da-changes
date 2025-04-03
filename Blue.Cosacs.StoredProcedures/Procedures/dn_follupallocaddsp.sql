SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[dn_follupallocaddsp]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[dn_follupallocaddsp]
GO

CREATE procedure dn_FollupallocAddsp
@acctno varchar (12),
@empeeno integer,
@user integer, 
@return integer out
as declare
@dateallocated datetime,
@arrears money,
@allocno integer,
--@employeetype varchar (4),
@empTypeReference integer, -- UAT(5.2) - 754

@empeenodeallocated integer ,
@empeenocount integer,
@empeenodeallocatedcount integer ,
@now datetime

set @return=0

--------------------------------------------------------------

/*if this is the Bailliff then the date allocated is only set when it is printed
is available only have picked it up by then*/
   set @now =getdate()
if (admin.CheckPermission(@empeeno,381) = 1)  -- UAT(5.2) - 754
    set @dateallocated = null
else
    set @dateallocated=@now

select @empeenodeallocated = isnull (empeeno, 0) from follupalloc
where acctno =@acctno and datedealloc is null

select @allocno= isnull(Max (allocno),0) from follupalloc
where acctno =@acctno
set @allocno=@allocno + 1
/*updating alphabetically to prevent deadlocks*/
update courtsperson set alloccount = alloccount- 1
where userid =@empeenodeallocated

select @empeenodeallocatedcount=alloccount 
from courtsperson 
WHERE userid =@empeenodeallocated

update courtsperson 
set alloccount = alloccount+ 1
where userid = @empeeno

select @empeenocount =alloccount 
from courtsperson where
userid = @empeeno

-- RD 09/06/06 682727 to ensure that we do not have negative alloccount
UPDATE 	courtsperson 
SET alloccount = 0
WHERE	UserId =@empeeno
AND	alloccount < 0

update follupalloc set datedealloc = @now,empeenodealloc=@user
where datedealloc is null and acctno =@acctno

select @arrears =arrears from acct where acctno =@acctno

insert into follupalloc
(acctno, empeeno, datealloc,
allocno,datedealloc, allocarrears,
bailfee,allocprtflag,empeenoalloc, empeenodealloc)
values(@acctno,@empeeno,@dateallocated,
@allocno,null, @arrears , 
0 ,'N', @user , 0)


GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

