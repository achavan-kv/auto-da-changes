SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_InterfaceErrorLoadSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_InterfaceErrorLoadSP]
GO

CREATE PROCEDURE 	dbo.DN_InterfaceErrorLoadSP
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : DN_InterfaceErrorLoadSP.prc
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Load Interface Errors
-- Author       : ??
-- Date         : ??
--
-- This procedure will load the interface error for the selected interface & runno.
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 06/04/11 jec  RI - Add check on interface start date to cater for reruns
-- ================================================
	-- Add the parameters for the stored procedure here
			@interface varchar(12),
			@runno int,
			@datestart datetime,
			@return int OUTPUT

AS
	SET @return = 0		--initialise return code

	SELECT	interface,
			runno,
			severity,
			errordate,
			errortext
	FROM		interfaceerror
	WHERE	interface = @interface
	AND		runno = @runno
	and  errordate>@datestart		-- jec 06/04/11
	ORDER BY 	errordate ASC

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

