/****** Object:  StoredProcedure [dbo].[DN_SRDeleteTechnicianSP]    Script Date: 10/18/2006 17:10:38 ******/
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID('[dbo].[DN_SRDeleteTechnicianSP]') AND OBJECTPROPERTY(id,'IsProcedure') = 1)
DROP PROCEDURE [dbo].[DN_SRDeleteTechnicianSP]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Peter Chong
-- Create date: 18-Oct-2006
-- Description:	Delete a technician
-- =============================================
CREATE PROCEDURE DN_SRDeleteTechnicianSP
(
	@TechnicianId int,
	@return int output
)	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	UPDATE SR_Technician 
	SET DELETED = 1 
	WHERE TechnicianId = @TechnicianId
	
	SET @return = @@error
END
GO
