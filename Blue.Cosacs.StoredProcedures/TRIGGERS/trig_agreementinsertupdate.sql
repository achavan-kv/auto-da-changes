if exists (select * from sysobjects where name = 'trig_agreementinsertupdate')
drop trigger trig_agreementinsertupdate
go
--create trigger trig_agreementinsertupdate
--on agreement for update, insert
--as declare @acctno char(12), @agrmtno int, @orig_agrmtno int, @error varchar(256)
--select
--@acctno = inserted.acctno,@agrmtno = inserted.agrmtno,@orig_agrmtno = deleted.agrmtno
--from inserted, deleted
--where inserted.acctno = deleted.acctno and right(left(inserted.acctno,4),1) <> '5'
--if @agrmtno <> 1 
--begin
--	set @error ='error agrmtno should be 1 as this is not a special account. Acctno ' + @acctno + '; Current agrmtno is ' +  convert(varchar,@orig_agrmtno) + '; New agrmtno is ' +  convert(varchar,@agrmtno)
--  		RAISERROR(@error, 16, 1)
--end
--go
