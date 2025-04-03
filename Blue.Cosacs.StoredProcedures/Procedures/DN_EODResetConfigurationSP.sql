SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_EODResetConfigurationSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_EODResetConfigurationSP]
GO

CREATE PROCEDURE dbo.DN_EODResetConfigurationSP
    @configurationName      varchar(12),
    @return                 int OUTPUT    

AS

    -- Clear the result status and current steps ready for a new EOD run

    SET @return = 0

    UPDATE EodConfigurationOption
    SET    Status = '',
           CurrentStep = 0
    WHERE  ConfigurationName = @configurationName


    SET @return = @@error

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

