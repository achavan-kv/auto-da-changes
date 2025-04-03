
IF (OBJECT_ID('File', 'U') IS NULL)
BEGIN -- if the table doesn't exist
    CREATE TABLE [File] (
        [Id]              [uniqueidentifier] NOT NULL,
        [FileName]        [varchar] (200)    NOT NULL,
        [FileSize]        [int]              NOT NULL,
        [FileContent]     [image]            NOT NULL,
        [FileExtension]   [varchar] (10)     NOT NULL,
        [FileContentType] [varchar] (100)    NOT NULL,
        [CreatedOn]       [datetime]         NULL,
        [CreatedBy]       [varchar] (100)    NULL,
        [IsComplete]      [bit]              NOT NULL,
        [Tags]            [varchar] (1000)   NULL,
        CONSTRAINT [PK_File] PRIMARY KEY CLUSTERED ([Id] ASC) ON [PRIMARY]
    ) ON [PRIMARY]
END
GO

IF NOT EXISTS (SELECT *
               FROM sys.all_columns C
               JOIN sys.tables T ON T.object_id = C.object_id
               JOIN sys.default_constraints D ON C.default_object_id = D.object_id
               WHERE T.name = 'File' AND C.name = 'Id')
BEGIN -- if default constraint doesn't exist
    ALTER TABLE [File]
    ADD CONSTRAINT [DF_File_Id] DEFAULT (newid()) FOR [Id]
END
GO
