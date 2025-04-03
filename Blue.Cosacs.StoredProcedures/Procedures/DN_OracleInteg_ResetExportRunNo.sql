SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_OracleInteg_ResetExportRunNo]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_OracleInteg_ResetExportRunNo]
GO

--Author : Mohamed Nasmi

CREATE PROCEDURE [dbo].[DN_OracleInteg_ResetExportRunNo]
	@runNo   int OUTPUT,
    @return  int OUTPUT    

AS

    SET @return = 0
	
	UPDATE LineItemOracleExport
	SET RunNo = 0
	WHERE RunNo = @runNo 
	
	UPDATE FinTransOracleExport
	SET RunNo = 0
	WHERE RunNo = @runNo
	
    SET @return = @@error
	
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO