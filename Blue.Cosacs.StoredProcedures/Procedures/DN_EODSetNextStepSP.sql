SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects
where id = object_id('[dbo].[DN_EODSetNextStepSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_EODSetNextStepSP]
GO

CREATE PROCEDURE dbo.DN_EODSetNextStepSP
    @configurationName      varchar(12),
    @optionCode             varchar(12),
    @stepNo                 int,
    @nextStepNo             int OUTPUT,
    @return                 int OUTPUT    
AS
    
    SET @return = 0    

    IF (@StepNo = 0)
    BEGIN
        UPDATE EodConfigurationOption
        SET    CurrentStep = CurrentStep + 1
        WHERE  ConfigurationName = @configurationName
        AND    OptionCode = @optionCode
        AND    TotalSteps > CurrentStep
    END
    ELSE
    BEGIN
        UPDATE EodConfigurationOption
        SET    CurrentStep = @stepNo
        WHERE  ConfigurationName = @configurationName
        AND    OptionCode = @optionCode
        AND    TotalSteps >= @stepNo
    END
    
    SELECT @nextStepNo = CurrentStep
    FROM   EodConfigurationOption
    WHERE  ConfigurationName = @configurationName
    AND    OptionCode = @optionCode

    IF (@@error != 0)
    BEGIN
        SET @return = @@error
    END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

