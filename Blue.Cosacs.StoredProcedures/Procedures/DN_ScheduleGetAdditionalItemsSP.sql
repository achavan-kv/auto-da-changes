SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_ScheduleGetAdditionalItemsSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_ScheduleGetAdditionalItemsSP]
GO

CREATE PROCEDURE 	dbo.DN_ScheduleGetAdditionalItemsSP
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : DN_ScheduleGetAdditionalItemsSP.prc
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
-- 18/06/11  IP  CR1212 - RI - #4042 - RI Changes for Changes after scheduling
-- ================================================
            @acctno varchar(12),
			@loadno smallint,
			@picklistno int,
			@picklistbranch smallint,
			@return int OUTPUT

AS

    SET    @return = 0			--initialise return code

    CREATE TABLE #additions
    (
		acctno varchar(12), 
		delnotebranch smallint, 
		buffno int,
		stocklocn smallint, 
		datereqdel datetime, 
		dateremoved datetime,
        itemno varchar(18),								--IP - 18/06/11 - CR1212 - RI - #4042
        itemdescr1 varchar(70), 
        empeeno int,
		holdprop varchar(4), 
		originalitem varchar(18),						--IP - 18/06/11 - CR1212 - RI - #4042
		agrmtno int,					
		itemID int,										--IP - 18/06/11 - CR1212 - RI - #4042
		origItemItemID int							--IP - 18/06/11 - CR1212 - RI - #4042
    )
    
    SELECT  acctno, s.IUPC, originallocation, loadno, transchedno,				--IP - 18/06/11 - CR1212 - RI - #4042
			transchednobranch, type, empeeno, dateremoved, stocklocn, itemID	--IP - 18/06/11 - CR1212 - RI - #4042
	INTO	#changes
	FROM	order_removed o INNER JOIN StockInfo s ON o.itemID = s.ID	--IP - 18/06/11 - CR1212 - RI - #4042
	WHERE	acctno = @acctno
	AND 	loadno = @loadno
	AND 	transchedno = @picklistno
	AND 	transchednobranch = @picklistbranch
	AND		type = 'R'

    IF(@@rowcount > 0)
    BEGIN
		INSERT INTO #additions
		(
			acctno, delnotebranch, buffno,
			stocklocn, datereqdel, dateremoved,
			itemno, itemdescr1, empeeno,
			holdprop, originalitem, agrmtno, itemID, origItemItemID	--IP - 18/06/11 - CR1212 - RI - #4042		
		)
		SELECT  l.acctno,
          		l.delnotebranch,
          		0,
				l.stocklocn,
          		l.datereqdel,
				null,
          		st.IUPC as itemno,
				st.itemdescr1 + ' ' + st.itemdescr2,
				0,
				CASE a.holdprop WHEN 'Y' THEN 'No' WHEN 'N' THEN 'Yes' ELSE '' END,
				'',
				a.agrmtno,
				l.itemID,				--IP - 18/06/11 - CR1212 - RI - #4042
				0						--IP - 18/06/11 - CR1212 - RI - #4042
		FROM	lineitem l, agreement a, stockitem st
		WHERE	l.acctno = @acctno
		AND		l.acctno = a.acctno
		AND		l.agrmtno = a.agrmtno
		AND		l.itemid = st.itemid
		AND 	l.stocklocn = st.stocklocn
		AND		l.quantity > 0
		AND		st.iupc NOT IN('DT', 'SD', 'STAX', 'ADDDR', 'ADDCR')
		--AND		st.category NOT IN(12,82,36,37,38,39,46,47,48,86,87,88)
		AND		st.category NOT IN(select distinct code from code where category in ('WAR', 'PCDIS')) --IP - 29/10/09 - CoSACS Improvement - Remove hardcoded discounts & warranties
		AND	NOT EXISTS(	SELECT	1
						FROM	schedule s
						WHERE	l.acctno = s.acctno
						AND		l.agrmtno = s.agrmtno
						AND		l.itemid = s.itemid
						AND 	l.stocklocn = s.stocklocn
						AND 	loadno = @loadno
						AND 	transchedno =  @picklistno
						AND 	transchednobranch = @picklistbranch)

		DELETE
		FROM	#additions
		WHERE EXISTS( SELECT 1
					  FROM	 order_removed r
					  WHERE	 #additions.acctno = r.acctno
					  AND	 #additions.agrmtno = r.agrmtno
					  AND	 #additions.itemid = r.itemid
					  AND	 #additions.stocklocn = r.stocklocn)
					  
		DELETE
		FROM	#additions
		WHERE EXISTS(SELECT	1
					 FROM	schedule s
					 WHERE	#additions.acctno = s.acctno
					 AND	#additions.agrmtno = s.agrmtno
					 AND	#additions.itemid = s.itemid
					 AND 	#additions.stocklocn = s.stocklocn
					 AND	s.loadno > 0)	
					  
		UPDATE	#additions
		SET		buffno = s.buffno
		FROM	schedule s, #additions a
		WHERE	a.acctno = s.acctno
		AND		a.agrmtno = s.agrmtno
		AND		a.itemid = s.itemid
		AND 	a.stocklocn = s.stocklocn

		UPDATE	#additions
		SET		dateremoved = c.dateremoved,
				empeeno = c.empeeno,
				--originalitem = c.itemid
				originalitem = s.IUPC,									--IP - 18/06/11 - CR1212 - RI - #4042
				origItemItemID = c.itemID							--IP - 18/06/11 - CR1212 - RI - #4042								
		FROM	#changes c INNER JOIN StockInfo s ON c.itemID = s.ID	--IP - 18/06/11 - CR1212 - RI - #4042
		WHERE 	c.loadno = @loadno
		AND 	c.transchedno =  @picklistno
		AND 	c.transchednobranch = @picklistbranch					
		AND		c.type = 'R'
	END	

    SELECT  acctno,
          	delnotebranch,
          	buffno,
			stocklocn,
          	datereqdel,
			dateremoved,
          	stockinfo.IUPC as itemno,									--IP - 20/06/11 - CR1212 - RI - #4042
			a.itemdescr1,
			empeeno,
			holdprop,
			originalitem,
			agrmtno,
			itemID,														--IP - 18/06/11 - CR1212 - RI - #4042
			origItemItemID											--IP - 18/06/11 - CR1212 - RI - #4042
    FROM	#additions a
    INNER JOIN stockinfo ON a.itemid = StockInfo.ID

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

