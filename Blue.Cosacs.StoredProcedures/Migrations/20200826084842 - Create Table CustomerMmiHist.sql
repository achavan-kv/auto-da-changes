-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here


IF NOT EXISTS (	SELECT	* 
					FROM	SYS.OBJECTS 
					WHERE	OBJECT_ID = OBJECT_ID('CustomerMmiHist') 
							AND TYPE IN (N'U'))
BEGIN

	CREATE TABLE [dbo].[CustomerMmiHist](
		[CustId] [varchar](20) NOT NULL,
		[AcctNo] [char](12) NOT NULL,
		[DateProp] [datetime] NULL,
		[Points] [smallint] NULL,
		[MmiLimit] [money] NULL,
		[DateChange] [datetime] NOT NULL,
		[EmpNo] [int] NULL,
		[ReasonChanged] [varchar](12) NULL
	 CONSTRAINT [pk_CustomerMmiHist] PRIMARY KEY CLUSTERED 
	(
		[CustId] ASC,
		[DateChange] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]

END