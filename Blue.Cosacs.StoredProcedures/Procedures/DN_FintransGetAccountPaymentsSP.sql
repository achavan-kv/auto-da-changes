SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_FintransGetAccountPaymentsSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_FintransGetAccountPaymentsSP]
GO

CREATE PROCEDURE DN_FintransGetAccountPaymentsSP

    -- Parameters
    @acctno varchar(12),
    @return int OUTPUT

AS

    SET @return = 0

    -- Return all PAY and DPY (BDW) transactions for the account, 
    -- with an indicator for corrections already done
    -- RD 18/11/04 Added codes for Giro to allow correction FR66185
    -- RD 24/10/05 67592 Added link to fintransaccount AND FT.acctno = cp.acctno 
    -- to ensure that the correct transactions is maked as corrected
    SELECT  FT.transrefno,
            FT.datetrans,
            FT.transtypecode,
            C.code,
            C.codedescript,
            -(FT.transvalue) as transvalue,
            FT.chequeno,
            FT.bankacctno,
            FT.bankcode,
            FT.BranchNo,
            CASE ISNULL(cp.paymentref,'') WHEN '' THEN '0' ELSE '1' END AS Corrected,
            x.storecardno AS CardNumber
    FROM    code C, fintrans FT
    LEFT OUTER JOIN CorrectedPayments cp ON cp.paymentref = FT.transrefno AND FT.acctno = cp.acctno 
    LEFT JOIN finxfr x ON fT.acctno = x.acctno and fT.transrefno = x.transrefno  AND fT.datetrans = x.datetrans 
    WHERE   FT.acctno         = @acctno
    AND     FT.transtypecode  IN ('PAY','DPY', 'DDN', 'DDE', 'DDR', 'DDG','SCT')
    AND	    FT.transvalue     < 0	-- RD 18/11/04 Added to ensure that only credit transactions are corrected
    AND     C.Category        = 'FPM'
    AND     C.Code            = FT.PayMethod            

    SET @return = @@error

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO
