-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
INSERT INTO Admin.Permission (CategoryId, Id, Name, [Description])
	VALUES (21, 2132, 'RetailPriceView', 'Allows user view the retail price data'),
		   (21, 2133, 'RetailPriceEdit', 'Allows user create and edit retail prices')