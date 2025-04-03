if exists (select * from sysobjects where name = 'trig_country')
	drop trigger trig_country
go

create trigger trig_country on country
for update -- of countrymaintenance table from country
as declare @codename varchar (32),@value varchar (128),@type varchar (14), @datatype varchar (14),@intvalue integer,
@statement sqltext, @charvalue varchar(128),@column_name varchar (32)
--select ' fetch ' + convert (varchar,@@fetch_status)
if not exists (select * from countryupdated where isnull(dateupdate,'1-jan-1900') > dateadd (second, - 10, getdate()) AND updatedby='Net')
begin
   update countryupdated set dateupdate = getdate(),updatedby='OpenROAD'
   set nocount on
			DECLARE component22 CURSOR local
			FOR 	
				SELECT 	column_name 
				FROM 	information_schema.columns
				WHERE	table_name = 'country' --AND COLUMN_NAME ='systemopen'

			OPEN component22
		
			FETCH NEXT FROM component22
			INTO @column_name

			WHILE @@fetch_status = 0
			BEGIN
            set @statement = 'update countrymaintenance set value = convert (varchar,country.'  + @column_name
            + ') from country where countrymaintenance.type !=''checkbox'' and countrymaintenance.codename =' + '''' + @column_name + ''''
            + ' and value != convert (varchar,country.'  + @column_name + ')'
				exec sp_executeSQL @statement	
            --print 'here'
            set @statement = 'update countrymaintenance set value = ''False'' ' 
            + ' from country where countrymaintenance.type =''checkbox'' and countrymaintenance.codename =' + '''' + @column_name + ''''
            + ' and convert(varchar,country.' + @column_name + ') in (''N'', ''0'')  ' +
            ' and value !=  ''False'' ' 

				exec sp_executeSQL @statement	
            set @statement = 'update countrymaintenance set value= ''True'' '  
            + ' from country where countrymaintenance.type =''checkbox'' and countrymaintenance.codename =' + '''' + @column_name + ''''
            + ' and convert(varchar,country.' + @column_name + ') in (''Y'', ''1'') '
            + ' and value !=  ''True'' ' 
				exec sp_executeSQL @statement	

				FETCH NEXT FROM component22
				INTO @column_name
			END

	CLOSE component22 		
	DEALLOCATE component22 
end
go
