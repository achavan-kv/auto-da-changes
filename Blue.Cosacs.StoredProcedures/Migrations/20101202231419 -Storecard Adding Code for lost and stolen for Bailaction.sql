-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here


INSERT INTO dbo.code (
	origbr,	category,
	code,	codedescript,
	statusflag,	sortorder,
	reference,	additional
) VALUES ( 0,'FUP',
'STL','StoreCard Lost/Stolen',
'L', 0,
0,'N')