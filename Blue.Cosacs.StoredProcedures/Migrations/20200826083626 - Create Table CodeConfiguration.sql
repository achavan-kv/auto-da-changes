-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here


IF NOT EXISTS (		SELECT	1 
					FROM	SYS.OBJECTS 
					WHERE	OBJECT_ID = OBJECT_ID('CodeConfiguration') 
							AND TYPE IN (N'U'))
BEGIN

		CREATE TABLE [dbo].[CodeConfiguration](
			[Category] [varchar](12) NOT NULL,
			[Code] [varchar](18) NOT NULL,
			[IsMmiApplicable] [bit] NOT NULL,
		 CONSTRAINT [pk_CodeConfiguration] PRIMARY KEY CLUSTERED 
		(
			[Category] ASC,
			[Code] ASC
		)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
		) ON [PRIMARY]


		ALTER TABLE [dbo].[CodeConfiguration] ADD  DEFAULT ((0)) FOR [IsMmiApplicable]


END 