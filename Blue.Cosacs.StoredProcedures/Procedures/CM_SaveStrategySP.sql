
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID('[dbo].[CM_SaveStrategySP]') AND OBJECTPROPERTY(id,'IsProcedure') = 1)
DROP PROCEDURE [dbo].[CM_SaveStrategySP]
GO
-- =============================================
-- Author:		Jez Hemans
-- Create date: 20/03/2007
-- Description:	Saves a new strategy to the database table CMStrategy

-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 11/09/09  jec UAT856 New Column and check for existing strategy
-- 03/02/12 jec #9521 CR9417 - duplication of existing strategies
-- =============================================
--exec CM_SaveStrategySP 'ABC','NewTest',0

CREATE PROCEDURE [dbo].[CM_SaveStrategySP] 
	@strategy varchar(7),		-- #9521
    @description VARCHAR(128),
    @manual SMALLINT,
    @return INT OUTPUT
AS
BEGIN
    SET @return = 0
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	IF NOT EXISTS(SELECT * FROM CMStrategy WHERE strategy=@strategy)
	Begin
		INSERT INTO CMStrategy(Strategy,Description,IsActive,ReadOnly,Manual)
		VALUES(@strategy,@description,1,0,@manual)
    
    End

 SET @return = @@error
END
GO

-- End End End End End End End End End End End End End End End End End End End End End End End End End End End End