IF EXISTS ( SELECT * 
			FROM sysobjects
			WHERE xtype = 'V'
			AND name = 'CashierTotalWriteOffView')
BEGIN
	DROP VIEW CashierTotalWriteOffView
END
GO

CREATE VIEW CashierTotalWriteOffView
AS
SELECT t.empeeno, acctno, datetrans, f.transvalue AS difference
FROM cashiertotals t
INNER JOIN cashiertotalsbreakdown b on b.cashiertotalid = t.id
INNER JOIN fintrans f on FintransId = f.id
