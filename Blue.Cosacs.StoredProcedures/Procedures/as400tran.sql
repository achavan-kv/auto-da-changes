SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[as400tran]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[as400tran]
GO



create procedure as400tran
			@account	char(12),
			@datetran	datetime,
			@trancode	varchar(3)
AS
	delete
	from	as400trans 
	where	acctno = @account 
	and	transtypecode = @trancode; 

	/* STL 5-July-2000 (Copied CED 290800) */
	set @account = isnull(@account,'');
	/* End STL 05-July-2000 */

	insert into as400trans
	(	acctno,
		datetrans,
		transtypecode)
	values 
	(	@account,
		@datetran,
		@trancode);

RETURN



GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

