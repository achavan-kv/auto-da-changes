
/****** Object:  StoredProcedure [dbo].[DN_SRGetTechnicianZonesSP]    Script Date: 10/17/2006 14:23:33 ******/
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID('[dbo].[DN_SRGetTechnicianZonesSP]') AND OBJECTPROPERTY(id,'IsProcedure') = 1)
DROP PROCEDURE [dbo].[DN_SRGetTechnicianZonesSP]
GO 

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Peter Chong
-- Create date: 17-Oct-2006
-- Description:	Gets the zones that have been allocated to a technician
-- =============================================
CREATE PROCEDURE DN_SRGetTechnicianZonesSP 
	-- Add the parameters for the stored procedure here
	@TechnicianId int = null,
	@return int output
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT 
		TechnicianId
		,C.Code  
		,C.Codedescript
	FROM 
		SR_Zone Z  JOIN 
		Code C ON Z.Code = C.Code AND C.Category = 'SRZONE'
	WHERE 
		(@TechnicianId IS NULL OR TechnicianId = @TechnicianId)
	
	SET @return = @@error

END
GO
