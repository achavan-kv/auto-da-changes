SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[DN_TermsTypeSummaryGetSP]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[DN_TermsTypeSummaryGetSP]
GO


-- =============================================
-- Modified By:	Jez Hemans
-- Modified On date: 26/07/2007
-- Description:	CR903 - Additional field 'StoreType' to be retrieved from termstypetable as well
-- =============================================



CREATE PROCEDURE 	dbo.DN_TermsTypeSummaryGetSP
			@countryCode varchar(2),
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	--
    -- DSR Now joins with TermsTypeAccountType to load a set of Terms Types
    -- for each account type. In .NET the list must be filtered by account type.
    -- A complete list is also loaded with a blank account type.
    --
	

SELECT	DISTINCT
			(tt.termstype + N' - ' + tt.description) as termstype,
			tt.termstype as termstypecode,
			tt.band,
			tt.Affinity,
			tt.IncludeWarranty,
			tt.paymentholidays,
			tt.delnonstocks,
			--tt.PClubTier1,
			--tt.PClubTier2,
			at.accounttype,
            t.StoreType,
            t.LoanNewCustomer,
            t.LoanRecentCustomer,
            t.LoanExistingCustomer,
            t.LoanStaff,
			t.isLoan,
			CONVERT(bit,t.isactive) AS isactive,
			t.instalpredel
	FROM	TermsTypeAllBands tt, TermsTypeAccountType at,TermsTypeTable t
	WHERE	tt.termstype != N''
    AND at.termstype = tt.termstype
    AND t.termstype = tt.termstype
    AND t.countrycode = @countryCode
	UNION
	SELECT	DISTINCT
			(tt.termstype + N' - ' + tt.description) as termstype,
			tt.termstype as termstypecode,
			tt.band,
			tt.Affinity,
			tt.IncludeWarranty,
			tt.paymentholidays,
			tt.delnonstocks,
			--tt.PClubTier1,
			--tt.PClubTier2,
			'' AS accounttype,
            t.StoreType,
            t.LoanNewCustomer,
            t.LoanRecentCustomer,
            t.LoanExistingCustomer,
            t.LoanStaff,
			t.isLoan,
			CONVERT(bit,t.isactive) AS isactive,
			t.instalpredel
	FROM	TermsTypeAllBands tt INNER JOIN TermsTypeTable t ON tt.termstype = t.termstype
	WHERE	tt.termstype != N''
	AND t.countrycode = @countryCode
	UNION
	SELECT (tt.termstype + N' - ' + tt.description),
      '' AS termstypecode,
			i.Band AS band,
			'' AS affinity,
			'' AS includewarranty,
			'' AS paymentholidays,
			'' AS delnostocks,
			--tt.PClubTier1,
			--tt.PClubTier2,
			'' AS accounttype,
            '' AS storetype,
            '' AS loanNewCustomer,
            '' AS loanRecentCustomer,
            '' AS loanExistingCustomer,
            '' AS loanStaff,
			'' AS isloan,
			CONVERT(BIT,0) AS isactive,
			tt.instalpredel
	FROM TermsTypeTable  tt
	INNER JOIN intratehistory i ON tt.termstype = i.termstype
	

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO


