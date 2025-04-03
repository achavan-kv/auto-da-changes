IF EXISTS (SELECT * FROM sysobjects WHERE NAME = 'CustTPContract')
	BEGIN
		DROP TABLE [dbo].[CustTPContract]
	END
GO

CREATE TABLE [dbo].[CustTPContract](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[AcctNo] [varchar](20) NOT NULL,
	[CustId] [varchar](20) NOT NULL,
	[IsTPContractUpload] [bit] NULL,
	[IsTPContractSend] [bit] NULL,
	[CreatedDate] [datetime] NULL,
	[UpdateDate] [datetime] NULL,
 CONSTRAINT [PK_CustTPContract] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]		