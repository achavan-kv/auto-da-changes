IF EXISTS (SELECT * FROM sysobjects 
   WHERE NAME = 'MailTemplate'
   )
BEGIN
DROP table MailTemplate
END 
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MailTemplate](
	[TemplateId] [int] IDENTITY(1,1) NOT NULL,
	[TemplateName] [nvarchar](500) NULL,
	[MailCC] [nvarchar](500) NULL,
	[MailTo] [nvarchar](500) NULL,
	[MailSubject] [nvarchar](500) NULL,
	[MailBody] [nvarchar](max) NULL,
	[UpdatedDate] [datetime] NULL,
 CONSTRAINT [PK_MailTemplate] PRIMARY KEY CLUSTERED 
(
	[TemplateId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET IDENTITY_INSERT [dbo].[MailTemplate] ON 

INSERT [dbo].[MailTemplate] ([TemplateId], [TemplateName], [MailCC], [MailTo], [MailSubject], [MailBody], [UpdatedDate]) VALUES (1, N'CreditApproval', N'kelly_durham@unicomer.com', N'serah_radhaykissoon@unicomer.com', N'Subject - New Credit Application No @AccountNo', N'', CAST(N'2018-10-17 07:41:45.170' AS DateTime))
INSERT [dbo].[MailTemplate] ([TemplateId], [TemplateName], [MailCC], [MailTo], [MailSubject], [MailBody], [UpdatedDate]) VALUES (2, N'TPCreditApproval', N'kelly_durham@unicomer.com', N'bhupesh.badwaik@gmail.com', N'Subject - Third Party Credit Application no @AccountNo from EMMA.', N'', CAST(N'2018-10-17 07:41:45.170' AS DateTime))
INSERT [dbo].[MailTemplate] ([TemplateId], [TemplateName], [MailCC], [MailTo], [MailSubject], [MailBody], [UpdatedDate]) VALUES (3, N'UpdateTransaction', N'umesh.k@zensar.com', N'bhupesh.badwaik@gmail.com', N'Subject - Update Transaction @AccountNo from EMMA.', N'', CAST(N'2018-10-17 07:41:45.170' AS DateTime))
SET IDENTITY_INSERT [dbo].[MailTemplate] OFF
