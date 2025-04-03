
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID('[dbo].[postaladdressview]') AND OBJECTPROPERTY(id, 'IsView') = 1)
DROP VIEW [dbo].[postaladdressview]
GO

--68750 [2006-01-05] Created [PC]
--This is based on home address view but is designed to pick up if a postal addresses first if they exist.
CREATE VIEW dbo.postaladdressview AS

SELECT 
	ca.acctno,
	ca.custid,
	isnull(cu.cusaddr1,'') as cusaddr1,
	isnull(cu.cusaddr2,'') as cusaddr2,
	isnull(cu.cusaddr3,'') as cusaddr3,
	isnull(cu.cuspocode,'') as cuspocode
FROM 
	custacct ca LEFT JOIN 
	custaddress cu ON ca.custid = cu.custid
WHERE 
	ca.hldorjnt = 'H'							AND 
	isnull(cu.DateMoved, '') = ''				AND 
	cu.addtype ='H'								AND   
	NOT EXISTS (   SELECT *
                   FROM   custaddress cust 
                   WHERE  
						cust.custid = cu.custid AND	  
						cust.datein > cu.datein AND    
						cust.addtype = 'H' AND    
						isnull(cust.DateMoved, '') = ''
                 ) AND 
	NOT EXISTS (	SELECT * 
					FROM custaddress cust1
					WHERE 
						cust1.custid = cu.custid AND	   
						cust1.addtype = 'P' AND    
						isnull(cust1.DateMoved, '') = ''
)

UNION
SELECT 
	ca.acctno,
	ca.custid,
	isnull(cu.cusaddr1,'') as cusaddr1,
	isnull(cu.cusaddr2,'') as cusaddr2,
	isnull(cu.cusaddr3,'') as cusaddr3,
	isnull(cu.cuspocode,'') as cuspocode
FROM 
	custacct ca LEFT JOIN 
	custaddress cu ON ca.custid = cu.custid
WHERE 
	ca.hldorjnt = 'H'							AND 
	isnull(cu.DateMoved, '') = ''				AND 
	cu.addtype ='P'								AND   
	NOT EXISTS (   SELECT *
                   FROM   custaddress cust 
                   WHERE  
						cust.custid = cu.custid AND	  
						cust.datein > cu.datein AND    
						cust.addtype = 'P' AND    
						isnull(cust.DateMoved, '') = ''
                 )

GO

GRANT SELECT ON postaladdressview TO PUBLIC

GO





