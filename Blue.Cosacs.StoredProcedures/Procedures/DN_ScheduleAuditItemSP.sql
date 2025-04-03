
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS OFF
GO

if exists (select * from dbo.sysobjects
where id = object_id('[dbo].[DN_ScheduleAuditItemSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_ScheduleAuditItemSP]
GO


CREATE PROCEDURE dbo.DN_ScheduleAuditItemSP
    @acctNo varchar(12),
    @agreementNo int,
    @itemId int,
    @location smallint,
    @buffBranch int,
    @buffNo int,
    @return int OUTPUT

AS

    SET @return = 0

    INSERT INTO ScheduleAudit
        (origbr, acctno, agrmtno, datedelplan, delorcoll, itemno, ItemID, stocklocn, quantity,
         retstocklocn, retitemno, retval, vanno, buffbranchno, buffno, loadno, dateprinted,
         printedby, Picklistnumber, undeliveredflag, datePicklistprinted, picklistbranchNumber)
    SELECT origbr, acctno, agrmtno, datedelplan, delorcoll, itemno, ItemID, stocklocn, quantity,
           retstocklocn, retitemno, retval, vanno, buffbranchno, buffno, loadno, dateprinted,
           printedby, Picklistnumber, undeliveredflag, datePicklistprinted, picklistbranchNumber
    FROM   Schedule
    WHERE  acctno       = @acctNo
    AND    agrmtno      = @agreementNo
    AND    ItemID       = @itemId
    AND    stocklocn    = @location
    --AND    buffbranchno = @buffBranch
    AND    buffno       = @buffno

    SET @return = @@error

GO
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO
