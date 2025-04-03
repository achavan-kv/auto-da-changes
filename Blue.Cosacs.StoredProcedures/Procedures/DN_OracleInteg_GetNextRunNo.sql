SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_OracleInteg_GetNextRunNo]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_OracleInteg_GetNextRunNo]
GO

--Author : Mohamed Nasmi

CREATE PROCEDURE [dbo].[DN_OracleInteg_GetNextRunNo]
	@interfaceName          varchar(12),
    @runNo                  int OUTPUT,
    @return                 int OUTPUT    

AS

    SET @return = 0
	SET @runNo = 0
	
    -- InterfaceControl
	IF @runno = 0
	BEGIN
		SELECT @runNo = ISNULL(MAX(RunNo),0) + 1
		FROM   InterfaceControl
		WHERE  Interface = @interfaceName

		INSERT INTO InterfaceControl
			(Interface, RunNo, DateStart, DateFinish)
		VALUES
			(@interfaceName, @runNo, GETDATE(), '')
	END
 
    SET @return = @@error
	
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO