DELETE FROM dbo.codecat
WHERE category = 'INSTCHRGBDWN'

DELETE FROM dbo.code
WHERE category = 'INSTCHRGBDWN'

INSERT INTO dbo.codecat
( category, catdescript, codelgth ,
  forcenum, forcenumdesc, usermaint 
)        
VALUES
( 'INSTCHRGBDWN' , -- category - varchar(12)
  'Installation Charge To Breakdown' , -- catdescript - nvarchar(64)
  6 , -- codelgth - int
  'N' , -- forcenum - char(1)
  'N' , -- forcenumdesc - char(1)
  'N'  -- usermaint - char(1)
)

INSERT INTO dbo.code
        ( category ,
          code ,
          codedescript ,
          statusflag ,
          sortorder ,
          reference 
        )
VALUES
( 'INSTCHRGBDWN' , -- category - varchar(12)
  'PRTC' , -- code - varchar(12)
  'Parts Courts' , -- codedescript - nvarchar(64)
  'L' , -- statusflag - char(1)
  1 , -- sortorder - smallint
  '0' -- reference - varchar(12)
)

INSERT INTO dbo.code
        ( category ,
          code ,
          codedescript ,
          statusflag ,
          sortorder ,
          reference 
        )
VALUES
( 'INSTCHRGBDWN' , -- category - varchar(12)
  'PRTO' , -- code - varchar(12)
  'Parts Other' , -- codedescript - nvarchar(64)
  'L' , -- statusflag - char(1)
  2 , -- sortorder - smallint
  '0' -- reference - varchar(12)
)

INSERT INTO dbo.code
        ( category ,
          code ,
          codedescript ,
          statusflag ,
          sortorder ,
          reference 
        )
VALUES
( 'INSTCHRGBDWN' , -- category - varchar(12)
  'PRTTOT' , -- code - varchar(12)
  'Parts Total' , -- codedescript - nvarchar(64)
  'L' , -- statusflag - char(1)
  3 , -- sortorder - smallint
  '0' -- reference - varchar(12)
)

INSERT INTO dbo.code
        ( category ,
          code ,
          codedescript ,
          statusflag ,
          sortorder ,
          reference 
        )
VALUES
( 'INSTCHRGBDWN' , -- category - varchar(12)
  'LBR' , -- code - varchar(12)
  'Labour Total' , -- codedescript - nvarchar(64)
  'L' , -- statusflag - char(1)
  4 , -- sortorder - smallint
  '0' -- reference - varchar(12)
)

INSERT INTO dbo.code
        ( category ,
          code ,
          codedescript ,
          statusflag ,
          sortorder ,
          reference 
        )
VALUES
( 'INSTCHRGBDWN' , -- category - varchar(12)
  'TOT' , -- code - varchar(12)
  'Total' , -- codedescript - nvarchar(64)
  'L' , -- statusflag - char(1)
  5 , -- sortorder - smallint
  '0' -- reference - varchar(12)
)

        