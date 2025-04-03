IF EXISTS (SELECT * FROM sysobjects
		   WHERE xtype = 'V'
		   AND name = 'View_FintransPayMethod')
BEGIN
	DROP VIEW View_FintransPayMethod
END
GO

CREATE VIEW View_FintransPayMethod
AS
SELECT f.acctno,f.agrmtno,SUM(transvalue) AS transvalue,bankacctno,bankcode,chequeno,
       codedescript AS PayMethodDesc, paymethod, storecardno
FROM fintrans f
INNER JOIN code ON f.paymethod = code.code
LEFT JOIN finxfr ON f.acctno = finxfr.acctno AND f.transrefno = finxfr.transrefno AND f.datetrans = finxfr.datetrans
WHERE code.category = 'FPM'
AND code.statusflag = 'L'
AND f.transtypecode IN ('PAY','SCT','REF','STR')
GROUP BY f.acctno,f.agrmtno,bankacctno,bankcode,chequeno,
       codedescript, paymethod, storecardno

