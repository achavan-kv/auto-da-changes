-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
IF NOT EXISTS (SELECT * FROM dbo.codecat WHERE category = 'STPM')
INSERT INTO dbo.codecat (
	origbr,	category,	catdescript,
	codelgth,	forcenum,	forcenumdesc,
	usermaint,	CodeHeaderText,	DescriptionHeaderText,
	SortOrderHeaderText,	ReferenceHeaderText,	AdditionalHeaderText,
	ToolTipText
) VALUES ( 
	/* origbr - smallint */ 0,
	/* category - varchar(12) */ 'STPM',
	/* catdescript - nvarchar(64) */ N'Store Card Payment Method',
	/* codelgth - int */ 4,
	/* forcenum - char(1) */ 'N',
	/* forcenumdesc - char(1) */ 'N',
	/* usermaint - char(1) */ 'N',
	/* CodeHeaderText - varchar(30) */ 'Payment Method',
	/* DescriptionHeaderText - varchar(30) */ 'Description',
	/* SortOrderHeaderText - varchar(30) */ '',
	/* ReferenceHeaderText - varchar(30) */ '',
	/* AdditionalHeaderText - varchar(30) */ '',
	/* ToolTipText - varchar(300) */ 'Store Card Payment method' ) 
GO
DELETE FROM code WHERE category = 'STPM'
INSERT INTO code
        ( origbr ,          category ,          code ,
          codedescript ,          statusflag ,          sortorder ,
          reference ,          additional
        )
VALUES 
        ( 0 , 'STPM' ,  'CSH', -- code - varchar(12)
          'Cash' , 'L' , -- statusflag - char(1)
          1 , -- sortorder - smallint
          '0' , -- reference - varchar(12)
          NULL  -- additional - varchar(15)
        )

INSERT INTO code
        ( origbr ,          category ,          code ,
          codedescript ,          statusflag ,          sortorder ,
          reference ,          additional
        )
VALUES
        ( 0 , 'STPM' , -- category - varchar(12)
          'CHQ', -- code - varchar(12)
          'Cheque' , -- codedescript - nvarchar(64)
          'L' , -- statusflag - char(1)
          1 , -- sortorder - smallint
          '0' , -- reference - varchar(12)
          NULL  -- additional - varchar(15)
        )
INSERT INTO code
        ( origbr ,          category ,          code ,
          codedescript ,          statusflag ,          sortorder ,
          reference ,          additional
        )
VALUES
        ( 0 , 'STPM' , -- category - varchar(12)
          'DD', -- code - varchar(12)
          'Direct Debit ' , -- codedescript - nvarchar(64)
          'L' , -- statusflag - char(1)
          1 , -- sortorder - smallint
          '0' , -- reference - varchar(12)
          NULL  -- additional - varchar(15)
        )

IF NOT EXISTS (SELECT * FROM dbo.codecat WHERE category = 'STPO')
INSERT INTO dbo.codecat (
	origbr,	category,	catdescript,
	codelgth,	forcenum,	forcenumdesc,
	usermaint,	CodeHeaderText,	DescriptionHeaderText,
	SortOrderHeaderText,	ReferenceHeaderText,	AdditionalHeaderText,
	ToolTipText
) VALUES ( 
	/* origbr - smallint */ 0,
	/* category - varchar(12) */ 'STPO',
	/* catdescript - nvarchar(64) */ N'Store Card Payment Option',
	/* codelgth - int */ 4,
	/* forcenum - char(1) */ 'N',
	/* forcenumdesc - char(1) */ 'N',
	/* usermaint - char(1) */ 'N',
	/* CodeHeaderText - varchar(30) */ 'Payment Option',
	/* DescriptionHeaderText - varchar(30) */ 'Description',
	/* SortOrderHeaderText - varchar(30) */ '',
	/* ReferenceHeaderText - varchar(30) */ '',
	/* AdditionalHeaderText - varchar(30) */ '',
	/* ToolTipText - varchar(300) */ 'Store Card Payment Option- do NOT edit' ) 
GO
INSERT INTO code
        ( origbr ,          category ,          code ,
          codedescript ,          statusflag ,          sortorder ,
          reference ,          additional
        )
VALUES
        ( 0 , 'STPO' , -- category - varchar(12)
          'FB', -- code - varchar(12)
          'Full Balance' , -- codedescript - nvarchar(64)
          'L' , -- statusflag - char(1)
          1 , -- sortorder - smallint
          '0' , -- reference - varchar(12)
          NULL  -- additional - varchar(15)
        )
INSERT INTO code
        ( origbr ,          category ,          code ,
          codedescript ,          statusflag ,          sortorder ,
          reference ,          additional
        )
VALUES
        ( 0 , 'STPO' , -- category - varchar(12)
          'POB', -- code - varchar(12)
          'Percentage of Balance - enter Percentage' , -- codedescript - nvarchar(64)
          'L' , -- statusflag - char(1)
          1 , -- sortorder - smallint
          '0' , -- reference - varchar(12)
          NULL  -- additional - varchar(15)
        )
INSERT INTO code
        ( origbr ,          category ,          code ,
          codedescript ,          statusflag ,          sortorder ,
          reference ,          additional
        )
VALUES
        ( 0 , 'STPO' , -- category - varchar(12)
          'Min', -- code - varchar(12)
          'Minimum Amount - enter' , -- codedescript - nvarchar(64)
          'L' , -- statusflag - char(1)
          1 , -- sortorder - smallint
          '0' , -- reference - varchar(12)
          NULL  -- additional - varchar(15)
        )