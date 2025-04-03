IF EXISTS(SELECT name FROM sysobjects WHERE name = 'trig_follupalloc_insertupdate' 
  	  		AND type = 'TR')
BEGIN
	DROP TRIGGER trig_follupalloc_insertupdate
END
GO

-- 65732 unable to verify completeness of accounts -- all non Bailliff should always have date of allocation set
-- one instance when not so in Thailand-impossible to recreate so
-- as workaround creating trigger to resolve this issue.
CREATE TRIGGER trig_follupalloc_insertupdate
ON  follupalloc 
FOR update, insert
AS
   DECLARE @empeeno  integer, @type int, @datealloc datetime, @acctno char(12),@allocno integer

   SELECT @empeeno = empeeno, @datealloc = datealloc,@acctno = acctno,@allocno = allocno
   FROM    inserted

	if (Admin.CheckPermission(@empeeno,381) != 1) AND (@datealloc is null or @datealloc ='1-jan-1900')
		update follupalloc 
		set @datealloc = getdate() 
		where acctno =@acctno and allocno= @allocno and empeeno =@empeeno
GO
