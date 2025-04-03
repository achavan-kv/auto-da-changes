/* Author: K Fernandez
   Date:   29th November 2004
   Description: FR66479 - Stored procedure to terminate a promotion for any item
   Execution: execute dbterminatepromotion <itemno>
              Replacing <itemno> with the item that needs to have the promotion terminated
*/

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO
SET ANSI_WARNINGS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[dbterminatepromotion]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[dbterminatepromotion]
GO

CREATE PROCEDURE 	dbo.dbterminatepromotion
			@itemno varchar(8)
AS
	--find most recent rows to update
	select	stocklocn, hporcash, max(fromdate) as promostartdate
	into	#promo
	from	promoprice
	where 	itemno = @itemno
	and	todate > convert(datetime,convert(varchar,getdate(),101)) --no point resetting if not after toady
	group by stocklocn, hporcash

	UPDATE	promoprice
	SET	todate =  convert(datetime,convert(varchar,getdate()-1,101))  --remove time part and set to yesterday
	FROM	#promo p
	WHERE	itemno = @itemno
	AND	fromdate = promostartdate
	AND	promoprice.stocklocn = p.stocklocn
	AND	promoprice.hporcash = p.hporcash
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_WARNINGS OFF 
GO
