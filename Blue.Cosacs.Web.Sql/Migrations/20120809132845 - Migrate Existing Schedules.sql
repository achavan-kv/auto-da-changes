-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

/*
	script to migrate outstanding schedules to warehouse.bookings 
	
		create warehouse.booking row
		create lineitemBooking row
		create lineitemBookinSchedule row
*/

IF NOT EXISTS (SELECT * FROM syscolumns
			   WHERE name = 'ItemID'
               AND OBJECT_NAME(id) = 'LineItemBookingSchedule')
BEGIN

  ALTER TABLE LineItemBookingSchedule ADD ItemID int 
  ALTER TABLE LineItemBookingSchedule ADD StockLocn int 
  ALTER TABLE LineItemBookingSchedule ADD Price money 

END


--IF NOT EXISTS(SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ScheduleToBookings]') AND type in (N'U'))
--BEGIN
--	DROP TABLE ScheduleToBookings
--END

-- select existing schedules for accounts that are not settled
Select s.*,l.price,l.Id as LineitemId,l.datereqdel					-- #13834
into #newBookings 
from schedule s INNER JOIN acct a on s.acctno = a.acctno
				INNER JOIN custacct ca on s.acctno = ca.acctno and hldorjnt='H'
				INNER JOIN lineitem l on s.acctno=l.acctno and s.agrmtno=l.agrmtno and s.itemid=l.itemid and s.stocklocn=l.stocklocn and s.parentItemId=l.parentItemId
				INNER JOIN stockinfo si on s.itemid = si.id
				LEFT OUTER JOIN code c on CAST(c.code as INT)=si.category and c.category in ('pce','pcf','pcw') 				
		where a.accttype!='S' and currstatus!='S' and l.itemtype='S'
				and ca.custid not like '%Paid%' 
				and ((l.datereqdel>dateadd(yy,-1,getdate()) and codedescript !='Free Gifts')
					or (l.datereqdel>dateadd(mm,-6,getdate()) and codedescript ='Free Gifts'))
and ((l.quantity>0 and s.delorcoll = 'D')	or (s.delorcoll = 'C'))	-- existing order
AND NOT EXISTS(SELECT * FROM order_removed o WHERE o.acctno = s.acctno AND s.agrmtno=o.agrmtno and s.itemid=o.itemid and s.stocklocn=o.stocklocn and s.buffno = o.buffno) -- exclude removed schedules (data issue)

				--and s.dateprinted is null			-- ?????
		
alter TABLE #newBookings add  ID INT IDENTITY

go

IF NOT EXISTS(SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ScheduleToBookings]') AND type in (N'U'))
BEGIN

	declare @schedcount INT, @bookingIdStart INT, @maxlo INT,@bookingIdEnd INT

	select @schedcount=COUNT(*) from #newBookings s --INNER JOIN acct a on s.acctno = a.acctno
					--where currstatus!='S'
	select @schedcount

	select @bookingIdStart = NextHi from dbo.HiLo where Sequence='Warehouse.Booking'
	select @maxlo = maxlo from dbo.HiLo where Sequence='Warehouse.Booking'

	-- set last Booking ID = to multiple of HiLo increment e.g. if no of schedules 248 and increment = 100 then Last booking id = StartId + 300
	select @bookingIdEnd=@bookingIdStart+ceiling(@schedcount/100.00)* @maxlo

	---- create warehouse.booking 
	insert into Warehouse.Booking (Id,CustomerName,AddressLine1,AddressLine2,AddressLine3,PostCode,StockBranch,DeliveryBranch,DeliveryOrCollection,DeliveryOrCollectionDate,
		ItemNo,ItemId,ItemUPC,ProductDescription,ProductBrand,ProductModel,ProductArea,ProductCategory,Quantity,RepoItemId,Comment,DeliveryZone,ContactInfo,
		OrderedOn,Damaged,AssemblyReq,AcctNo,
		--OriginalId,TruckId,PickingId,PickingAssignedBy,PickQuantity,PickingComment,PickingRejectedReason,PickingRejected,
		--ScheduleId,ScheduleComment,ScheduleSequence,PickingAssignedDate,
		UnitPrice,[Path],
		--ScheduleRejected,ScheduleRejectedReason,
		--DeliveryRejected,DeliveryRejectedReason,DeliveryConfirmedBy,DeliveryRejectionNotes,ScheduleQuantity,DeliverQuantity,Exception,
		Express,AddressNotes,BookedBy,Fascia,PickUp
	) 

	select @bookingIdStart+nb.Id,cu.title + ' ' + cu.firstname + ' ' + cu.name,	
			ISNULL(cadd.cusaddr1,'Check Address'),ISNULL(cadd.cusaddr2,'Check Address'),ISNULL(cadd.cusaddr3,'Check Address'),ISNULL(cadd.cuspocode,'Check Address'),		
			nb.StockLocn,case when ISNULL(nb.RetStockLocn,0)=0 then l.DelNoteBranch else nb.RetStockLocn end,nb.DelOrColl,
			--case when nb.dateDelPlan < DATEADD(dd, DATEDIFF(dd,0,GETDATE()), 0) then DATEADD(dd, DATEDIFF(dd,0,GETDATE()), 0) else nb.dateDelPlan end,
			--case when nb.dateDelPlan = '01-01-1900' then DATEADD(dd, DATEDIFF(dd,0,GETDATE()), 0) else nb.dateDelPlan end,	--#11252
			case when nb.dateReqdel = '01-01-1900' then DATEADD(dd, DATEDIFF(dd,0,GETDATE()), 0) else DATEADD(Hour, DATEDIFF(Hour, GETUTCDATE(), GETDATE()), nb.dateReqdel) end,	--#13834		
			si.ItemNo,si.Id,si.IUPC,si.itemdescr1 + ' ' + si.itemdescr2, isnull(si.Brand,''),isnull(si.VendorLongStyle,''),'',CAST(si.category as VARCHAR(3)),
			ABS(nb.Quantity),ISNULL(sir.id,0),l.notes,l.deliveryarea,
			'Home ' + Ltrim(ISNULL(H.DialCode,'')) + ' ' + LTRIM(ISNULL(H.telno,'')) + ' ' + LTRIM(ISNULL(H.extnno,'')) + ' ' +
			'Work ' + Ltrim(ISNULL(W.DialCode,'')) + ' ' + LTRIM(ISNULL(W.telno,'')) + ' ' + LTRIM(ISNULL(W.extnno,'')) + ' ' +
			'Mobile ' + Ltrim(ISNULL(M.DialCode,'')) + ' ' + LTRIM(ISNULL(M.telno,'')) + ' ' + LTRIM(ISNULL(M.extnno,'')),
			--a.DateAcctOpen,
			DATEADD(Hour, DATEDIFF(Hour, GETUTCDATE(), GETDATE()), a.DateAcctOpen) ,	--#13834
			case when l.Damaged='Y' then 1 else 0 end,case when l.assemblyrequired='Y' then 1 else 0 end,
			nb.AcctNo, case when nb.DelOrColl!='R' then l.Price else ISNULL(spr.CashPrice,0) end,CAST(@bookingIdStart+nb.Id as VARCHAR(6))+'.',
			case when ISNULL(l.Express,'N')='Y' then 1 else 0 end,
			cadd.Notes,ag.createdBy,b.StoreType,case when l.DeliveryProcess='S' then 0 else 1 end


	from #newBookings nb INNER JOIN LineItem l on nb.LineItemId=l.Id
					INNER JOIN Branch b on ISNULL(l.SalesBrnNo,Left(l.acctno,3))=b.BranchNo	
					INNER JOIN Custacct ca on l.Acctno=ca.AcctNo and ca.HldOrJnt='H' 
					INNER JOIN Acct a on ca.AcctNo= a.acctno 
					INNER JOIN Agreement ag on nb.acctno=ag.acctno and nb.agrmtno=ag.agrmtno
					INNER JOIN Customer cu on ca.custId=cu.CustId
					LEFT OUTER JOIN CustAddress cadd on cu.CustId=cadd.CustId and cadd.AddType=l.DeliveryAddress
					--LEFT OUTER JOIN CustAddress caddH on cu.CustId=caddH.CustId and caddH.AddType='H'	and caddH.DateMoved is NULL
					INNER JOIN StockInfo si on nb.ItemId=si.Id
					LEFT OUTER JOIN StockInfo sir on si.IUPC=sir.IUPC and sir.RepossessedItem=1
					LEFT OUTER JOIN StockPrice spr on sir.Id=spr.Id and spr.BranchNo=nb.StockLocn
					LEFT OUTER JOIN Custtel H on cu.CustId=H.CustId and H.TelLocn='H' and H.DateDiscon is null 
					LEFT OUTER JOIN Custtel W on cu.CustId=W.CustId and W.TelLocn='W' and W.DateDiscon is null
					LEFT OUTER JOIN Custtel M on cu.CustId=M.CustId and M.TelLocn='M' and M.DateDiscon is null
				
	Where  cadd.DateMoved is NULL
		and si.RepossessedItem=0
	

	-- create lineitemBooking#

	insert into LineItemBooking (ID,LineItemId,Quantity)
	select @bookingIdStart+Id,LineItemId,ABS(Quantity)
	from #newBookings

	-- create lineitemBookinSchedule

	insert into dbo.LineItemBookingSchedule (LineItemID,DelOrColl,RetItemID,RetVal,RetStockLocn,BookingId,Quantity,ItemId,StockLocn,Price)		-- #13467 
	select LineItemId, DelOrColl, RetItemId, ISNULL(RetVal,0), ISNULL(RetStockLocn,0), @bookingIdStart+Id, Quantity,ItemId,StockLocn,Price
	from #newBookings

	-- save Schedule Items converted to Bookings

	--Select s.*,l.Id as LineitemId
	--into ScheduleToBookings 
	--from schedule s INNER JOIN acct a on s.acctno = a.acctno
	--				INNER JOIN lineitem l on s.acctno=l.acctno and s.agrmtno=l.agrmtno and s.itemid=l.itemid and s.stocklocn=l.stocklocn and s.parentItemId=l.parentItemId				
	--		where currstatus!='S' and l.itemtype='S'

	Select s.*,l.price,l.Id as LineitemId						-- #13467
	into ScheduleToBookings
	from schedule s INNER JOIN acct a on s.acctno = a.acctno
					INNER JOIN custacct ca on s.acctno = ca.acctno and hldorjnt='H'
					INNER JOIN lineitem l on s.acctno=l.acctno and s.agrmtno=l.agrmtno and s.itemid=l.itemid and s.stocklocn=l.stocklocn and s.parentItemId=l.parentItemId
					INNER JOIN stockinfo si on s.itemid = si.id
					LEFT OUTER JOIN code c on CAST(c.code as INT)=si.category and c.category in ('pce','pcf','pcw') 				
			where a.accttype!='S' and currstatus!='S' and l.itemtype='S' 
					and ca.custid not like '%Paid%' 
					and ((l.datereqdel>dateadd(yy,-1,getdate()) and codedescript !='Free Gifts')
						or (l.datereqdel>dateadd(mm,-6,getdate()) and codedescript ='Free Gifts'))
	and ((l.quantity>0 and s.delorcoll = 'D')	or (s.delorcoll = 'C'))	-- existing order
	AND NOT EXISTS(SELECT * FROM order_removed o WHERE o.acctno = s.acctno AND s.agrmtno=o.agrmtno and s.itemid=o.itemid and s.stocklocn=o.stocklocn and s.buffno = o.buffno) -- exclude removed schedules (data issue)

		
	-- Delete items from Schedule
	Delete S
	From Schedule s, ScheduleToBookings b
	where s.acctno=b.acctno and s.agrmtno=b.agrmtno and s.itemid=b.itemid and s.stocklocn=b.stocklocn and s.parentItemId=b.parentItemId	
	 
	-- Update HiLo

	UPDATE HiLo
		set NextHi = @bookingIdEnd
	Where Sequence='Warehouse.Booking' 	

END
go

