	--22 Jan 2010 AA Version 3....
IF EXISTS (SELECT * FROM sysobjects WHERE NAME = 'CSK_Logoff') 
DROP PROCEDURE CSK_Logoff
go
create PROCEDURE CSK_Logoff
@machid varchar(6)
As
begin
delete from accountlocking where lockedby=@machid
end
GO
IF NOT EXISTS (SELECT * FROM sysobjects WHERE NAME = 'CSK_Get_Audit') 
CREATE TABLE [dbo].[csk_get_audit](
	[acctno] [char](12) NOT NULL,
	[custid] [varchar](20) NULL,
	[NAME] [varchar](60) NOT NULL,
	[AcctType] [char](1) NOT NULL,
	[DateOpened] [datetime] NULL,
	[AgreementTotal] [money] NULL,
	[Balance] [money] NOT NULL,
	[Arrears] [money] NOT NULL,
	[CurrentStatus] [char](1) NULL,
	[HighestStatus] [char](1) NULL,
	[AmountPaid] [money] NULL,
	[PercentagePaid] [smallint] NOT NULL,
	[datelastpaid] [datetime] NULL,
	[termstype] [nvarchar](2) NOT NULL,
	[rebate] [money] NULL,
	[EarlySettle] [money] NULL,
	[datelast] [datetime] NULL,
	[instalment] [money] NULL,
	[FinalInstalment] MONEY NULL, 
	[Creditlimit] [money] NULL,
	[AvailableSpend] [money] NULL,
	[DueDate] [datetime] NULL,
	[DATEFIRST] [datetime] NULL,
	[instalno] [smallint] NULL,
	[datedel] [datetime] NULL,
	[deposit] [money] NOT NULL,
	[ServiceCharge] [money] NOT NULL,
	[Segment] [varchar](32) NULL,
	[AddtoPotential] [money] NULL,
	[FinalPayment] [char](1) NULL
) ON [PRIMARY]

GO
