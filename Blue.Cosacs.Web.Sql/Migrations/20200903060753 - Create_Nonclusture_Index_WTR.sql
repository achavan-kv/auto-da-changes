-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here


-- Create Nonclusture Index IX_WTR on Table [dbo].[delivery]
IF EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[delivery]') AND name = N'IX_WTR')
BEGIN
	DROP INDEX [dbo].[delivery].IX_WTR
END

CREATE NONCLUSTERED INDEX [IX_WTR]
ON [dbo].[delivery] ([datetrans])
INCLUDE ([acctno],[agrmtno],[delorcoll],[quantity],[transvalue],[ItemID])


-- Create Nonclusture Index IX_WTR1 on Table [dbo].[delivery]
IF EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[delivery]') AND name = N'IX_WTR1')
BEGIN
	DROP INDEX [dbo].[delivery].IX_WTR1
END

CREATE NONCLUSTERED INDEX [IX_WTR1]
ON [dbo].[delivery] ([ItemID],[datetrans])
INCLUDE ([acctno],[quantity],[transvalue])
 

-- Create Nonclusture Index IX_WTR_2 on Table [dbo].[delivery]
IF EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[delivery]') AND name = N'IX_WTR_2')
BEGIN
	DROP INDEX [dbo].[delivery].IX_WTR_2
END
CREATE NONCLUSTERED INDEX [IX_WTR_2]
ON [dbo].[delivery] ([datetrans])
INCLUDE ([acctno],[agrmtno],[delorcoll],[itemno],[stocklocn],[quantity],[transvalue],[contractno],[ItemID],[ParentItemID])