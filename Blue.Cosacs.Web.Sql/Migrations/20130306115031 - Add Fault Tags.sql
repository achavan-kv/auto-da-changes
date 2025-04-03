create table Service.FaultTag
(
	Id int IDENTITY(1,1) not null,
	RequestId int not null,
	Tag varchar(128)
)

ALTER TABLE Service.FaultTag
ADD CONSTRAINT [PK_FaultTag] PRIMARY KEY (
Id
)

ALTER TABLE [Service].[FaultTag]  WITH CHECK ADD  CONSTRAINT [FK_FaultTag_Request] FOREIGN KEY([RequestId])
REFERENCES [Service].[Request] ([Id])
ON DELETE CASCADE
GO
