SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_GetRebateForecastReportB_BrnSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_GetRebateForecastReportB_BrnSP]
GO

CREATE PROCEDURE 	dbo.DN_GetRebateForecastReportB_BrnSP
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : DN_GetRebateForecastReportB_BrnSP.prc
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Rebate Forecast Report B by Branch
-- Author       : John Croft
-- Date         : 04/04/2008
--
-- This procedure will get the Rebate Forecast report B by branch.
-- 
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 
-- ================================================
	-- Add the parameters for the stored procedure here
			@periodend varchar(12) OUT,
			@branchno int,
			@return int OUTPUT

AS

	DECLARE @endperiod datetime

	SET 	@return = 0			--initialise return code

	SELECT @endperiod = CONVERT(datetime, @periodend, 102)

	SELECT	Branchno as BranchNo,ArrearsLevel as 'Arrears Level', P1, P2, P3, P4, P5,
		P6, P7, P8, P9, P10, P11, P12
	FROM	RebateforecastB
	WHERE	periodend = @endperiod
		and (@branchno=branchno or (@branchno=0 and branchno!=0))
	ORDER BY BranchNo,sequence asc

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO