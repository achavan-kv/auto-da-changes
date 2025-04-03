IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.tables WHERE table_name ='temp_Summary1_MR')
CREATE TABLE [dbo].[temp_Summary1_MR](
	[acctno] [char](12) NOT NULL,
	[acctchar4] [char](1) NOT NULL,
	[accttypegroup] [varchar](3) NOT NULL,
	[acctlife] [int] NOT NULL,
	[outstbalcorr] [money] NOT NULL,
	[dayssincereposs] [int] NOT NULL,
	[spaflag] [char](1) NOT NULL,
	[datespaadded] [smalldatetime] NULL,
	[datedel1] [datetime] NULL,
	[delamount] [money] NOT NULL,
	[monthsarrears] [smallint] NOT NULL,
	[baldue12mths] [money] NULL,
	[baldueafter12mths] [money] NULL,
	[payamount] [money] NOT NULL,
	[fullydeliveredflag] [char](1) NOT NULL,
	[monthsarrearsnew] [smallint] NOT NULL,
	[outstbalcorrnew] [money] NOT NULL,
	[bdw] [money] NOT NULL,
	[cancelledflag] [char](1) NOT NULL,
	[servicechg] [money] NOT NULL,
	[deposit] [money] NOT NULL,
	[acctstatusdescr] [varchar](4) NOT NULL,
	[Datefullydelivered] [datetime] NULL,
	[dateagrmtrevised] [smalldatetime] NULL,
	[deliveryflag] [char](1) NOT NULL,
	[daysarrears] [int] NULL,
	[StatusCodeBand] [varchar](3) NULL,
	[repoamt] [money] NULL,
 CONSTRAINT [temp_Summary1_MR_acctno] PRIMARY KEY CLUSTERED 
(
	[acctno] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO





if exists(select * FROM dbo.sysobjects 
               WHERE ID = object_id('dbo.vw_Summary1_C2_Fintrans') AND OBJECTPROPERTY(id, 'IsView') = 1)
	drop view dbo.vw_Summary1_C2_Fintrans
GO 

create view dbo.vw_Summary1_C2_Fintrans
as
	Select f.acctno,sum(f.transvalue) as transvalue
		From fintrans f, temp_Summary1_MR s1
			Where f.acctno = s1.acctno
				AND f.transtypecode in ( 'REP','RDL','RPO') 
				group by f.acctno
GO

