-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

-------------------------------------------
---   Remove FK and any existing data   ---
-------------------------------------------
IF EXISTS(SELECT 1 FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_NAME ='FK_Call_CustomerSalesPerson')
BEGIN 
	ALTER TABLE [SalesManagement].[Call] DROP CONSTRAINT [FK_Call_CustomerSalesPerson]
END

DELETE SalesManagement.CustomerSalesPerson

-------------------------------------------------------------------------------------
---   A customer can have many contatcs of the same time but only one is active   ---
---   so with this we make sure that we get the lastest active contact per type   ---
-------------------------------------------------------------------------------------

SELECT CustId AS CostomerId, MAX(datechange) datechange, tellocn AS ContactType 
INTO #Contacts
FROM custtel
WHERE datediscon is null AND LEN(ISNULL(telno, '')) > 0 AND tellocn IN ('h','W','M')
GROUP BY custid, tellocn

CREATE TABLE CustomerContact
(
	CustomerId varchar(20) NOT NULL,
	Contact varchar(20) NULL
)  

ALTER TABLE CustomerContact ADD CONSTRAINT
	PK_CustomerContact PRIMARY KEY CLUSTERED 
(
	CustomerId
) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]


--------------------------------------------------------------------
---   On contacts importance order is this: Home, Mobile, Work   ---
--------------------------------------------------------------------

INSERT INTO CustomerContact
	(CustomerId, Contact)
SELECT DISTINCT 
	c.custid,
	h.telno
FROM 
	customer c
	LEFT JOIN 
	(
		SELECT w.custid, w.telno
		FROM custtel w INNER JOIN #Contacts cont ON cont.CostomerId = w.custid AND cont.datechange = w.datechange and w.tellocn = cont.ContactType and w.datediscon is null WHERE w.tellocn = 'H' AND LEN(ISNULL(w.telno, '')) > 0 
	) h
		ON c.custid = h.custid

UPDATE CustomerContact
SET Contact = Data.telno
FROM 
(
	SELECT w.custid, w.telno
	FROM custtel w INNER JOIN #Contacts cont ON cont.CostomerId = w.custid AND cont.datechange = w.datechange and w.tellocn = cont.ContactType and w.datediscon is null WHERE w.tellocn = 'M' AND LEN(ISNULL(w.telno, '')) > 0
) Data
WHERE
	CustomerContact.CustomerId = Data.custid AND Contact IS NULL

UPDATE CustomerContact
SET Contact = Data.telno
FROM 
(
	SELECT w.custid, w.telno
	FROM custtel w INNER JOIN #Contacts cont ON cont.CostomerId = w.custid AND cont.datechange = w.datechange and w.tellocn = cont.ContactType and w.datediscon is null WHERE w.tellocn = 'W' AND LEN(ISNULL(w.telno, '')) > 0
) Data
WHERE
	CustomerContact.CustomerId = Data.custid AND Contact IS NULL

DROP TABLE #Contacts

-----------------------------------------------------------------------------------
---   All accounts/customers are going to be inserted in this table             ---
---   To be easy to handle in the same table will e stored the branch as well   ---
-----------------------------------------------------------------------------------

CREATE TABLE CustomerAccount
(
	Account Char(12) NOT NULL,
	CustomerId VarChar(20) NOT NULL, 
	Branch SmallInt NOT NULL
)

-- this key is not really needed besides speed up the next migration
ALTER TABLE CustomerAccount ADD CONSTRAINT
	PK_CustomerAccount PRIMARY KEY CLUSTERED 
(
	Account, CustomerId
) 

INSERT INTO CustomerAccount
	(Account, CustomerId, Branch)
SELECT 
	ac.acctno, 
	ac.custid,
	branch.branchno
FROM 
	custacct ac
	INNER JOIN branch 
		ON CONVERT(SmallInt, LEFT(ac.acctno, 3)) = branch.branchno
	INNER JOIN customer cust
		ON ac.custid = cust.custid
WHERE
	ac.hldorjnt = 'H'

--------------------------------------------------------------------------------
---   On this area we get the last person that interact with each customer   ---
--------------------------------------------------------------------------------

CREATE TABLE LineItemAuditTmp
(
	Id int NOT NULL,
	Account char(12) NOT NULL,
	UserId int NOT NULL
) 

ALTER TABLE LineItemAuditTmp ADD CONSTRAINT
	PK_LineItemAuditTmp PRIMARY KEY CLUSTERED 
(
	Id
) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

INSERT INTO LineItemAuditTmp
	(Id, Account, UserId)
SELECT 
	l.LineItemAuditID,
	l.acctno,
	l.Empeenochange
FROM 
	(
		Select 
			MAX(i.LineitemAuditId) AS LineItemAuditId
		From 
			LineitemAudit i
			INNER JOIN CustomerAccount c
				ON i.acctno = c.Account
				AND i.agrmtno = 1
		WHERE 
			LTRIM(RTRIM(i.[source])) In ('NewAccount', 'Revise') 
		GROUP BY 
			c.CustomerId
	) MaxData
	INNER JOIN LineitemAudit l
		ON MaxData.LineItemAuditId = l.LineItemAuditID
		AND l.agrmtno = 1
	INNER JOIN Admin.[User] u
		ON u.id = l.Empeenochange