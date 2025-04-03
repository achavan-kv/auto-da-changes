-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
-- Related to issue: #12852 (UAT 12566)

SELECT SortColumnName, MIN(SortOrder) AS SortOrder
INTO #CMWorkListSortOrder_test
FROM CMWorkListSortOrder
GROUP BY SortColumnName

DELETE CMWorkListSortOrder
FROM CMWorkListSortOrder t1 INNER JOIN #CMWorkListSortOrder_test t2 ON t1.SortColumnName = t2.SortColumnName
WHERE t1.SortOrder != t2.SortOrder