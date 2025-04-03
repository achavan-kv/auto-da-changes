-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

UPDATE delivery 
	SET ParentItemID =ISNULL(S.ID,0)
FROM delivery d INNER JOIN stockinfo s on d.ParentItemNo = s.itemno
WHERE ParentItemNo !='' and ISNULL(ParentItemID, 0) = 0