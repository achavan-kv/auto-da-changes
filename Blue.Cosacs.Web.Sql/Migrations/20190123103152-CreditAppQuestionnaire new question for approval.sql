IF EXISTS (SELECT * FROM sysobjects 
   WHERE NAME = 'CreditAppQuestionnaire'
   )
BEGIN
DROP table CreditAppQuestionnaire
END 
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[CreditAppQuestionnaire](
	[QuestionId] [int] IDENTITY(1,1) NOT NULL,
	[Question] [nvarchar](500) NOT NULL,
	[InputType] [nvarchar](50) NOT NULL,
	[InputCategory] [nvarchar](50) NOT NULL,
	[CategorySection] [nvarchar](50) NULL,
	[IsMandatory] [bit] NOT NULL CONSTRAINT [DF_Questionnaire_IsMandatory]  DEFAULT ((0)),
	[IsActive] [bit] NOT NULL CONSTRAINT [DF_CreditAppQuestionnaire_IsActive]  DEFAULT ((0)),
	[Category] [varchar](18) NULL CONSTRAINT [DF_CreditAppQuestionnaire_Code]  DEFAULT (''),
 CONSTRAINT [PK_Questionnaire] PRIMARY KEY CLUSTERED 
(
	[QuestionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
SET IDENTITY_INSERT [dbo].[CreditAppQuestionnaire] ON 

GO
INSERT [dbo].[CreditAppQuestionnaire] ([QuestionId], [Question], [InputType], [InputCategory], [CategorySection], [IsMandatory], [IsActive], [Category]) VALUES (1001, N'Enter your DOB', N'DATE', N'NONE', N'Personal', 1, 1, NULL)
GO
INSERT [dbo].[CreditAppQuestionnaire] ([QuestionId], [Question], [InputType], [InputCategory], [CategorySection], [IsMandatory], [IsActive], [Category]) VALUES (1002, N'Enter your Title', N'TEXTBOX', N'NONE', N'Personal', 0, 0, N'TTL')
GO
INSERT [dbo].[CreditAppQuestionnaire] ([QuestionId], [Question], [InputType], [InputCategory], [CategorySection], [IsMandatory], [IsActive], [Category]) VALUES (1003, N'Enter your Address Type', N'TEXTBOX', N'NONE', N'Personal', 0, 0, N'CT1')
GO
INSERT [dbo].[CreditAppQuestionnaire] ([QuestionId], [Question], [InputType], [InputCategory], [CategorySection], [IsMandatory], [IsActive], [Category]) VALUES (1004, N'Enter your Home Address1', N'TEXTBOX', N'NONE', N'Personal', 1, 1, NULL)
GO
INSERT [dbo].[CreditAppQuestionnaire] ([QuestionId], [Question], [InputType], [InputCategory], [CategorySection], [IsMandatory], [IsActive], [Category]) VALUES (1005, N'Enter your Home Address2', N'TEXTBOX', N'STRING', N'Personal', 0, 0, NULL)
GO
INSERT [dbo].[CreditAppQuestionnaire] ([QuestionId], [Question], [InputType], [InputCategory], [CategorySection], [IsMandatory], [IsActive], [Category]) VALUES (1006, N'Enter your Home Address3', N'TEXTBOX', N'STRING', N'Personal', 0, 0, NULL)
GO
INSERT [dbo].[CreditAppQuestionnaire] ([QuestionId], [Question], [InputType], [InputCategory], [CategorySection], [IsMandatory], [IsActive], [Category]) VALUES (1007, N'Enter your Delivery Area', N'TEXTBOX', N'NONE', N'Personal', 0, 0, N'CA1')
GO
INSERT [dbo].[CreditAppQuestionnaire] ([QuestionId], [Question], [InputType], [InputCategory], [CategorySection], [IsMandatory], [IsActive], [Category]) VALUES (1008, N'Enter your Date In', N'DATE', N'NONE', N'Personal', 0, 0, NULL)
GO
INSERT [dbo].[CreditAppQuestionnaire] ([QuestionId], [Question], [InputType], [InputCategory], [CategorySection], [IsMandatory], [IsActive], [Category]) VALUES (1009, N'Enter your Marital Status', N'TEXTBOX', N'NONE', N'Personal', 1, 1, N'MS1')
GO
INSERT [dbo].[CreditAppQuestionnaire] ([QuestionId], [Question], [InputType], [InputCategory], [CategorySection], [IsMandatory], [IsActive], [Category]) VALUES (1010, N'Enter your Number of Dependents', N'TEXTBOX', N'NUMBER', N'Personal', 1, 1, NULL)
GO
INSERT [dbo].[CreditAppQuestionnaire] ([QuestionId], [Question], [InputType], [InputCategory], [CategorySection], [IsMandatory], [IsActive], [Category]) VALUES (1011, N'Enter your Nationality', N'TEXTBOX', N'NONE', N'Personal', 1, 1, N'NA2')
GO
INSERT [dbo].[CreditAppQuestionnaire] ([QuestionId], [Question], [InputType], [InputCategory], [CategorySection], [IsMandatory], [IsActive], [Category]) VALUES (1012, N'Enter your Occupation', N'TEXTBOX', N'NONE', N'Employment', 1, 1, N'WT1')
GO
INSERT [dbo].[CreditAppQuestionnaire] ([QuestionId], [Question], [InputType], [InputCategory], [CategorySection], [IsMandatory], [IsActive], [Category]) VALUES (1013, N'Enter your Pay frequency', N'TEXTBOX', N'NONE', N'Employment', 1, 1, N'PF1')
GO
INSERT [dbo].[CreditAppQuestionnaire] ([QuestionId], [Question], [InputType], [InputCategory], [CategorySection], [IsMandatory], [IsActive], [Category]) VALUES (1014, N'Enter your Telephone number', N'TEXTBOX', N'PHONE', N'Employment', 1, 1, NULL)
GO
INSERT [dbo].[CreditAppQuestionnaire] ([QuestionId], [Question], [InputType], [InputCategory], [CategorySection], [IsMandatory], [IsActive], [Category]) VALUES (1015, N'Enter your Current employment start date', N'DATE', N'NONE', N'Employment', 1, 1, NULL)
GO
INSERT [dbo].[CreditAppQuestionnaire] ([QuestionId], [Question], [InputType], [InputCategory], [CategorySection], [IsMandatory], [IsActive], [Category]) VALUES (1016, N'Enter your Prev employment start date', N'DATE', N'NONE', N'Employment', 0, 0, NULL)
GO
INSERT [dbo].[CreditAppQuestionnaire] ([QuestionId], [Question], [InputType], [InputCategory], [CategorySection], [IsMandatory], [IsActive], [Category]) VALUES (1017, N'Enter your Employee status', N'TEXTBOX', N'NONE', N'Employment', 0, 0, N'ES1')
GO
INSERT [dbo].[CreditAppQuestionnaire] ([QuestionId], [Question], [InputType], [InputCategory], [CategorySection], [IsMandatory], [IsActive], [Category]) VALUES (1018, N'Enter your Date in prev address', N'DATE', N'NONE', N'Residential', 0, 0, NULL)
GO
INSERT [dbo].[CreditAppQuestionnaire] ([QuestionId], [Question], [InputType], [InputCategory], [CategorySection], [IsMandatory], [IsActive], [Category]) VALUES (1019, N'Enter your Net Income', N'TEXTBOX', N'NUMBER', N'Financial', 1, 1, NULL)
GO
INSERT [dbo].[CreditAppQuestionnaire] ([QuestionId], [Question], [InputType], [InputCategory], [CategorySection], [IsMandatory], [IsActive], [Category]) VALUES (1020, N'Enter your Additional income', N'TEXTBOX', N'NUMBER', N'Financial', 0, 0, NULL)
GO
INSERT [dbo].[CreditAppQuestionnaire] ([QuestionId], [Question], [InputType], [InputCategory], [CategorySection], [IsMandatory], [IsActive], [Category]) VALUES (1021, N'Enter your Utilities', N'TEXTBOX', N'NUMBER', N'Financial', 0, 0, NULL)
GO
INSERT [dbo].[CreditAppQuestionnaire] ([QuestionId], [Question], [InputType], [InputCategory], [CategorySection], [IsMandatory], [IsActive], [Category]) VALUES (1022, N'Enter your Additional Expenditure1', N'TEXTBOX', N'NUMBER', N'Financial', 0, 0, NULL)
GO
INSERT [dbo].[CreditAppQuestionnaire] ([QuestionId], [Question], [InputType], [InputCategory], [CategorySection], [IsMandatory], [IsActive], [Category]) VALUES (1023, N'Enter your Additional Expenditure2', N'TEXTBOX', N'NUMBER', N'Financial', 0, 0, NULL)
GO
INSERT [dbo].[CreditAppQuestionnaire] ([QuestionId], [Question], [InputType], [InputCategory], [CategorySection], [IsMandatory], [IsActive], [Category]) VALUES (1024, N'Enter your Bank detail', N'TEXTBOX', N'STRING', N'Financial', 0, 0, NULL)
GO
INSERT [dbo].[CreditAppQuestionnaire] ([QuestionId], [Question], [InputType], [InputCategory], [CategorySection], [IsMandatory], [IsActive], [Category]) VALUES (1025, N'Enter your Bank account open date', N'DATE', N'NONE', N'Financial', 0, 0, NULL)
GO
INSERT [dbo].[CreditAppQuestionnaire] ([QuestionId], [Question], [InputType], [InputCategory], [CategorySection], [IsMandatory], [IsActive], [Category]) VALUES (1026, N'Enter your Account type', N'TEXTBOX', N'NONE', N'Financial', 0, 0, N'CT1')
GO
INSERT [dbo].[CreditAppQuestionnaire] ([QuestionId], [Question], [InputType], [InputCategory], [CategorySection], [IsMandatory], [IsActive], [Category]) VALUES (1027, N'Enter your Miscellaneous expense', N'TEXTBOX', N'NUMBER', N'Financial', 0, 0, NULL)
GO
INSERT [dbo].[CreditAppQuestionnaire] ([QuestionId], [Question], [InputType], [InputCategory], [CategorySection], [IsMandatory], [IsActive], [Category]) VALUES (1028, N'Enter your Reference 1 Contact', N'CONTACT', N'PHONE', N'Reference1', 1, 1, NULL)
GO
INSERT [dbo].[CreditAppQuestionnaire] ([QuestionId], [Question], [InputType], [InputCategory], [CategorySection], [IsMandatory], [IsActive], [Category]) VALUES (1029, N'Enter your Reference 1 Last name', N'TEXTBOX', N'STRING', N'Reference1', 0, 0, NULL)
GO
INSERT [dbo].[CreditAppQuestionnaire] ([QuestionId], [Question], [InputType], [InputCategory], [CategorySection], [IsMandatory], [IsActive], [Category]) VALUES (1030, N'Enter your Reference 1 Relationship', N'TEXTBOX', N'STRING', N'Reference1', 0, 0, NULL)
GO
INSERT [dbo].[CreditAppQuestionnaire] ([QuestionId], [Question], [InputType], [InputCategory], [CategorySection], [IsMandatory], [IsActive], [Category]) VALUES (1031, N'Enter your Reference 1 Home address', N'TEXTBOX', N'STRING', N'Reference1', 0, 0, NULL)
GO
INSERT [dbo].[CreditAppQuestionnaire] ([QuestionId], [Question], [InputType], [InputCategory], [CategorySection], [IsMandatory], [IsActive], [Category]) VALUES (1032, N'Enter your Reference 1 Home telephone', N'TEXTBOX', N'PHONE', N'Reference1', 0, 0, NULL)
GO
INSERT [dbo].[CreditAppQuestionnaire] ([QuestionId], [Question], [InputType], [InputCategory], [CategorySection], [IsMandatory], [IsActive], [Category]) VALUES (1033, N'Enter your Reference 1 Date checked', N'DATE', N'NONE', N'Reference1', 0, 0, NULL)
GO
INSERT [dbo].[CreditAppQuestionnaire] ([QuestionId], [Question], [InputType], [InputCategory], [CategorySection], [IsMandatory], [IsActive], [Category]) VALUES (1034, N'Enter your Reference 1 Comments', N'TEXTBOX', N'STRING', N'Reference1', 0, 0, NULL)
GO
INSERT [dbo].[CreditAppQuestionnaire] ([QuestionId], [Question], [InputType], [InputCategory], [CategorySection], [IsMandatory], [IsActive], [Category]) VALUES (1035, N'Enter your Reference 2 First name', N'TEXTBOX', N'STRING', N'Reference2', 0, 0, NULL)
GO
INSERT [dbo].[CreditAppQuestionnaire] ([QuestionId], [Question], [InputType], [InputCategory], [CategorySection], [IsMandatory], [IsActive], [Category]) VALUES (1036, N'Enter your Reference 2 Last name', N'TEXTBOX', N'STRING', N'Reference2', 0, 0, NULL)
GO
INSERT [dbo].[CreditAppQuestionnaire] ([QuestionId], [Question], [InputType], [InputCategory], [CategorySection], [IsMandatory], [IsActive], [Category]) VALUES (1037, N'Enter your Reference 2 Relationship', N'TEXTBOX', N'STRING', N'Reference2', 0, 0, NULL)
GO
INSERT [dbo].[CreditAppQuestionnaire] ([QuestionId], [Question], [InputType], [InputCategory], [CategorySection], [IsMandatory], [IsActive], [Category]) VALUES (1038, N'Enter your Reference 2 Home address', N'TEXTBOX', N'STRING', N'Reference2', 0, 0, NULL)
GO
INSERT [dbo].[CreditAppQuestionnaire] ([QuestionId], [Question], [InputType], [InputCategory], [CategorySection], [IsMandatory], [IsActive], [Category]) VALUES (1039, N'Enter your Reference 2 Home telephone', N'TEXTBOX', N'PHONE', N'Reference2', 0, 0, NULL)
GO
INSERT [dbo].[CreditAppQuestionnaire] ([QuestionId], [Question], [InputType], [InputCategory], [CategorySection], [IsMandatory], [IsActive], [Category]) VALUES (1040, N'Enter your Reference 2 Date checked', N'DATE', N'NONE', N'Reference2', 0, 0, NULL)
GO
INSERT [dbo].[CreditAppQuestionnaire] ([QuestionId], [Question], [InputType], [InputCategory], [CategorySection], [IsMandatory], [IsActive], [Category]) VALUES (1041, N'Enter your Reference 2 Comments', N'TEXTBOX', N'STRING', N'Reference2', 0, 0, NULL)
GO
INSERT [dbo].[CreditAppQuestionnaire] ([QuestionId], [Question], [InputType], [InputCategory], [CategorySection], [IsMandatory], [IsActive], [Category]) VALUES (1042, N'Upload your ID proof 1', N'IMAGE', N'NONE', N'DOCUMENTS', 1, 1, NULL)
GO
INSERT [dbo].[CreditAppQuestionnaire] ([QuestionId], [Question], [InputType], [InputCategory], [CategorySection], [IsMandatory], [IsActive], [Category]) VALUES (1043, N'Upload your Address proof', N'IMAGE', N'NONE', N'DOCUMENTS', 1, 1, NULL)
GO
INSERT [dbo].[CreditAppQuestionnaire] ([QuestionId], [Question], [InputType], [InputCategory], [CategorySection], [IsMandatory], [IsActive], [Category]) VALUES (1044, N'Upload your Income proof', N'IMAGE', N'NONE', N'DOCUMENTS', 1, 1, NULL)
GO
INSERT [dbo].[CreditAppQuestionnaire] ([QuestionId], [Question], [InputType], [InputCategory], [CategorySection], [IsMandatory], [IsActive], [Category]) VALUES (1045, N'Enter your Product Category', N'TEXTBOX', N'NONE', N'PRODUCT', 0, 0, NULL)
GO
INSERT [dbo].[CreditAppQuestionnaire] ([QuestionId], [Question], [InputType], [InputCategory], [CategorySection], [IsMandatory], [IsActive], [Category]) VALUES (1046, N'Enter your Gender', N'TEXTBOX', N'NONE', N'Personal', 1, 1, N'GEN')
GO
INSERT [dbo].[CreditAppQuestionnaire] ([QuestionId], [Question], [InputType], [InputCategory], [CategorySection], [IsMandatory], [IsActive], [Category]) VALUES (1047, N'Enter Current Employer name', N'TEXTBOX', N'NONE', N'Employment', 1, 1, NULL)
GO
INSERT [dbo].[CreditAppQuestionnaire] ([QuestionId], [Question], [InputType], [InputCategory], [CategorySection], [IsMandatory], [IsActive], [Category]) VALUES (1048, N'Enter Current Work Address', N'TEXTBOX', N'NONE', N'Employment', 1, 1, NULL)
GO
INSERT [dbo].[CreditAppQuestionnaire] ([QuestionId], [Question], [InputType], [InputCategory], [CategorySection], [IsMandatory], [IsActive], [Category]) VALUES (1049, N'Enter Current Resident status', N'TEXTBOX', N'NONE', N'Residential', 1, 1, N'RS1')
GO
INSERT [dbo].[CreditAppQuestionnaire] ([QuestionId], [Question], [InputType], [InputCategory], [CategorySection], [IsMandatory], [IsActive], [Category]) VALUES (1050, N'Enter years at Current Address', N'TEXTBOX', N'NUMBER', N'Residential', 1, 1, NULL)
GO
INSERT [dbo].[CreditAppQuestionnaire] ([QuestionId], [Question], [InputType], [InputCategory], [CategorySection], [IsMandatory], [IsActive], [Category]) VALUES (1051, N'Enter your Living Expenses', N'TEXTBOX', N'NUMBER', N'Financial', 0, 0, NULL)
GO
INSERT [dbo].[CreditAppQuestionnaire] ([QuestionId], [Question], [InputType], [InputCategory], [CategorySection], [IsMandatory], [IsActive], [Category]) VALUES (1052, N'Enter your Loan Expenses', N'TEXTBOX', N'NUMBER', N'Financial', 1, 1, NULL)
GO
INSERT [dbo].[CreditAppQuestionnaire] ([QuestionId], [Question], [InputType], [InputCategory], [CategorySection], [IsMandatory], [IsActive], [Category]) VALUES (1053, N'Upload your ID proof 2', N'IMAGE', N'NONE', N'DOCUMENTS', 1, 1, NULL)
GO
SET IDENTITY_INSERT [dbo].[CreditAppQuestionnaire] OFF
GO
