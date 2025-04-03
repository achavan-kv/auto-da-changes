SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_InstalplanPopulateSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_InstalplanPopulateSP]
GO

CREATE PROCEDURE 	dbo.DN_InstalplanPopulateSP
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : DN_InstalplanPopulateSP
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Return Instalplan details
-- Author       : ??
-- Date         : ??
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 04/03/11  ip  Sprint 5.11 - #3275 - Return InstalmentWaived column
-- ================================================
	-- Add the parameters for the stored procedure here
			@origbr smallint OUT,
			@acctno varchar(12),
			@agrmtno int OUT,
			@datefirst datetime OUT,
			@instalno smallint OUT,
			@instalfreq char(1) OUT,
			@datelast datetime OUT,
			@instalamount money OUT,
			@finalinstalamt money OUT,
			@instaltotal money OUT,
			@mthsintfree smallint OUT,
			@scoringband varchar(4) OUT ,
			@instalmentWaived bit OUT,		--IP - 04/03/11 - #3275
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	SELECT	@origbr	= origbr,
			@acctno = acctno,
			@agrmtno = agrmtno,
			@datefirst = datefirst,	
			@instalno = instalno,
			@instalfreq = instalfreq,
			@datelast = datelast,
			@instalamount = instalamount,
			@finalinstalamt = fininstalamt,
			@instaltotal = instaltot,
			@mthsintfree = mthsintfree,
			@scoringband = scoringband,
			@instalmentWaived = InstalmentWaived	--IP - 04/03/11 - #3275
	FROM		instalplan
	WHERE	acctno = @acctno 
	AND		agrmtno = @agrmtno

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END


GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

