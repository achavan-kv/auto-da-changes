

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

IF OBJECT_ID('dbo.HierarchyExclusion', 'U') IS NOT NULL 
  DROP TABLE dbo.HierarchyExclusion; 
go

CREATE TABLE [dbo].[HierarchyExclusion]
(
	[Division] [varchar](50) NULL,
	[Department] [varchar](50) NULL,
	[Class] [varchar](50) NULL
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


INSERT [dbo].[HierarchyExclusion] ([Division], [Department], [Class]) VALUES (N'1 - ASHLEY', N'2 - ASHLEY ACCENTS', N'3 - ASH ACCESORY')
INSERT [dbo].[HierarchyExclusion] ([Division], [Department], [Class]) VALUES (N'1 - AUDIO VISUAL', N'2 - VISION', N'3 - VIDEO ACCESSORIES')
INSERT [dbo].[HierarchyExclusion] ([Division], [Department], [Class]) VALUES (N'1 - AUDIO VISUAL', N'2 - VISION', N'3 - VIDEO ACCESSORIES')
INSERT [dbo].[HierarchyExclusion] ([Division], [Department], [Class]) VALUES (N'1 - AUDIO VISUAL', N'2 - VISION', N'3 - MEDIA PLAYER')
INSERT [dbo].[HierarchyExclusion] ([Division], [Department], [Class]) VALUES (N'1 - COMPUTER AND OFFICE', N'2 - CELLPHONES AND ACCESSORIES', N'3 - AIR TIME')
INSERT [dbo].[HierarchyExclusion] ([Division], [Department], [Class]) VALUES (N'1 - COMPUTER AND OFFICE', N'2 - CELLPHONES AND ACCESSORIES', N'3 - MOBILE ACCESSORIES')
INSERT [dbo].[HierarchyExclusion] ([Division], [Department], [Class]) VALUES (N'1 - COMPUTER AND OFFICE', N'2 - COMPUTER AND ACCESSORIESÿ', N'3 - PERIPHERALS COMPUTER ACC')
INSERT [dbo].[HierarchyExclusion] ([Division], [Department], [Class]) VALUES (N'1 - COMPUTER AND OFFICE', N'2 - COMPUTER AND ACCESSORIESÿ', N'3 - PRINTER SUPPLIES')
INSERT [dbo].[HierarchyExclusion] ([Division], [Department], [Class]) VALUES (N'1 - ELECTRONICS', N'2 - MUSICAL INSTRUMENTS', N'3 - MUSICAL INST ACCESSORIES')
INSERT [dbo].[HierarchyExclusion] ([Division], [Department], [Class]) VALUES (N'1 - FURNITURE', N'2 - FURNITURE OCCASIONAL MISC', N'3 - OCCASIONAL SINGLE PIECES')
INSERT [dbo].[HierarchyExclusion] ([Division], [Department], [Class]) VALUES (N'1 - FURNITURE', N'2 - OFFICE', N'3 - SINGLE PIECES')
INSERT [dbo].[HierarchyExclusion] ([Division], [Department], [Class]) VALUES (N'1 - HARDW AGRIC INDUST', N'2 - HARDWARE', N'3 - ENGINES AND POWER SOURCES')
INSERT [dbo].[HierarchyExclusion] ([Division], [Department], [Class]) VALUES (N'1 - HARDW AGRIC INDUST', N'2 - ELECTRICAL GENERATORS', N'3 - SOLAR PANELS')
INSERT [dbo].[HierarchyExclusion] ([Division], [Department], [Class]) VALUES (N'1 - HARDW AGRIC INDUST', N'2 - ELECTRICAL GENERATORS', N'3 - WATER SYSTEMS')
INSERT [dbo].[HierarchyExclusion] ([Division], [Department], [Class]) VALUES (N'1 - HOME', N'2 - DECORATIVE ACCESSORIESÿ', N'3 - ACCENT FURNITURE')
INSERT [dbo].[HierarchyExclusion] ([Division], [Department], [Class]) VALUES (N'1 - HOME', N'2 - COOK AND PANTRY', N'3 - BAKEWARE')
INSERT [dbo].[HierarchyExclusion] ([Division], [Department], [Class]) VALUES (N'1 - HOME', N'2 - DECORATIVE ACCESSORIESÿ', N'3 - CANDLEHOLDERS')
INSERT [dbo].[HierarchyExclusion] ([Division], [Department], [Class]) VALUES (N'1 - HOME', N'2 - LIGHTING', N'3 - CEILING LAMPS')
INSERT [dbo].[HierarchyExclusion] ([Division], [Department], [Class]) VALUES (N'1 - HOME', N'2 - COOK AND PANTRY', N'3 - COOKING UTENSILS AND GADGE')
INSERT [dbo].[HierarchyExclusion] ([Division], [Department], [Class]) VALUES (N'1 - HOME', N'2 - COOK AND PANTRY', N'3 - COOKWARE')
INSERT [dbo].[HierarchyExclusion] ([Division], [Department], [Class]) VALUES (N'1 - HOME', N'2 - DINNERWARE', N'3 - DINNERWARE')
INSERT [dbo].[HierarchyExclusion] ([Division], [Department], [Class]) VALUES (N'1 - HOME', N'2 - DECORATIVE ACCESSORIESÿ', N'3 - FLORAL ACCESSORIES')
INSERT [dbo].[HierarchyExclusion] ([Division], [Department], [Class]) VALUES (N'1 - HOME', N'2 - LIGHTING', N'3 - FLOOR LAMPS')
INSERT [dbo].[HierarchyExclusion] ([Division], [Department], [Class]) VALUES (N'1 - HOME', N'2 - DECORATIVE ACCESSORIESÿ', N'3 - HOME ACCENTS')
INSERT [dbo].[HierarchyExclusion] ([Division], [Department], [Class]) VALUES (N'1 - HOME', N'2 - LIGHTING', N'3 - LIGHTING ACCESSORIES')
INSERT [dbo].[HierarchyExclusion] ([Division], [Department], [Class]) VALUES (N'1 - HOME', N'2 - DECORATIVE ACCESSORIESÿ', N'3 - SCULPTURES')
INSERT [dbo].[HierarchyExclusion] ([Division], [Department], [Class]) VALUES (N'1 - HOME', N'2 - DECORATIVE ACCESSORIESÿ', N'3 - VASES BOWLS')
INSERT [dbo].[HierarchyExclusion] ([Division], [Department], [Class]) VALUES (N'1 - HOME', N'2 - DECORATIVE ACCESSORIESÿ', N'3 - WALL DECOR ARTWORK')
INSERT [dbo].[HierarchyExclusion] ([Division], [Department], [Class]) VALUES (N'1 - MAJOR WHITES', N'2 - FRIDGE', N'3 - FRIDGE ACCESSORIES')
INSERT [dbo].[HierarchyExclusion] ([Division], [Department], [Class]) VALUES (N'1 - MAJOR WHITES', N'2 - STOVES', N'3 - STOVES ACCESSORIES')
INSERT [dbo].[HierarchyExclusion] ([Division], [Department], [Class]) VALUES (N'1 - MATTRESS', N'2 - BEDDING', N'3 - PILLOWS')
INSERT [dbo].[HierarchyExclusion] ([Division], [Department], [Class]) VALUES (N'1 - MATTRESS', N'2 - BEDDING', N'3 - SHEETS AND COMFORTERS')
INSERT [dbo].[HierarchyExclusion] ([Division], [Department], [Class]) VALUES (N'1 - RADIO SHACK', N'2 - RADIOSHACK', N'3 - CHANGERS AND RECORDS ACC')
INSERT [dbo].[HierarchyExclusion] ([Division], [Department], [Class]) VALUES (N'1 - RADIO SHACK', N'2 - RADIOSHACK', N'3 - COMM CBS WALKIE TALKIE')
INSERT [dbo].[HierarchyExclusion] ([Division], [Department], [Class]) VALUES (N'1 - RADIO SHACK', N'2 - RADIOSHACK', N'3 - HOME ELECTRONICS AND DIMME')
INSERT [dbo].[HierarchyExclusion] ([Division], [Department], [Class]) VALUES (N'1 - RADIO SHACK', N'2 - RADIOSHACK', N'3 - MIKES AND HEADPHONES')
INSERT [dbo].[HierarchyExclusion] ([Division], [Department], [Class]) VALUES (N'1 - RADIO SHACK', N'2 - RADIOSHACK', N'3 - RS BATTERIES')
INSERT [dbo].[HierarchyExclusion] ([Division], [Department], [Class]) VALUES (N'1 - RADIO SHACK', N'2 - RADIOSHACK', N'3 - RS CALCULATORS')
INSERT [dbo].[HierarchyExclusion] ([Division], [Department], [Class]) VALUES (N'1 - RADIO SHACK', N'2 - RADIOSHACK', N'3 - RS RADIOS')
INSERT [dbo].[HierarchyExclusion] ([Division], [Department], [Class]) VALUES (N'1 - RADIO SHACK', N'2 - RADIOSHACK', N'3 - TAPE AND ACCESORIES')
INSERT [dbo].[HierarchyExclusion] ([Division], [Department], [Class]) VALUES (N'1 - RADIO SHACK', N'2 - RADIOSHACK', N'3 - TAPE RECORDERS AND DECKS')
INSERT [dbo].[HierarchyExclusion] ([Division], [Department], [Class]) VALUES (N'1 - RADIO SHACK', N'2 - RADIOSHACK', N'3 - TIMERS CLOCKS WATCHES')
INSERT [dbo].[HierarchyExclusion] ([Division], [Department], [Class]) VALUES (N'1 - RADIO SHACK', N'2 - RADIOSHACK', N'3 - TEST EQUIPMENT')
INSERT [dbo].[HierarchyExclusion] ([Division], [Department], [Class]) VALUES (N'1 - RADIO SHACK', N'2 - RADIOSHACK', N'3 - TOYS')
INSERT [dbo].[HierarchyExclusion] ([Division], [Department], [Class]) VALUES (N'1 - RADIO SHACK', N'2 - RADIOSHACK', N'3 - TRANSFORMERS')
INSERT [dbo].[HierarchyExclusion] ([Division], [Department], [Class]) VALUES (N'1 - RADIO SHACK', N'2 - RADIOSHACK', N'3 - TV ANTENNAS ANDÿ ACCESORIE')
INSERT [dbo].[HierarchyExclusion] ([Division], [Department], [Class]) VALUES (N'1 - SDA', N'2 - SMALL APPLIANCES', N'3 - MISCELLANEOUS')
INSERT [dbo].[HierarchyExclusion] ([Division], [Department], [Class]) VALUES (N'1 - SPECIALTIES', N'2 - OUTDOOR SPORT', N'3 - COOLER')
INSERT [dbo].[HierarchyExclusion] ([Division], [Department], [Class]) VALUES (N'1 - SPECIALTIES', N'2 - OUTDOOR SPORT', N'3 - OTHERS')

GO


