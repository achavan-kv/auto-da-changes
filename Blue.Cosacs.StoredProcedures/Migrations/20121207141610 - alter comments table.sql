IF EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'service' 
                 AND  TABLE_NAME = 'Comments')
DROP TABLE Service.Comments
GO

CREATE TABLE Service.Comment(
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[RequestId] [int] NOT NULL,
	[Date] [datetime] NOT NULL,
	[AddedBy] [varchar](50) NOT NULL,
	[Text] [varchar](4000) NOT NULL,
 )
GO


ALTER TABLE Service.[Comment]
ADD CONSTRAINT [PK_ServiceComment_Id]
PRIMARY KEY CLUSTERED
(
	Id
)

CREATE NONCLUSTERED INDEX IX_FK_ServiceComment_RequestId
    ON Service.Comment (RequestId)
GO

