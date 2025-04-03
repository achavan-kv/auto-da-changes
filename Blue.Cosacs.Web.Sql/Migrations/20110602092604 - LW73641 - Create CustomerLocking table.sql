-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF  NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[CustomerLocking]') AND type in (N'U'))
BEGIN
    CREATE TABLE CustomerLocking(
    [CustID] [varchar](20) NOT NULL,
	[LockedBy] [int] NOT NULL,
	[LockedAt] [datetime] NOT NULL,
	[LockCount] [int] NOT NULL,
	[CurrentAction] [char](1) NULL,
 CONSTRAINT [pk_customerlocking] PRIMARY KEY CLUSTERED 
(
	[LockedBy] ASC,
	[CustID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO