SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO

if exists (select * from sysobjects where name ='DelNotificationAuditGetSP')
drop procedure DelNotificationAuditGetSP
go

create procedure DelNotificationAuditGetSP
 @acctno varchar(12),
 @rowcount int,
 @return int OUT
 
-- **********************************************************************
-- Title: DelNotificationAuditGetSP.sql
-- Developer: Ilyas Parker
-- Date: 18/09/2009
-- Purpose: 

-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------

-- **********************************************************************
	-- Add the parameters for the stored procedure here
as

	set @return = 0    --initialise return code
 
	set	rowcount @rowcount
	
	SELECT 
		SI.IUPC as ItemNo,
		s.BuffNo,
		s.DateRemoved,
		s.RemovedBy,
		CASE 
			WHEN s.TYPE = 'R' THEN 'DN Removed'
			WHEN s.type = 'I' then 'Item Removed' 
			WHEN s.type = 'D' then 'DN Deleted'  
		END AS Action		
	FROM ScheduleRemoval s
	INNER JOIN dbo.StockInfo SI ON s.ItemID = SI.ID
	WHERE s.AcctNo = @acctno
	
	set	rowcount 0
    
    
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

