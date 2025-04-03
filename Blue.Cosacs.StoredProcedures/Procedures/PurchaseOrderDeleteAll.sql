IF EXISTS (SELECT * FROM SYSOBJECTS WHERE NAME ='PurchaseOrderDeleteAll')
DROP PROCEDURE PurchaseOrderDeleteAll
GO
CREATE PROCEDURE PurchaseOrderDeleteAll
AS 

DELETE FROM PurchaseOrderOutstanding