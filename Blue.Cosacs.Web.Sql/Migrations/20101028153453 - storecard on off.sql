
DECLARE @max INT

SELECT @max = MAX(code) + 1
FROM code
WHERE category = 'CMC'

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
          'CMC' , -- category - varchar(12)
          @max , -- code - varchar(12)
          'StoreCard' , -- codedescript - nvarchar(64)
          'L' , -- statusflag - char(1)
          1 , -- sortorder - smallint
          '0' , -- reference - varchar(12)
          NULL  -- additional - varchar(15)
        )



INSERT INTO CountryMaintenance
        ( CountryCode ,
          ParameterCategory ,
          Name ,
          Value ,
          Type ,
          Precision ,
          OptionCategory ,
          OptionListName ,
          Description ,
          CodeName
        )
SELECT CountryCode,
          @max , -- ParameterCategory - varchar(4)
          'Enable StoreCard' , -- Name - varchar(50)
          'False' , -- Value - varchar(1500)
          'checkbox' , -- Type - varchar(10)
          0 , -- Precision - int
          '' , -- OptionCategory - varchar(4)
          '' , -- OptionListName - varchar(50)
          'This will enable all storecard functionality.' , -- Description - varchar(1500)
          'StoreCardEnabled'  -- CodeName - varchar(30)
        FROM country
        