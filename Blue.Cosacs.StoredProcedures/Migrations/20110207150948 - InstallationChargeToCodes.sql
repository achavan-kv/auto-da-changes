UPDATE dbo.codecat
SET codelgth = 6
WHERE category = 'INSTCHARGE'

INSERT INTO dbo.code
        ( category ,
          code ,
          codedescript ,
          statusflag ,
          sortorder ,
          reference 
        )
VALUES
        ( 'INSTCHARGE' , -- category - varchar(12)
          'INSTE' , -- code - varchar(12)
          'Electrical' , -- codedescript - nvarchar(64)
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
        ( 'INSTCHARGE' , -- category - varchar(12)
          'INSTF' , -- code - varchar(12)
          'Furniture' , -- codedescript - nvarchar(64)
          'L' , -- statusflag - char(1)
          2 , -- sortorder - smallint
          '0' -- reference - varchar(12)
        )