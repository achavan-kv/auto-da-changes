-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here


IF EXISTS(select * from countrymaintenance where codename = 'ServicePartsMarkUp')
BEGIN
	update countrymaintenance 
	set [description] = 'The percentage increase of non courts parts costs when charging the customer'
	where codename = 'ServicePartsMarkUp'
END
GO
