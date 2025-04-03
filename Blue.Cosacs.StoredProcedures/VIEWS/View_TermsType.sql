/* KEF Amended termstype view so it includes a row for every termstype in Intratehistory */
/* KEF 24/08/04 Taking DtNetFirstIn from Intratehistory and not Termstypetable
AA 26/08/04 adding payment holidays */
-- DSR 22 Jun 2005 - Additional columns
-- DSR 21 Mar 2006 - Additional Privilege Club columns
-- DSR 26 May 2006 - Additional Terms Type Band columns
-- DSR 19 Jun 2006 - New view to return all bands - original view to return Band A only


IF EXISTS (SELECT * FROM dbo.sysobjects
           WHERE ID = object_id('TermsType')
           AND OBJECTPROPERTY(id, 'IsView') = 1)
DROP VIEW TermsType
GO

SET QUOTED_IDENTIFIER OFF
SET ANSI_NULLS ON
GO

CREATE VIEW TermsType AS
    SELECT  * FROM TermsTypeAllBands
    WHERE   Band = 'A' OR Band = ''

GO

SET QUOTED_IDENTIFIER OFF
SET ANSI_NULLS ON
GO

GRANT SELECT on TermsType to PUBLIC
GO

