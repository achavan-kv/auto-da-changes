
if exists (select * from sysobjects where name ='CMEod_CreateSMSdatafileSP')
drop procedure CMEod_CreateSMSdatafileSP
go
create procedure CMEod_CreateSMSdatafileSP 
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : CMEod_CreateSMSdatafileSP.prc
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Create SMS data file
-- Author       : John Croft
-- Date         : June 2008
--
-- This procedure will create populate a table that will be exported to a csv datafile of SMS messages.
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 08/07/10 jec UAT1043 Allow for PTP value in SMS.
-- 13/08/10 jec CR1084 Allow for new values in SMS
-- 13/09/10 jec Multiple values error
-- 07/10/10 jec exclude discontined numbers!
-- 13/02/12 ip  Replace GETDATE() with @rundate
-- ================================================
	-- Add the parameters for the stored procedure here
	@runno int output,
	@rundate DATETIME,										--IP - 13/02/12 	 
	@return int OUTPUT
as

	set @return=0

	IF EXISTS (SELECT table_name FROM INFORMATION_SCHEMA.TABLES
		   WHERE  table_schema = 'dbo' and table_name = 'SMSdatafile')
		truncate table .dbo.SMSdatafile        
	else

	create table SMSdatafile
(
	SMSmobile		varchar(20),
	SMSaccount		varchar(12),
	SMStext			varchar(3000)
)

	declare @datestart datetime,
			@datefinish datetime,
			@maxrunno int
	-- get latest run info
	set @maxrunno= (select MAX(runno) from interfacecontrol where interface='Collections' and result='P') 
	set @datestart = (select datestart from interfacecontrol where interface='Collections' and runno=@maxrunno) 
	set @datefinish = (select datefinish from interfacecontrol where interface='Collections' and runno=@maxrunno) 
	set @runno=@maxrunno

	insert into SMSdatafile
	select ltrim(RTRIM(t.DialCode)+LTRIM(t.telno)),s.acctno,tx.SMSText
	from sms s INNER JOIN custacct ca on s.acctno = ca.acctno
				INNER JOIN custtel t on ca.custid = t.custid and tellocn='M'  -- mobile only
										and t.datediscon is null	-- jec 07/10/10
				INNER JOIN CMSMS tx on s.code=tx.SMSName
	where s.dateadded between @datestart and @datefinish
	and datalength(ltrim(RTRIM(t.DialCode)+LTRIM(t.telno))) between 6 and 20
	and isnumeric(t.telno)=1

	set @return=@@ERROR

-- insert account number in text
	UPDATE SMSdatafile
		set SMStext=REPLACE(SMStext,'{AN}',SMSaccount)

-- insert Customer Name in text
	UPDATE SMSdatafile
		set SMStext=REPLACE(SMStext,'{CN}',(select firstname from customer c 
				INNER JOIN custacct ca on c.custid = ca.custid 
				where ca.acctno=SMSaccount and ca.hldorjnt='H'))

-- insert Due Day in text
--	UPDATE SMSdatafile
--		set SMStext=REPLACE(SMStext,'{DD}',(select dueday from instalplan
--									where acctno=SMSaccount))

--or full datedue	
	-- why is day in datenextdue one day behind dueday?? 
	UPDATE SMSdatafile
		set SMStext=REPLACE(SMStext,'{DD}',(select CAST(DATEADD(d,1,datenextdue) as varchar(12)) from agreement
									where acctno=SMSaccount))
-- insert Instalment in text
	UPDATE SMSdatafile
		set SMStext=REPLACE(SMStext,'{IN}',(select instalamount from instalplan
									where acctno=SMSaccount))
-- insert Arrears in text
	UPDATE SMSdatafile
		set SMStext=REPLACE(SMStext,'{AR}',(select arrears from acct
									where acctno=SMSaccount))
-- insert Arrears + Fee in text			-- 13/08/10 jec
	UPDATE SMSdatafile
		set SMStext=REPLACE(SMStext,'{AF}',(select MAX(arrears)-ISNULL(SUM(transvalue),0) from acct a 
		LEFT outer JOIN fintrans f on a.acctno=f.acctno and transtypecode in('FEE') where a.acctno=SMSaccount))
