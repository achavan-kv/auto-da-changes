-- Author; John Croft  
-- Date; May 2004

/*
   This script will ftp data using a script table 
   
*/


if exists (select * from dbo.sysobjects where id = object_id('[dbo].[ftp]') 
            and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure ftp
Go

create proc ftp
-- Purpose: Execute an FTP script 
-- Author:  Andrew Zanevsky, 21st Sentry, Inc. (www.pinnaclepublishing.com)
	@script_table sysname,
	@column_name  sysname,
	@ftp_cmd_line varchar(255) = ' '
as
set nocount on
declare @line     varchar(255), 
        @cmd      varchar(255), 
        @pipe     varchar(2), 
        @ftp_file varchar(255)
select  @pipe = '>', 
        @ftp_file = 'd:\users\default\~~ftp_temp_' + 
            convert( varchar, @@spid ) + '.ftp'

create table #script ( 
        line varchar(255) not null )
exec( 'insert #script select ' + @column_name + 
      ' from ' + @script_table )

declare script cursor 
for select line from #script

open script 
fetch script into @line
while @@fetch_status = 0 begin
	select @cmd = 'echo ' + @line + @pipe + @ftp_file
	exec master..xp_cmdshell @cmd
	fetch script into @line
	select @pipe = '>>'
end

close script
deallocate script

select @cmd = 'ftp -s:' + @ftp_file + ' ' + 
              @ftp_cmd_line
exec master..xp_cmdshell @cmd
go


/* To test the ftp procedure execute the following code between dashes:
-------------------------------------------------
drop table ftp_script
drop table ftp_result 

set nocount on
create table ftp_script ( 
        script_line varchar(255) not null )
create table ftp_result ( 
        result_line varchar(255) null )

insert ftp_script values ( 'scorexftp' )
insert ftp_script values ( 'scoretrack' )
insert ftp_script values ( 'put d:\users\default\ScorexDLQ.csv ScorexDLQsv' )
insert ftp_script values ( 'quit' )

insert ftp_result ( result_line ) 
    exec ftp 
        'ftp_script', 
        'script_line', 
        '193.123.196.141'

select * from ftp_result

-- drop table #ftp_script
-- drop table #ftp_result 
-------------------------------------------------
*/
