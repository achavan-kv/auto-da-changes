if exists ( select * from sysobjects where name = 'trig_PropFlagUpdate')

drop trigger trig_PropFlagUpdate
go

--------------------------------------------------------------------------------
--
-- Project      : CoSACS .NET
-- File Name    : trig_PropFlagUpdate.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        :
-- Author       : Ilyas Parker
-- Date         : 27/06/11
--
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 28/06/11  IP  5.13 - LW73619 - #3751 - If holdprop = 'N' and account sanction stages are re-opened
--				 then need to insert a record into delauthorise so that these are visible in Incomplete Credit screen.
--				 Once the stages are marked as complete and if holdprop = 'N' (not awaiting DA) then this entry previously
--				 added must be removed.
--------------------------------------------------------------------------------

create trigger trig_PropFlagUpdate
on proposalflag
for update
as 
declare @acctno char(12),
		@checktype varchar(4),
		@datecleared datetime,
		@olddatecleared datetime, 
		@holdprop char(1)

select  @acctno = acctno,
		@checktype = checktype,
        @datecleared = datecleared 
from inserted


select @olddatecleared = datecleared
from deleted 


select @holdprop = holdprop from agreement where acctno = @acctno

if(@checktype in('S1', 'S2', 'DC', 'R') and @olddatecleared is not null and @datecleared is null and @holdprop = 'N')
begin
	 EXECUTE dbnewauth @acctno = @acctno
end

--If this account is being referred for the first time where holdprop = N (meaning not awaiting DA)
if(@checktype = 'R' and @olddatecleared is null and @datecleared is null and @holdprop = 'N')
begin
	 EXECUTE dbnewauth @acctno = @acctno
end
