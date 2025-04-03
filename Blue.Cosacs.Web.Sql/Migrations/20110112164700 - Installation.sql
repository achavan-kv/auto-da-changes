IF  NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Installation]') AND type in (N'U'))
CREATE TABLE [dbo].[Installation]
(
    [InstallationNo] int NOT NULL IDENTITY
    ,[BranchNo] smallint NOT NULL
    ,[AcctNo] CHAR(12) NOT NULL
    ,[ItemNo] VARCHAR(8) NOT NULL
    ,[Status] VARCHAR(10) NOT NULL
	,[CreatedBy] INT NULL
	,[LastUpdatedBy] INT NULL
	,[CreatedOn] datetime NULL
	,[LastUpdatedOn] datetime NULL	    
    ,[Comment] varchar(80) NULL
    CONSTRAINT [PK_Installation] PRIMARY KEY CLUSTERED ([InstallationNo])
)

GO