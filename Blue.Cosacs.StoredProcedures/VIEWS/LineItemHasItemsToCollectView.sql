IF OBJECT_ID('LineItemHasItemsToCollectView') IS NOT NULL
	DROP VIEW LineItemHasItemsToCollectView
GO

CREATE VIEW LineItemHasItemsToCollectView
AS
	SELECT
		l.acctno,
		l.agrmtno,
		l.stocklocn,
		l.contractno,
		l.ItemID,
		l.ParentItemID
	FROM 
		lineitem l
		INNER JOIN LineItemBookingSchedule LBS
			ON l.ID = LBS.LineItemID
			AND lbs.DelOrColl = 'C'
			AND LBS.Quantity < 0