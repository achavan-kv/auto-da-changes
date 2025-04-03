IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE Table_Name = 'XMLFileDataNew'
               AND TABLE_SCHEMA = 'Merchandising')
BEGIN
CREATE TABLE  [Merchandising].[XMLFileDataNew](
	[PoNumber] [int] NOT NULL,
	[XML] [xml] NULL,
	[Createdte] [datetime] NOT NULL,
	[FileCreateStatus] [varchar](1) NULL,
	[RowNo] [int] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

END
GO
