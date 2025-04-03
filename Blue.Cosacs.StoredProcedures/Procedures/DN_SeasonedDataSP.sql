SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO

IF EXISTS (SELECT * FROM SysObjects
           WHERE Name = 'DN_SeasonedDataSP' AND Type = 'P')
BEGIN
    DROP PROCEDURE DN_SeasonedDataSP
END
GO

CREATE PROCEDURE DN_SeasonedDataSP

--------------------------------------------------------------------------------
--
-- Project      : CoSACS .NET
-- File Name    : DN_SeasonedDataSP.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Seasoned / UnSeasoned Report Data
-- Author       : Rupal Desai
-- Date         : 02 May 2006
--
-- Change Control
-- --------------
-- Date      	By  Description
-- ----      	--  -----------
--



--declare 
@return int OUTPUT

AS

BEGIN
    SET @Return = 0


TRUNCATE TABLE summary10


-- Insert all the transactions for all securitised accounts

--print 'rupal testing'
insert into summary10
		(acctno, datesecuritised, datetrans, transtypecode,
		transvalue, asatdate)
select	f.acctno, datesecuritised, datetrans, transtypecode,
		transvalue, getdate()
from	sec_account s, fintrans f
where	s.acctno = f.acctno



--Create Indexes
if exists (select * from sysindexes where name = 'ix_summary10')
DROP INDEX [summary10].[ix_summary10]

CREATE INDEX [ix_summary10] ON [summary10] ([datesecuritised]);


if exists (select * from sysindexes where name = 'ix_summary10_2')
DROP INDEX [summary10].[ix_summary10_2]

CREATE INDEX [ix_summary10_2] ON [summary10] ([datesecuritised],[transvalue]);


if exists (select * from sysindexes where name = 'ix_summary10_3')
DROP INDEX [summary10].[ix_summary10_3]


CREATE INDEX [ix_summary10_3] ON [summary10] ([acctno],[datesecuritised]);


if exists (select * from sysindexes where name = 'ix_summary10_4')
DROP INDEX [summary10].[ix_summary10_4]

CREATE INDEX [ix_summary10_4] ON [summary10] ([acctno],[datesecuritised],[transvalue]);


	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
END
GO

GRANT EXECUTE ON DN_SeasonedDataSP TO PUBLIC
GO
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO
