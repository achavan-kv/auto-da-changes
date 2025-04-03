
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID('[dbo].[GetSMSForAWorklistAccountSP]') AND OBJECTPROPERTY(id,'IsProcedure') = 1)
DROP PROCEDURE [dbo].[GetSMSForAWorklistAccountSP]
GO
-- =============================================
-- Author:		Jez Hemans
-- Create date: 01/06/2007
-- Description:	Returns all the SMS's for a particular account
-- =============================================
CREATE PROCEDURE GetSMSForAWorklistAccountSP
	@acctno varchar(12),
	@return int OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    SET 	@return = 0			--initialise return code

-- FA - UAT 896 Added description field and link to CMSMS table
    SELECT s.code, c.description, CONVERT(DATETIME,CONVERT(VARCHAR(10),dateadded,103),103) AS dateadded
    FROM SMS S INNER JOIN cmsms C ON s.code = c.SMSName
    WHERE s.acctno = @acctno
    ORDER BY dateadded DESC

	SET	@return = @@error
END
GO
