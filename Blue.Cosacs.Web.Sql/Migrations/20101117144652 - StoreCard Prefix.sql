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
SELECT    MIN(CountryCode) , -- CountryCode - char(1)
          MIN(code) , -- ParameterCategory - varchar(4)
          'Store Card Issuer Identification Number' , -- Name - varchar(50)
          '0000' , -- Value - varchar(1500)
          'numeric' , -- Type - varchar(10)
          0, -- Precision - int
          '' , -- OptionCategory - varchar(4)
          '' , -- OptionListName - varchar(50)
          'The numeric prefix to be used on the the StoreCard. Must be a maximum 6 digits in length.' , -- Description - varchar(1500)
          'StoreCardPrefix'  -- CodeName - varchar(30)
        
FROM country, code
WHERE category = 'CMC'
AND codedescript = 'storecard'



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
SELECT    MIN(CountryCode) , -- CountryCode - char(1)
          MIN(code) , -- ParameterCategory - varchar(4)
          'Current StoreCard Number' , -- Name - varchar(50)
          '1' , -- Value - varchar(1500)
          'numeric' , -- Type - varchar(10)
          0, -- Precision - int
          '' , -- OptionCategory - varchar(4)
          '' , -- OptionListName - varchar(50)
          'The current number of the next storecard (without prefix or check digit).' , -- Description - varchar(1500)
          'StoreCardNumber'  -- CodeName - varchar(30)
FROM country, code
WHERE category = 'CMC'
AND codedescript = 'storecard'