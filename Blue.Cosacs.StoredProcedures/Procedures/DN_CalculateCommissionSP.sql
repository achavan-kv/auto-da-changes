SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_CalculateCommissionSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_CalculateCommissionSP]
GO

CREATE PROCEDURE 	dbo.DN_CalculateCommissionSP
			@branchno int,
			@empeetype varchar(3), --IP - 03/06/08 - Credit Collections - Need to cater for (3) character Employee Types.
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	SELECT	b.empeeno, SUM(b.transvalue) as transvalue
	INTO  	#temp_commn
	FROM 	bailiffcommn b
	INNER JOIN admin.UserRole ur ON ur.UserId = b.empeeno
    INNER JOIN Admin.[User] u ON ur.UserId = u.Id
	WHERE u.branchno =  @branchno
	AND	ur.roleid = @empeetype
    --and status != 'P'
	and status = 'P'		-- #14537 - correct previous erroneous change
	GROUP BY b.empeeno
	
    UPDATE  c
	SET     commndue = 0
	FROM CourtspersonTable c
    INNER JOIN admin.UserRole ur ON ur.UserId = c.UserId
    INNER JOIN Admin.[User] u ON ur.UserId = u.Id
	WHERE u.branchno =  @branchno
	AND	ur.roleid = @empeetype

	
	UPDATE  courtspersontable
	SET     commndue = #temp_commn.transvalue
	FROM    #temp_commn
	WHERE   courtspersontable.UserID = #temp_commn.empeeno
	
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO


