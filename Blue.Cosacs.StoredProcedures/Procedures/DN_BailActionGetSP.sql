SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_BailActionGetSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_BailActionGetSP]
GO

CREATE PROCEDURE  dbo.DN_BailActionGetSP
    @acctno varchar(12),
    @return int OUTPUT

AS

    SET @return = 0            --initialise return code

    SELECT  ba.AcctNo,
            ba.AllocNo,
            ba.ActionNo,
            ba.EmpeeNo,
            ba.DateAdded,
            ba.AddedBy,
            ba.Code,
            ba.Notes,
            ba.ActionValue,
            isnull(convert(varchar,ba.DateDue,103),' ') as DateDue,
            ba.AmtCommPaidOn,
            ISNULL(c.Reference,0) AS CommissionDays
    FROM    BAILACTION ba
    LEFT OUTER JOIN Code c
    ON c.Category = 'FUP' and c.Code = ba.Code
    WHERE   ba.AcctNo = @acctno
    ORDER BY ba.DateAdded DESC

    SET @return = @@error
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

