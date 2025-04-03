-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
SET ARITHABORT ON

;with xmlnamespaces(default 'http://www.bluebridgeltd.com/cosacs/2012/schema.xsd')
update m
set [index] = isnull(l.acctno, b.acctno)
from hub.[message] m
left outer join lineitembookingschedule s 
	on s.bookingid = body.value('/WarehouseDeliver[1]/Id[1]', 'int') 
		or s.bookingid = body.value('/WarehouseDeliver[1]/OrigBookingId[1]', 'int') 
		or s.bookingid = body.value('(/BookingSubmit[1]/Id)[1]', 'int') 
		or s.bookingid =  body.value('(/BookingCancel[1]/Id)[1]', 'int')
		or s.bookingid = body.value('(/WarehouseCancel[1]/Id)[1]', 'int') 
		or s.bookingid = body.value('/WarehouseCancel[1]/OrigBookingId[1]', 'int') 
left outer join lineitem l
	on l.id = s.lineitemid
left outer join warehouse.booking b
	on b.id  = body.value('/WarehouseDeliver[1]/Id[1]', 'int') 
		or b.id = body.value('/WarehouseDeliver[1]/OrigBookingId[1]', 'int') 
		or b.id = body.value('(/BookingSubmit[1]/Id)[1]', 'int') 
		or b.id =  body.value('(/BookingCancel[1]/Id)[1]', 'int')
		or b.id = body.value('(/WarehouseCancel[1]/Id)[1]', 'int') 
		or b.id = body.value('/WarehouseCancel[1]/OrigBookingId[1]', 'int') 