-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

Update u
Set BranchId = l.Id
From Admin.[User] u
Inner join Merchandising.Location l on u.BranchNo = l.SalesId