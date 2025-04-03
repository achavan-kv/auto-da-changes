SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[dn_cashtillopenloadempeeno]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[dn_cashtillopenloadempeeno]
GO

CREATE procedure dn_cashtillopenloadempeeno
@user integer,
@return integer OUT
as 
   set @return= 0
    select t.empeeno, 
           u.FullName as name,
           t.timeopen ,
           c.codedescript as Description, -- reason for opening till
           t.tillid
    from cashtillopen t
    INNER JOIN Admin.[User] u ON t.empeeno = u.id
    INNER JOIN code c ON t.ReasonCode = c.code AND c.category = 'CTO'
	AND  t.timeopen > dateadd (day, - 3, getdate()) --only interested in recent till openings
    ORDER BY t.timeopen DESC
    
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

