SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_RebateTotalGetSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_RebateTotalGetSP]
GO
create procedure DN_RebateTotalGetSP 
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : DN_RebateTotalGetSP.prc
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Get Rebate Totals  
-- Author       : ??
-- Date         : ??
--
-- This procedure will get the Rebate Totals.
-- 
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 14/05/09 jec  Retrieve latest Rebate totals (Rebates_total table now holds historical totals)
-- ================================================
	-- Add the parameters for the stored procedure here
@branchno smallint,
@Return INTEGER OUTPUT
as
set @return = 0
set nocount on

declare @PEDate char(24)
select @PEDate=MAX(PeriodEndDate) from rebates_total		-- jec 14/05/09
-- query to retrieve data for 
declare @statement sqltext
SET @statement = 'SELECT branchno, arrearsgroup, rebate, rebatewithin12mths,'
			+ ' rebateafter12mths from rebates_total where 1=1 '
			+ ' and convert(char(24),PeriodEndDate)=' + '''' + @PEDate + '''' + ''	-- jec 14/05/09

IF @branchno >= 0
BEGIN
   SET @statement = @statement + 'and branchno = ' + CONVERT(VARCHAR,@branchno)
END

SET @statement = @statement + ' order by branchno, sequence'

execute sp_executesql @statement

set @return = @@error

if @return = 0
BEGIN
	select sequence,arrearsgroup, sum(rebate) as rebate, 
		sum(rebatewithin12mths) as rebatewithin12mths,
		sum(rebateafter12mths) as rebateafter12mths
	from rebates_total
	where @PEDate=convert(char(24),PeriodEndDate)		-- jec 14/05/09 
	group by sequence,arrearsgroup	
	order by sequence
END

/* code for testing only
declare @return integer
exec DN_RebateTotalGetSP
@branchno =720,
@return = 0 */

go


-- End End End End End End End End End End End End End End End End End End End End End End End End End End

