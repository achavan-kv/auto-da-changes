SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_DeliveryScheduleGetItemsSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_DeliveryScheduleGetItemsSP]
GO
CREATE PROCEDURE 	dbo.DN_DeliveryScheduleGetItemsSP
-- =============================================
-- Author:		?
-- Create date: ?
-- Description:	
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 21/09/11  IP  RI - #8222 - CR8201 - Delivery Schedule Printout - description needs to be: descr+brand+vendor style long
--				 Also display the Courts Code on the Delivery Schedule printout dependent on Country Parameter.
-- =============================================
			@buffbranchno smallint,
			@buffno int,
			@return int OUTPUT

AS

--IP - 21/09/11 - RI - #8222 - CR8201
Declare @dispCourtsCode bit

select @dispCourtsCode = value from countrymaintenance where codename = 'RIDispCourtsCode'


	SET 	@return = 0			--initialise return code

	SELECT	s.quantity,
			st.IUPC AS ItemNo,
			st.itemdescr1 + ' ' + st.itemdescr2 as description,
			l.price,
			s.ItemID,
			rtrim(ltrim(isnull(st.VendorLongStyle,''))) as Style,					--IP - 21/09/11 - RI - #8222 - CR8201
			rtrim(ltrim(isnull(st.Brand,''))) as Brand,								--IP - 21/09/11 - RI - #8222 - CR8201
			case when @dispCourtsCode = 0 then '' 
				else case when st.ItemNo!="" then "("+ st.ItemNo+")" 
				else '' end
					end as CourtsCode	--IP - 21/09/11 - RI - #8222 - CR8201
			
	FROM	schedule s, lineitem l, stockitem st
    WHERE	s.buffno = @buffno
    AND     @buffbranchno = (CASE WHEN ISNULL(s.retstocklocn,0) = 0 THEN s.stocklocn ELSE s.retstocklocn END)
	AND     l.acctno = s.acctno
    AND		l.agrmtno = s.agrmtno
	AND     l.ItemID = s.ItemID
    AND		l.StockLocn = s.stocklocn
	AND		l.ParentItemId = s.ParentItemID
	AND     st.stocklocn = s.stocklocn
    AND		st.ItemID = s.ItemID
	AND     l.iskit=0

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO



