-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
insert into codecat (
	origbr,
	category,
	catdescript,
	codelgth,
	forcenum,
	forcenumdesc,
	usermaint,
	CodeHeaderText,
	DescriptionHeaderText,
	SortOrderHeaderText,
	ReferenceHeaderText,
	AdditionalHeaderText,
	ToolTipText
) VALUES ( 
	/* origbr - smallint */ 0,
	/* category - varchar(12) */ 'SRREASON',
	/* catdescript - nvarchar(64) */ 'Reassign Technician Reason Codes',
	/* codelgth - int */ 5,
	/* forcenum - char(1) */ 'N',
	/* forcenumdesc - char(1) */ 'N',
	/* usermaint - char(1) */ 'Y',
	/* CodeHeaderText - varchar(30) */ 'Reason Code',
	/* DescriptionHeaderText - varchar(30) */ 'Reason Description',
	/* SortOrderHeaderText - varchar(30) */ 'Sort Order',
	/* ReferenceHeaderText - varchar(30) */ 'Reference',
	/* AdditionalHeaderText - varchar(30) */ 'N/A',
	/* ToolTipText - varchar(300) */ '' ) 
	
insert into dbo.code (
	origbr,
	category,
	code,
	codedescript,
	statusflag,
	sortorder,
	reference,
	additional
) VALUES ( 
	/* origbr - smallint */ 0,
	/* category - varchar(12) */ 'SRREASON',
	/* code - varchar(12) */ 'DFT',
	/* codedescript - nvarchar(64) */ N'No Reasons set up',
	/* statusflag - char(1) */ 'L',
	/* sortorder - smallint */ 0,
	/* reference - varchar(12) */ '',
	/* additional - varchar(15) */ '' ) 
	
Alter TABLE SR_Allocation add ReAssignCode VARCHAR(12) null

CREATE TABLE [dbo].[SR_AllocationAudit](
	ServiceRequestNo int NOT NULL,
	DateAllocated smalldatetime NOT NULL,
	Zone varchar(12) NOT NULL,
	TechnicianId int NOT NULL,
	PartsDate smalldatetime NOT NULL,
	RepairDate smalldatetime NOT NULL,
	IsAM char(1) NOT NULL,
	Instructions varchar(200) NOT NULL,
	AllocatedBy int  NOT NULL,
	ReAssignCode VARCHAR(12) null,
	DateChange DATETIME
PRIMARY KEY CLUSTERED 
(
	[ServiceRequestNo] ASC,
	[DateChange] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO