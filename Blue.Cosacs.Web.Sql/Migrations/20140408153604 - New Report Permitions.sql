-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
INSERT INTO Admin.Permission
	(Id, Name, CategoryId, Description)
VALUES
    (2009, 'Report - Extended Warranty Percentages', 20, 'Extended Warranty Percentages Report'),
    (2010, 'Report - Service Request Resolution', 20, 'Service Request Resolution Report'),
	(2011, 'Report - Technician Cancellations', 20, 'Technician Cancellations Report'),
	(2012, 'Report - Customer Feedback/Happy Call', 20, 'Customer Feedback/Happy Call Report'),
	(2013, 'Report - Spare Parts', 20, 'Spare Parts Report')