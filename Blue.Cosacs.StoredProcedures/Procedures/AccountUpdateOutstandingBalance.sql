IF EXISTS (SELECT * FROM sysobjects
               WHERE NAME = 'AccountUpdateOutstandingBalance'
               AND xtype = 'P')
BEGIN
	DROP PROCEDURE AccountUpdateOutstandingBalance
END
GO

CREATE PROCEDURE AccountUpdateOutstandingBalance
@acctno VARCHAR(20)
AS
BEGIN

UPDATE acct
SET outstbal = (SELECT SUM(transvalue) 
				FROM fintrans	
				WHERE fintrans.acctno = acct.acctno)
WHERE acctno = @acctno
END
GO