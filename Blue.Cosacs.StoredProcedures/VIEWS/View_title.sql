/*******************************************************/
/**	Create view title			      **/
/*******************************************************/
If exists (select * from information_schema.tables where table_name = 'title' and table_type ='BASE TABLE')
drop table title
GO
drop view title
GO

create view title as select codedescript as title, sortorder from code where category = 'TTL'
go

grant DELETE, INSERT, REFERENCES, SELECT, UPDATE on title to public
--grant all on title to public
--print 'creating title view'
--go

