
/****** Object:  StoredProcedure [dbo].[FinTransCheckForCommittedDataSP]    Script Date: 02/26/2008 16:07:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID('[dbo].[FinTransCheckForCommittedDataSP]') AND OBJECTPROPERTY(id,'IsProcedure') = 1)
DROP PROCEDURE [dbo].[FinTransCheckForCommittedDataSP]
GO
-- =============================================
-- Author:		Jez Hemans
-- Create date: 26/02/2008
-- Description:	Checks to see whether or not payment transaction has been committed to the fintrans table
-- =============================================
CREATE PROCEDURE [dbo].[FinTransCheckForCommittedDataSP]
	@acctno VARCHAR(12),
	@transrefno INT,
	@datetrans DATETIME
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT 1 FROM fintrans WHERE acctno = @acctno AND transrefno = @transrefno AND CONVERT(SMALLDATETIME, CONVERT(VARCHAR(10), datetrans, 105), 105) = @datetrans
END
