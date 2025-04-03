SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[CM_GetFollupAllocForActionSheet]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[CM_GetFollupAllocForActionSheet]
GO

-- =============================================
-- Author:		Mohamed Nasmi
-- Create date: 16/03/2009
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[CM_GetFollupAllocForActionSheet] 
	@empeeNo int,
	@acctNo varchar(12),
	@return int output
AS
BEGIN
    SET @return = 0    --initialise return code

	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    Select * from follupalloc 
	where empeeno = @empeeNo and acctno = @acctNo and  datedealloc IS NULL
	order by DeadLinedate DESC

    IF (@@error <> 0)
    BEGIN
        SET @return = @@error
    END

END

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO
