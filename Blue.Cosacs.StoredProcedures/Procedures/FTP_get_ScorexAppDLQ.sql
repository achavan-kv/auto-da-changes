if exists (select * from dbo.sysobjects where id = object_id('[dbo].[FTP_get_ScorexAppDLQ]') 
                        and OBJECTPROPERTY(id, 'IsProcedure') = 1)
Drop Procedure FTP_get_ScorexAppDLQ
go
Create Procedure FTP_get_ScorexAppDLQ

-- Author: John Croft based on [Andrew Zanevsky, 21st Sentry, Inc. (www.pinnaclepublishing.com)]
-- Procedure to FTP Scorex Application & Delinquency data (get)
@Filename varchar (30)

as

set nocount on
create table #ftp_script ( 
        script_line varchar(255) not null )
create table #ftp_result ( 
        result_line varchar(255) null )

insert #ftp_script values ( 'scorexftp' )  -- user name
insert #ftp_script values ( 'scoretrack' ) -- password

-- The following statement is country specific - change country id here>>>^^<
insert #ftp_script values ( 'get ' + @Filename + ' d:\users\default\'
				 + @Filename ) -- get new file
insert #ftp_script values ( 'rename ' + @Filename + ' '
			 + @Filename + '.REN') -- rename prev file to .REN
insert #ftp_script values ( 'quit' )

insert #ftp_result ( result_line ) 
    exec ftp 
        '#ftp_script', 
        'script_line', 
        '217.196.251.123'	--'scorexftp.courtsbiz.com'	--'193.123.196.141'

drop table #ftp_script
drop table #ftp_result 

-- End End End End End End End End End End End End End End End End End End End End End End

GO
