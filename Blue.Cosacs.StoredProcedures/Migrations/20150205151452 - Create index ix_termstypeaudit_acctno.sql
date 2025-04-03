-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF EXISTS(select * from sys.indexes where name = 'ix_termstypeaudit_acctno' and object_id = OBJECT_ID('termstypeaudit'))
BEGIN
    DROP INDEX [ix_termstypeaudit_acctno] ON [dbo].[termstypeaudit]
END
GO

CREATE NONCLUSTERED INDEX ix_termstypeaudit_acctno
ON [dbo].[termstypeaudit] ([acctno])