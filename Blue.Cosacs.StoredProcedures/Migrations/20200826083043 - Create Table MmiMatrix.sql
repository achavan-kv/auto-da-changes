-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here


IF NOT EXISTS (	SELECT	* 
					FROM	SYS.OBJECTS 
					WHERE	OBJECT_ID = OBJECT_ID('MmiMatrix') 
							AND TYPE IN (N'U'))
BEGIN
		CREATE TABLE [dbo].[MmiMatrix](
			[Label] [nvarchar](50) NOT NULL,
			[FromScore] [smallint] NOT NULL,
			[ToScore] [smallint] NOT NULL,
			[MmiPercentage] [float] NOT NULL,
			[ConfiguredDate] [datetime] NOT NULL,
			[ConfiguredBy] [int] NOT NULL
		) ON [PRIMARY]

		ALTER TABLE [dbo].[MmiMatrix] ADD  CONSTRAINT [DF_MmiMatrix_FromScore]  DEFAULT ((0)) FOR [FromScore]
		ALTER TABLE [dbo].[MmiMatrix] ADD  CONSTRAINT [DF_MmiMatrix_ToScore]  DEFAULT ((0)) FOR [ToScore]
		ALTER TABLE [dbo].[MmiMatrix] ADD  CONSTRAINT [DF_MmiMatrix_MmiPercentage]  DEFAULT ((0)) FOR [MmiPercentage]

END