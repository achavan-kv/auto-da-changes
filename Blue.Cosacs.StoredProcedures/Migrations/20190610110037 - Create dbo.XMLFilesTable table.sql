IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE Table_Name = 'XMLFilesTable'
               AND TABLE_SCHEMA = 'dbo')
BEGIN

CREATE TABLE [dbo].[XMLFilesTable](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[FileName] [varchar](100) NULL,
	[XMLData] [xml] NULL,
	[LoadedDateTime] [datetime] NULL,
	[ReadStatus] [int] NULL CONSTRAINT [DF_XMLFilesTable_ReadStatus]  DEFAULT ((0)),
	[CreationDate] [date] NULL,
	[actionRequestIndicator] [varchar](50) NULL,
	[PONumber] [int] NULL,
 CONSTRAINT [PK__XMLFiles__3214EC0761D23681] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

END
GO