SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_EODStartNextRunSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_EODStartNextRunSP]
GO

CREATE PROCEDURE dbo.DN_EODStartNextRunSP
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : DN_EODStartNextRunSP.prc
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Start Next EOD run
-- Author       : ??
-- Date         : ??
--
-- This procedure will Start the next EOD run option.
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 06/04/11 jec  RI - Check for previous interface being ReRun
-- 08/04/11 jec  RI - Indicate rerun
-- 17/10/11 ip/jec - #8406 - LW74094 - If re-run then set the ReRunTimes column (these will be excluded from the Broker Export)
-- ================================================
	-- Add the parameters for the stored procedure here
	
    @configurationName      varchar(12),
    @optionCode             varchar(12),
    @runNo                  int OUTPUT,
    @rerun					BIT output,
    @fileYYMMDD				CHAR(6) output,
    @return                 int OUTPUT    

AS

    SET @return = 0
    SET @runNo = 0
    SET @rerun = 0

   declare @result varchar(1), @ReRunNo int
   if @optionCode in ('COS FACT','CHARGES')
      set @result = 'P'
   else
      set @result = '%'
   if @optionCode ='CHARGES'
      	SELECT	@runno = lastchargesweekno + 1
      	FROM    	country

	-- Check for Rerun		jec 06/04/11
	if @runNo=0
	Begin
		-- set runNo to reRunNo
		select @RunNo= ISNULL(ReRunNo,0) from EodConfigurationOption 
			where ConfigurationName=@configurationName and OptionCode=@optionCode
		if @runNo !=0
			set @rerun=1	-- this is a rerun of a previous run
	End		
	
    -- InterfaceControl
   if @runno =0
     SELECT @runNo = ISNULL(MAX(RunNo),0) + 1
     FROM   InterfaceControl
     WHERE  Interface = @optionCode and result like @result

    INSERT INTO InterfaceControl
        (Interface, RunNo, DateStart, DateFinish, ReRunTimes)  --IP/JEC - 17/10/11 - Added ReRunTimes
    VALUES
        (@optionCode, @runNo, GETDATE(), '', @rerun)		   --IP/JEC - 17/10/11 - Added @rerun

    -- EodConfigurationOption
    UPDATE EodConfigurationOption
    SET    Status = 'R',
           CurrentStep = 0
    WHERE  ConfigurationName = @configurationName
    AND    OptionCode = @optionCode

	-- Get input file date for RI2Cosacs interface files
	declare @interfacedate DATETIME
	
	select @interfacedate = (select top 1 DateStart from InterfaceControl where runno=@runno and interface='RI2Cosacs')
	-- If time is between midnight & 8am - assume files for previous day
	if (CONVERT(varchar,@interfacedate,8)<'08:00:00')
		select @fileYYMMDD =CONVERT(varchar,DATEADD(d,-1,@interfacedate),12)
	else
		select @fileYYMMDD=CONVERT(varchar,@interfacedate,12)
	
	
    SET @return = @@error

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

-- End End End End End End End End End End End End End End End End End End End End End End End End End