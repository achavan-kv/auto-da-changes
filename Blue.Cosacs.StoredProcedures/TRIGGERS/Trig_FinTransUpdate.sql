if exists (select * from sysobjects where name = 'Trig_FinTransUpdate')
drop trigger Trig_FinTransUpdate

GO 
CREATE Trigger [dbo].[Trig_FinTransUpdate] ON [dbo].[fintrans]
For Update

AS
--- We are now sending interest on settled accounts to Oracle. We sending totals grouped by empeeno and transtype code to save sending 
-- too many transactions at once. 
IF EXISTS (SELECT * FROM CountryMaintenance WHERE CodeName = 'OracleLineExport' and Value IN ('F','P','L'))
BEGIN
	INSERT INTO FinTransOracleExport (AcctNo, BranchNo, TransRefNo, DateTrans, TransValue, 
				EmpeeNo, PayMethod, TransTypeCode, ChequeNo, BankCode, RunNo, OracleReceiptNo)
	SELECT i.AcctNo, i.BranchNo, MAX(i.TransRefNo), GETDATE(), SUM(ROUND(i.TransValue,0)), 
				i.EmpeeNo, 0, i.TransTypeCode, '', '', 0, NULL
	FROM inserted i, deleted f
	WHERE i.acctno= f.acctno AND i.transtypecode =f.transtypecode
	AND i.transrefno = f.transrefno AND i.runno>0 AND f.runno =0
	AND f.datetrans = i.datetrans 
	AND i.transtypecode IN ('INT','ADM') 
    GROUP BY i.acctno,i.BranchNo,i.EmpeeNo, i.TransTypeCode
END
GO 
