SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_GetlatestscorecardSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_GetlatestscorecardSP]
GO


CREATE procedure DN_GetlatestscorecardSP @return int output as
set @return = 0
select referscore as ReferScore,
      acceptscore as AcceptScore
from scorecard
where datechanged >=(select Max(datechanged) from scorecard)
return @return

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

