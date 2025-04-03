SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InstallationBooking]') AND type in (N'U'))
BEGIN
	DROP TABLE [dbo].[InstallationBooking]
END   

CREATE TABLE [dbo].[InstallationBooking](
	[InstallationNo] [int] NOT NULL,
	[InstallationDate] [datetime] NOT NULL,
	[TechnicianId] [int] NOT NULL,
	[StartSlot] [smallint] NOT NULL,
	[NoOfSlots] [smallint] NOT NULL,
	[Instructions] [varchar](200) NULL,
	[BookedBy] [int] NOT NULL,
	[BookedOn] [datetime] NOT NULL,	
	[RebookingReasonCode] [varchar](12) NULL,
	[IsDeleted] [bit] NOT NULL,
	[DeletedBy] [int] NULL,
	[DeletedOn] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[InstallationNo] ASC,
	[BookedOn] ASC	
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[InstallationBooking] ADD  DEFAULT ((0)) FOR [IsDeleted]
GO


