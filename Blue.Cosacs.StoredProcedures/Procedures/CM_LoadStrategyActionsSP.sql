
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID('[dbo].[CM_LoadStrategyActionsSP]') AND OBJECTPROPERTY(id,'IsProcedure') = 1)
DROP PROCEDURE [dbo].[CM_LoadStrategyActionsSP]
GO
-- =============================================
-- Author:		Jez Hemans
-- Create date: 21/03/2007
-- Description:	Returns all the actions associated with the passed strategy
-- Change Control
-- --------------
-- =============================================
CREATE PROCEDURE [dbo].[CM_LoadStrategyActionsSP] 
	@strategy varCHAR(7),		-- #9521	
    @return INT OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
    SET @return = 0
    -- Insert statements for procedure here
	SELECT Strategy, ActionCode, Description AS 'Action',CONVERT(bit,0) as 'AllowReuse' FROM dbo.CMStrategyActions
    WHERE Strategy = @strategy		

     SET @return = @@error
END
GO

-- End End End End End End End End End End End End End End End End End End End End End End End End End End End End