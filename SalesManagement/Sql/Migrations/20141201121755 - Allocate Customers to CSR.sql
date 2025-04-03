-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

INSERT INTO SalesManagement.CustomerSalesPerson
	(SalesPersonId, CustomerId, PhoneNumber, CustomerBranch, DoNotCallAgain)
SELECT 
	l.UserId AS SalesPersonId,
	ca.CustomerId AS CustomerId,
	cc.Contact as Phone,
	ca.Branch AS CustomerBranch, 
	0 as DoNotCallAgain
FROM 
	LineItemAuditTmp l
	INNER JOIN CustomerAccount ca
		ON l.Account = ca.Account
	INNER JOIN CustomerContact cc
		ON ca.CustomerId = cc.CustomerId
WHERE
	LEN(ISNULL(cc.Contact, '')) > 0