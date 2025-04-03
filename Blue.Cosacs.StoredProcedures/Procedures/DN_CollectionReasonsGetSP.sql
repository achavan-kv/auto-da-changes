GO
/****** Object:  StoredProcedure [dbo].[DN_CollectionReasonsGetSP]    Script Date: 10/02/2006 16:50:11 ******/
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID('[dbo].[DN_CollectionReasonsGetSP]') AND OBJECTPROPERTY(id,'IsProcedure') = 1)
DROP PROCEDURE [dbo].[DN_CollectionReasonsGetSP]


SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Peter Chong
-- Create date: 02-10-2006
-- Description:	CR 822 - Get a list of warranty collection reasons for the account
-- =============================================
CREATE PROCEDURE DN_CollectionReasonsGetSP 
(
	@acctNo varchar(12) ,
	@return int output
)	
	
AS
BEGIN

	SET NOCOUNT ON;

	SELECT [AcctNo]
      ,[ItemNo]
      ,[CollectionReason]
      ,[DateAuthorised]
      ,[EmpeenoAuthorised]
      ,[DateCommissionCalculated]
	FROM [CollectionReason]
	WHERE [AcctNo] = @acctNo

	SET @return =  @@ERROR
END


GO

