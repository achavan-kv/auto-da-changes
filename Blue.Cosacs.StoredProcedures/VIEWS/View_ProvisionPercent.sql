IF EXISTS (SELECT * FROM sysobjects
               WHERE xtype = 'V'
               AND name = 'View_ProvisionPercent')
BEGIN
	DROP VIEW View_ProvisionPercent
END
GO

CREATE VIEW View_ProvisionPercent
AS
	SELECT  acctno,ISNULL((SELECT provision 
													FROM provisions 
													WHERE provisions.acctype = CASE WHEN a.accttype != 'C' THEN 'R' 
																											ELSE 'C' END
													AND isnumeric(a.currstatus) = 1 
													AND Convert(INT,CASE WHEN ISNUMERIC(a.currstatus) = 1 THEN a.currstatus ELSE -1 END) BETWEEN statuslower AND statusupper
													AND dbo.fn_MonthsInArrears(acctno)  >= monthslower 
													AND dbo.fn_MonthsInArrears(acctno)  < monthsupper),0) AS provision
	FROM acct a