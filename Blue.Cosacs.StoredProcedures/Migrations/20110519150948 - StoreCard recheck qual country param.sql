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
SELECT  countrycode, -- CountryCode - char(1)
          '33' , -- ParameterCategory - varchar(4)
          'StoreCard recheck qualifcation.' , -- Name - varchar(50)
          'True' , -- Value - varchar(1500)
          'Checkbox' , -- Type - varchar(10)
          0 , -- Precision - int
          '' , -- OptionCategory - varchar(4)
          '' , -- OptionListName - varchar(50)
          'This option will check the customer still qualifies for StoreCard when setting status to awaiting activation.' , -- Description - varchar(1500)
          'StoreCardCheckQual'  -- CodeName - varchar(30)
        FROM country