-- insert PTP duedate in text	
	UPDATE SMSdatafile
		set SMStext=REPLACE(SMStext,'{PD}',ISNULL((select CAST(DATEADD(d,0,MIN(datedue)) as varchar(12)) 
				from bailaction b INNER JOIN sms on b.acctno=sms.acctno 
					and sms.dateadded between @datestart and @datefinish	--jec 13/09/10
									--where b.acctno=SMSaccount and datedue >= SMS.dateadded and b.code='PTP'),GETDATE()))
									where b.acctno=SMSaccount and datedue >= SMS.dateadded and b.code='PTP'),@rundate))			--IP - 13/02/12 - use @rundate
-- insert PTP Instalment in text
	UPDATE SMSdatafile
		set SMStext=REPLACE(SMStext,'{PT}',ISNULL((select CAST(MAX(actionvalue) as money) --jec 13/09/10
				from bailaction b INNER JOIN sms on b.acctno=sms.acctno 
					and sms.dateadded between @datestart and @datefinish	--jec 13/09/10
									where b.acctno=SMSaccount and datedue >= SMS.dateadded and b.code='PTP'),0))									
-- insert Balance Incl. charges in text			CR1084
	UPDATE SMSdatafile
		set SMStext=REPLACE(SMStext,'{BIC}',(select outstbal from acct
									where acctno=SMSaccount))
-- insert Balance Excl. charges in text		CR1084
	UPDATE SMSdatafile
		set SMStext=REPLACE(SMStext,'{BEC}',(select MAX(outstbal)-ISNULL(SUM(transvalue),0) from acct a 
		LEFT Outer JOIN fintrans f on a.acctno=f.acctno and transtypecode in('Int','ADM') where a.acctno=SMSaccount))

-- insert Total Fee + charges in text		CR1084
	UPDATE SMSdatafile
		set SMStext=REPLACE(SMStext,'{FC}',(select ISNULL(SUM(transvalue),0) 
				from  fintrans f where transtypecode in('Int','ADM','FEE') and f.acctno=SMSaccount))
-- insert Fees  in text		CR1084
	UPDATE SMSdatafile
		set SMStext=REPLACE(SMStext,'{FEE}',(select ISNULL(SUM(transvalue),0) 
				from  fintrans f where transtypecode in('FEE') and f.acctno=SMSaccount))		
-- insert Interest  in text		CR1084
	UPDATE SMSdatafile
		set SMStext=REPLACE(SMStext,'{INT}',(select ISNULL(SUM(transvalue),0) 
				from  fintrans f where transtypecode in('INT') and f.acctno=SMSaccount))
-- insert Admin Fee  in text		CR1084
	UPDATE SMSdatafile
		set SMStext=REPLACE(SMStext,'{ADM}',(select ISNULL(SUM(transvalue),0) 
				from  fintrans f where transtypecode in('ADM') and f.acctno=SMSaccount))
	
													
-- Replace any commas in text
	UPDATE SMSdatafile
		set SMStext=REPLACE(SMStext,',','.')

-- This code needs to be in .net to work
---- now export data to csv file with date tag
--declare @path varchar(200), @date CHAR(8)
--set @date=CONVERT(CHAR(8),GETDATE(),112)
----set @date=REPLACE(@date,' ','_')
--
--set @path = '"c:\program files\microsoft sql server\90\tools\binn\BCP" ' + +db_name()+'..SMSdatafile' + ' out ' +
--'d:\users\default\SMSmessage'+ RTRIM(@date) +'.csv ' + '-c -t, -q -T'
--
--exec master.dbo.xp_cmdshell @path
--
--c:program files\microsoft sql server\80\tools\binn\BCP";
--           proc.StartInfo.Arguments = dbname + @".dbo.SMSdatafile out d:\users\default\SMSmessage_"+ Convert.ToString(runno) + ".csv -c -t, -q -T";
--


--End End End End End End End End End End End End End End End End End End End End End End End End End 