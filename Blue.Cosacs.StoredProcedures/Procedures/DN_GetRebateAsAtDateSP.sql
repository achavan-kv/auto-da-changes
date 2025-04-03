
if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_GetRebateAsAtDateSP]')
					and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_GetRebateAsAtDateSP]
GO

CREATE PROCEDURE [dbo].[DN_GetRebateAsAtDateSP]
    @asatdate 	datetime  OUTPUT,
    @return 	int   OUTPUT
AS
   
set @return = 0

select @asatdate = isnull(max(asatdate),'01-Jan-1900') from rebates_asat

IF (@@error != 0)
BEGIN
	SET @return = @@error
END
GO

SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO



