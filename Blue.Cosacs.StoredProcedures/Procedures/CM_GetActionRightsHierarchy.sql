SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[CM_GetActionRightsHierarchy]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[CM_GetActionRightsHierarchy]
GO

-- =============================================
-- Author:		Mohamed Nasmi
-- Create date: 18/02/2009
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 04/06/09 jec Return strategy description instead of strategy code
-- 27/11/09 FA Added Branch column	
-- =============================================
CREATE PROCEDURE [dbo].[CM_GetActionRightsHierarchy] 
		@return int output
AS  
	SET @return = 0    --initialise return code	
	
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

--//-------------------------------------------------
	SELECT 
	cast(AR.EmpeeNo as varchar) AS 'Emp_or_Type', AR.Action, C.codedescript AS 'Description' ,  isnull(s.Description,'All') as Strategy, AR.EmpeeNo, EMP.UserId as 'Name', u.branchno AS 'Branch'
	FROM cmactionrights AR
	INNER JOIN Admin.[User] u ON ar.EmpeeNo = u.Id
	INNER JOIN code C on C.code = AR.Action and C.category = 'FUA'
	INNER JOIN courtsperson EMP on EMP.UserId = AR.EmpeeNo
	left outer JOIN CMStrategy s on AR.Strategy=s.Strategy		-- jec 04/06/09
	WHERE AR.EmpeeNo != 0


--//-------------------------------------------------


	SET @return = @@error

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

-- End End End End End End End End End End End End End End End End End End End End End End End End End End End End End