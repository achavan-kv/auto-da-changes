-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF OBJECT_ID('Admin.AuthorisationData') IS NOT NULL BEGIN
	DROP TABLE [Admin].[AuthorisationData]
END
GO

CREATE TABLE [Admin].[AuthorisationData](
	[Id] [int] NOT NULL IDENTITY (1, 1),
	[UserName] [varchar](256) NOT NULL,
	[AuthorisationId] [int] NOT NULL,
	[IpAddress] [varchar](16) NULL,
	[AuthorisatioDataJson] [nvarchar](max) NULL,
	[CsrUserId] [int] NOT NULL,
	CreatedOn smalldatetime NOT NULL
 CONSTRAINT [PK_AuthorisationData] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

ALTER TABLE [Admin].[AuthorisationData] ADD CONSTRAINT
	DF_AuthorisationData_CreatedOn DEFAULT GETDATE() FOR CreatedOn
GO