SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_ScheduleGetAssociatedItemsSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_ScheduleGetAssociatedItemsSP]
GO

CREATE PROCEDURE 	dbo.DN_ScheduleGetAssociatedItemsSP
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : DN_ScheduleGetAssociatedItemsSP.PRC
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : 
-- Author       : ??
-- Date         : ??
--
-- 
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 17/05/11  ip  RI Integration changes - CR1212 - #3627 - Changed join between lineitem, stockitem, schedule table to now use ItemID and ParentItemID
--				 rather than itemno and parentitemno. Selecting retitemno as IUPC
-- 12/01/12  IP/JC #9440 - Include Installation Items
-- 15/03/12  IP  #9797 - Discount Item was not collected when linked to an item then re-linked to a different item.
-- =================================================================================
			@acctno varchar(12),
			@agreementno int,
			@itemID int,					--IP - 17/05/11 - CR1212 - #3627
			--@itemno varchar(10),
			@location smallint,	
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	--SELECT	s.ItemID,st.IUPC as ItemNo, s.stocklocn, s.RetItemID,s.retitemno, s.retstocklocn, l.contractno		--IP - 17/05/11 - CR1212 - #3627 - selecting ItemID and RetItemID. Changed itemno to select IUPC
	--FROM	 lineitem l, stockitem st, schedule s
	--LEFT JOIN stockitem sr on s.RetItemID = sr.ItemID and s.retstocklocn = sr.stocklocn
	--WHERE	s.acctno = @acctNo
	--AND 	s.agrmtno= @agreementNo
	--AND	s.acctno = l.acctno
	----AND	s.itemno = l.itemno
	--AND	s.ItemID = l.ItemID					--IP - 17/05/11 - CR1212 - #3627
	--AND	s.stocklocn = l.stocklocn
	--AND	s.contractno = l.contractno
	----AND	s.itemno = st.itemno
	--AND	s.ItemID = st.ItemID				--IP - 17/05/11 - CR1212 - #3627
	--AND	s.stocklocn = st.stocklocn
	----AND	st.category IN(12,82,36,37,38, 39, 46,47,48,86,87,88)
	--AND	st.category IN(select distinct code from code where category in ('WAR', 'PCDIS')) --IP - 29/10/09 - CoSACS Improvement - Remove hardcoded discounts & warranties
	----AND	l.parentitemno = @itemno
	--AND	l.ParentItemID = @itemID			--IP - 17/05/11 - CR1212 - #3627
	--AND	l.parentlocation = @location
	--AND	s.quantity < 0
	
	SELECT	s.ItemID,st.IUPC as ItemNo, s.stocklocn, s.RetItemID,sr.IUPC as retitemno, s.retstocklocn, l.contractno		--IP - 17/05/11 - CR1212 - #3627 - selecting ItemID and RetItemID. Changed itemno to select IUPC
	FROM	schedule s
	INNER JOIN lineitem l ON s.acctno = l.acctno
	--AND	s.itemno = l.itemno
	AND	s.ItemID = l.ItemID					--IP - 17/05/11 - CR1212 - #3627
	AND	s.stocklocn = l.stocklocn
	AND	s.contractno = l.contractno
	AND s.ParentItemID = l.ParentItemID		--IP - 15/03/12 - #9797
	INNER JOIN stockitem st ON s.ItemID = st.ItemID				--IP - 17/05/11 - CR1212 - #3627
	--AND	s.itemno = st.itemno
	AND	s.stocklocn = st.stocklocn
	LEFT JOIN stockitem sr ON s.RetItemID = sr.ItemID
	AND s.retstocklocn = sr.stocklocn
	WHERE	s.acctno = @acctNo
	AND 	s.agrmtno= @agreementNo
	--AND	st.category IN(12,82,36,37,38, 39, 46,47,48,86,87,88)
	--AND	st.category IN(select distinct code from code where category in ('WAR', 'PCDIS', 'INST')) --IP - 29/10/09 - CoSACS Improvement - Remove hardcoded discounts & warranties
	AND	(st.category IN(select distinct code from code where category in ('WAR', 'PCDIS'))			   
			  or	st.category IN(select distinct reference from code where category in ('INST'))			--IP/JC  12/01/12 - #9440
              or    st.itemno IN(select code from code where category in ('ASSY'))
              or    st.itemno IN(select code from code where category in ('ANNSERVCONT'))
              or    st.itemno IN(select code from code where category in ('GENSERVICE')))			
	--AND	l.parentitemno = @itemno
	AND	l.ParentItemID = @itemID			--IP - 17/05/11 - CR1212 - #3627
	AND	l.parentlocation = @location
	AND	s.quantity < 0

	
	SET @return = @@error

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO
