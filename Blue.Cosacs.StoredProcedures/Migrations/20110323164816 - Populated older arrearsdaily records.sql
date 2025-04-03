-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
INSERT INTO ArrearsDaily (
	Acctno,
	arrears,
	datefrom,
	dateto
) 
SELECT 	d.acctno,
	arrears,
	dateadd(minute,1,d.Currentmonth),
	dateadd(MONTH,1,d.Currentmonth)
FROM accountmonths2 d
WHERE NOT EXISTS (SELECT * FROM ArrearsDaily X
WHERE x.Acctno = d.acctno AND x.datefrom < d.Currentmonth)
AND d.arrears >0 AND d.Currentmonth > '1-jan-2007'
order by d.acctno,d.Currentmonth