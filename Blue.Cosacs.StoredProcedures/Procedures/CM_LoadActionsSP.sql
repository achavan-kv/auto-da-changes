
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID('[dbo].[CM_LoadActionsSP]') AND OBJECTPROPERTY(id,'IsProcedure') = 1)
DROP PROCEDURE [dbo].[CM_LoadActionsSP]
GO
-- =============================================
-- Author:		Jez Hemans
-- Create date: 21/03/2007
-- Description:	Returns all the actions from the code table with a category of FUA.
-- =============================================
CREATE PROCEDURE [dbo].[CM_LoadActionsSP]
	@return INT OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    SET @return = 0
    
    SELECT code AS 'ActionCode',codedescript AS 'Action',CONVERT(bit,0) as 'AllowReuse' FROM dbo.code WHERE category = 'FUA'

    SET @return = @@error
END
GO

