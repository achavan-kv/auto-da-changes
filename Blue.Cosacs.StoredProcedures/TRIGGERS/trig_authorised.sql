

IF EXISTS(SELECT name FROM sysobjects WHERE name = 'trig_authorised' 
  	  	AND type = 'TR')

BEGIN
	DROP TRIGGER trig_authorised
END
GO


CREATE TRIGGER trig_authorised
ON	agreement
FOR	update
	/* ----------------------------------------------------------- */
	/* where old.holdprop !='N' and new.holdprop = 'N'  	       */
	/* execute procedure "ingres".dbremoveauth(acctno=new.acctno)  */
	/* ----------------------------------------------------------- */
AS
	DECLARE	@new_acctno	char(12)
	DECLARE	@new_holdprop	char(1)
	DECLARE	@old_holdprop	char(1)

	SELECT	@new_holdprop =	holdprop,
		@new_acctno =	acctno
	FROM	inserted
	SELECT	@old_holdprop = holdprop
	FROM	deleted

	IF((@old_holdprop != 'N') AND (@new_holdprop = 'N'))
	BEGIN
		EXECUTE dbremoveauth @acctno = @new_acctno
	END

GO