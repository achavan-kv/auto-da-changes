-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here



IF EXISTS(SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[LineItemBooking]') AND type in (N'U'))
BEGIN

	DROP TABLE LineItemBooking
END

	
	CREATE TABLE LineItemBooking
	(
		ID INT PRIMARY KEY,
		LineItemID INT NOT NULL DEFAULT 0,
		Quantity FLOAT NOT NULL DEFAULT 0
	)
	

