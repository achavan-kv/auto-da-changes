SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_CountrySetSystemStatusSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_CountrySetSystemStatusSP]
GO

CREATE PROCEDURE  dbo.DN_CountrySetSystemStatusSP
   			@status char(10),
			@country char(1),
   			@return int OUTPUT
 
AS
 
 	SET  @return = 0   --initialise return code
 	
	UPDATE	CountryMaintenance
    SET		Value = @status
    WHERE	codename = 'systemopen'
    AND		CountryCode = @country

 	IF (@@error != 0)
 	BEGIN
  		SET @return = @@error
 	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

