if exists (select * from sysobjects where name = 'CM_WorklistsAcctLoadbyAcctno')
drop  procedure CM_WorklistsAcctLoadbyAcctno 
go
create procedure CM_WorklistsAcctLoadbyAcctno
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : CM_WorklistsAcctLoadbyAcctno.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Get Worklist details
-- Author       : Jez Hemans
-- Date         : 29 May 2007
--
-- Description:		Returns the worklist account details
--
-- Change Control
-- --------------
-- Date      By  Description-- 
-- 24/09/10 jec UAT1005 return better column names & return blank dateto if null
-- 06/01/12 jec #3588 LW73586 - Different date formats
-- ================================================
	-- Add the parameters for the stored procedure here 
@acctno char(12) ,
@return int OUTPUT
AS
set @return = 0

select
acctno as 'AccountNumber',
s.strategy + ' ' +S.[description] as Strategy,
Worklist,
datefrom,
dateto	
--isnull(convert(varchar,dateto,103),' ') as dateto		--UAT1005
from CMWorklistsAcct w INNER JOIN CMStrategy s on w.Strategy=s.strategy
where acctno = @acctno

go

-- End End End End End End End End End End End End End End End End End End End End End End End End


