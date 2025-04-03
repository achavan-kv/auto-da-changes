
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--exec DN_SRCheckPartsDateSP 18352,0

/****** Object:  StoredProcedure [dbo].[DN_SRCheckPartsDateSP]    Script Date: 02/15/2007 16:06:31 ******/
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID('[dbo].[DN_SRCheckPartsDateSP]') AND OBJECTPROPERTY(id,'IsProcedure') = 1)
DROP PROCEDURE [dbo].[DN_SRCheckPartsDateSP]

GO
-- =============================================
-- Author:		Jez Hemans
-- Create date: 15/02/2007
-- Description:	Returns the TecnicianID and Parts Date for a specified SR number. 
-- =============================================
CREATE PROCEDURE DN_SRCheckPartsDateSP
	@SRno INT,
    @Return  INT OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON

    SET @Return = 0

    SELECT TechnicianId,PartsDate FROM dbo.SR_Allocation
    WHERE ServiceRequestNo = @SRno

    SET @Return = @@error

	SET NOCOUNT OFF
	RETURN @Return
END
GO
