-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here


--Delete:
--Name: Cash and Go
--Description: Cash and Go - Allows user access to the Cash and Go screen via the Account Menu
--Category: 10


DELETE 
FROM
    admin.[RolePermission]
WHERE
        PermissionId = 64
DELETE 
FROM
    admin.[permission]
WHERE
    id = 64


--Name: Cash and Go - Instant Replacement
--Description: Cash and Go - Allows the sale of instant replacement warranties on cash and go accounts
--Category: 10

DELETE 
FROM
    admin.[RolePermission]
WHERE
        PermissionId = 133
DELETE 
FROM
    admin.[permission]
WHERE
    id = 133

--Name: Cash And Go - Legacy Returns
--Description: Cash and Go Returns - Allows user access to the Legacy Cash and Go Returns screen via the Account Menu
--Category: 10

DELETE 
FROM
    admin.[RolePermission]
WHERE
        PermissionId = 156
DELETE 
FROM
    admin.[permission]
WHERE
    id = 156


--Name: Cash and Go - Supashield Warranties
--Description: Cash and Go - Allows the sale of supashield/extended warranties on cash and go accounts
--Category: 10

DELETE 
FROM
    admin.[RolePermission]
WHERE
        PermissionId = 134
DELETE 
FROM
    admin.[permission]
WHERE
    id = 134

--Name: Cash and Go - Warranties on Credit
--Description: Cash and Go - Enables the buy on credit check box when selling warranties on cash and go to allow for the warranty to be purchased
--Category: 10
DELETE 
FROM
    admin.[RolePermission]
WHERE
        PermissionId = 131
DELETE 
FROM
    admin.[permission]
WHERE
    id = 131


--Name: Search Cash and Go - Print All
--Description: Search Cash and Go - Access to the Print all button on the search cash and go screen
--Category: 1

DELETE 
FROM
    admin.[RolePermission]
WHERE
        PermissionId = 378
DELETE 
FROM
    admin.[permission]
WHERE
    id = 378

--Name: Search Cash and Go - Reprint Receipt
--Description: Search Cash and Go - Access to the Reprint Receipt option when right clicking on an account that has come up in the search window
--Category: 1

DELETE 
FROM
    admin.[RolePermission]
WHERE
        PermissionId = 377
DELETE 
FROM
    admin.[permission]
WHERE
    id = 377