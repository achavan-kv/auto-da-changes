SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

if exists (select * from dbo.sysobjects
           where id = object_id('TermsTypeGetDetailsbyAcctno') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure TermsTypeGetDetailsbyAcctno
GO
CREATE PROCEDURE TermsTypeGetDetailsbyAcctno
    @AcctNo CHAR(12),
    @return int OUTPUT
AS

    SET @return = 0            --initialise return code
    
  SELECT DISTINCT ISNULL(tt.termstype + N' - ' + tt.DESCRIPTION,'Terms Type') AS termstype, 
         ltrim(rtrim(isnull(ip.ScoringBand, ''))) as ScoringBand,
         ISNULL(c.ScoreCardType,'A') AS scorecardtype,
         ISNULL(c.StoreType,'C') AS storetype,
         ISNULL(tt.isLoan,0) AS isloan,
         ISNULL(i.includewarranty,0) AS includewarranty,
         ISNULL(tt.PaymentHolidays,0) AS paymentholidays,
         CASE WHEN Affinity = 'Y' THEN CONVERT(BIT,1) ELSE CONVERT(BIT,0) END AS affinity
  FROM acct
  INNER JOIN intratehistory i ON acct.termstype = i.termstype
  INNER JOIN termstypetable tt ON i.termstype = tt.TermsType
  INNER JOIN custacct ca ON acct.acctno = ca.acctno
  INNER JOIN customer c ON ca.custid = c.custid
  INNER JOIN instalplan ip ON acct.acctno = ip.acctno
  LEFT OUTER JOIN revisedhist ON acct.acctno = revisedhist.acctno
  WHERE acct.acctno = @Acctno
  AND ISNULL(dateagrmtrevised,dateacctopen) BETWEEN i.datefrom AND CASE WHEN i.dateto < '1990-01-01' THEN '2050-01-01' ELSE i.dateto END
    
    SET @return = @@error
GO 

SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO

