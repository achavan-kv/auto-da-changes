
if exists (select * from dbo.sysobjects where id = object_id('[dbo].[FTP_ScorexDLQ]') 
            and OBJECTPROPERTY(id, 'IsProcedure') = 1)
Drop Procedure FTP_ScorexDLQ
go

Create Procedure FTP_ScorexDLQ

         @return int OUTPUT 

-- Author: John Croft based on [Andrew Zanevsky, 21st Sentry, Inc. (www.pinnaclepublishing.com)]
-- Procedure to FTP Scorex Delinquency data

-- modified 14/07/05 - change IP address to DNS name
--                   - Country table added as per Application data script
-- John Croft 29/03/06  Mods for .Net EOD CR781, error checking
   
as

set nocount on

declare @country char(2),
	@countrycode char(2),
    @status integer

SET 	@return = 0			--initialise return code
SET 	@status = 0
-- Country not using ISA server - execute FTP routines

If (select value from countrymaintenance where CodeName = 'HasISAserver')!='True'

Begin
--select countrycode from country
set @countrycode=(select countrycode from country)
set @country=case
	when @countrycode='N' then 'AN'
	when @countrycode='B' then 'BB'
	when @countrycode='Z' then 'BZ'
	when @countrycode='D' then 'DM'
	when @countrycode='F' then 'FJ'
	when @countrycode='H' then 'HC'
	when @countrycode='G' then 'GR'
	when @countrycode='A' then 'GY'
	when @countrycode='I' then 'IN'
	when @countrycode='J' then 'JA'
	when @countrycode='C' then 'MG'
	when @countrycode='Y' then 'MY'
	when @countrycode='M' then 'MR'
	when @countrycode='P' then 'PN'
	when @countrycode='S' then 'SP'
	when @countrycode='K' then 'SK'
	when @countrycode='L' then 'SL'
	when @countrycode='V' then 'SV'
	when @countrycode='H' then 'TH'
	when @countrycode='T' then 'TR'
	else 'XX'
	end

--select @country

create table #ftp_script ( 
        script_line varchar(255) not null )
create table #ftp_result ( 
        result_line varchar(255) null )

insert #ftp_script values ( 'scorexftp' )
insert #ftp_script values ( 'scoretrack' )
-- The following statement is country specific - change country id here>>>^^<
insert #ftp_script values ( 'put d:\users\default\ScorexDLQ.csv ScorexDLQ_' + @Country + '.csv' )
insert #ftp_script values ( 'quit' )

insert #ftp_result ( result_line ) 
    exec ftp 
        '#ftp_script', 
        'script_line', 
	'scorexftp.courtsbiz.com'  --        '193.123.196.141'

drop table #ftp_script
drop table #ftp_result 

End

IF @@error != 0
	BEGIN
		SET @return = @@error
	END

Go
-- End End End End End End End End End End End End End End End End End End End End End End