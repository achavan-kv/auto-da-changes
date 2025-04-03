INSERT INTO CountryMaintenance
        (
          CountryCode ,
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
        'Magnetic Stripe Reader Name' ,
        'MagTek Msr' ,
        Type ,
        Precision ,
        OptionCategory ,
        OptionListName ,
        'The name of the Magnetic Stripe Card Reader installed.',
        'StoreCardMagStripeReaderName'
FROM CountryMaintenance
WHERE CodeName ='StoreCardStatementFrequency'
        
        
