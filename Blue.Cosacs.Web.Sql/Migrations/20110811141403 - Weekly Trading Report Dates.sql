-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here


IF  NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[WTRDates]') AND type in (N'U'))
BEGIN
	
	CREATE TABLE WTRDates
	(
		ID int identity,
		DtStartCY1 datetime null,
		DtEndCY1 datetime null,
		DtStartLY1 datetime null,
		DtEndLY1 datetime null,
		DtActive1 bit default 0 not null,
		DtFilename1 varchar(30) null,
		
		DtStartCY2 datetime null,
		DtEndCY2 datetime null,
		DtStartLY2 datetime null,
		DtEndLY2 datetime null,
		DtActive2 bit default 0 not null,
		DtFilename2 varchar(30) null,
		
		DtStartCY3 datetime null,
		DtEndCY3 datetime null,
		DtStartLY3 datetime null,
		DtEndLY3 datetime null,
		DtActive3 bit default 0 not null,
		DtFilename3 varchar(30) null,
		
		DtStartCY4 datetime null,
		DtEndCY4 datetime null,
		DtStartLY4 datetime null,
		DtEndLY4 datetime null,
		DtActive4 bit default 0 not null,
		DtFilename4 varchar(30) null,
		
		DtStartCY5 datetime null,
		DtEndCY5 datetime null,
		DtStartLY5 datetime null,
		DtEndLY5 datetime null,
		DtActive5 bit default 0 not null,
		DtFilename5 varchar(30) null,
		
		CONSTRAINT [pk_WTRDates] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
	

END