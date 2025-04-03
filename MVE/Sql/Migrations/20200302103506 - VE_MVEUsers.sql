IF EXISTS (SELECT * FROM sysobjects WHERE NAME = 'VE_MVEUser')
	BEGIN
		Select 1
	END
IF NOT EXISTS (SELECT * FROM sysobjects WHERE NAME = 'VE_MVEUser')
	BEGIN
	CREATE TABLE [dbo].[VE_MVEUser](
		[CosacsUserId] [int] NOT NULL,
		[MVEUserId] [nvarchar](50) NOT NULL
	) ON [PRIMARY]
END