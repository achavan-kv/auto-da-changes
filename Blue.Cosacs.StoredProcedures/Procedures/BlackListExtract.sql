
/****** Object:  StoredProcedure [dbo].[BlackListExtract]    Script Date: 12/14/2007 12:07:46 ******/
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].BlackListExtract') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].BlackListExtract
GO

create  proc BlackListExtract
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : BlackListExtract.prc
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : BlackList Account Extract
-- Author       : ??
-- Date         : ??
--
-- This procedure will retrieve BlackListed accounts for CLMS.
-- 
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 08/02/10 jec CR909 Malaysia v4 merge
-- ================================================
	-- Add the parameters for the stored procedure here
as
-- black list
--select acct.acctno, custid, ((arrears - intcharges) / instalamount)
select '"Blacklisted","' +acct.acctno + '","' + custid + '"'
	from acct
	inner join (Select	acctno,	SUM(transvalue) AS intcharges From fintrans	Where transtypecode in ('INT', 'ADM','DDF') Group by acctno) ACINTO 
		on acct.acctno = ACINTO.acctno
	inner join instalplan
		on acct.acctno = instalplan.acctno
	inner join custacct
			on acct.acctno = custacct.acctno and hldorjnt = 'H'
	where instalamount <> 0
		and ((arrears - intcharges) / instalamount) > 3.5


go
-- End End End End End End End End End End End End End End End End End End End End End End End End End End End End
