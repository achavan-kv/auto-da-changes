DELETE FROM CountryMaintenance
WHERE CodeName IN ('MaxShortageDaily','MaxShortageMonthly','MaxShortageWeekly','MaxShortageYearly')

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
        'MaximumShortage',
        0,
        'numeric',
        0,
        OptionCategory ,
        OptionListName ,
        'Total Shortage Values below this will be written off',
        'CashierMaxShortage'
FROM CountryMaintenance
WHERE CodeName = 'allowsafedeposits' UNION ALL
SELECT  CountryCode ,
        ParameterCategory ,
        'MaximumOverage',
        0,
        'numeric',
        0,
        OptionCategory ,
        OptionListName ,
        'Total Overage values below this will be written off',
        'CashierMaxOverage' 
FROM CountryMaintenance
WHERE CodeName = 'allowsafedeposits' UNION ALL
SELECT  CountryCode ,
        ParameterCategory ,
        'MaximumTimeLimit',
        'Day',
        'dropdown',
        0,
        OptionCategory ,
        OptionListName ,
        'Time Limit before write off (1 day or 1 week)',
        'MaxTimeLimit' 
FROM CountryMaintenance
WHERE CodeName = 'allowsafedeposits' UNION ALL
SELECT  CountryCode ,
        ParameterCategory ,
        'Shortage/Overage Write off Account',
        '',
        'text',
        0,
        OptionCategory ,
        OptionListName ,
        'Write off account to post shortage or overage',
        'WriteOffAccount' 
FROM CountryMaintenance
WHERE CodeName = 'allowsafedeposits' 

