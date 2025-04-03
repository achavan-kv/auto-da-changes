IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.tables	WHERE table_name ='CashierTotalledView')
DROP VIEW CashierTotalledView 
GO 
CREATE VIEW CashierTotalledView AS 

SELECT 
t.datefrom ,t.dateto,t.empeeno, 

b.paymethod, b.systemtotal AS NetReceipts, b.usertotal , 
b.deposit, b.[difference], b.reason AS Comments, 
b.Payments , 
b.Corrections ,
b.Refunds ,
b.PettyCash ,
b.Remittances ,
b.Allocations  ,
b.Disbursements ,
C.codedescript 
	
 FROM CashierTotalsBreakdown b 
 	
JOIN CashierTotals t ON  t.id = b.cashiertotalid 
JOIN code c ON c.code = CONVERT(VARCHAR,b.paymethod)
WHERE  c.category = 'FPM'
GO 
 