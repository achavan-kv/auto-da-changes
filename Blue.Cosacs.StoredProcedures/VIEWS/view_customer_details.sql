

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID('[dbo].[view_cust_details]') AND OBJECTPROPERTY(id, 'IsView') = 1)
DROP VIEW [dbo].[view_cust_details]
GO

CREATE VIEW view_cust_details
AS SELECT 
a.acctno, 
ca.custid, 
title, 
NAME AS surname, 
firstname+' '+alias AS other_names, 
cusaddr1 AS homeaddress1, 
cusaddr2 AS homeaddress2,
cusaddr3 AS homeaddress3, 
cuspocode AS homepostcode,
w.telno AS workphone,
h.telno AS homephone,
m.telno AS mobilephone,
email AS email,
hldorjnt,
currstatus,
sc.datestatchge,
custstat.statuscode AS cust_stat,
custstat.datestatchge AS cust_stat_date
from   
custacct ca, 
acct a,  
-- customer current status
(SELECT  csd.custid, MAX(statuscode)AS statuscode, MAX(s2.datestatchge) AS datestatchge
FROM status AS s2,
(select ca.custid, max(datestatchge) as datestatchge 
from status AS s3,
custacct AS ca
WHERE s3.acctno=ca.acctno
AND hldorjnt='H'
group by ca.custid) csd,
custacct AS ca3
WHERE s2.acctno=ca3.acctno
AND hldorjnt='H'
AND ca3.custid=csd.custid
AND s2.datestatchge=csd.datestatchge
GROUP BY csd.custid) custstat,
(select acctno, statuscode, max(datestatchge) as datestatchge from status
group by acctno,statuscode) sc, 
customer c
LEFT OUTER JOIN custaddress cadd ON cadd.custid=c.custid
 	and		addtype = 'H'
	and    datemoved is NULL
LEFT OUTER JOIN custtel h ON c.custid=h.custid
				and H.tellocn = 'H'
				and H.datediscon is NULL
LEFT OUTER JOIN custtel m ON c.custid=m.custid
		and m.tellocn = 'M'
		and m.datediscon is NULL
LEFT OUTER JOIN custtel w ON c.custid=w.custid
		and w.tellocn = 'W'
		and w.datediscon is NULL
WHERE
	ca.acctno = a.acctno 
	and ca.custid = c.custid
	and sc.acctno=a.acctno
	and sc.statuscode=a.currstatus
	and ca.custid=custstat.custid

