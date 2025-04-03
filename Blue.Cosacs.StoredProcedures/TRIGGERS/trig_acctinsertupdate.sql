-- 11 Nov 2004 AA  changing to prevent performance problems for arenas calculation the agreement total should only be changed
-- one account at a time
if exists (select * from sysobjects  where name =  'trig_acctinsertupdate' )
drop trigger trig_acctinsertupdate
go
create trigger trig_acctinsertupdate
on acct
for insert, update as declare @acctno char(12),@accttype varchar (2),
@errortext varchar (128),@datelast DateTime,@agrmttotal money,@count integer
 select @count =count (*)  from inserted
 if @count <3
 begin

--need to make sure that the termstype exists for hp accounts , but only if the termstype is changing
       if exists (select inserted.termstype from inserted where not exists (select t.termstype from 
            		termstypetable t where t.termstype = inserted.termstype) and inserted.acctno like '___0%' 
                    and not exists (select * from deleted where
                    inserted.acctno = deleted.acctno and inserted.termstype =deleted.termstype))
      begin
         select @acctno = acctno from inserted
         set @errortext= 'invalid termstype saving account ' + @acctno
         rollback
        RAISERROR ( @errortext, 16, 1,0,0)
      end


  select  @acctno = acctno,
        @accttype =accttype,
        @agrmttotal=agrmttotal

          from inserted

   if not exists (select * from deleted where acctno =@acctno) and @accttype = 'R' -- needs del authorise if this is a new account
   BEGIN
      EXECUTE dbnewauth @acctno = @acctno
   END

   if exists (select d.agrmttotal from deleted d,inserted i where d.acctno = i.acctno and d.agrmttotal !=i.agrmttotal)
   begin
         -- getting latest date of revision since last summary report was run
			select @datelast =isnull (max(datefinish), getdate()) from interfacecontrol where interface ='Summary1'
		   
			update revisedhist set dateagrmtrevised = getdate(),agrmttotal= i.agrmttotal
         from inserted i where revisedhist.acctno =i.acctno and revisedhist.dateagrmtrevised>@datelast

			insert into revisedhist (origbr, acctno, dateagrmtrevised, agrmttotalorig, agrmttotal)
 			select  0,i.acctno,getdate(),d.agrmttotal,i.agrmttotal from inserted i,deleted d
         where i.acctno =d.acctno and i.agrmttotal !=d.agrmttotal and i.dateacctopen < @Datelast
         and not exists (select * from 
			revisedhist r where r.acctno =i.acctno and r.dateagrmtrevised>@datelast)

   end
 end -- only doing this if less than 2 rows updated to prevent performance problems
go
