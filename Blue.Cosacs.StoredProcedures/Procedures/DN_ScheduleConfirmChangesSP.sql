SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_ScheduleConfirmChangesSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_ScheduleConfirmChangesSP]
GO

CREATE PROCEDURE 	dbo.DN_ScheduleConfirmChangesSP
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : DN_ScheduleConfirmChangesSP.prc
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
-- 20/06/11  IP  CR1212 - RI - #4042 - RI Changes for Changes after scheduling
-- ================================================
            @acctno varchar(12),
            @agrmtno int,
            --@itemno varchar(10),
            @itemID int,						--IP - 18/06/11 - CR1212 - RI - #4042
            @stocklocn smallint,
            @buffno int,
			@origbuffno int,
			@empeeno int,
			@removal char(1),
			@loadno smallint,
			@picklistno int,
			@picklistbranch smallint,
			@transchedno int,
			@transchednobranch int,
			--@origitemno char(10),
			@origItemID int,				--IP - 20/06/11 - CR1212 - RI - #4042
			@return int OUTPUT

AS

    SET    @return = 0			--initialise return code
    
    --DECLARE @v_item varchar(10)
    DECLARE @v_item int							--IP - 18/06/11 - CR1212 - RI - #4042

	IF(@removal = 'A')
		--SET @v_item = @origitemno
		  SET @v_item = @origItemID			--IP - 20/06/11 - CR1212 - RI - #4042
	ELSE
		--SET @v_item = @itemno
		SET @v_item = @itemID					--IP - 18/06/11 - CR1212 - RI - #4042

	IF(@removal = 'R')
	BEGIN
	    UPDATE	Schedule
	    SET		loadno = 0,
				picklistnumber = 0,
				picklistbranchnumber = 0,
				transchedno = 0,
				transchednobranch = 0,
	            printedby = 0,
	            dateprinted = NULL
	    WHERE	acctno = @acctno
	    AND		agrmtno = @agrmtno
	    --AND		itemno = @itemno
	    AND		itemID = @itemID			--IP - 18/06/11 - CR1212 - RI - #4042
	    AND		stocklocn = @stocklocn
	    AND		buffno = @buffno
	END
	ELSE
	BEGIN
	    UPDATE	Schedule
		SET		loadno = @loadno,
				picklistnumber = @picklistno,
				picklistbranchnumber = @picklistbranch,
				transchedno = @transchedno,
				transchednobranch = @transchednobranch
	    WHERE	acctno = @acctno
	    AND		agrmtno = @agrmtno
	    --AND		itemno = @itemno
	    AND     itemID = @itemID			--IP - 18/06/11 - CR1212 - RI - #4042
	    AND		stocklocn = @stocklocn
	    AND		buffno = @buffno

        INSERT INTO deliveryload(origbr, branchno, datedel, loadno, buffbranchno, buffno)
        SELECT	DISTINCT 0, d.branchno, d.datedel, @loadno,d.buffbranchno, @buffno
        FROM	deliveryload d, order_removed o
	    WHERE	o.acctno = @acctno
	    AND		o.agrmtno = @agrmtno
	    --AND		o.itemno = @v_item
	    AND		o.itemID = @v_item			--IP - 18/06/11 - CR1212 - RI - #4042			
	    AND		o.stocklocn = @stocklocn
	    AND		o.buffno = @origbuffno
		AND		o.loadno = @loadno
		AND 	o.buffno = d.buffno
		AND 	o.buffbranchno = d.buffbranchno
		AND 	o.loadno = d.loadno
	END

    UPDATE	order_removed
    SET		dateconfirmed = GETDATE(),
            confirmedby = @empeeno
    WHERE	acctno = @acctno
    AND		agrmtno = @agrmtno
    --AND		itemno = @v_item
    AND		itemID = @v_item				--IP - 18/06/11 - CR1212 - RI - #4042	
    AND		stocklocn = @stocklocn
    AND		buffno = @origbuffno

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

