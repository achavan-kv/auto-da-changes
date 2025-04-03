SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID('[dbo].[GetWorklistsForAWorklistAccountSP]') AND OBJECTPROPERTY(id,'IsProcedure') = 1)
DROP PROCEDURE [dbo].[GetWorklistsForAWorklistAccountSP]
GO

-- =============================================
-- Author:		Jez Hemans
-- Create date: 01/06/2007
-- Description:	Returns all the worklists for a particular account
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 16/11/09 jec  UAT794 Duplicate rows in the Telephone action screen.
-- =============================================
CREATE PROCEDURE [dbo].[GetWorklistsForAWorklistAccountSP]
	@acctno varchar(12),
	@return int OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    SET 	@return = 0			--initialise return code

	SELECT Distinct Strategy			-- UAT797 jec 16/11/09
          ,W.Worklist
          ,Description 
          ,CONVERT(DATETIME,CONVERT(VARCHAR(10),datefrom,103),103) AS datefrom
          ,CONVERT(DATETIME,CONVERT(VARCHAR(10),dateto,103),103) AS dateto
    FROM CMWorklistsAcct A INNER JOIN CMWorkList W ON A.[Worklist] = W.[WorkList]
    WHERE [acctno] = @acctno
    ORDER BY datefrom DESC

	SET	@return = @@error
END

-- End End End End End End End End End End End End End End End End End End End End

