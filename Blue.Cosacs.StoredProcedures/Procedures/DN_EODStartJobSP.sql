SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_EODStartJobSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_EODStartJobSP]
GO

CREATE PROCEDURE  dbo.DN_EODStartJobSP
   			@return int OUTPUT
 
AS
 
 	SET  @return = 0   --initialise return code

    EXECUTE msdb.dbo.sp_start_job @job_name = 'CoSACS EOD Run' 

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

 
