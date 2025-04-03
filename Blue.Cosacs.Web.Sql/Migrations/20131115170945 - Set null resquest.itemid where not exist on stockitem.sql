-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
UPDATE  Service.Request
SET itemid = Data.itemid
FROM
(
	SELECT 
		i.itemid,
		r.Id
	FROM 
		service.Request r 
		LEFT JOIN stockitem i
			ON r.ItemId = i.ItemID
) Data
WHERE
	Data.Id = Service.Request.Id