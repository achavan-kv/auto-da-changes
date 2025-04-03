
GO
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].ReviseAccountExtract') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].ReviseAccountExtract
GO

create  proc ReviseAccountExtract
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : ReviseAccountExtract.prc
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Revise Account Extract
-- Author       : ??
-- Date         : ??
--
-- This procedure will retrieve Revised accounts for CLMS.
-- 
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 08/02/10 jec CR909 Malaysia v4 merge
-- 17/03/10 jec UAT10 Remove null rows
-- ================================================
	-- Add the parameters for the stored procedure here
@datestart	datetime,
@datefinish	datetime

as
-- revise agreement
SELECT 
	DISTINCT  
	'"' + ELACC_Y.acctno + '","' +
	title + '","' +
	firstname + ' ' + [name] + '","' +
	cusaddr1 + '","' +
	cusaddr2 + '","' +
	cusaddr3 + '","' +
	ELCUS_Y.custid + '","' +
	CONVERT(VARCHAR(12),agreement.agrmtTotal) + '","' +
	CONVERT(VARCHAR(12),instalamount) + '","' +
	ISNULL(REPLACE(CONVERT(CHAR(8), datedel, 3), '/', SPACE(0)),'') + '","' +		-- jec remove null row
	CASE WHEN HOMETEL.telno IS NULL THEN '' WHEN LEN(HOMETEL.telno)  < 2 Then '' else LEFT(HOMETEL.telno,2) END + '","' +
	CASE WHEN HOMETEL.telno IS NULL THEN '' WHEN LEN(HOMETEL.telno)  < 2 Then '' ELSE RIGHT(HOMETEL.telno,LEN(HOMETEL.telno) -2) END + '","' +

	CASE WHEN MOBILETEL.telno IS NULL THEN ''  WHEN LEN(MOBILETEL.telno)  < 3 Then '' else LEFT(MOBILETEL.telno,3) END + '","' +
	CASE WHEN MOBILETEL.telno IS NULL THEN ''  WHEN LEN(MOBILETEL.telno)  < 3 Then '' ELSE RIGHT(MOBILETEL.telno,LEN(MOBILETEL.telno) -3) END + '","' +
	CONVERT(VARCHAR(12),(agreement.agrmtTotal - agreement.servicechg)) + '","' +
	acctType + '"'
FROM 
	acct ELACC_Y
		inner join custacct
			on ELACC_Y.acctno = custacct.acctno and hldorjnt = 'H'
		INNER JOIN customer ELCUS_Y
			on custacct.custid = ELCUS_Y.custid
		inner join custaddress
			on ELCUS_Y.custid = custAddress.custid and addtype='H' and datemoved is null
		inner join instalplan
			on ELACC_Y.acctno = instalplan.acctno
		inner join agreement
			on  ELACC_Y.acctno = agreement.acctno
		left outer join agreementaudit
			on agreement.acctno = agreementaudit.acctno
		left outer JOIN custtel HOMETEL
			ON custacct.custid = HOMETEL.custid  AND HOMETEL.tellocn='H' AND HOMETEL.datediscon IS NULL
		left outer JOIN custtel MOBILETEL
			ON custacct.custid = MOBILETEL.custid  AND MOBILETEL.tellocn='M' AND MOBILETEL.datediscon IS null
WHERE 
	ELACC_Y.currstatus in('0','1','2','3')
	AND ELCUS_Y.custid  <> ''
	and substring(ELACC_Y.acctno,4,1) in('0','1','2','3')
	and agreementaudit.source ='Revise'
	and agreementaudit.datechange > @datestart and  agreementaudit.datechange <= @datefinish
ORDER BY 
	1

go
-- End End End End End End End End End End End End End End End End End End End End End End End End End End End End
