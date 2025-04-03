-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here



DECLARE @name VARCHAR(50)				    
				    
SET @name =(SELECT name FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[SR_PartListResolved]') AND name like N'PK__SR_PartListResol%')


IF @name is not null
	EXEC ('ALTER TABLE SR_PartListResolved DROP CONSTRAINT ' + @name)
	
GO


IF NOT EXISTS(SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[SR_PartListResolved]') AND name = N'pk_SR_PartListResolved')
BEGIN
	ALTER TABLE [dbo].[SR_PartListResolved] ADD CONSTRAINT [pk_SR_PartListResolved] PRIMARY KEY CLUSTERED 
	(
		[ServiceRequestNo] ASC,
		[PartNo] ASC,
		[PartID] ASC
	)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]

END

GO

ALTER TABLE SR_PartListResolved ALTER COLUMN PartNo VARCHAR(18) not null
go

