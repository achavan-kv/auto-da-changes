SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_ScheduleGetRevisedDetailsSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_ScheduleGetRevisedDetailsSP]
GO


CREATE PROCEDURE 	dbo.DN_ScheduleGetRevisedDetailsSP
			@acctno varchar(12),
			@buffno int,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

    SELECT    st.IUPC as itemno,
			  o.itemid,
              o.originallocation,
              o.quantity,
			  o.delnotebranch,
              o.buffno,
              o.datereqdel,
              o.dateremoved,
              st.itemdescr1 + ' ' + st.itemdescr2 as itemdescr1,
              o.empeeno,
              a.empeenoauth,
			   --IP - 03/11/08 - UAT(352) - If 'R' then account changed in 'Revise' screen else if 'C' then changed in 'Change Order Details' screen.
              CASE o.type WHEN 'R' THEN 'Revised' WHEN 'C' THEN 'Changed' ELSE '' END AS type,
              ISNULL(s.buffno, 0) as newbuffno,
              a.agrmtno,
              o.stocklocn,
              CASE a.holdprop WHEN 'Y' THEN 'No' WHEN 'N' THEN 'Yes' ELSE '' END AS holdprop
	FROM	  order_removed o
			  INNER JOIN agreement a ON o.acctno = a.acctno
									 AND o.agrmtno = a.agrmtno
			  INNER JOIN stockinfo st ON o.itemID = st.ID
			  LEFT JOIN schedule s ON o.acctno = s.acctno
								   AND o.agrmtno = s.agrmtno
								   AND o.itemID = s.itemID
								   AND o.stocklocn = s.stocklocn
    WHERE     o.acctno = @acctno
    AND       o.buffno = @buffno
   	AND		  o.dateconfirmed IS NULL
 

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

