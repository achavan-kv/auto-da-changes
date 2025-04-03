-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

--Fix for issue: #29800 - Incorrect Warranty Sale records

UPDATE
    LineItemBookingSchedule
SET
    Quantity = 0
 from 
    LineItemBookingSchedule ls
inner join 
    lineitem l on ls.LineItemID = l.ID
where 
    ls.Quantity < 0
    and ls.DelOrColl = 'C'
    and not exists (select 1 from                              
                        hub.QueueMessage qm
                    inner join 
                        hub.Message m on qm.MessageId= m.Id
                    where
                        m.CorrelationId = l.acctno
                    and qm.State in ('P', 'X'))
    and not exists (select 1 from                           
                        LineItemBookingFailures lf
                    where (lf.BookingID = ls.BookingId
                               or lf.OriginalBookingID = ls.BookingId)
                               and lf.Actioned is null)
    and exists(select 1 from Warehouse.Booking wb
                        where wb.Id = ls.BookingId
                        and (wb.PickingRejected is not null
                                or wb.ScheduleRejected is not null
                                or wb.DeliveryRejected is not null))