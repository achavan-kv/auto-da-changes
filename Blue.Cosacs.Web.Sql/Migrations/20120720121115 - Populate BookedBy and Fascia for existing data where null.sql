-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

UPDATE warehouse.booking set BookedBy = 99999 where BookedBy is null
UPDATE warehouse.booking set Fascia = 'C' where Fascia is null