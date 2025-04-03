GO
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].NewRFAccountExport') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].NewRFAccountExport
GO


create proc NewRFAccountExport
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : NewRFAccountExport.prc
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : New RF Account extract
-- Author       : Richard Boyce
-- Date         : March 2008
--
-- This procedure will extract the New RF account details
-- 
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 21/05/08	 jec UAT65 change order of columns
-- 08/02/10 jec CR909 Malaysia v4 merge
-- 17/03/10 jec UAT10 Remove null rows
-- ================================================
	-- Add the parameters for the stored procedure here
@datestart	datetime,
@datefinish	datetime

as


select
	'"' + Title + '","' +
	firstname + ' ' + name + '","' +
	cusaddr1 + '","' +
	cusaddr2 + '","' +
	cusaddr3 + '","' +
	cuspocode + '","' +
	customer.custid + '","' +	
	-- returning null rows when "Then null" used
	CASE WHEN home.telno IS NULL THEN '' WHEN LEN(home.telno)  < 2 Then '' else LEFT(home.telno,2) END + '","' +
	CASE WHEN home.telno IS NULL THEN '' WHEN LEN(home.telno)  < 2 Then '' ELSE RIGHT(home.telno,LEN(home.telno) -2) END + '","' +

	CASE WHEN hp.telno IS NULL THEN ''  WHEN LEN(hp.telno)  < 3 Then '' else LEFT(hp.telno,3) END + '","' +
	CASE WHEN hp.telno IS NULL THEN ''  WHEN LEN(hp.telno)  < 3 Then '' ELSE RIGHT(hp.telno,LEN(hp.telno) -3) END + '","' +

	CASE WHEN emp.telno IS NULL THEN ''  WHEN LEN(emp.telno)  < 3 Then '' else LEFT(emp.telno,3) END + '","' +
	CASE WHEN emp.telno IS NULL THEN ''  WHEN LEN(emp.telno)  < 3 Then '' ELSE RIGHT(emp.telno,LEN(emp.telno) -3) END + '","' +
	acct.acctno + '","' +
	REPLACE(CONVERT(VARCHAR(10), dateacctopen, 103), '/', '') + '","' +
	convert(VARCHAR(12),RFCreditLimit) + '","' +
	convert(VARCHAR(12),AvailableSpend) + '"'
	--convert(VARCHAR(12),appnumber) + '","' +
	
	from acct
		inner join custacct
			on acct.acctno = custacct.acctno and hldorjnt = 'H'
		inner join customer
			on custacct.custid = customer.custid
		inner join custaddress
			on custacct.custid = custAddress.custid and addtype='H' and datemoved is null
		left outer join custtel home
			on custacct.custid = home.custid and home.tellocn = 'H' and home.datediscon is null
		left outer join custtel hp 
			on custacct.custid = hp.custid and hp.tellocn = 'M' and hp.datediscon is null
		left outer join custtel emp 
			on custacct.custid = emp.custid and emp.tellocn = 'W' and emp.datediscon is null
		inner join proposal 
			on acct.acctno = proposal.acctno 
	where
		acct.accttype = 'R'
		AND customer.custid  <> ''
	and acct.dateacctopen > @datestart and  acct.dateacctopen <= @datefinish
ORDER BY 1

go
-- End End End End End End End End End End End End End End End End End End End End End End End End End End End End
