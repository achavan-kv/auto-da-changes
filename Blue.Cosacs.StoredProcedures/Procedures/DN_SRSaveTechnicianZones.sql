
/****** Object:  StoredProcedure [dbo].[DN_SRSaveTechnicianZones]    Script Date: 10/17/2006 15:08:15 ******/
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID('[dbo].[DN_SRSaveTechnicianZones]') AND OBJECTPROPERTY(id,'IsProcedure') = 1)
DROP PROCEDURE [dbo].[DN_SRSaveTechnicianZones]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Peter Chong
-- Create date: 17-Oct-2006
-- Description:	Saves a zone for a specific technician or many technicians
-- =============================================
CREATE PROCEDURE DN_SRSaveTechnicianZones
	-- Add the parameters for the stored procedure here
	@TechnicianId int = null,
	@ZoneCode  varchar(100),
	@return int output
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @Zone VARCHAR(12)

	IF EXISTS(SELECT * FROM [SR_Zone] WHERE [TechnicianId] = @TechnicianId)
	BEGIN
		DELETE FROM [SR_Zone] 
		WHERE [TechnicianId] = @TechnicianId
		WHILE CHARINDEX(',',@ZoneCode)>1
		BEGIN
		SET @Zone = LEFT(@ZoneCode, CHARINDEX(',', @ZoneCode)-1)
		INSERT INTO [SR_Zone] ([TechnicianId],[Code])
		VALUES (@TechnicianId,@Zone)
		SET @ZoneCode = RIGHT(@ZoneCode, LEN(@ZoneCode) - CHARINDEX(',', @ZoneCode))
		END
	END
	ELSE
	BEGIN
		WHILE CHARINDEX(',',@ZoneCode)>1
		BEGIN
		SET @Zone = LEFT(@ZoneCode, CHARINDEX(',', @ZoneCode)-1)
		INSERT INTO [SR_Zone]
           ([TechnicianId]
           ,[Code])
		SELECT T.TechnicianId
		, @Zone 
		FROM SR_Technician T LEFT OUTER JOIN [SR_Zone] Z ON T.TechnicianId = Z.TechnicianId AND  Z.Code <> @Zone
		WHERE Deleted = 0  AND
		(@TechnicianId IS NULL OR T.TechnicianId = @TechnicianId) AND
		NOT EXISTS(SELECT * FROM dbo.SR_Zone WHERE TechnicianID = T.TechnicianId AND Code = @Zone)
		GROUP BY T.[TechnicianId]
		SET @ZoneCode = RIGHT(@ZoneCode, LEN(@ZoneCode) - CHARINDEX(',', @ZoneCode))
		END
	END
           
	
	SET @return = @@error
END
GO
