--

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_DeliveryLoadAcctsSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_DeliveryLoadAcctsSP]
GO

CREATE PROCEDURE [dbo].[DN_DeliveryLoadAcctsSP]
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : DN_DeliveryLoadAcctsSP.prc
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Load Accounts for Delivery
-- Author       : ??
-- Date         : ??
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 18/05/11  jec RI Integration 
-- 20/09/11  ip  RI - #8220 - CR8201 - Delivery Note print out - description needs to be: descr+brand+vendor style long
-- ================================================
	-- Add the parameters for the stored procedure here  
    		@branchno int,
    		@buffno int,
		@return int OUTPUT 
AS
	SELECT @return = 0

	SET NOCOUNT ON 
	
	DECLARE @deliveries TABLE (
	acctno char(12)  NOT NULL,
	itemno varchar(18)  NOT NULL,		-- RI
	quantity float NOT NULL,
	delqty float NOT NULL,
	itemnotes varchar(200)  NOT NULL,
	stocklocn smallint NULL,
	price money NOT NULL,
	ordval money NOT NULL,
	datereqdel datetime NULL,
	timereqdel varchar(12)  NULL,
	dateplandel datetime NULL,
	itemdescr1 varchar(35)  NOT NULL,
	itemdescr2 varchar(40)  NOT NULL,
	deliveryaddress char(2)  NOT NULL,
	agrmtno int NOT NULL,
	empeenosale int NOT NULL,
	delorcoll char(1)  NOT NULL,
	retitemno varchar(18)  NOT NULL,	-- RI
	retval float NULL,
	dateprinted datetime NULL,
	loadno SMALLINT NULL,
	origBuffBranchNo SMALLINT NULL,		-- UAT(5.2) - 568	
	supplier varchar(40) NULL,  --IP - 09/02/10 - CR1048 (Ref:3.1.2) Merged - Malaysia Enhancements (CR1072)
	printorder int NOT NULL, --IP - 09/02/10 - CR1048 (Ref:3.1.3) Merged - Malaysia Enhancements (CR1072)
	GRTnotes varchar(200),
	removed CHAR(1),
	scheduledateprinted DATETIME,
	ItemID INT,		-- RI
	RetItemId INT,
	Style varchar(25) NULL,	--IP - 20/09/11 - RI - #8220 - CR8201
	Brand varchar(25) NULL 	--IP - 20/09/11 - RI - #8220 - CR8201
	)
	
	INSERT INTO @deliveries (
		acctno,		itemno,		quantity,
		delqty,		itemnotes,		stocklocn,
		price,		ordval,		datereqdel,
		timereqdel,		dateplandel,		itemdescr1,
		itemdescr2,		deliveryaddress,		agrmtno,
		empeenosale,		delorcoll,		retitemno,
		retval,		dateprinted, loadno, origBuffBranchNo, -- UAT(5.2) - 568
		supplier,  printorder,GRTnotes,ItemID , RetItemId, 
		Style, Brand	--IP - 20/09/11 - RI - #8220 - CR8201
	) 
	SELECT	DISTINCT	l.acctno , st.IUPC,		-- RI  l.itemno , 
						s.quantity as quantity, 
						l.delqty ,  l.notes ,  CASE WHEN ISNULL(s.retstocklocn,0) = 0 THEN l.stocklocn ELSE s.retstocklocn END,
						l.price, l.ordval ,     l.datereqdel , 
						l.timereqdel , l.dateplandel ,st.itemdescr1 , 
						st.itemdescr2 ,	l.deliveryaddress ,l.agrmtno,
						a.empeenosale,s.delorcoll,ISNULL(sr.IUPC,'')	-- RI 	ISNULL(s.retitemno, '') 
						, s.retval, s.dateprinted,s.loadno, s.buffbranchno, -- UAT(5.2) - 568
						st.supplier, isnull(l.PrintOrder,0), s.GRTnotes, l.ItemID, ISNULL(sr.ID, 0),	-- RI
						rtrim(ltrim(isnull(st.VendorLongStyle,''))), ltrim(isnull(st.Brand,''))			--IP - 20/09/11 - RI - #8220 - CR8201
	FROM	schedule s 
	INNER join lineitem l ON s.acctno = l.acctno AND s.ItemID=L.ItemID	-- RI  s.itemno = l.itemno 
						     AND s.stocklocn = l.stocklocn AND s.ParentItemID = l.ParentItemID
    INNER JOIN  agreement a ON s.acctno = a.acctno
    INNER join custacct c ON s.acctno = c.acctno
    INNER JOIN stockitem st ON s.ItemID=st.ItemID	-- RI	s.itemno = st.itemno
    LEFT OUTER JOIN stockitem sr ON s.RetItemID=sr.ID	-- RI	s.itemno = st.itemno
	WHERE  	s.buffno = @buffno
	-- UAT 219 --AND    	ISNULL(s.retstocklocn, s.stocklocn) = @branchno
    AND     @branchno = (CASE WHEN ISNULL(s.retstocklocn,0) = 0 THEN s.stocklocn ELSE s.retstocklocn END)
	-- UST 219 --AND		ISNULL(s.retstocklocn, s.stocklocn) = st.stocklocn
    AND     st.stocklocn = (CASE WHEN ISNULL(s.retstocklocn,0) = 0 THEN s.stocklocn ELSE s.retstocklocn END)

    -- 70609 updating to mark reprinted delivery notes on the printer... 
	UPDATE d 
	SET dateprinted = s.dateprinted, scheduledateprinted = s.dateprinted 
	FROM @deliveries d,schedule s 
	WHERE s.acctno= d.acctno AND s.ItemID=d.ItemID	-- RI	s.itemno= d.itemno
	AND s.stocklocn = d.stocklocn AND s.agrmtno= 1 AND d.delorcoll = s.delorcoll
	AND d.delorcoll = 'D'
	AND s.loadno = d.loadno
	
	UPDATE d 
	SET dateprinted = s.Dateremoved, removed = 'Y'
	FROM @deliveries d,Scheduleremoval s 
	WHERE s.acctno= d.acctno AND s.ItemID=d.ItemID	-- RI  s.itemno= d.itemno
	AND s.stocklocn = d.stocklocn AND s.agrmtno= 1
	AND d.delorcoll = 'D'	
	AND s.loadno = d.loadno
	
	
	SELECT acctno,		   itemno,		   quantity,
		   delqty,		   itemnotes,		   stocklocn,
		   price,		   ordval,		   datereqdel,
		   timereqdel,		   dateplandel,		   itemdescr1,
		   itemdescr2,		   deliveryaddress,		   agrmtno,
		   empeenosale,		   delorcoll,		   retitemno,
		   retval,		   dateprinted,		origBuffBranchNo,
		   supplier, printorder,GRTnotes,removed,scheduledateprinted,
		   ItemID, RetItemId,
		   Style, Brand														--IP - 20/09/11 - RI - #8220 - CR8201
    FROM @deliveries 
    ORDER BY PrintOrder	-- CR 1048
    -- are returning where dateprinted set so can return error to application
    --WHERE dateprinted IS NULL 

GO


SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO
