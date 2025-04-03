-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
IF (SELECT COUNT(1) FROM country c WHERE c.countrycode = 'T') > 0
BEGIN 

	UPDATE Service.Request
		SET ManWarrantyLength = 13,
		WarrantyLength = 12
	WHERE
		ID IN (596542, 610689, 605514, 628932, 669714)
END