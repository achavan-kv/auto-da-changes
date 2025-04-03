SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS OFF
GO

IF EXISTS (SELECT * FROM SysObjects
           WHERE id = object_id('[dbo].OMig_AccountBalances') and OBJECTPROPERTY(id, 'IsProcedure') = 1)

BEGIN
    DROP PROCEDURE OMig_AccountBalances
END
GO

CREATE PROCEDURE dbo.OMig_AccountBalances

-- ================================================
-- Project      : CoSACS .NET
-- File Name    : OMig_AccountBalances.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Migrate Account Balances Details
-- Author       : John Croft
-- Date         : 16 July 2008
--
-- This procedure will create csv file for the Account Balances Interface into Oracle
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 12/08/08 jec Trim Custid and convert to uppercase
-- 18/08/08 aa  balances based on run number to better match report 11 (really should identically match report 11)
-- ================================================
	-- Add the parameters for the stored procedure here
	@date datetime
as

--set @date =DATEADD(d,1,@date) -- Add 1 day to ensure all transaction for date entered are included

IF EXISTS (SELECT table_name FROM INFORMATION_SCHEMA.TABLES
	   WHERE  table_schema = 'dbo' and table_name = 'AccountBalances')
	drop table AccountBalances

DECLARE @fintransrunno INT 

SELECT @fintransrunno=MAX(runno) FROM interfacecontrol WHERE datestart < DATEADD (hour,29,@date)
AND interface = 'updsmry' 


--SELECT * FROM interfacecontrol WHERE interface='updsmry' AND datestart BETWEEN '2-aug-2008' AND '5-aug-2008'
--select @fintransrunno
select RTRIM(LTRIM(UPPER(ca.custid))) as custid, a.acctno, a.branchno as Branch, ROUND(SUM(f.transvalue),2) as Balance 
Into AccountBalances
From acct a INNER JOIN custacct ca on a.acctno = ca.acctno and hldorjnt='H'
			INNER JOIN fintrans f on a.acctno = f.acctno
WHere (datetrans <dateadd(hour,23,@date) AND transtypecode IN ('int','adm') )
OR (runno <=@fintransrunno AND runno !=0 --and  datetrans <dateadd(hour,23,@date) 
AND transtypecode NOT IN ('int','adm') )
Group by  a.branchno,  ca.custid, a.acctno
Having SUM(f.transvalue)!=0  -- non_zero balance	--> 0 debit balance

-- export csv file
declare @path varchar(200)
SELECT SUM(Balance) FROM  AccountBalances
set @path = '"c:\program files\microsoft sql server\80\tools\binn\BCP" ' + db_name()+'..AccountBalances' + ' out ' +
'd:\users\default\AccountBalances.csv ' + '-c -t, -q -T'

exec master.dbo.xp_cmdshell @path

-- End End End End End End End End End End End End End End End End End End End End End End End End 
GO