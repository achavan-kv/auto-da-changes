
                    CREATE TABLE [dbo].[CLAmortizationSchedule](
	                    [acctno] [char](12) NOT NULL,
	                    [instalduedate] [datetime] NOT NULL,
	                    [openingbal] [decimal](15, 2) NOT NULL,
	                    [instalment] [decimal](15, 2) NOT NULL,
	                    [principal] [decimal](15, 2) NOT NULL,
	                    [servicechg] [decimal](15, 2) NOT NULL,
	                    [closingbal] [decimal](15, 2) NOT NULL
                    ) ON [PRIMARY]

                    GO

                    SET ANSI_PADDING OFF
                    GO
                