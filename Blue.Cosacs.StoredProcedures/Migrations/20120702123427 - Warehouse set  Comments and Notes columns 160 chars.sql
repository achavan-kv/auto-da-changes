-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

UPDATE Warehouse.Booking set PickingComment = LEFT(PickingComment,160) where LEN(PickingComment)>160
UPDATE Warehouse.Booking set ScheduleComment = LEFT(ScheduleComment,160) where LEN(ScheduleComment)>160
UPDATE Warehouse.Booking set DeliveryRejectionNotes = LEFT(DeliveryRejectionNotes,160) where LEN(DeliveryRejectionNotes)>160
UPDATE Warehouse.Cancellation set Reason = LEFT(Reason,160) where LEN(Reason)>160
UPDATE Warehouse.Picking set Comment = LEFT(Comment,160) where LEN(Comment)>160

ALTER TABLE Warehouse.Booking alter column PickingComment  VARCHAR(160) 
ALTER TABLE Warehouse.Booking alter column ScheduleComment  VARCHAR(160)
ALTER TABLE Warehouse.Booking alter column DeliveryRejectionNotes  VARCHAR(160) 
ALTER TABLE Warehouse.Cancellation alter column Reason  VARCHAR(160)  
ALTER TABLE Warehouse.Picking alter column Comment  VARCHAR(160)    

