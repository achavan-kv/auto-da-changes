SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_ScheduleAlignItemsSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_ScheduleAlignItemsSP]
GO

CREATE PROCEDURE 	dbo.DN_ScheduleAlignItemsSP
			@acctno varchar(12),
			@agrmtno int,
			@itemno varchar(8), 
			@stocklocn smallint,
			@quantity float, 
			@return int OUTPUT
AS

	-- 68421 removed as no longer required
	

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO
