--------------------------------------------------------------------------------
--
-- Project      : Securitisation Reports
-- File Name    : dbo.Report14SecuritiseAccounts.PRC
-- File Type    : MSSQL Server Stored Procedure Script
-- Description  : Update summary11 table with accounts from current run for Management Reports
-- Author       : K Fernandez (Strategic Thought)
-- Date         : 01 June 2004
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
--------------------------------------------------------------------------------

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[Report14SecuritiseAccounts]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[Report14SecuritiseAccounts]
GO

CREATE PROCEDURE Report14SecuritiseAccounts

@runno int,
@return int out

AS
BEGIN
    SET @return = 0
    
    UPDATE acct  
    SET    outstbal = convert(money,f.finbal) 
    FROM   finbal f
    WHERE  acct.acctno = f.acctno
    and	   outstbal !=finbal

/* Truncate summay11 */
    truncate table summary11

/* Populate summary11 table */
    insert into summary11
	(acctno, a.branchno, branchname, outstbal, agrmttotal, runno, asatdate)
    select
	s.acctno, a.branchno, branchname, outstbal, agrmttotal, s.runno, getdate()
    from
	sec_account s, acct a, branch b
    where
	s.acctno = a.acctno
    and	s.runno = @runno
    and a.branchno = b.branchno


    if exists (select * from sysindexes where name = 'ix_summary11_1')
	DROP INDEX [dbo].[summary11].[ix_summary11_1]

    CREATE CLUSTERED INDEX [ix_summary11_1] ON [dbo].[summary11] ([acctno], [branchno]);

    SET @return = @@ERROR
    RETURN @return
END
GO
