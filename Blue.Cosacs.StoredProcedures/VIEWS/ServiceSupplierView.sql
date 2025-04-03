IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID('ServiceSupplierView'))
DROP VIEW  ServiceSupplierView
Go

CREATE VIEW ServiceSupplierView
AS
	SELECT 
		Id, 
		Supplier 
	FROM 
		service.ServiceSupplier