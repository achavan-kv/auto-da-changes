INSERT INTO code
        ( origbr ,
          category ,
          code ,
          codedescript ,
          statusflag ,
          sortorder ,
          reference ,
          additional
        )
VALUES
        ( 0 , -- origbr - smallint
          'FPM' , -- category - varchar(12)
          '13' , -- code - varchar(12)
          'StoreCard' , -- codedescript - nvarchar(64)
          'L' , -- statusflag - char(1)
          0 , -- sortorder - smallint
          0 , -- reference - varchar(12)
          null  -- additional - varchar(15)
        )