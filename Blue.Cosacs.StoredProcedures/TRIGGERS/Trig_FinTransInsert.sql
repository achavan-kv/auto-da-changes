if exists (select * from sysobjects where name = 'Trig_FinTransInsert')
drop trigger Trig_FinTransInsert
GO

CREATE Trigger [dbo].[Trig_FinTransInsert] ON [dbo].[fintrans]
For INSERT

AS
DECLARE @datenow DATETIME
SET @datenow = GETDATE() -- using current date rather than saved date --only really an issue for standing order payments
IF EXISTS (SELECT * FROM CountryMaintenance WHERE CodeName = 'OracleLineExport' and Value IN ('F','P','L'))
BEGIN
	INSERT INTO FinTransOracleExport (AcctNo, BranchNo, TransRefNo, DateTrans, TransValue, 
				EmpeeNo, PayMethod, TransTypeCode, ChequeNo, BankCode, RunNo, OracleReceiptNo)
	SELECT AcctNo, BranchNo, TransRefNo, @datenow, TransValue, 
				EmpeeNo, PayMethod, TransTypeCode, ChequeNo, BankCode, 0, NULL
	FROM Inserted
	Where TransTypeCode not IN('DEL','GRT','REP','RDL') -- Deliveries/Returns/Reposs/Re-deliver
	AND NOT (transtypecode IN ('INT','ADM') AND runno = 0) -- exclude unearned interest and admin charges - inserted when account settled. 
END


GO 