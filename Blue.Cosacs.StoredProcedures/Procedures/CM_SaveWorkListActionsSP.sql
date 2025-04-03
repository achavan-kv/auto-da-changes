
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[CM_SaveWorkListActionsSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[CM_SaveWorkListActionsSP]
GO


CREATE PROCEDURE  dbo.CM_SaveWorkListActionsSP
				@worklist varchar(10),
				@action varchar(20),
				@effect bit,
				@return	int	OUTPUT

AS

    SET @return = 0    --initialise return code

	INSERT INTO CMWorkListActions(WorkList, Action, WorkListEffect)
	VALUES (@worklist, @action, @effect)

    IF (@@error != 0)
    BEGIN
        SET @return = @@error
    END
GO
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO

