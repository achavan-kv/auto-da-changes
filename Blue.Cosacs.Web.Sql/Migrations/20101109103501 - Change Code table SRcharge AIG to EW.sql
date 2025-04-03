-- Update code AIG to EW

UPDATE code set code ='EW',codedescript='EW'
Where category='SRCHARGE' and code='AIG'
-- update All Sr's where Primary charge is AIG to EW
UPDATE dbo.SR_Resolution set Chargeto='EW' where Chargeto='AIG'
