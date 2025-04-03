ALTER TABLE Service.Contact 
ADD CONSTRAINT FK_Contact_Request 
FOREIGN KEY ( RequestId ) REFERENCES Service.Request (Id) 
ON DELETE  CASCADE 
GO	 
	 


/****** Object:  Index [PK_Table_1]    Script Date: 10/02/2012 11:34:01 ******/
IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[Service].[Contact]') AND name = N'PK_Table_1')
ALTER TABLE [Service].[Contact] DROP CONSTRAINT [PK_Table_1]
GO

/****** Object:  Index [PK_Table_1]    Script Date: 10/02/2012 11:34:01 ******/
ALTER TABLE [Service].[Contact] ADD  CONSTRAINT [PK_Contact] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO

