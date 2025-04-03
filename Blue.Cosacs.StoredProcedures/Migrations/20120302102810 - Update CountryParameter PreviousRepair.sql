-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF EXISTS(select * from CountryMaintenance where CodeName = 'PreviousRepair')
BEGIN
	update CountryMaintenance
	set Name = 'Previous Repair %',
		[Description] = 'This is the cost of previous repairs. If the total of previous repairs for an item exceeds this % of the Cost Price, a pop-up message will appear when the allocate button is selected in the Soft Script screen. This value is also used for the Previous Repair Total Exceeded filter in the Service Management Review screen'
	where CodeName = 'PreviousRepair'
END
