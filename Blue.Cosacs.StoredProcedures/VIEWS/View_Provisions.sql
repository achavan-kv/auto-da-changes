IF EXISTS (SELECT * FROM sysobjects
               WHERE xtype = 'V'
               AND name = 'view_provision')
BEGIN
	DROP VIEW View_Provision
END
GO

CREATE VIEW View_Provision
AS
	SELECT f.acctno,  SUM(transvalue) * ISNULL((SELECT provision 
													FROM provisions,acct 
													WHERE provisions.acctype = CASE WHEN acct.accttype != 'C' THEN 'R' 
																											ELSE 'C' END
													AND isnumeric(acct.currstatus) = 1 
													AND Convert(INT,CASE WHEN ISNUMERIC(acct.currstatus) = 1 THEN acct.currstatus ELSE -1 END) BETWEEN statuslower AND statusupper
													AND dbo.fn_MonthsInArrears(acctno)  >= monthslower 
													AND dbo.fn_MonthsInArrears(acctno)  < monthsupper
													AND f.acctno = acct.acctno),0) / 100 AS provision
	 FROM fintrans f
	 WHERE f.transtypecode NOT IN('int','adm','dff')
	 GROUP BY f.acctno
