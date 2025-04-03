Declare @Country char(2)
select   @Country=   countrycode  from country
select @Country

If (@Country='B')
-- Script Comment : Insert Equifax_Variable
BEGIN
IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='AGE' and Flag_CustomerStatus='N')
Insert into Equifax_Variable values ('AGE','0.01789115177675','N')


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='AVG_AGREEMENT_TOTAL_1M_SQ' and Flag_CustomerStatus='N')
Insert into Equifax_Variable values ('AVG_AGREEMENT_TOTAL_1M_SQ','0','N')


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='AVG_BALANCE_ARREARS_12M_LN' and Flag_CustomerStatus='N')
Insert into Equifax_Variable values ('AVG_BALANCE_ARREARS_12M_LN','0','N')


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='AVG_BALANCE_ARREARS_1M_LN' and Flag_CustomerStatus='N')
Insert into Equifax_Variable values ('AVG_BALANCE_ARREARS_1M_LN','0','N')


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='BALANCEARREARS_POUND_6M' and Flag_CustomerStatus='N')
Insert into Equifax_Variable values ('BALANCEARREARS_POUND_6M','0','N')


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='BALANCEARREARS_POUND_6M_LN' and Flag_CustomerStatus='N')
Insert into Equifax_Variable values ('BALANCEARREARS_POUND_6M_LN','0','N')


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='COUNT_DAYSARREAR_30MORE_17M_LN' and Flag_CustomerStatus='N')
Insert into Equifax_Variable values ('COUNT_DAYSARREAR_30MORE_17M_LN','0','N')


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='COUNT_DAYSARREAR_60MORE_17M_LN' and Flag_CustomerStatus='N')
Insert into Equifax_Variable values ('COUNT_DAYSARREAR_60MORE_17M_LN','0','N')


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='DAYSARREARS_POUND_6M' and Flag_CustomerStatus='N')
Insert into Equifax_Variable values ('DAYSARREARS_POUND_6M','0','N')


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='EMPLOYMENTSTATUS_WOE' and Flag_CustomerStatus='N')
Insert into Equifax_Variable values ('EMPLOYMENTSTATUS_WOE','0.32537448789595','N')


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='FLAG_CUSTOMERSTATUS_HIS_WOE' and Flag_CustomerStatus='N')
Insert into Equifax_Variable values ('FLAG_CUSTOMERSTATUS_HIS_WOE','0.37435497794162','N')


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='GENDER_WOE' and Flag_CustomerStatus='N')
Insert into Equifax_Variable values ('GENDER_WOE','0.90654235559051','N')


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='MARITALSTATUS_WOE' and Flag_CustomerStatus='N')
Insert into Equifax_Variable values ('MARITALSTATUS_WOE','0.59378063831625','N')


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='MAX_PERC_OUTS_3M_SQ' and Flag_CustomerStatus='N')
Insert into Equifax_Variable values ('MAX_PERC_OUTS_3M_SQ','0','N')


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='MAX_PERC_OUTSARREARS_6M_LN' and Flag_CustomerStatus='N')
Insert into Equifax_Variable values ('MAX_PERC_OUTSARREARS_6M_LN','0','N')


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='MOBILENUMBER_WOE' and Flag_CustomerStatus='N')
Insert into Equifax_Variable values ('MOBILENUMBER_WOE','0','N')


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='NEWEST_CREDIT_SQ' and Flag_CustomerStatus='N')
Insert into Equifax_Variable values ('NEWEST_CREDIT_SQ','0','N')



IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='NUMBER_ACCOUNT_17M' and Flag_CustomerStatus='N')
Insert into Equifax_Variable values ('NUMBER_ACCOUNT_17M','0','N')


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='NUMBER_ACCOUNT_OPENED_3M' and Flag_CustomerStatus='N')
Insert into Equifax_Variable values ('NUMBER_ACCOUNT_OPENED_3M','0','N')


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='NUMBER_ACCOUNT_OPENED_3M_CR' and Flag_CustomerStatus='N')
Insert into Equifax_Variable values ('NUMBER_ACCOUNT_OPENED_3M_CR','0','N')


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='NUMBERDEPENDENTS' and Flag_CustomerStatus='N')
Insert into Equifax_Variable values ('NUMBERDEPENDENTS','-0.20201609100055','N')


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='NUMBERDEPENDENTS_CR' and Flag_CustomerStatus='N')
Insert into Equifax_Variable values ('NUMBERDEPENDENTS_CR','0','N')


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='NUMBERDEPENDENTS_SQ' and Flag_CustomerStatus='N')
Insert into Equifax_Variable values ('NUMBERDEPENDENTS_SQ','0','N')


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='OCCUPATION_WOE' and Flag_CustomerStatus='N')
Insert into Equifax_Variable values ('OCCUPATION_WOE','0.73191017217147','N')


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='OLDEST_CREDIT_LN' and Flag_CustomerStatus='N')
Insert into Equifax_Variable values ('OLDEST_CREDIT_LN','0','N')


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='POSTCODE_WOE' and Flag_CustomerStatus='N')
Insert into Equifax_Variable values ('POSTCODE_WOE','0.69664548012405','N')


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='RATIO_NDEPENT_TO_AGE' and Flag_CustomerStatus='N')
Insert into Equifax_Variable values ('RATIO_NDEPENT_TO_AGE','0','N')


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='RATIO_TCURRENTEMPLOY_TO_AGE' and Flag_CustomerStatus='N')
Insert into Equifax_Variable values ('RATIO_TCURRENTEMPLOY_TO_AGE','0.06818109783034','N')


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='RESIDENTIALSTATUS_WOE' and Flag_CustomerStatus='N')
Insert into Equifax_Variable values ('RESIDENTIALSTATUS_WOE','0','N')


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='TIMECURRENTADDRESS' and Flag_CustomerStatus='N')
Insert into Equifax_Variable values ('TIMECURRENTADDRESS','0.00066619629058','N')


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='TIMECURRENTADDRESS_LN' and Flag_CustomerStatus='N')
Insert into Equifax_Variable values ('TIMECURRENTADDRESS_LN','0','N')


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='TIMECURRENTEMPLOYMENT' and Flag_CustomerStatus='N')
Insert into Equifax_Variable values ('TIMECURRENTEMPLOYMENT','0','N')


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='TIMECURRENTEMPLOYMENT_LN' and Flag_CustomerStatus='N')
Insert into Equifax_Variable values ('TIMECURRENTEMPLOYMENT_LN','0','N')


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='TIMECURRENTEMPLOYMENT_SR' and Flag_CustomerStatus='N')
Insert into Equifax_Variable values ('TIMECURRENTEMPLOYMENT_SR','0','N')


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='AGE' and Flag_CustomerStatus='E')		
Insert into Equifax_Variable values ('AGE','0.00701946513294','E')	

		
IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='AVG_AGREEMENT_TOTAL_1M_SQ' and Flag_CustomerStatus='E')
Insert into Equifax_Variable values ('AVG_AGREEMENT_TOTAL_1M_SQ','0','E')


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='AVG_BALANCE_ARREARS_12M_LN' and Flag_CustomerStatus='E')
Insert into Equifax_Variable values ('AVG_BALANCE_ARREARS_12M_LN','0','E')


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='AVG_BALANCE_ARREARS_1M_LN' and Flag_CustomerStatus='E')
Insert into Equifax_Variable values ('AVG_BALANCE_ARREARS_1M_LN','0','E')


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='BALANCEARREARS_POUND_6M' and Flag_CustomerStatus='E')
Insert into Equifax_Variable values ('BALANCEARREARS_POUND_6M','-0.00245861354357','E')


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='BALANCEARREARS_POUND_6M_LN' and Flag_CustomerStatus='E')	
Insert into Equifax_Variable values ('BALANCEARREARS_POUND_6M_LN','0','E')


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='COUNT_DAYSARREAR_30MORE_17M_LN' and Flag_CustomerStatus='E')
Insert into Equifax_Variable values ('COUNT_DAYSARREAR_30MORE_17M_LN','-0.64699067735794','E')


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='COUNT_DAYSARREAR_60MORE_17M_LN' and Flag_CustomerStatus='E')
Insert into Equifax_Variable values ('COUNT_DAYSARREAR_60MORE_17M_LN','0','E')	


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='DAYSARREARS_POUND_6M' and Flag_CustomerStatus='E')
Insert into Equifax_Variable values ('DAYSARREARS_POUND_6M','0','E')


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='EMPLOYMENTSTATUS_WOE' and Flag_CustomerStatus='E')
Insert into Equifax_Variable values ('EMPLOYMENTSTATUS_WOE','0','E')


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='FLAG_CUSTOMERSTATUS_HIS_WOE' and Flag_CustomerStatus='E')
Insert into Equifax_Variable values ('FLAG_CUSTOMERSTATUS_HIS_WOE','0','E')	


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='GENDER_WOE' and Flag_CustomerStatus='E')
Insert into Equifax_Variable values ('GENDER_WOE','0','E')	


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='MARITALSTATUS_WOE' and Flag_CustomerStatus='E')
Insert into Equifax_Variable values ('MARITALSTATUS_WOE','0.51855141815965','E')


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='MAX_PERC_OUTS_3M_SQ' and Flag_CustomerStatus='E')
Insert into Equifax_Variable values ('MAX_PERC_OUTS_3M_SQ','0','E')


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='MAX_PERC_OUTSARREARS_6M_LN' and Flag_CustomerStatus='E')
Insert into Equifax_Variable values ('MAX_PERC_OUTSARREARS_6M_LN','-0.11298922787705','E')	


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='MOBILENUMBER_WOE' and Flag_CustomerStatus='E')
Insert into Equifax_Variable values ('MOBILENUMBER_WOE','0','E')	


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='NEWEST_CREDIT_SQ' and Flag_CustomerStatus='E')
Insert into Equifax_Variable values ('NEWEST_CREDIT_SQ','0','E')


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='NUMBER_ACCOUNT_17M' and Flag_CustomerStatus='E')
Insert into Equifax_Variable values ('NUMBER_ACCOUNT_17M','0','E')


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='NUMBER_ACCOUNT_OPENED_3M' and Flag_CustomerStatus='E')
Insert into Equifax_Variable values ('NUMBER_ACCOUNT_OPENED_3M','0','E')


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='NUMBER_ACCOUNT_OPENED_3M_CR' and Flag_CustomerStatus='E')	
Insert into Equifax_Variable values ('NUMBER_ACCOUNT_OPENED_3M_CR','0','E')


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='NUMBERDEPENDENTS' and Flag_CustomerStatus='E')
Insert into Equifax_Variable values ('NUMBERDEPENDENTS','-0.06073277191621','E')


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='NUMBERDEPENDENTS_CR' and Flag_CustomerStatus='E')	
Insert into Equifax_Variable values ('NUMBERDEPENDENTS_CR','0','E')


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='NUMBERDEPENDENTS_SQ' and Flag_CustomerStatus='E')
Insert into Equifax_Variable values ('NUMBERDEPENDENTS_SQ','0','E')


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='OCCUPATION_WOE' and Flag_CustomerStatus='E')	
Insert into Equifax_Variable values ('OCCUPATION_WOE','0.90003020063526','E')


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='OLDEST_CREDIT_LN' and Flag_CustomerStatus='E')
Insert into Equifax_Variable values ('OLDEST_CREDIT_LN','0.4688105472347','E')


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='POSTCODE_WOE' and Flag_CustomerStatus='E')
Insert into Equifax_Variable values ('POSTCODE_WOE','0.72378117713995','E')	

IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='RATIO_NDEPENT_TO_AGE' and Flag_CustomerStatus='E')
Insert into Equifax_Variable values ('RATIO_NDEPENT_TO_AGE','0','E')	


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='RATIO_TCURRENTEMPLOY_TO_AGE' and Flag_CustomerStatus='E')
Insert into Equifax_Variable values ('RATIO_TCURRENTEMPLOY_TO_AGE','0','E')	


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='RESIDENTIALSTATUS_WOE' and Flag_CustomerStatus='E')
Insert into Equifax_Variable values ('RESIDENTIALSTATUS_WOE','0','E')


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='TIMECURRENTADDRESS' and Flag_CustomerStatus='E')
Insert into Equifax_Variable values ('TIMECURRENTADDRESS','0','E')	


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='TIMECURRENTADDRESS_LN' and Flag_CustomerStatus='E')
Insert into Equifax_Variable values ('TIMECURRENTADDRESS_LN','0.05017094607041','E')


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='TIMECURRENTEMPLOYMENT' and Flag_CustomerStatus='E')
Insert into Equifax_Variable values ('TIMECURRENTEMPLOYMENT','0','E')


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='TIMECURRENTEMPLOYMENT_LN' and Flag_CustomerStatus='E')
Insert into Equifax_Variable values ('TIMECURRENTEMPLOYMENT_LN','0.12899445557771','E')


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='TIMECURRENTEMPLOYMENT_SR' and Flag_CustomerStatus='E')	
Insert into Equifax_Variable values ('TIMECURRENTEMPLOYMENT_SR','0','E')
	
END

-----******************************************************************************************************************
If (@Country='Z')
-- Script Comment : Insert Equifax_Variable

BEGIN
IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='AGE' and Flag_CustomerStatus='N')
Insert into Equifax_Variable values ('AGE','0','N')


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='AVG_AGREEMENT_TOTAL_1M_SQ' and Flag_CustomerStatus='N')
Insert into Equifax_Variable values ('AVG_AGREEMENT_TOTAL_1M_SQ','0','N')


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='AVG_BALANCE_ARREARS_12M_LN' and Flag_CustomerStatus='N')
Insert into Equifax_Variable values ('AVG_BALANCE_ARREARS_12M_LN','0','N')


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='AVG_BALANCE_ARREARS_1M_LN' and Flag_CustomerStatus='N')
Insert into Equifax_Variable values ('AVG_BALANCE_ARREARS_1M_LN','0','N')


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='BALANCEARREARS_POUND_6M' and Flag_CustomerStatus='N')
Insert into Equifax_Variable values ('BALANCEARREARS_POUND_6M','0','N')


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='BALANCEARREARS_POUND_6M_LN' and Flag_CustomerStatus='N')
Insert into Equifax_Variable values ('BALANCEARREARS_POUND_6M_LN','0','N')


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='COUNT_DAYSARREAR_30MORE_17M_LN' and Flag_CustomerStatus='N')
Insert into Equifax_Variable values ('COUNT_DAYSARREAR_30MORE_17M_LN','0','N')


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='COUNT_DAYSARREAR_60MORE_17M_LN' and Flag_CustomerStatus='N')
Insert into Equifax_Variable values ('COUNT_DAYSARREAR_60MORE_17M_LN','0','N')


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='DAYSARREARS_POUND_6M' and Flag_CustomerStatus='N')
Insert into Equifax_Variable values ('DAYSARREARS_POUND_6M','0','N')


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='EMPLOYMENTSTATUS_WOE' and Flag_CustomerStatus='N')
Insert into Equifax_Variable values ('EMPLOYMENTSTATUS_WOE','0','N')


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='FLAG_CUSTOMERSTATUS_HIS_WOE' and Flag_CustomerStatus='N')
Insert into Equifax_Variable values ('FLAG_CUSTOMERSTATUS_HIS_WOE','0.49090005694173','N')


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='GENDER_WOE' and Flag_CustomerStatus='N')
Insert into Equifax_Variable values ('GENDER_WOE','0','N')


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='MARITALSTATUS_WOE' and Flag_CustomerStatus='N')
Insert into Equifax_Variable values ('MARITALSTATUS_WOE','0.50394529744539','N')


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='MAX_PERC_OUTS_3M_SQ' and Flag_CustomerStatus='N')
Insert into Equifax_Variable values ('MAX_PERC_OUTS_3M_SQ','0','N')


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='MAX_PERC_OUTSARREARS_6M_LN' and Flag_CustomerStatus='N')
Insert into Equifax_Variable values ('MAX_PERC_OUTSARREARS_6M_LN','0','N')


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='MOBILENUMBER_WOE' and Flag_CustomerStatus='N')
Insert into Equifax_Variable values ('MOBILENUMBER_WOE','0','N')


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='NEWEST_CREDIT_SQ' and Flag_CustomerStatus='N')
Insert into Equifax_Variable values ('NEWEST_CREDIT_SQ','0','N')


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='NUMBER_ACCOUNT_17M' and Flag_CustomerStatus='N')
Insert into Equifax_Variable values ('NUMBER_ACCOUNT_17M','0','N')


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='NUMBER_ACCOUNT_OPENED_3M' and Flag_CustomerStatus='N')
Insert into Equifax_Variable values ('NUMBER_ACCOUNT_OPENED_3M','0','N')


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='NUMBER_ACCOUNT_OPENED_3M_CR' and Flag_CustomerStatus='N')
Insert into Equifax_Variable values ('NUMBER_ACCOUNT_OPENED_3M_CR','0','N')


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='NUMBERDEPENDENTS' and Flag_CustomerStatus='N')
Insert into Equifax_Variable values ('NUMBERDEPENDENTS','0','N')


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='NUMBERDEPENDENTS_CR' and Flag_CustomerStatus='N')
Insert into Equifax_Variable values ('NUMBERDEPENDENTS_CR','0','N')


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='NUMBERDEPENDENTS_SQ' and Flag_CustomerStatus='N')
Insert into Equifax_Variable values ('NUMBERDEPENDENTS_SQ','0','N')


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='OCCUPATION_WOE' and Flag_CustomerStatus='N')
Insert into Equifax_Variable values ('OCCUPATION_WOE','0.85125933033099','N')


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='OLDEST_CREDIT_LN' and Flag_CustomerStatus='N')
Insert into Equifax_Variable values ('OLDEST_CREDIT_LN','0','N')


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='POSTCODE_WOE' and Flag_CustomerStatus='N')
Insert into Equifax_Variable values ('POSTCODE_WOE','0','N')


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='RATIO_NDEPENT_TO_AGE' and Flag_CustomerStatus='N')
Insert into Equifax_Variable values ('RATIO_NDEPENT_TO_AGE','0','N')


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='RATIO_TCURRENTEMPLOY_TO_AGE' and Flag_CustomerStatus='N')
Insert into Equifax_Variable values ('RATIO_TCURRENTEMPLOY_TO_AGE','0','N')


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='RESIDENTIALSTATUS_WOE' and Flag_CustomerStatus='N')
Insert into Equifax_Variable values ('RESIDENTIALSTATUS_WOE','0.63590562642187','N')


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='TIMECURRENTADDRESS' and Flag_CustomerStatus='N')
Insert into Equifax_Variable values ('TIMECURRENTADDRESS','0','N')


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='TIMECURRENTADDRESS_LN' and Flag_CustomerStatus='N')
Insert into Equifax_Variable values ('TIMECURRENTADDRESS_LN','0.14539927590789','N')


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='TIMECURRENTEMPLOYMENT' and Flag_CustomerStatus='N')
Insert into Equifax_Variable values ('TIMECURRENTEMPLOYMENT','0','N')


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='TIMECURRENTEMPLOYMENT_LN' and Flag_CustomerStatus='N')
Insert into Equifax_Variable values ('TIMECURRENTEMPLOYMENT_LN','0','N')


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='TIMECURRENTEMPLOYMENT_SR' and Flag_CustomerStatus='N')
Insert into Equifax_Variable values ('TIMECURRENTEMPLOYMENT_SR','0.0247509262978','N')


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='AGE' and Flag_CustomerStatus='E')
Insert into Equifax_Variable values ('AGE','0','E')	

		
IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='AVG_AGREEMENT_TOTAL_1M_SQ' and Flag_CustomerStatus='E')
Insert into Equifax_Variable values ('AVG_AGREEMENT_TOTAL_1M_SQ','2.646859707716','E')


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='AVG_BALANCE_ARREARS_12M_LN' and Flag_CustomerStatus='E')
Insert into Equifax_Variable values ('AVG_BALANCE_ARREARS_12M_LN','-0.50680132086331','E')

IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='AVG_BALANCE_ARREARS_1M_LN' and Flag_CustomerStatus='E')
Insert into Equifax_Variable values ('AVG_BALANCE_ARREARS_1M_LN','-0.05935987834987','E')

IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='BALANCEARREARS_POUND_6M' and Flag_CustomerStatus='E')
Insert into Equifax_Variable values ('BALANCEARREARS_POUND_6M','0','E')


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='BALANCEARREARS_POUND_6M_LN' and Flag_CustomerStatus='E')
Insert into Equifax_Variable values ('BALANCEARREARS_POUND_6M_LN','0','E')


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='COUNT_DAYSARREAR_30MORE_17M_LN' and Flag_CustomerStatus='E')
Insert into Equifax_Variable values ('COUNT_DAYSARREAR_30MORE_17M_LN','0','E')


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='COUNT_DAYSARREAR_60MORE_17M_LN' and Flag_CustomerStatus='E')
Insert into Equifax_Variable values ('COUNT_DAYSARREAR_60MORE_17M_LN','0','E')	


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='DAYSARREARS_POUND_6M' and Flag_CustomerStatus='E')
Insert into Equifax_Variable values ('DAYSARREARS_POUND_6M','-0.00325823942201','E')


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='EMPLOYMENTSTATUS_WOE' and Flag_CustomerStatus='E')
Insert into Equifax_Variable values ('EMPLOYMENTSTATUS_WOE','0.4426268809289','E')


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='FLAG_CUSTOMERSTATUS_HIS_WOE' and Flag_CustomerStatus='E')
Insert into Equifax_Variable values ('FLAG_CUSTOMERSTATUS_HIS_WOE','0','E')	


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='GENDER_WOE' and Flag_CustomerStatus='E')
Insert into Equifax_Variable values ('GENDER_WOE','0','E')	


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='MARITALSTATUS_WOE' and Flag_CustomerStatus='E')
Insert into Equifax_Variable values ('MARITALSTATUS_WOE','0.38162349168862','E')

IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='MAX_PERC_OUTS_3M_SQ' and Flag_CustomerStatus='E')
Insert into Equifax_Variable values ('MAX_PERC_OUTS_3M_SQ','-0.0000634270804','E')


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='MAX_PERC_OUTSARREARS_6M_LN' and Flag_CustomerStatus='E')
Insert into Equifax_Variable values ('MAX_PERC_OUTSARREARS_6M_LN','0','E')	


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='MOBILENUMBER_WOE' and Flag_CustomerStatus='E')
Insert into Equifax_Variable values ('MOBILENUMBER_WOE','0','E')	


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='NEWEST_CREDIT_SQ' and Flag_CustomerStatus='E')
Insert into Equifax_Variable values ('NEWEST_CREDIT_SQ','-0.00042911119893','E')


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='NUMBER_ACCOUNT_17M' and Flag_CustomerStatus='E')
Insert into Equifax_Variable values ('NUMBER_ACCOUNT_17M','0.10795612253096','E')


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='NUMBER_ACCOUNT_OPENED_3M' and Flag_CustomerStatus='E')
Insert into Equifax_Variable values ('NUMBER_ACCOUNT_OPENED_3M','0','E')


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='NUMBER_ACCOUNT_OPENED_3M_CR' and Flag_CustomerStatus='E')
Insert into Equifax_Variable values ('NUMBER_ACCOUNT_OPENED_3M_CR','-0.25726470098179','E')


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='NUMBERDEPENDENTS' and Flag_CustomerStatus='E')
Insert into Equifax_Variable values ('NUMBERDEPENDENTS','0','E')


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='NUMBERDEPENDENTS_CR' and Flag_CustomerStatus='E')
Insert into Equifax_Variable values ('NUMBERDEPENDENTS_CR','-0.08243045881606','E')


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='NUMBERDEPENDENTS_SQ' and Flag_CustomerStatus='E')
Insert into Equifax_Variable values ('NUMBERDEPENDENTS_SQ','0','E')


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='OCCUPATION_WOE' and Flag_CustomerStatus='E')
Insert into Equifax_Variable values ('OCCUPATION_WOE','0.59107127179839','E')


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='OLDEST_CREDIT_LN' and Flag_CustomerStatus='E')
Insert into Equifax_Variable values ('OLDEST_CREDIT_LN','0.25788368598204','E')


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='POSTCODE_WOE' and Flag_CustomerStatus='E')
Insert into Equifax_Variable values ('POSTCODE_WOE','0','E')	


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='RATIO_NDEPENT_TO_AGE' and Flag_CustomerStatus='E')
Insert into Equifax_Variable values ('RATIO_NDEPENT_TO_AGE','0','E')	


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='RATIO_TCURRENTEMPLOY_TO_AGE' and Flag_CustomerStatus='E')
Insert into Equifax_Variable values ('RATIO_TCURRENTEMPLOY_TO_AGE','0','E')	


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='RESIDENTIALSTATUS_WOE' and Flag_CustomerStatus='E')
Insert into Equifax_Variable values ('RESIDENTIALSTATUS_WOE','0.57551451353115','E')


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='TIMECURRENTADDRESS' and Flag_CustomerStatus='E')
Insert into Equifax_Variable values ('TIMECURRENTADDRESS','0','E')	


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='TIMECURRENTADDRESS_LN' and Flag_CustomerStatus='E')
Insert into Equifax_Variable values ('TIMECURRENTADDRESS_LN','0.05576635625236','E')


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='TIMECURRENTEMPLOYMENT' and Flag_CustomerStatus='E')
Insert into Equifax_Variable values ('TIMECURRENTEMPLOYMENT','0.00170617052626','E')


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='TIMECURRENTEMPLOYMENT_LN' and Flag_CustomerStatus='E')
Insert into Equifax_Variable values ('TIMECURRENTEMPLOYMENT_LN','0','E')


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='TIMECURRENTEMPLOYMENT_SR' and Flag_CustomerStatus='E')
Insert into Equifax_Variable values ('TIMECURRENTEMPLOYMENT_SR','0','E')	

