
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS OFF
GO

if exists (select * from dbo.sysobjects
where id = object_id('[dbo].[DN_AuditDiscountSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_AuditDiscountSP]
GO

CREATE PROCEDURE  dbo.DN_AuditDiscountSP
    @AcctNo          char(12),
    @AgrmtNo         int,
    @DiscountItemNo  varchar(8),
    @ParentItemNo    varchar(8),
    @StockLocn       smallint,
    @Amount          money,
    @SalesPerson     int,
    @AuthorisedBy    int,
    @Return          int OUTPUT

AS

BEGIN
    SET @return = 0

    -- Audit the authorised discount
    INSERT INTO DiscountsAuthorised
        (AcctNo, AgrmtNo, DiscountItemNo, ParentItemNo,
         StockLocn, Amount, SalesPerson, DateAuthorised, AuthorisedBy)
    VALUES
        (@AcctNo, @AgrmtNo, @DiscountItemNo, @ParentItemNo,
         @StockLocn, @Amount, @SalesPerson, GETDATE(), @AuthorisedBy)

    SET @return = @@error
END
GO
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO

