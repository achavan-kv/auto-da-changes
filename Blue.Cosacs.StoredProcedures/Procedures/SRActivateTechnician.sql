IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID('[dbo].[SRActivateTechnician]') AND OBJECTPROPERTY(id,'IsProcedure') = 1)
DROP PROCEDURE [dbo].[SRActivateTechnician]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Ilyas Parker
-- Create date: 14/02/11
-- Description:	Activate a technician that is currently marked as inactive
-- =============================================
CREATE PROCEDURE SRActivateTechnician
(
	@TechnicianId int
)	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	UPDATE SR_Technician 
	SET DELETED = 0 
	WHERE TechnicianId = @TechnicianId
	
END
GO
