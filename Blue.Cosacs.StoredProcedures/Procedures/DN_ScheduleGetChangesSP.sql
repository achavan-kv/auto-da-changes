SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_ScheduleGetChangesSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_ScheduleGetChangesSP]
GO


CREATE PROCEDURE 	DN_ScheduleGetChangesSP
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : DN_ScheduleGetChangesSP.prc
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
			@buffno int,
            @locn smallint,
            --@itemno varchar(10),
            @itemID int,					--IP - 18/06/11 - CR1212 - RI - #4042
			@return int OUTPUT
AS

SET @return = 0

    SELECT    l.stocklocn,
              l.delnotebranch,
              l.deliveryaddress, 
              l.datereqdel,
              l.timereqdel,
              l.damaged,
              l.deliveryprocess,
              l.quantity,
              l.deliveryarea,
              l.notes,
              ISNULL(s.buffno, 0) as newbuffno,
              o.buffno,
              --l.itemno
              st.IUPC as itemno,							--IP - 18/06/11 - CR1212 - RI - #4042
              l.itemID										--IP - 18/06/11 - CR1212 - RI - #4042							
    FROM	  order_removed o
			  INNER JOIN stockinfo st ON o.itemid = st.id 
			  INNER JOIN lineitem l ON o.acctno = l.acctno
									AND o.agrmtno = l.agrmtno
									AND o.itemid = l.itemid
									AND o.stocklocn = l.stocklocn
									AND l.quantity > 0
			  LEFT JOIN schedule s ON l.acctno = s.acctno
								   AND l.agrmtno = s.agrmtno
								   AND l.itemid= s.itemid
								   AND l.stocklocn = s.stocklocn
    WHERE     o.acctno = @acctno
    AND       o.buffno = @buffno
	AND       o.originallocation = @locn
    --AND       o.itemid = @itemno
    AND       o.itemid = @itemID						--IP - 18/06/11 - CR1212 - RI - #4042

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO


