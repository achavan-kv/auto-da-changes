SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[dn_cashtillopencheckempeeno]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[dn_cashtillopencheckempeeno]
GO


CREATE procedure dn_cashtillopencheckempeeno
			@user integer,
			@tillid varchar(16),
			@empeeno int OUT,
			@return integer OUT
as 
	SET 		@return= 0
	
	-- select 0 if not logged on already today then open till
	SELECT 	@empeeno = isnull(empeeno,0)
	FROM 		cashtillopen 
	WHERE 	timeopen> dateadd (hour, -8, getdate())
	AND 		tillid =@tillid
	AND 		empeeno =@user
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

