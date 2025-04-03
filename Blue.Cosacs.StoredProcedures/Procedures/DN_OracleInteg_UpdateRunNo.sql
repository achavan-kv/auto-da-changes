SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_OracleInteg_UpdateRunNo]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_OracleInteg_UpdateRunNo]
GO

--Author : Mohamed Nasmi

CREATE PROCEDURE [dbo].[DN_OracleInteg_UpdateRunNo]
	@interfaceName	varchar(12),
    @runNo          int OUTPUT,
    @result			varchar(1),
    @return         int OUTPUT    

AS

    SET @return = 0
	
    -- InterfaceControl
	UPDATE InterfaceControl
	SET Result = @result,
		DateFinish = GetDate()
	WHERE  Interface = @interfaceName and RunNo = @runNo

    SET @return = @@error
	
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO