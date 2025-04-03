
IF EXISTS (	SELECT	1 
			FROM	dbo.sysobjects
			WHERE	id = object_id('[dbo].[DN_TermsTypeLoadDetailsSP]') 
					AND OBJECTPROPERTY(id, 'IsProcedure') = 1)
BEGIN
	DROP PROCEDURE [dbo].[DN_TermsTypeLoadDetailsSP]
END
GO


SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO

-- =============================================
-- Modified By:	Jez Hemans
-- Modified On date: 26/07/2007
-- Description:	CR903 - Additional field 'StoreType' to be retrieved from termstypetable as well
-- =============================================

CREATE PROCEDURE [dbo].[DN_TermsTypeLoadDetailsSP]
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : DN_TermsTypeLoadDetailsSP.prc
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Load TermsType Details
-- Author       : ??
-- Date         : ??
--
-- This procedure will load the basic TermsType Details
-- 
-- Change Control
-- --------------
-- Date			By		Description
-- ----			--		-----------
-- 05/09/07		jec		CR906 changes
-- 07 July 20	Amit	Added new columns for MMI feature
-- ================================================
	-- Add the parameters for the stored procedure here
    @termstype varchar(2),
    @return int OUTPUT

AS

    SET @return = 0            --initialise return code
	-- Table 1 - Termstype
    SELECT  origbr,
            countrycode,
            termstype,
            description,
            mthsintfree,
            depositpaid,
            instalpredel,
            dtnetfirstin,
            affinity,
            minterm,
            maxterm,
            agrtext,
            agrtextx,
            agrtexty,
            noarrearsletters,
            defaultdeposit,
            defaultterm,
            apr,
            CashBackMonth,
            CashBackPc,
            CashBackAmount,
            AgreementPrint,
            DeferredMonths ,--as DeferredMonths,
            FullRebateDays,
            STCPc,
            STCAmount,
            isactive,
            hasfreeinstalment,
            ISNULL(DepositIsPercentage,0) AS DepositIsPercentage,
            paymentholidays,
			allow as avvtt,
			delnonstocks,
			DoNotSecuritise,
			StoreType,
            IsLoan,
			LoanNewCustomer,			-- CR906
            LoanRecentCustomer,
            LoanExistingCustomer,
            LoanStaff,
			IsMmiActive,
			MmiThresholdPercentage
			--PClubTier1,
			--PClubTier2
    FROM    termstypetable,avvtt
    WHERE   termstype = @termstype
	-- Table 2 - TermsTypeAccountType
    SELECT  TT.termstype,
            TT.accounttype,
            AT.description
    FROM    termstypeaccounttype TT INNER JOIN accttype AT
    ON      TT.accounttype = AT.accttype
    WHERE   termstype = @termstype
    ORDER BY AT.description ASC
	-- Table 3 - IntRateHistory
    SELECT  termstype,
            datefrom,
            dateto,
            intrate,
            inspcent,
            AdminPcent,
            InsIncluded,
            CASE InsIncluded
				WHEN 1 THEN 'Yes'
				WHEN 0 THEN 'No'
			END AS InsIncludedText,
            IncludeWarranty,      
            CASE IncludeWarranty
				WHEN 1 THEN 'Yes'
				WHEN 0 THEN 'No'
			END AS IncWarrantyText,
            ISNULL(ratetype,-1) AS ratetype,
            codedescript AS ratetypedesc,
            paymentholidaymin,
            paymentholidayarrears,
            Band,
            PointsFrom,
            PointsTo,
            AdminValue
    FROM    intratehistory
            LEFT OUTER JOIN code
            ON ratetype = code
               and category = 'IRT'
    WHERE   termstype = @termstype
    ORDER BY datefrom DESC, band ASC
	-- Table 4 - TermsTypeLength
    SELECT  termstype,
            length
    FROM    TermsTypeLength
    WHERE   termstype = @termstype
    ORDER BY length ASC
	-- Table 5 - TermsTypeFreeInstallments
    SELECT  termstype,
            intratefrom,
            intrateto,
            datefrom,
            dateto,
            month
    FROM    TermsTypeFreeInstallments
    WHERE   termstype = @termstype
    ORDER BY datefrom DESC
	-- Table 6 - TermsTypeVariableRates
    SELECT  termstype,
            intratefrom,
            intrateto,
            frommonth,
            tomonth,
            rate
    FROM    TermsTypeVariableRates
    WHERE   termstype = @termstype
    ORDER BY frommonth ASC


    IF (@@error != 0)
    BEGIN
        SET @return = @@error
    END
