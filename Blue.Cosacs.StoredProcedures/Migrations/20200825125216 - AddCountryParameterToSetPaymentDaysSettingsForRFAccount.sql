-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

-- *************************************************************************************************
-- Developer:    Rahul Sonawane
-- Date:         25 Aug 2020
-- Purpose:      Include setting for-
--				 "Ability Set First Payment Date" in CountryMaintenance to "Configure first payment date for RF account".
--				 "Payment Day After Booking", in CountryMaintenance to "Gives no.of days after Booking for first payment date".
--				 "Max Delay in Delivery", in CountryMaintenance to "Gives no.of days delay after Booking for first payment date".
--				 "Days Allowed between Delivery and Payment Day", in CountryMaintenance to "Gives No.of days allowed between Delivery Date and Payment Date".
-- *************************************************************************************************

DECLARE @CountryCode CHAR(2)
DECLARE @ParameterCategory CHAR(2) = (SELECT code FROM code WHERE codedescript = 'Instalments' AND category = 'CMC')
SELECT TOP 1 @CountryCode = countrycode FROM country WITH(NOLOCK)   

	IF NOT EXISTS (SELECT * FROM [CountryMaintenance] WITH(NOLOCK) WHERE CodeName='AbilitySetFirstPaymentDate')
    BEGIN            
			INSERT INTO CountryMaintenance (CountryCode,ParameterCategory,Name,Value,[Type],[Precision],OptionCategory,OptionListName,[Description],CodeName)
            VALUES ( 	 @CountryCode
                        ,@ParameterCategory
                        ,'Ability to set the First Payment Date'
                        ,'False'
                        ,'checkbox'
                        ,0
                        ,''
                        ,''
                        ,'If set True then it determine the calculation of First Payment Date as enable.'
                        ,'AbilitySetFirstPaymentDate');
    END
	   
    IF NOT EXISTS (SELECT * FROM [CountryMaintenance] WITH(NOLOCK) WHERE CodeName='NoOfDaysAfterBookFirstPayDate')
    BEGIN            
			INSERT INTO CountryMaintenance (CountryCode,ParameterCategory,Name,Value,[Type],[Precision],OptionCategory,OptionListName,[Description],CodeName)
            VALUES ( 		@CountryCode
                        ,@ParameterCategory
                        ,'Payment Day After Booking'
                        ,33
                        ,'numeric'
                        ,0
                        ,''
                        ,''
                        ,'This will be number of Days after Booking for first payment date.'
                        ,'NoOfDaysAfterBookFirstPayDate');
    END

	IF NOT EXISTS (SELECT * FROM [CountryMaintenance] WITH(NOLOCK) WHERE CodeName='MaxDaysDelayDelAfterBook')
    BEGIN            
			INSERT INTO CountryMaintenance (CountryCode,ParameterCategory,Name,Value,[Type],[Precision],OptionCategory,OptionListName,[Description],CodeName)
            VALUES ( 		@CountryCode
                        ,@ParameterCategory
                        ,'Max Delay in Delivery'
                        ,7
                        ,'numeric'
                        ,0
                        ,''
                        ,''
                        ,'Max number of delay (days) allowed for delivery after booking.'
                        ,'MaxDaysDelayDelAfterBook');
    END

	IF NOT EXISTS (SELECT * FROM [CountryMaintenance] WITH(NOLOCK) WHERE CodeName='MinDiffBtwDelDateAndPayDate')
    BEGIN            
			INSERT INTO CountryMaintenance (CountryCode,ParameterCategory,Name,Value,[Type],[Precision],OptionCategory,OptionListName,[Description],CodeName)
            VALUES ( 		@CountryCode
                        ,@ParameterCategory
                        ,'Days Allowed between Delivery and Payment Day'
                        ,14
                        ,'numeric'
                        ,0
                        ,''
                        ,''
                        ,'No of Days allowed between Delivery Date and Payment Date.'
                        ,'MinDiffBtwDelDateAndPayDate');
    END