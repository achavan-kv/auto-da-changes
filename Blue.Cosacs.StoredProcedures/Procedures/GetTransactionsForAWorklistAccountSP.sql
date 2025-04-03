
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID('[dbo].[GetTransactionsForAWorklistAccountSP]') AND OBJECTPROPERTY(id,'IsProcedure') = 1)
DROP PROCEDURE [dbo].[GetTransactionsForAWorklistAccountSP]
GO
-- =============================================
-- Author:		Jez Hemans
-- Create date: 01/06/2007
-- Description:	Returns all the transactions for a particular account
-- =============================================
CREATE PROCEDURE GetTransactionsForAWorklistAccountSP
	@acctno varchar(12),
	@return int OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    SET 	@return = 0			--initialise return code

	SELECT	transtypecode,transvalue,CONVERT(DATETIME,CONVERT(VARCHAR(10),datetrans,103),103) AS DateTrans
	FROM    dbo.fintrans
	WHERE	AcctNo    = @acctno
    	ORDER BY datetrans DESC

	SET	@return = @@error
END
GO
