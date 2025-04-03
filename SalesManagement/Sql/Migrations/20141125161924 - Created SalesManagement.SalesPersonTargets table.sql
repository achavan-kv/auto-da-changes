-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

CREATE TABLE SalesManagement.SalesPersonTargets
 (
	 Id smallint NOT NULL IDENTITY (1, 1),
	 Year smallint NOT NULL,
	 TargetYear [dbo].[BlueAmount] NOT NULL,
	 CreatedOn smalldatetime NOT NULL,
	 CreatedBy int NOT NULL
CONSTRAINT [PK_SalesPersonTargets] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
 ) ON [PRIMARY]

GO
