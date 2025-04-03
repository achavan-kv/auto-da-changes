SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[BailactionMaxactionGetRecent]') 
		and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[BailactionMaxactionGetRecent]
GO
CREATE PROCEDURE 	[dbo].[BailactionMaxactionGetRecent]
--------------------------------------------------------------------------------
--
-- Project      : CoSACS .NET
-- File Name    : BailactionMaxactionGetRecent.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Get recent bailaction.
-- Author       : John Croft
-- Date         : 15th September 2010
--
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------

--------------------------------------------------------------------------------
    -- Parameters
		@acctno	CHAR(12),
		@return INT output

as   
	set @return=0
	DECLARE @DaysSinceAction int
	SELECT @DaysSinceAction = VALUE FROM countrymaintenance WHERE codename = 'NoOfDaysSinceAction'	
	
	select acctno,dateadded,RecentCode
	from bailactionmaxaction 
	Where acctno=@acctno
		and dateadded > DATEADD(d,-@DaysSinceAction,GETDATE())
	
	set @return = @@ERROR
    
    
 -- End End End End End End End End End End End End End End End End End End End End End End End
 
 Go