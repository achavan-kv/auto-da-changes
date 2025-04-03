SELECT Id ,
        Name 
INTO #temp
FROM Admin.PermissionCategory

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[Admin].[FK_Permission_PermissionsCategory]') AND parent_object_id = OBJECT_ID(N'[Admin].[Permission]'))
ALTER TABLE [Admin].[Permission] DROP CONSTRAINT [FK_Permission_PermissionsCategory]
GO

update [Admin].[Permission] set categoryId=12 where categoryId=13

DROP TABLE  [Admin].[PermissionCategory]
GO

CREATE TABLE [Admin].[PermissionCategory](
	[Id] int NOT NULL,
	[Name] varchar(50) NOT NULL,
 CONSTRAINT [PK_PermissionsCategory] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

INSERT INTO Admin.[PermissionCategory]
(id,name)
SELECT id, name FROM #temp
WHERE name != 'Cosacs'


ALTER TABLE [Admin].[Permission]  WITH CHECK ADD  CONSTRAINT [FK_Permission_PermissionsCategory] FOREIGN KEY([CategoryId])
REFERENCES [Admin].[PermissionCategory] ([Id])
GO

ALTER TABLE [Admin].[Permission] CHECK CONSTRAINT [FK_Permission_PermissionsCategory]
GO

DROP TABLE #temp
GO

