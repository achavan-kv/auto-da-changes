IF NOT EXISTS (SELECT * FROM code
			   WHERE code = 'CASHIERWO'
			   AND category = 'EDC')
BEGIN
INSERT INTO code
            ( origbr ,
              category ,
              code ,
              codedescript ,
              statusflag ,
              sortorder ,
              reference ,
              additional ,
              Additional2
            )
    VALUES  ( 0 , -- origbr - smallint
              'EDC' , -- category - varchar(12)
              'CASHIERWO' , -- code - varchar(18)
              'Cashier Totals Write Off' , -- codedescript - nvarchar(64)
              'L' , -- statusflag - char(1)
              5 , -- sortorder - smallint
              '0' , -- reference - varchar(12)
              null , -- additional - varchar(15)
              null  -- Additional2 - varchar(15)
            )
END