
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[CM_SaveSMSSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[CM_SaveSMSSP]
GO


CREATE PROCEDURE  dbo.CM_SaveSMSSP
				@name varchar(10),
				@body SQLTEXT,
				@description nvarchar(64),
				@return	int	OUTPUT

AS

    SET @return = 0    --initialise return code

	INSERT INTO CMSMS(SMSName, SMSText, description)
	VALUES (@name, @body,@description)

    IF (@@error != 0)
    BEGIN
        SET @return = @@error
    END
GO
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO

