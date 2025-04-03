/****** Object:  StoredProcedure [dbo].[ELMAH_LogError]    Script Date: 09/21/2010 13:16:14 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ELMAH_LogError]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[ELMAH_LogError]
GO

/****** Object:  StoredProcedure [dbo].[ELMAH_LogError]    Script Date: 09/21/2010 13:16:14 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[ELMAH_LogError]
(
    @ErrorId UNIQUEIDENTIFIER,
    @Application NVARCHAR(60),
    @Host NVARCHAR(30),
    @Type NVARCHAR(100),
    @Source NVARCHAR(60),
    @Message NVARCHAR(500),
    @User NVARCHAR(50),
    @AllXml NTEXT,
    @StatusCode INT,
    @TimeUtc DATETIME
)
AS

    SET NOCOUNT ON

    if (select count(*) from [ELMAH_Error] where ErrorId = @ErrorId) = 0
    begin
		INSERT
		INTO
			[ELMAH_Error]
			(
				[ErrorId],
				[Application],
				[Host],
				[Type],
				[Source],
				[Message],
				[User],
				[AllXml],
				[StatusCode],
				[TimeUtc]
			)
		VALUES
			(
				@ErrorId,
				@Application,
				@Host,
				@Type,
				@Source,
				@Message,
				@User,
				@AllXml,
				@StatusCode,
				@TimeUtc
			)
	end

GO


