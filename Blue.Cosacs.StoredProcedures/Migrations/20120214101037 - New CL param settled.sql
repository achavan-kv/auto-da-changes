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
        Name + ' - CashLoans' ,
        Value ,
        Type ,
        Precision ,
        OptionCategory ,
        OptionListName ,
        Description ,
        'CL_settledmonths'
FROM CountryMaintenance
WHERE codeNAME = 'IC_settledmonths'


UPDATE CountryMaintenance
SET name =  Name + ' - Instant Credit'
WHERE codeNAME = 'IC_settledmonths'


