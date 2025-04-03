if  exists (select * from sysobjects  where name =  'trig_fintrans' )
drop trigger trig_fintrans
go

-- All this now in INSTEAD of trigger
