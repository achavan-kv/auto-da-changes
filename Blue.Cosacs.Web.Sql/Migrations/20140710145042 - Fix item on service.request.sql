-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
UPDATE Service.Request
SET Item = Data.Item,
ItemNumber = Data.ItemNumber
FROM 
(
	SELECT 
		LTRIM(RTRIM(s.itemdescr1)) + ' ' +  LTRIM(RTRIM(s.itemdescr2)) Item, r.Id, s.itemno AS ItemNumber
	FROM 
		service.request r
		join stockinfo s
			on (r.ItemId = s.id AND r.item != (rtrim(s.itemdescr1) + ' ' + rtrim(s.itemdescr2)))
	WHERE
		r.Type IN ('SI', 'S', 'II')
) Data
WHERE
	Service.Request.Id = Data.Id