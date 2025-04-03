SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].PeriodEndDatesSP') 
            and OBJECTPROPERTY(id, N'IsProcedure') = 1)
BEGIN
    DROP PROCEDURE dbo.PeriodEndDatesSP
END
GO

CREATE PROCEDURE dbo.PeriodEndDatesSP

-- =============================================
-- Author:		John Croft
-- Create date: 5th May 2009
-- Description:	Period End Dates
--
--	This procedure will insert new period end dates into the RebateForecast_PeriodEndDates table 
--  if the maximum period end date is less than one year > than the financial year end date 
--  for the current year.
--
-- Change Control
-----------------
-- 11/01/10 jec 71852 Cater for December Financial Year End. Mauritius, Madagascar & PNG
-- =============================================
	-- Add the parameters for the stored procedure here
	--@PeriodEndType bit	-- 0=MonthEnd 1=5-4-4 week periods starting at week1 enddate
As

set nocount on

declare @Country VARCHAR(2),@StartYear int
declare @fyeMonth INT,@prevEOYdayofweek int
declare @maxPEdate datetime,@currDate datetime, 
		@fyeDate datetime,@maxPos int,@days int,@loop int,@PeriodEndType BIT
select @country=CountryCode from Country
select @currDate=Convert(datetime,CAST(GETDATE() as CHAR(12)),103)
-- Financial year end date for current year - 31st March or 31st December
if @country in('P','M','C')		-- Png, Mauritius & Madagascar
	select @fyeDate=cast(datepart(year,@currDate) as char(4))+'-12-31 00:00:00.00'
else
	select @fyeDate=cast(datepart(year,@currDate) as char(4))+'-03-31 00:00:00.00'

set @fyeMonth=DATEPART(m,@fyeDate)
select @maxPEdate=max(enddate) from RebateForecast_PeriodEndDates
select @maxPos=max(position) from RebateForecast_PeriodEndDates
select @PeriodEndType=case when value='false' then 0 else 1 end	
		 from CountryMaintenance where codename='PeriodEndType'

-- table to hold year end dates for previous and next two years
set @startYear=datepart(year,@currDate)-1
declare @yearEnd TABLE (yedate datetime,EOYdayofweek INT)
while @startyear< datepart(year,@currDate)+2
BEGIN
	insert into @yearend (yedate,EOYdayofweek)
	values(cast(@startyear as char(4))+'-'+cast(@fyemonth as char(2))+'-31 00:00:00.00',
		datepart(dw,cast(@startyear as char(4))+'-'+cast(@fyemonth as char(2))+'-31 00:00:00.00'))
	set @startyear=@startyear+1
END

----set @PeriodEndType=1			--Testing

set @loop=1
set @days=0

-- if < 12 period enddates from current financial year - add up to 12 dates
if @maxPEDate<dateadd(m,24,@fyeDate)
Begin
-- Calculate calendar Month End Dates
while @maxPEdate<dateadd(m,24,@fyeDate) and @PeriodEndType=0
	Begin
	
	set @maxPos=@maxPos+1
	set @maxPEdate=dateadd(d,-1,dateadd(m,1,dateadd(d,1,@maxPEdate)))

	insert into RebateForecast_PeriodEndDates (position,enddate,rundate)
	values (@maxpos,@maxPEdate,'1900-01-01')
	end

If @fyeMonth=3		-- March year end
Begin
-- Calculate 5-4-4 week Period End Dates
while @maxPEdate<dateadd(m,24,@fyeDate) and @PeriodEndType=1 and @loop<13
	Begin
	set @days=case 
		-- @loop is period no
			when @loop =1 then case		-- First period
					when datepart(dw,@fyeDate)<=4 then 28+(8-datepart(dw,@fyeDate)) -- 31st March no later than a Thursday - short week 1
					else 35+(8-datepart(dw,@fyeDate))	-- 31st March later than a Thursday - long week 1
					end
			-- 5 week periods
			when @loop in(4,7,10) then 35
			-- calculate days to 31st March		
			when @loop = 12 then datediff(d,@maxPEdate,cast(datepart(year,@maxPEdate) as char(4))+'-03-31 00:00:00.00')
			-- 4 week periods
			else 28
			end
	set @maxPos=@maxPos+1
	set @maxPEdate=dateadd(d,@days,@maxPEdate)

	insert into RebateForecast_PeriodEndDates (position,enddate,rundate)
	values (@maxpos,@maxPEdate,'1900-01-01')
	-- next period
	set @loop=@loop+1
	end

end

If @fyeMonth=12		-- December year end
Begin
-- Calculate 5-4-4 week Period End Dates
select @prevEOYdayofweek=EOYdayofweek from @yearend where yedate<=@maxPEdate
while @maxPEdate<dateadd(m,24,@fyeDate) and @PeriodEndType=1 and @loop<13
	Begin
	set @days=case 
		-- @loop is period no
			when @loop =1 then case		-- First period
					when @prevEOYdayofweek<=5 then 28+(8-@prevEOYdayofweek) -- 31st December no later than a Thursday - short week 1
					else 35+(8-@prevEOYdayofweek)	-- 31st December later than a Thursday - long week 1
					end
			-- 5 week periods
			when @loop in(4,7,10) then 35
			-- calculate days to 31st December		
			when @loop = 12 then datediff(d,@maxPEdate,cast(datepart(year,@maxPEdate) as char(4))+'-12-31 00:00:00.00')
			-- 4 week periods
			else 28
			end
	set @maxPos=@maxPos+1
	set @maxPEdate=dateadd(d,@days,@maxPEdate)

	insert into RebateForecast_PeriodEndDates (position,enddate,rundate)
	values (@maxpos,@maxPEdate,'1900-01-01')
	-- next period
	set @loop=@loop+1
	end
	
end

end

-- select dates

--Select top 24 enddate as PeriodEndDate, rundate	as RunDate
--From RebateForecast_PeriodEndDates
--where enddate>dateadd(m,-12,@fyeDate)


go

-- End End End End End End End End End End End End End End End End End End End End End End End End End End End End 



