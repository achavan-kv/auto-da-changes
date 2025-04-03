-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
DELETE Service.Payment
FROM 
	Service.Payment p 
	LEFT JOIN Service.Request r
		ON p.RequestId = r.id
WHERE
	r.id IS NULL