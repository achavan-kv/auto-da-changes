
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID('[dbo].[GetStrategiesForAWorklistAccountSP]') AND OBJECTPROPERTY(id,'IsProcedure') = 1)
DROP PROCEDURE [dbo].[GetStrategiesForAWorklistAccountSP]
GO
-- =============================================
-- Author:		Jez Hemans
-- Create date: 01/06/2007
-- Description:	Returns all the strategies for a particular account
-- =============================================
CREATE PROCEDURE GetStrategiesForAWorklistAccountSP
	@acctno varchar(12),
	@return int OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    SET 	@return = 0			--initialise return code

	SELECT S.Strategy
      ,Description 
      ,CONVERT(DATETIME,CONVERT(VARCHAR(10),datefrom,103),103) AS datefrom
      ,CONVERT(DATETIME,CONVERT(VARCHAR(10),dateto,103),103) AS dateto
    FROM dbo.CMStrategy S INNER JOIN [CMStrategyAcct] A ON S.[Strategy] = A.[Strategy]
    WHERE [acctno] = @acctno
    ORDER BY datefrom Desc

	SET	@return = @@error
END
GO
