IF EXISTS (SELECT * 
		   FROM sysobjects 
		   WHERE name = 'VIEW_StoreCardStatusLatest'
		   AND xtype = 'V')
BEGIN
	DROP VIEW VIEW_StoreCardStatusLatest
END
GO

CREATE VIEW VIEW_StoreCardStatusLatest
AS
SELECT S.CardNumber ,
        DateChanged ,
        StatusCode ,
        EmpeeNo ,
        BranchNo ,
        Notes ,
        AcctNo ,
        storecard.CardName
FROM StoreCardStatus S 
INNER JOIN storecard ON S.CardNumber = StoreCard.CardNumber
WHERE DateChanged = (SELECT MAX(datechanged)
				     FROM storecardstatus s2
				     WHERE s2.CardNumber = S.CardNumber)