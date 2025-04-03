
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID('[dbo].[CM_SaveStrategyActionsSP]') AND OBJECTPROPERTY(id,'IsProcedure') = 1)
DROP PROCEDURE [dbo].[CM_SaveStrategyActionsSP]
GO
-- =============================================
-- Author:		Jez Hemans
-- Create date: 22/03/2007
-- Description:	Saves new action detalis to the table CMStrategyActions for the passed strategy
-- Change Control
-- --------------
-- 03/02/12 jec #9521 CR9417 - duplication of existing strategies	
-- =============================================
CREATE PROCEDURE [dbo].[CM_SaveStrategyActionsSP]
	@strategy varchar(7),		-- #9521
    @actioncode VARCHAR(12),
    @action     VARCHAR(64),
    @empeeno	INT,		--UAT987
    @return     INT OUTPUT
AS
BEGIN
    SET @return = 0
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    INSERT INTO CMStrategyActions
           (Strategy,ActionCode,Description)
     VALUES
           (@strategy,@actioncode,@action)
    
    -- UAT987      
    insert into CMActionRights  
	select distinct empeeno,@strategy,@actioncode,@empeeno,0,0 
	from CMActionRights r
	where not exists(select * from CMActionRights r2 where r2.strategy=@strategy and r2.action=@actioncode)
	and r.strategy=@strategy

   SET @return = @@error
END
GO

-- End End End End End End End End End End End End End End End End End End End End End End End End End End End End