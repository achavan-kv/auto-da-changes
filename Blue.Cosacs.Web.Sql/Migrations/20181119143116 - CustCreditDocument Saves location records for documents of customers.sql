IF EXISTS (SELECT * FROM sysobjects 
   WHERE NAME = 'CustCreditDocuments'
   )
BEGIN
DROP table CustCreditDocuments
END 
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CustCreditDocuments](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CustId] [nvarchar](30) NOT NULL,
	[FolderPath] [nvarchar](300) NOT NULL,
	[FileName] [nvarchar](50) NOT NULL,
	[AccountNumber] [nvarchar](50) NULL,
	[CreatedOn] [datetime] NOT NULL,
 CONSTRAINT [PK_CustCreditDocuments] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
