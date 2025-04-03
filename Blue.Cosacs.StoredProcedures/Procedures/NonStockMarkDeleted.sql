IF EXISTS (SELECT * FROM sysobjects WHERE NAME ='NonStockMarkDeleted')
DROP PROCEDURE NonStockMarkDeleted
GO 
CREATE PROCEDURE NonStockMarkDeleted
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : NonStockMarkDeleted.prc
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : NonStock Mark Deleted
-- Author       : Alex Ayscough
-- Date         : 15 February 2011
--
-- This procedure will mark Non-stock items as deleted when the deletion date is reached.
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 24/02/11  jec Update the deleted flag
-- ================================================
	-- Add the parameters for the stored procedure here 
AS 
DECLARE @currentdate DATETIME

SET @currentdate = GETDATE()
-- if run after midnight only remove those from the previous day
IF DATEPART(hour,GETDATE()) < 10
 SET @currentdate = dateadd(hour, -10, GETDATE())

UPDATE stockinfo SET prodstatus = 'D' WHERE
EXISTS ( SELECT * FROM NonStockDeletionDates d
WHERE d.itemno= stockinfo.itemno AND @currentdate  > d.DeletionDate)

UPDATE stockquantity SET deleted = 'Y' WHERE
EXISTS ( SELECT * FROM NonStockDeletionDates d
WHERE d.itemno= stockquantity.itemno AND @currentdate  > d.DeletionDate)

GO 

-- end end end end end end end end end end end end end end end end end end end end end end end 