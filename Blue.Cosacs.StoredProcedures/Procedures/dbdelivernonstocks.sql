SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[dbdelivernonstocks]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[dbdelivernonstocks]
GO

create procedure dbdelivernonstocks @min integer,@Max integer as
declare @status integer
BEGIN TRAN
	CREATE TABLE #tmp_NSToFollow
	    (AcctNo VARCHAR(12) NOT NULL DEFAULT '',
	     Order_Total MONEY NOT NULL DEFAULT 0.0,
	     Delivered_Total MONEY NOT NULL DEFAULT 0.0,
	     NS_Total MONEY NOT NULL DEFAULT 0.0,
	     NS_Delivered MONEY NOT NULL DEFAULT 0.0,
	     ToFollow MONEY NOT NULL DEFAULT 0.0,
	     NS_ToFollow MONEY NOT NULL DEFAULT 0.0,
	     NS_ItemNo VARCHAR(8) NOT NULL DEFAULT '',
	     NS_StockLocn SMALLINT NOT NULL DEFAULT 0,
	     NS_Contractno VARCHAR(10) NOT NULL DEFAULT '',
	     NS_ordval money not null default 0,
	     NS_quantity int not null default 1)

	INSERT INTO #tmp_NSToFollow (AcctNo, Order_Total)
	SELECT v.AcctNo, ISNULL(SUM(l.OrdVal),0)
	FROM   delivery v, LineItem l,agreement g 
	WHERE  l.AcctNo = v.AcctNo and g.acctno=l.acctno
	AND    l.IsKit = 0
   and v.runno between @min and @Max
	GROUP BY v.AcctNo, g.agrmttotal
	HAVING	abs(g.agrmttotal - sum(ordval)) <= 0.01

	UPDATE #tmp_NSToFollow
	SET    Delivered_Total = (SELECT ISNULL(SUM(f.TransValue),0)
                          FROM   FinTrans f
                          WHERE  f.AcctNo = #tmp_NSToFollow.AcctNo
                          AND    f.TransTypeCode IN ('ADD','REB','DEL','GRT','RDL','REP'))

	UPDATE #tmp_NSToFollow
	SET    NS_Total = (SELECT ISNULL(SUM(l.OrdVal),0)
                   FROM   LineItem l, StockItem s
                   WHERE  l.AcctNo = #tmp_NSToFollow.AcctNo
                   AND    l.IsKit = 0
                   AND    s.ItemNo = l.ItemNo
                   AND    s.StockLocn = l.StockLocn
                   AND    s.ItemType = 'N')

	UPDATE #tmp_NSToFollow
	SET    NS_Delivered = (SELECT ISNULL(SUM(l.OrdVal),0)
                       FROM   LineItem l, Delivery d, StockItem s
                       WHERE  l.AcctNo = #tmp_NSToFollow.AcctNo
                       AND    l.IsKit = 0
                       AND    s.ItemNo = l.ItemNo
                       AND    s.StockLocn = l.StockLocn
                       AND    s.ItemType = 'N'
                       AND    d.AcctNo = l.AcctNo
                       AND    d.ItemNo = l.ItemNo
                       AND    d.StockLocn = l.StockLocn)


	-- ToFollow = Order_Total - Delivered_Total
	-- NS_ToFollow = NS_Total - NS_Delivered

	UPDATE #tmp_NSToFollow
	SET    ToFollow    = (Order_Total - Delivered_Total),
	       NS_ToFollow = (NS_Total - NS_Delivered)

	DELETE FROM #tmp_NSToFollow
	WHERE  ToFollow < 0.01
	OR     ABS(ToFollow - NS_ToFollow) >= 0.01

	UPDATE #tmp_NSToFollow
	SET    NS_ItemNo    = l.ItemNo,
	       NS_StockLocn = l.StockLocn,
			NS_Contractno = l.contractno,
			NS_ordval = l.ordval,
			NS_quantity = l.quantity
	FROM   LineItem l
	WHERE  l.AcctNo = #tmp_NSToFollow.AcctNo
	AND    abs(l.OrdVal - #tmp_NSToFollow.NS_ToFollow) < 1

	DELETE FROM #tmp_NSToFollow
	WHERE	isnull(NS_itemno,'') = ''

	ALTER TABLE #tmp_NSToFollow ADD LineID integer IDENTITY(1,1)  not null

COMMIT

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

