SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects
where id = object_id('[dbo].[DN_EODSetRunCompleteSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_EODSetRunCompleteSP]
GO

CREATE PROCEDURE [dbo].[DN_EODSetRunCompleteSP]
    @configurationName      varchar(12),
    @optionCode             varchar(12),
    @runNo                  int,
    @eodResult              char(1),
    @return                 int OUTPUT    
AS
    
    SET @return = 0    
	-- lw69180 rdb 25/09/07 if serveral run numbers existed for a failed job all
	-- would be updated here, so have changed to only update last run
    -- InterfaceControl
    UPDATE InterfaceControl
    SET    Result = @eodResult,
           DateFinish = GETDATE()
    WHERE  Interface = @optionCode
    AND    RunNo = @runNo
	AND		DateStart = 
		(select max(DateStart) 
			from interfaceControl 
			where Interface = @optionCode
				AND RunNo = @runNo)

    
    -- EodConfigurationOption
    -- Only reset the step counter after a successful run
    UPDATE EodConfigurationOption
    SET    Status = @eodResult,
           CurrentStep = CASE @eodResult WHEN 'P' THEN 0 ELSE CurrentStep END
    WHERE  ConfigurationName = @configurationName
    AND    OptionCode = @optionCode

	--#13820
	update EodConfigurationOption
	set CanReRun = 'D'
	where ConfigurationName = @configurationName
	and CanReRun = 'A'
	and @optionCode = 'STStatements'

    SET @return = @@error
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

