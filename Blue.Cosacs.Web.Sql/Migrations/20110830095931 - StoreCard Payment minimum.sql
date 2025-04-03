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
SELECT  CountryCode ,
        ParameterCategory ,
        'Minimum Payment percentage for StoreCard' ,
        '5' ,
        'Numeric' ,
        0 ,
        OptionCategory ,
        OptionListName ,
        'The minimum percent of outstanding balance a customer must pay for each bill.' ,
        'StoreCardPaymentPercent' FROM CountryMaintenance
        WHERE CodeName = 'storecardenabled' UNION ALL
        
        SELECT  CountryCode ,
        ParameterCategory ,
        'Minimum Payment amount for StoreCard' ,
        '5' ,
        'Numeric' ,
        0 ,
        OptionCategory ,
        OptionListName ,
        'The minimum payment amount per bill.' ,
        'StoreCardMinPayment' FROM CountryMaintenance
        WHERE CodeName = 'storecardenabled' 