SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[CM_GetWorkListRightsHierarchy]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[CM_GetWorkListRightsHierarchy]
GO

-- =============================================
-- Author:		Mohamed Nasmi
-- Create date: 18/02/2009
-- =============================================
CREATE PROCEDURE [dbo].[CM_GetWorkListRightsHierarchy] 
		@return int output
AS  
	SET @return = 0    --initialise return code	
	
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

--//-------------------------------------------------
	SELECT 
	cast(WLR.EmpeeNo as varchar) AS 'Emp_or_Type', WLR.worklist, WL.Description ,WLR.EmpeeNo, EMP.FullName as 'Name'
	FROM cmworklistrights WLR
	INNER JOIN CMWorkList WL on WL.WorkList = WLR.WorkList
	INNER JOIN Admin.[User] EMP on EMP.id = WLR.EmpeeNo
	WHERE WLR.EmpeeNo != 0

--//-------------------------------------------------


	SET @return = @@error

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO