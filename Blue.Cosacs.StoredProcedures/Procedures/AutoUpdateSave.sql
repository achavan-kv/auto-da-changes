IF EXISTS (SELECT * FROM sysobjects
               WHERE NAME = 'AutoUpdateSave'
               AND xtype = 'P')
BEGIN
	DROP PROCEDURE AutoUpdateSave
END
GO

CREATE PROCEDURE AutoUpdateSave
@machinename VARCHAR(200),
@domain VARCHAR(200),
@user VARCHAR(200),
@oldversion VARCHAR(200),
@newversion VARCHAR(200),
@return INT OUTPUT

AS
BEGIN
	INSERT INTO autoupdatelog (
		machinename,
		domain,
		winuser,
		oldversion,
		newversion,
		[date]
	) VALUES ( 
		/* machinename - VARCHAR(200) */ @machinename,
		/* domain - VARCHAR(200) */ @domain,
		/* winuser - VARCHAR(200) */ @user,
		/* oldversion - VARCHAR(200) */ @oldversion,
		/* newversion - VARCHAR(200) */ @newversion,
		/* date - DATETIME */ GETDATE() 
		)
END
GO