-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF(select countrycode from country) = 'T'
BEGIN
	IF NOT EXISTS(SELECT * FROM [Sales].[LinkedContractNames] WHERE [contract] = 'ReadyAssistTrinidad')
	BEGIN
		insert into [Sales].[LinkedContractNames]
		select 'ReadyAssistTrinidad'

		delete [Sales].[LinkedContractNames]
		where [contract] =  'ReadyAssistContract'
	END
END