-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

Create table [File].[FileDescription](
	[Id] [int] IDENTITY(1,1) NOT NULL,
    [FileId] UNIQUEIDENTIFIER ROWGUIDCOL NOT NULL UNIQUE,
	[FileName] varchar(200) NOT NULL,
	FileSize bigint NOT NULL,
	FileContent varbinary(max) NOT NULL,
	FileExtension varchar(10) NOT NULL,
	FileContentType varchar(100) null,
	CreatedOn datetime null,
    CreatedBy int null

	CONSTRAINT [PK_File_File] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)
)
