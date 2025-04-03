SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID('[dbo].[CM_GetStrategiesToSendToSP]') AND OBJECTPROPERTY(id,'IsProcedure') = 1)
DROP PROCEDURE [dbo].[CM_GetStrategiesToSendToSP]
GO
-- ============================================================================================
-- Author:		Ilyas Parker
-- Create date: 20/10/2008
-- Description:	This procedure will return all the strategies that are the strategies
--				defined as the 'exit' strategies for the strategy selected.
-- Change Control
-- --------------
-- 03/02/12 jec #9521 CR9417 - duplication of existing strategies		
-- ============================================================================================
CREATE PROCEDURE [dbo].[CM_GetStrategiesToSendToSP] 
	            @strategy varchar(7),		-- #9521
				@return	int	OUTPUT
as
begin
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	set nocount on;

    set @return = 0    --initialise return code


	select distinct cmc.actioncode as 'Strategy', cmc.actioncode + ' ' + c.description as 'Description'
	from cmstrategy c inner join cmstrategycondition cmc on
		cmc.actioncode = c.strategy
	where cmc.savedtype = 'X'
	and cmc.strategy = @strategy



    if (@@error != 0)
    begin
        set @return = @@error
    end
end

GO

-- End End End End End End End End End End End End End End End End End End End End End End End End End End End End