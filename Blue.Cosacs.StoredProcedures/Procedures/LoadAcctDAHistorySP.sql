SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO

if exists (select * from sysobjects where name ='LoadAcctDAHistorySP')
drop procedure LoadAcctDAHistorySP
go

create procedure LoadAcctDAHistorySP
 @acctno VARCHAR(12),
 @return int out
-- **********************************************************************
-- Title: LoadAcctDAHistorySP.sql
-- Developer: Ilyas Parker
-- Date: 04/02/2010
-- Purpose: Procedure returns the Delivery Authorisation history for an account
--			and displays the details in Account Details.

-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------

-- **********************************************************************
	-- Add the parameters for the stored procedure here
as

	set @return = 0    --initialise return code
 
    SELECT u.FullName AS 'Employee Name', 
		   d.empeeno AS 'Employee Number', 
		   d.dateauthorised AS 'Date Authorised', 
		   d.source AS 'Source'
    FROM delauthoriseaudit d
    LEFT JOIN Admin.[User] u ON d.empeeno = u.id
    WHERE d.acctno = @acctno
	
    
 IF (@@error != 0)
    BEGIN
        SET @return = @@error
    END
GO
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO

-- End End End End End End End End End End End End End End End End End End End End End End End End End End End End End 

