
-- Script for Address Standardization CR2019 - 025
--Script Description : To Change Char(2) to char(3) Datalength for custel table to save and edit and get purpose delivery mobile funtionality

IF OBJECT_ID('[dbo].[custtel_bk]', 'U') IS NOT NULL 
  DROP TABLE [dbo].[custtel_bk]; 
ELSE	
SELECT origbr,custid,tellocn,dateteladd,datediscon,telno,extnno,DialCode,empeenochange,datechange,CustTelId INTO [dbo].[custtel_bk] FROM custtel

ALTER TABLE [dbo].[custtel] DROP CONSTRAINT [PK_custtel]
GO



IF EXISTS (    SELECT     1
        FROM     sys.indexes
        WHERE     name='ix_custtel_datediscon'
                AND object_id = OBJECT_ID('[dbo].[custtel]'))
BEGIN
    DROP INDEX [ix_custtel_datediscon] ON [dbo].[custtel];
END


IF EXISTS (    SELECT     1
        FROM     sys.indexes
        WHERE     name='ix_custtel_tellocn'
                AND object_id = OBJECT_ID('[dbo].[custtel]'))
BEGIN
    DROP INDEX [ix_custtel_tellocn] ON [dbo].[custtel];
END

ALTER TABLE dbo.custtel ALTER COLUMN [tellocn] CHAR(3) NOT NULL;
GO

ALTER TABLE [dbo].[custtel] ADD  CONSTRAINT [PK_custtel] PRIMARY KEY CLUSTERED 
(
       [custid] ASC,
       [tellocn] ASC,
       [dateteladd] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO

CREATE NONCLUSTERED INDEX [ix_custtel_datediscon] ON [dbo].[custtel]
(
       [tellocn] ASC,
       [datediscon] ASC,
       [telno] ASC
)
INCLUDE (     [custid]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO
 
CREATE NONCLUSTERED INDEX [ix_custtel_tellocn] ON [dbo].[custtel]
(
       [tellocn] ASC
)
INCLUDE (     [custid],
       [telno],
       [extnno],
       [DialCode]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO
IF OBJECT_ID('[dbo].[custtel_bk]', 'U') IS NOT NULL 
  DROP TABLE [dbo].[custtel_bk]; 
 
