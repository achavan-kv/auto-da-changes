/* KEF Amended termstype view so it includes a row for every termstype in Intratehistory */
/* KEF 24/08/04 Taking DtNetFirstIn from Intratehistory and not Termstypetable
AA 26/08/04 adding payment holidays */
-- DSR 22 Jun 2005 - Additional columns
-- DSR 21 Mar 2006 - Additional Privilege Club columns
-- DSR 26 May 2006 - Additional Terms Type Band columns
-- DSR 19 Jun 2006 - New view to return all bands - original view to return Band A only
-- aa  7 Nov 2007 - putting in termstype view here to prevent errors occuring through incorrect order update
-- Amit 26 Aug 2020 - Added new column IsMmiActive,MmiThresholdPercentage
IF EXISTS (SELECT * FROM dbo.sysobjects
           WHERE ID = object_id('TermsTypeAllBands')
           AND OBJECTPROPERTY(id, 'IsView') = 1) 
DROP VIEW TermsTypeAllBands
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER OFF
GO


CREATE VIEW [dbo].[TermsTypeAllBands] AS
    SELECT  t.OrigBr,
            t.CountryCode,
            t.TermsType,
            t.Description,
            t.MthsIntFree,
            t.DepositPaid,
            t.Instalpredel,
            t.Affinity,
            t.MinTerm,
            t.MaxTerm,
            t.AgrText,
            t.AgrTextX,
            t.AgrTextY,
            t.NoArrearsLetters,
            t.DefaultDeposit,
            t.DefaultTerm,
            t.APR,
            t.IsActive,
            t.HasFreeInstalment,
            t.paymentholidays,
            t.delnonstocks,
            i.DateFrom,
            i.DateTo,
            i.IntRate AS ServPcent,
            i.InsPcent,
            i.AdminPcent,
            i.InsIncluded,
            i.IncludeWarranty,
            i.PaymentHolidayMin,
            i.DtNetFirstIn,
            t.CashBackMonth,
            t.CashBackPc,
            t.CashBackAmount,
            t.AgreementPrint,
            ISNULL(c.CodeDescript,'Agreement.xslt') as AgreementPrintDesc,
            t.DeferredMonths,
            t.FullRebateDays,
            t.STCPc,
            t.STCAmount,
            --t.PClubTier1,
            --t.PClubTier2,
            i.Band,
            i.PointsFrom,
            i.PointsTo,
			t.DoNotSecuritise,
            t.isloan,
            t.LoanNewCustomer,
            t.LoanRecentCustomer,
            t.LoanExistingCustomer,
            t.LoanStaff,
			t.IsMmiActive,
            t.MmiThresholdPercentage
    FROM   TermsTypeTable t
           LEFT OUTER JOIN Code c 
               ON c.code = t.AgreementPrint and c.category = 'AGT',
           IntRateHistory i, CountryMaintenance cm
    WHERE  i.TermsType = t.TermsType
    AND    (DATEDIFF(Day, GETDATE(), i.DateFrom) <= 0
    AND    i.DateTo = CONVERT(DATETIME, '01-01-1900', 105))
	--AND	   t.isAmortized = cm.Value
	--AND	   cm.CodeName='CL_Amortized'
    UNION
    SELECT  t.OrigBr,
            t.CountryCode,
            t.TermsType,
            t.Description,
            t.MthsIntFree,
            t.DepositPaid,
            t.Instalpredel,
            t.Affinity,
            t.MinTerm,
            t.MaxTerm,
            t.AgrText,
            t.AgrTextX,
            t.AgrTextY,
            t.NoArrearsLetters,
            t.DefaultDeposit,
            t.DefaultTerm,
            t.APR,
            t.IsActive,
            t.HasFreeInstalment,
            t.paymentholidays,
            t.delnonstocks,
            i.DateFrom,
            i.DateTo,
            i.IntRate AS ServPcent,
            i.InsPcent,
            i.AdminPcent,
            i.InsIncluded,
            i.IncludeWarranty,
            i.PaymentHolidayMin,
            i.DtNetFirstIn,
            t.CashBackMonth,
            t.CashBackPc,
            t.CashBackAmount,
            t.AgreementPrint,
            ISNULL(c.CodeDescript,'Agreement.xslt') as AgreementPrintDesc,
            t.DeferredMonths,
            t.FullRebateDays,
            t.STCPc,
            t.STCAmount,
            --t.PClubTier1,
            --t.PClubTier2,
            i.Band,
            i.PointsFrom,
            i.PointsTo,
			t.DoNotSecuritise,
            t.isloan,
            t.LoanNewCustomer,
            t.LoanRecentCustomer,
            t.LoanExistingCustomer,
            t.LoanStaff,
			t.IsMmiActive,
            t.MmiThresholdPercentage
    FROM   TermsTypeTable t
           LEFT OUTER JOIN Code c 
               ON c.code = t.AgreementPrint and c.category = 'AGT',
           IntRateHistory i, CountryMaintenance cm
    WHERE  i.TermsType = t.TermsType
    AND    (DATEDIFF(Day, GETDATE(), i.DateFrom) <= 0
    AND    DATEDIFF(Day, GETDATE(), i.DateTo) >= 0 --aa change --dsr added =
    AND    i.DateTo <> CONVERT(DATETIME, '01-01-1900', 105))
    AND    datefrom = (select max(datefrom)
                       from intratehistory i2
                       where i.termstype = i2.termstype and i2.datefrom <= getdate()
                       group by i2.termstype)
	--AND	   t.isAmortized = cm.Value
	--AND	   cm.CodeName='CL_Amortized'
GO

SET QUOTED_IDENTIFIER OFF
SET ANSI_NULLS ON
GO

GRANT SELECT on TermsTypeAllBands to PUBLIC
GO

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
