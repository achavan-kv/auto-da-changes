SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_OracleInteg_DeleteRunNo]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_OracleInteg_DeleteRunNo]
GO

--Author : Mohamed Nasmi

CREATE PROCEDURE [dbo].[DN_OracleInteg_DeleteRunNo]
	@interfaceName	varchar(12),
    @runNo          int OUTPUT,
    @return         int OUTPUT    

AS

    SET @return = 0
	
    -- InterfaceControl
	DELETE InterfaceControl
	WHERE  Interface = @interfaceName and RunNo = @runNo

    SET @return = @@error
	
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO