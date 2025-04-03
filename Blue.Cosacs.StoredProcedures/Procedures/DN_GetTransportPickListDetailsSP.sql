SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_GetTransportPickListDetailsSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_GetTransportPickListDetailsSP]
GO

CREATE PROCEDURE dbo.DN_GetTransportPickListDetailsSP
				@branch smallint,
				@transpicklistno int,
				@Return int OUTPUT

AS
BEGIN

    SET @Return = 0;

    SELECT DISTINCT ISNULL(s.BuffBranchNo,0) AS BuffBranchNo,
					ISNULL(s.BuffNo,0) AS BuffNo,
					ISNULL(s.AcctNo,'') AS AcctNo,
					ISNULL(s.AgrmtNo,0) AS AgrmtNo,
					ISNULL(SI.IUPC,'') AS ItemNo,
					ISNULL(s.Quantity,0) AS Quantity,
					ISNULL(s.StockLocn,0) AS StockLocn,
					ISNULL(s.DateDelPlan,'') AS DateDelPlan,
					ISNULL(s.LoadNo,0) AS LoadNo,
					ISNULL(s.picklistnumber,'') AS picklistnumber,
					ISNULL(l.delnotebranch,0) AS DelNoteBranch,
					ISNULL(s.ItemID, 0) AS ItemId
    FROM	Schedule s, lineitem l, dbo.StockInfo SI
	WHERE	s.transchedno = @transpicklistno
    AND		s.transchednobranch = @branch
    AND		s.acctno = l.acctno
    AND		s.ItemID = l.ItemID
    AND		l.stocklocn = s.stocklocn
    AND		s.ItemID = SI.ID
    AND		EXISTS(	SELECT  1 
					FROM	deliveryload d, transptsched t
					WHERE	s.buffno = d.buffno
					AND		s.buffbranchno = d.buffbranchno
					AND		s.loadno = d.loadno
					AND		d.branchno = t.branchno
					AND		d.datedel = t.datedel
					AND		d.loadno = t.loadno)	


    SET @Return = @@ERROR
    
END

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO
