
/****** Object:  StoredProcedure [dbo].[DN_SRDeleteTechnicianZones]    Script Date: 10/17/2006 14:56:37 ******/
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID('[dbo].[DN_SRDeleteTechnicianZones]') AND OBJECTPROPERTY(id,'IsProcedure') = 1)
DROP PROCEDURE [dbo].[DN_SRDeleteTechnicianZones]


SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Peter Chong 
-- Create date: 17-Oct-2006
-- Description:	Removes zones from a technician
-- =============================================
CREATE PROCEDURE DN_SRDeleteTechnicianZones
(
	@TechnicianId int = null, --Leave blank to delete zones from all technicians
	@return int output
)	
AS
BEGIN
	SET NOCOUNT ON;

	DELETE 
	FROM SR_Zone 
	WHERE (@TechnicianId IS NULL OR TechnicianId = @TechnicianId)

	SET @return = @@error
END
GO