END


-----******************************************************************************************************************

ELSE --IF(@Country='T') Default TT varibale insert
BEGIN
-- Script Comment : Insert Equifax_Variable
IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='AGE' and Flag_CustomerStatus='N')
Insert into Equifax_Variable values ('AGE','0.00901230642778','N')

IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='AVG_AGREEMENT_TOTAL_1M_SQ' and Flag_CustomerStatus='N')
Insert into Equifax_Variable values ('AVG_AGREEMENT_TOTAL_1M_SQ','0','N')


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='AVG_BALANCE_ARREARS_12M_LN' and Flag_CustomerStatus='N')
Insert into Equifax_Variable values ('AVG_BALANCE_ARREARS_12M_LN','0','N')


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='AVG_BALANCE_ARREARS_1M_LN' and Flag_CustomerStatus='N')
Insert into Equifax_Variable values ('AVG_BALANCE_ARREARS_1M_LN','0','N')


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='BALANCEARREARS_POUND_6M' and Flag_CustomerStatus='N')
Insert into Equifax_Variable values ('BALANCEARREARS_POUND_6M','0','N')


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='BALANCEARREARS_POUND_6M_LN' and Flag_CustomerStatus='N')
Insert into Equifax_Variable values ('BALANCEARREARS_POUND_6M_LN','0','N')


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='COUNT_DAYSARREAR_30MORE_17M_LN' and Flag_CustomerStatus='N')
Insert into Equifax_Variable values ('COUNT_DAYSARREAR_30MORE_17M_LN','0','N')


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='COUNT_DAYSARREAR_60MORE_17M_LN' and Flag_CustomerStatus='N')
Insert into Equifax_Variable values ('COUNT_DAYSARREAR_60MORE_17M_LN','0','N')


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='DAYSARREARS_POUND_6M' and Flag_CustomerStatus='N')
Insert into Equifax_Variable values ('DAYSARREARS_POUND_6M','0','N')


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='EMPLOYMENTSTATUS_WOE' and Flag_CustomerStatus='N')
Insert into Equifax_Variable values ('EMPLOYMENTSTATUS_WOE','0','N')


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='FLAG_CUSTOMERSTATUS_HIS_WOE' and Flag_CustomerStatus='N')
Insert into Equifax_Variable values ('FLAG_CUSTOMERSTATUS_HIS_WOE','0.43075297764048','N')


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='GENDER_WOE' and Flag_CustomerStatus='N')
Insert into Equifax_Variable values ('GENDER_WOE','0','N')


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='MARITALSTATUS_WOE' and Flag_CustomerStatus='N')
Insert into Equifax_Variable values ('MARITALSTATUS_WOE','0.67501445266091','N')


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='MAX_PERC_OUTS_3M_SQ' and Flag_CustomerStatus='N')
Insert into Equifax_Variable values ('MAX_PERC_OUTS_3M_SQ','0','N')


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='MAX_PERC_OUTSARREARS_6M_LN' and Flag_CustomerStatus='N')
Insert into Equifax_Variable values ('MAX_PERC_OUTSARREARS_6M_LN','0','N')


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='MOBILENUMBER_WOE' and Flag_CustomerStatus='N')
Insert into Equifax_Variable values ('MOBILENUMBER_WOE','0.48727526179351','N')


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='NEWEST_CREDIT_SQ' and Flag_CustomerStatus='N')
Insert into Equifax_Variable values ('NEWEST_CREDIT_SQ','0','N')


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='NUMBER_ACCOUNT_17M' and Flag_CustomerStatus='N')
Insert into Equifax_Variable values ('NUMBER_ACCOUNT_17M','0','N')


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='NUMBER_ACCOUNT_OPENED_3M' and Flag_CustomerStatus='N')
Insert into Equifax_Variable values ('NUMBER_ACCOUNT_OPENED_3M','0','N')


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='NUMBER_ACCOUNT_OPENED_3M_CR' and Flag_CustomerStatus='N')
Insert into Equifax_Variable values ('NUMBER_ACCOUNT_OPENED_3M_CR','0','N')


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='NUMBERDEPENDENTS' and Flag_CustomerStatus='N')
Insert into Equifax_Variable values ('NUMBERDEPENDENTS','0','N')


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='NUMBERDEPENDENTS_CR' and Flag_CustomerStatus='N')
Insert into Equifax_Variable values ('NUMBERDEPENDENTS_CR','0','N')


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='NUMBERDEPENDENTS_SQ' and Flag_CustomerStatus='N')
Insert into Equifax_Variable values ('NUMBERDEPENDENTS_SQ','0','N')


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='OCCUPATION_WOE' and Flag_CustomerStatus='N')
Insert into Equifax_Variable values ('OCCUPATION_WOE','0.81194844838299','N')


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='OLDEST_CREDIT_LN' and Flag_CustomerStatus='N')
Insert into Equifax_Variable values ('OLDEST_CREDIT_LN','0','N')


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='POSTCODE_WOE' and Flag_CustomerStatus='N')
Insert into Equifax_Variable values ('POSTCODE_WOE','0','N')


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='RATIO_NDEPENT_TO_AGE' and Flag_CustomerStatus='N')
Insert into Equifax_Variable values ('RATIO_NDEPENT_TO_AGE','6.06408716746626','N')


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='RATIO_TCURRENTEMPLOY_TO_AGE' and Flag_CustomerStatus='N')
Insert into Equifax_Variable values ('RATIO_TCURRENTEMPLOY_TO_AGE','0.06813657718939','N')


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='RESIDENTIALSTATUS_WOE' and Flag_CustomerStatus='N')
Insert into Equifax_Variable values ('RESIDENTIALSTATUS_WOE','0.42505197107747','N')


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='TIMECURRENTADDRESS' and Flag_CustomerStatus='N')
Insert into Equifax_Variable values ('TIMECURRENTADDRESS','0','N')

IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='TIMECURRENTADDRESS_LN' and Flag_CustomerStatus='N')
Insert into Equifax_Variable values ('TIMECURRENTADDRESS_LN','0','N')

IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='TIMECURRENTEMPLOYMENT' and Flag_CustomerStatus='N')
Insert into Equifax_Variable values ('TIMECURRENTEMPLOYMENT','0','N')


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='TIMECURRENTEMPLOYMENT_LN' and Flag_CustomerStatus='N')
Insert into Equifax_Variable values ('TIMECURRENTEMPLOYMENT_LN','0','N')


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='TIMECURRENTEMPLOYMENT_SR' and Flag_CustomerStatus='N')
Insert into Equifax_Variable values ('TIMECURRENTEMPLOYMENT_SR','0','N')


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='AGE' and Flag_CustomerStatus='E')
Insert into Equifax_Variable values ('AGE','0.00322246166688','E')	

		
IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='AVG_AGREEMENT_TOTAL_1M_SQ' and Flag_CustomerStatus='E')
Insert into Equifax_Variable values ('AVG_AGREEMENT_TOTAL_1M_SQ','0','E')


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='AVG_BALANCE_ARREARS_12M_LN' and Flag_CustomerStatus='E')
Insert into Equifax_Variable values ('AVG_BALANCE_ARREARS_12M_LN','0','E')


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='AVG_BALANCE_ARREARS_1M_LN' and Flag_CustomerStatus='E')
Insert into Equifax_Variable values ('AVG_BALANCE_ARREARS_1M_LN','0','E')


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='BALANCEARREARS_POUND_6M' and Flag_CustomerStatus='E')
Insert into Equifax_Variable values ('BALANCEARREARS_POUND_6M','0','E')


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='BALANCEARREARS_POUND_6M_LN' and Flag_CustomerStatus='E')
Insert into Equifax_Variable values ('BALANCEARREARS_POUND_6M_LN','0.19600841580768','E')


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='COUNT_DAYSARREAR_30MORE_17M_LN' and Flag_CustomerStatus='E')
Insert into Equifax_Variable values ('COUNT_DAYSARREAR_30MORE_17M_LN','0','E')


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='COUNT_DAYSARREAR_60MORE_17M_LN' and Flag_CustomerStatus='E')
Insert into Equifax_Variable values ('COUNT_DAYSARREAR_60MORE_17M_LN','0.4559863849995','E')	


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='DAYSARREARS_POUND_6M' and Flag_CustomerStatus='E')
Insert into Equifax_Variable values ('DAYSARREARS_POUND_6M','0','E')


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='EMPLOYMENTSTATUS_WOE' and Flag_CustomerStatus='E')
Insert into Equifax_Variable values ('EMPLOYMENTSTATUS_WOE','0','E')


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='FLAG_CUSTOMERSTATUS_HIS_WOE' and Flag_CustomerStatus='E')
Insert into Equifax_Variable values ('FLAG_CUSTOMERSTATUS_HIS_WOE','0','E')	


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='GENDER_WOE' and Flag_CustomerStatus='E')
Insert into Equifax_Variable values ('GENDER_WOE','0','E')	


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='MARITALSTATUS_WOE' and Flag_CustomerStatus='E')
Insert into Equifax_Variable values ('MARITALSTATUS_WOE','0.52261952777225','E')


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='MAX_PERC_OUTS_3M_SQ' and Flag_CustomerStatus='E')
Insert into Equifax_Variable values ('MAX_PERC_OUTS_3M_SQ','0.00006133430248','E')


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='MAX_PERC_OUTSARREARS_6M_LN' and Flag_CustomerStatus='E')
Insert into Equifax_Variable values ('MAX_PERC_OUTSARREARS_6M_LN','0','E')	


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='MOBILENUMBER_WOE' and Flag_CustomerStatus='E')
Insert into Equifax_Variable values ('MOBILENUMBER_WOE','0','E')	


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='NEWEST_CREDIT_SQ' and Flag_CustomerStatus='E')
Insert into Equifax_Variable values ('NEWEST_CREDIT_SQ','0.00077634040007','E')


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='NUMBER_ACCOUNT_17M' and Flag_CustomerStatus='E')
Insert into Equifax_Variable values ('NUMBER_ACCOUNT_17M','0','E')


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='NUMBER_ACCOUNT_OPENED_3M' and Flag_CustomerStatus='E')
Insert into Equifax_Variable values ('NUMBER_ACCOUNT_OPENED_3M','0.05472790025548','E')


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='NUMBER_ACCOUNT_OPENED_3M_CR' and Flag_CustomerStatus='E')
Insert into Equifax_Variable values ('NUMBER_ACCOUNT_OPENED_3M_CR','0','E')


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='NUMBERDEPENDENTS' and Flag_CustomerStatus='E')
Insert into Equifax_Variable values ('NUMBERDEPENDENTS','0','E')


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='NUMBERDEPENDENTS_CR' and Flag_CustomerStatus='E')
Insert into Equifax_Variable values ('NUMBERDEPENDENTS_CR','0','E')


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='NUMBERDEPENDENTS_SQ' and Flag_CustomerStatus='E')
Insert into Equifax_Variable values ('NUMBERDEPENDENTS_SQ','0.02803036004162','E')


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='OCCUPATION_WOE' and Flag_CustomerStatus='E')
Insert into Equifax_Variable values ('OCCUPATION_WOE','0.76513177312487','E')


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='OLDEST_CREDIT_LN' and Flag_CustomerStatus='E')
Insert into Equifax_Variable values ('OLDEST_CREDIT_LN','0.51712953769054','E')


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='POSTCODE_WOE' and Flag_CustomerStatus='E')
Insert into Equifax_Variable values ('POSTCODE_WOE','0','E')	


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='RATIO_NDEPENT_TO_AGE' and Flag_CustomerStatus='E')
Insert into Equifax_Variable values ('RATIO_NDEPENT_TO_AGE','0','E')	


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='RATIO_TCURRENTEMPLOY_TO_AGE' and Flag_CustomerStatus='E')
Insert into Equifax_Variable values ('RATIO_TCURRENTEMPLOY_TO_AGE','0','E')	


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='RESIDENTIALSTATUS_WOE' and Flag_CustomerStatus='E')
Insert into Equifax_Variable values ('RESIDENTIALSTATUS_WOE','0.53816488219771','E')


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='TIMECURRENTADDRESS' and Flag_CustomerStatus='E')
Insert into Equifax_Variable values ('TIMECURRENTADDRESS','0','E')	


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='TIMECURRENTADDRESS_LN' and Flag_CustomerStatus='E')
Insert into Equifax_Variable values ('TIMECURRENTADDRESS_LN','0','E')


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='TIMECURRENTEMPLOYMENT' and Flag_CustomerStatus='E')
Insert into Equifax_Variable values ('TIMECURRENTEMPLOYMENT','0','E')


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='TIMECURRENTEMPLOYMENT_LN' and Flag_CustomerStatus='E')
Insert into Equifax_Variable values ('TIMECURRENTEMPLOYMENT_LN','0','E')


IF NOT EXISTS (SELECT * FROM [Equifax_Variable] WHERE [Variable]='TIMECURRENTEMPLOYMENT_SR' and Flag_CustomerStatus='E')
Insert into Equifax_Variable values ('TIMECURRENTEMPLOYMENT_SR','0.03090140737295','E')	

END
update Equifax_Variable set variable='ratio_ndependent_to_age' where  variable='ratio_ndepent_to_age'

