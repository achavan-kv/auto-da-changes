IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE Table_Name = 'RenissanceCustomers'
               AND TABLE_SCHEMA = 'Merchandising')
BEGIN

CREATE TABLE [Merchandising].[RenissanceCustomers](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[title] [varchar](5) NULL,
	[firstName] [varchar](50) NULL,
	[lastName] [varchar](50) NULL,
	[alias] [varchar](50) NULL,
	[accountNo] [varchar](50) NULL,
	[relationship] [varchar](50) NULL,
	[dob] [datetime] NULL,
	[accountType] [varchar](20) NULL,
	[maidenName] [varchar](50) NULL,
	[CountryCode] [varchar](40) NULL,
	[maritalStat] [varchar](5) NULL,
	[dependants] [int] NULL,
	[nationality] [varchar](30) NULL,
	[branchNo] [int] NULL,
	[CreatedBy] [varchar](40) NULL,
	[Err] [varchar](400) NULL,
	[RecordIMpdate] [datetime] NULL,
	[InsertRecordStatus] [int] NULL,
	[custid] [varchar](20) NULL,
 CONSTRAINT [PK_RenissanceCustomers] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

END
Go