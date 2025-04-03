SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

if exists (select * from sysobjects where name ='CM_GetTraceDetails')
drop procedure CM_GetTraceDetails
go
create procedure CM_GetTraceDetails 
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : CM_GetTraceDetails.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Get Trace Details
-- Author       : John croft
-- Date         : 2nd June 2009
--
-- This procedure will retreive Trace details.
-- 
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 08/09/09 jec UAT836 only get trace details for outstanding trace
-- 07/07/10 jec UAT1040                        
-- =================================================================================
	-- Add the parameters for the stored procedure here
			@acctno	char(12),
			@return int OUTPUT

AS

select DISTINCT b.acctno,b.empeeno,b.datedue as traceinitiatedDate,
	case when b2.dateadded is null OR b2.dateadded<b.dateadded then 0 else 1 end as isResolved,b.notes as usernotes
from bailaction b left outer join bailaction b2 on b.acctno=b2.acctno and b2.code='TRR'
			LEFT OUTER JOIN custacct ca ON b2.acctno=ca.acctno AND ca.hldorjnt='H'	
			LEFT OUTER JOIN custcatcode cc ON cc.custid=ca.custid AND cc.code='T'
where b.code='TRC' and b.acctno=@acctno 
	AND datedeleted IS NULL
	AND b.dateadded>isnull(cc.datecoded,'1900-01-01')	-- UAT1040
	AND b.actionno=(SELECT MAX(actionno) FROM bailaction b3 WHERE b3.acctno=b.acctno AND b3.code='TRC')
	
IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END

Go

-- End End End End End End End End End End End End End End End End End End End End End End End End End End