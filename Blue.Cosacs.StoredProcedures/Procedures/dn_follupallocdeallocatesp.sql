SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[dn_follupallocdeallocatesp]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[dn_follupallocdeallocatesp]
GO

CREATE procedure dn_FollupallocDeallocatesp
@user integer,
@acctno varchar(12),
@return integer out
as declare
	@empeeno integer
	set @return = 0
	
	select @empeeno = isnull (empeeno, 0) from follupalloc
	where acctno =@acctno and datedealloc is null

if @empeeno != 0
begin

	update courtsperson set alloccount =alloccount- 1
	where userid = @empeeno

	-- RD 09/06/06 682727 to ensure that we do not have negative alloccount
	UPDATE 	courtsperson 
	SET alloccount = 0
	WHERE	userid =@empeeno
	AND	alloccount < 0


	update follupalloc 
	set empeenodealloc=@user,
	datedealloc = getdate() where acctno =@acctno
	and empeeno =@empeeno
	and datedealloc is null

end

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

