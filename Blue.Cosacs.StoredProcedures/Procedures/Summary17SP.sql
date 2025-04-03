IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Summary17SP]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[Summary17SP]
GO

SET ANSI_NULLS OFF
GO

SET QUOTED_IDENTIFIER OFF
GO



CREATE PROCEDURE [dbo].[Summary17SP]
--========================================================================================
			@return int
as 

set @return=0

TRUNCATE TABLE Summary17

declare @startdate datetime, @enddate datetime, @x int, @sql sqltext

--Set the start and end date for the last whole month from today
set @startDate = Convert(datetime,cast(dateadd(m, -12, dateadd(d, -(datepart(d, getdate())-1), getdate()))as char(12)),103)
set @endDate = dateadd(mi, -1, dateadd(m,12,@startDate))

--Update average for previous 12 months
set @x=12
	while @x >0
		begin		
				   Insert into summary17
				   select distinct @x as Monthid, cast(b.branchno as varchar(3)) + ' ' + b.branchname as branch,
				   a.accttype, 
				   s.category,
				   sum(d.quantity) as Qty,
				   sum(d.transvalue - l.taxamt) as SalesValue
				   --into #MthTransaction
			from   acct a
				   inner join lineitem l on
				   a.acctno = l.acctno	
				   inner join delivery d
				   on l.acctno = d.acctno
			and	   l.agrmtno = d.agrmtno
			--and	   l.itemno = d.itemno
			and	   l.ItemID = d.ItemID																		--IP - 09/08/11 - RI  
			and	   l.stocklocn = d.stocklocn
				   --inner join stockitem s on l.itemno=s.itemno	and l.stocklocn = s.stocklocn
				   inner join stockitem s on l.ItemID=s.ID	and l.stocklocn = s.stocklocn					--IP - 09/08/11 - RI
				   inner join branch b on
				   substring(l.acctno, 1,3) = b.branchno
				   
			where  d.datedel between @startDate and @endDate
			and	   d.delorcoll in('D','C')
			and	   l.ordval > 0
			and	   (l.itemtype = 'S' or l.itemno = 'LOAN')
			group by a.accttype, cast(b.branchno as varchar(3)) + ' ' + b.branchname,
				   s.category
			
		
			
			--Set the startdate to next month
			set @startDate = dateadd(m, 1, @startDate)
			set @endDate = dateadd(mi, -1, dateadd(m,1,@startDate))	
		
			set @x=@x-1

			
		
		end
		



GO

