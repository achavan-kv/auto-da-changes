SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_GetPickListScheduleSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_GetPickListScheduleSP]
GO

CREATE PROCEDURE DN_GetPickListScheduleSP

    -- Parameters
    @piFilter		    INTEGER,
    @piPickListNo		INTEGER,
    @piBuffNo           INTEGER,
    @Return             INTEGER OUTPUT

AS
BEGIN

    SET NOCOUNT ON

    SET @Return = 0;
    DECLARE @maxPicklist int 
    DECLARE @picklistbranchnumber int 

    -- Load Schedules (Delivery Notes)
    IF @piPickListNo = 0 
    BEGIN
       SELECT  top 1 @maxPicklist = picklistnumber, @picklistbranchnumber = picklistbranchnumber
         FROM  schedule
        WHERE  stocklocn = @piFilter
          AND  BuffNo = @piBuffNo
     ORDER BY  picklistnumber desc
    END 
    ELSE 
    BEGIN 
        SET @maxPicklist =  @piPickListNo
        SET @picklistbranchnumber = @piFilter
    END 
    IF @maxPicklist = 0
    BEGIN
        SET @maxPicklist = -1
    END 

    SELECT DISTINCT ISNULL(s.BuffBranchNo,0) AS BuffBranchNo,
           ISNULL(l.DelNoteBranch,0) AS DelNoteBranch,
           ISNULL(s.BuffNo,0) AS BuffNo,
           ISNULL(s.AcctNo,'') AS AcctNo,
           ISNULL(s.AgrmtNo,0) AS AgrmtNo,
           ISNULL(SI.IUPC,'') AS ItemNo,
           ISNULL(s.Quantity,0) AS Quantity,
           ISNULL(s.StockLocn,0) AS StockLocn,
           ISNULL(s.DateDelPlan,'') AS DateDelPlan,
           ISNULL(s.LoadNo,0) AS LoadNo,
           ISNULL(s.picklistnumber,'') AS picklistnumber,
           ISNULL(s.ItemID, 0) AS ItemId
    FROM   Schedule s, lineitem l, dbo.StockInfo SI
    WHERE  s.picklistnumber = @maxPicklist
    AND    s.picklistbranchnumber = @picklistbranchnumber
    AND    s.acctno = l.acctno
    AND    s.ItemID = l.ItemID
    AND	   s.stocklocn = l.stocklocn
    AND	   s.agrmtno = l.agrmtno
    AND	   s.ItemID = SI.ID

    SET @Return = @@ERROR
    
END

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO
