SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_LockItemSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_LockItemSP]
GO

CREATE PROCEDURE 	dbo.DN_LockItemSP
			@itemno varchar(10),
			@stocklocn smallint,
			@user int,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code
	DECLARE	@lockedby int
	DECLARE	@name varchar(20)

	SELECT	@name = u.FullName, @lockedby = IL.lockedby
	FROM	ItemLocking IL 
	INNER JOIN Admin.[User] u ON 	IL.lockedby = u.id
	WHERE	itemno = @itemno
	AND	stocklocn = @stocklocn
	AND 	IL.lockedby != @user
	AND 	(datediff(minute, IL.lockedat, getdate()) < 120)
 
	IF (@name IS NOT NULL)
	BEGIN
		--we cannot lock this record throw an exception
		RAISERROR ('Low Stock user %s (%d) has got this item currently locked', 16, 1, @name, @lockedby)
	END
	ELSE
	BEGIN
		--try to update any lock there is (which must be expired) to be our lock
		UPDATE	ItemLocking
		SET	lockedat = getdate()
		WHERE	itemno = @itemno
		AND	stocklocn = @stocklocn
		AND	lockedby = @user

		--if there were no locks to update, insert one
		IF(@@rowcount = 0)
		BEGIN
			INSERT INTO ItemLocking	(itemno, stocklocn, lockedat, lockedby, itemID)
			SELECT @itemno, @stocklocn, getdate(), @user, ItemID
			FROM stockitem
			WHERE itemno = @itemno
			AND stocklocn = @stocklocn
		END
	END

				
	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END

return @return
Go

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

