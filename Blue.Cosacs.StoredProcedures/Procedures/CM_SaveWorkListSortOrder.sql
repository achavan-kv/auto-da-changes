SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[CM_SaveWorkListSortOrder]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[CM_SaveWorkListSortOrder]
GO

-- =============================================
-- Author:		Mohamed Nasmi
-- Create date: 22/01/2009
-- Description:	Saves Work List Sort Order
-- =============================================

CREATE PROCEDURE 	[dbo].[CM_SaveWorkListSortOrder]
			@EmpeeType varchar(3),
			@SortColumnName varchar(32),
			@SortOrder smallint,
			@AscDesc varchar(4),
			@return int OUTPUT
AS
	-- SET NOCOUNT ON added to marginally improve performance 
	SET NOCOUNT ON;
    SET @return = 0
	
    INSERT INTO CMWorkListSortOrder (
		EmpeeType,
		SortColumnName,
		SortOrder,
		AscDesc
	) VALUES ( 
		@EmpeeType,
		@SortColumnName,
		@SortOrder,
		@AscDesc
	) 

	    SET @return = @@ERROR

	RETURN @return

GO
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO