  
IF EXISTS (SELECT * FROM sys.objects  WHERE object_id = OBJECT_ID(N'[dbo].[FailedDeliverySP]') AND type IN (N'P', N'PC'))
  DROP PROCEDURE [dbo].[FailedDeliverySP]
GO

create PROCEDURE [dbo].[FailedDeliverySP]
-- =============================================
-- Author:		John Croft	
-- Create date: 14th June 2012
-- Description:	Failed Delivery 
--
--	This report will show details of Failed Deliveries
-- 
-- Change Control
-----------------
-- 20/06/12 jec #10469 % Failed delivery column displays 'Infinity'
-- =============================================
	-- Add the parameters for the stored procedure here
		@FromDate  DATETIME, 
		@ToDate  DATETIME,
		@branch VARCHAR(MAX),	
		@Summary bit
		
	AS
	
	set @todate=DATEADD(mi,-1,(DATEADD(d,1,@ToDate)))		--UAT191 set date to end of day i.e 23:59
	
	declare @failed TABLE (StockLocation VARCHAR(500),RejectionReason VARCHAR(300),NumberRejections int,RejectionQuantity int,RejectionValue money,
						ScheduledQuantity INT ,ScheduledValue money,NumberScheduled int,BrSchdQty int)    
						
	;WITH WHscheduled AS
	(
		select wb.StockBranch,SUM(ISNULL(wb.ScheduleQuantity,0)) as ScheduledQuantity,SUM(wb.UnitPrice * ISNULL(wb.ScheduleQuantity,0)) as ScheduledValue,COUNT(*) as NumberScheduled
		From  Warehouse.Booking wb INNER JOIN Warehouse.[Load] wl on wb.ScheduleId = wl.Id
		Where DATEADD(Hour, DATEDIFF(Hour, GETUTCDATE(), GETDATE()), wl.CreatedOn) between @FromDate and @ToDate	-- convert dates to LocalTime when checking
		Group by StockBranch
	)
					
	insert into @failed
	Select CAST(wb.StockBranch as VARCHAR (5)) + ' ' + MAX(b.branchname) as StockLocation,DeliveryRejectedReason,
				COUNT(*),SUM(wb.ScheduleQuantity-wb.DeliverQuantity), SUM(wb.UnitPrice * (wb.ScheduleQuantity-wb.DeliverQuantity)),max(s.ScheduledQuantity),max(ScheduledValue),max(NumberScheduled),max(s.ScheduledQuantity) 
	From  Warehouse.Booking wb INNER JOIN splitFN(@Branch,',') br on wb.StockBranch=br.items
							INNER JOIN WHscheduled s on wb.StockBranch=s.StockBranch
							INNER JOIN Branch b on b.branchno=wb.StockBranch
							INNER JOIN Warehouse.[Load] wl on wb.ScheduleId = wl.Id
	Where DATEADD(Hour, DATEDIFF(Hour, GETUTCDATE(), GETDATE()), wl.CreatedOn) between @FromDate and @ToDate	-- convert dates to LocalTime when checking
		and DeliveryRejected = 'true'
	Group by wb.StockBranch,DeliveryRejectedReason
	
	-- zeroise all but one row for scheduled values - Report sums these
	;with maxrow as
	(
		select StockLocation,MAX(RejectionReason) as RejectionCode from @failed group by StockLocation
	) 
	
	UPDATE @failed
	set ScheduledQuantity=0,ScheduledValue=0,NumberScheduled=0
	from @failed f INNER JOIN maxrow x on f.StockLocation=x.StockLocation
	where ISNULL(f.RejectionReason,'')!=x.RejectionCode
	 
	-- return data					
	select StockLocation,RejectionReason,NumberRejections,RejectionQuantity,RejectionValue,
							ScheduledQuantity,ScheduledValue,NumberScheduled,BrSchdQty
	 from @failed 
							
	
