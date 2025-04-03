IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE Table_Name = 'TempPoCount'
               AND TABLE_SCHEMA = 'dbo')
BEGIN

CREATE TABLE [dbo].[TempPoCount](
	[id] [int] NOT NULL,
	[statusUpdate] [varchar](1) NOT NULL
) ON [PRIMARY]

END
GO