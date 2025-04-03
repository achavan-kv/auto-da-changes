
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID('[dbo].[CM_DeleteExistingStrategySP]') AND OBJECTPROPERTY(id,'IsProcedure') = 1)
DROP PROCEDURE [dbo].[CM_DeleteExistingStrategySP]
GO
-- ============================================================================================
-- Author:		Ilyas Parker
-- Create date: 22/09/2008
-- Description:	Permanently deletes the strategy, its conditions and actions from the database
--				and updates the 'CMStrategyAcct' table 'dateto' to move the account out of
--				the strategy being deleted.	
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 03/02/12 jec #9521 CR9417 - duplication of existing strategies			
-- ============================================================================================
CREATE PROCEDURE [dbo].[CM_DeleteExistingStrategySP] 
	            @strategy varchar(7),		-- #9521
				@return	int	OUTPUT
as
begin
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	set nocount on;

    set @return = 0    --initialise return code

    delete 
		    from cmstrategycondition
		    where strategy = @strategy

	delete 
		from cmstrategy 	
		where strategy = @strategy
	
	delete
		from cmstrategyactions
		where strategy = @strategy

	delete 
		from code
		where category = 'SS1'
		and code = @strategy
		
	-- delete if storecard  
	delete 
		from code
		where category = 'SS2'			-- #9521
		and code = @strategy

	--Update the accounts that are currently in the strategy that is being deleted 
	--to longer be in that strategy by setting the 'DateTo' as we no longer want
	--them to be worked on.
	update
		cmstrategyacct
		set dateto = getdate()
		where strategy = @strategy
		and dateto is null

    if (@@error != 0)
    begin
        set @return = @@error
    end
end

GO

-- End End End End End End End End End End End End End End End End End End End End End End End End End End End End