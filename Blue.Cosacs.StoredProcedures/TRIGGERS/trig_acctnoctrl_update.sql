
/************************************************/
/**  Create trigger trig_acctnoctrl_update     **/
/************************************************/

if exists (select * from sysobjects 
		where type='TR' and name = 'trig_acctnoctrl_update')

drop trigger trig_acctnoctrl_update
go

--CREATE TRIGGER trig_acctnoctrl_update
--ON acctnoctrl
--FOR update
--AS 
--  declare @hiallocated int, @branchno smallint, @acctcat varchar (3), @error varchar (500)
--	select @hiallocated = hiallocated from inserted
--	select @acctcat = acctcat from inserted
--	select @branchno = branchno from inserted

--   if (select hiallocated from deleted where acctcat = @acctcat and branchno = @branchno) > @hiallocated
--   begin
--	set @error = 'Highest Allocated must not be lower than previously set or you will have duplicate account numbers in FACT. If you need to do this then contact the CoSACS Support Centre.'
--  	RAISERROR(@error, 16, 1) 
--  end

