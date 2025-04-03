
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO


if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_GetExistingBuffNoSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_GetExistingBuffNoSP]
GO


CREATE procedure DN_GetExistingBuffNoSP
				 @acctno char(12),
				 @agrmtno INT OUT,
				 @return INT OUT

AS

SET @agrmtno= 0
SELECT  @agrmtno = agrmtno
FROM agreement
WHERE acctno = @acctno
SET @return = @@error
IF ( @return != 0)
	BEGIN
		SET @return = @@error
	END

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO


