
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID('[dbo].[GetLettersForAWorklistAccountSP]') AND OBJECTPROPERTY(id,'IsProcedure') = 1)
DROP PROCEDURE [dbo].[GetLettersForAWorklistAccountSP]
GO
-- =============================================
-- Author:		Jez Hemans
-- Create date: 01/06/2007
-- Description:	Returns all the letters for a particular account
-- =============================================
CREATE PROCEDURE GetLettersForAWorklistAccountSP
	@acctno varchar(12),
	@return int OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    SET 	@return = 0			--initialise return code

-- FA  - 17/11/09 - UAT 867 Added code column
    SELECT code, codedescript, CONVERT(DATETIME,CONVERT(VARCHAR(10),dateacctlttr,103),103) AS dateacctlttr
    FROM letter L INNER JOIN [code] C ON L.[lettercode] = C.[code]
    WHERE [acctno] = @acctno
            AND [category] like 'LT%' -- UAT(5.2) - 778
    ORDER BY dateacctlttr DESC

	SET	@return = @@error
END
GO
