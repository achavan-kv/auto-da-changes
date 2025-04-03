-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF NOT EXISTS (SELECT * FROM dbo.CountryMaintenance WHERE CodeName = 'SCManualCardOverride')
BEGIN
	--Late Payment Fees – applied if payment made after due date
	INSERT INTO CountryMaintenance
	(CountryCode,ParameterCategory,
	NAME,Value,[Type],PRECISION,
	OptionCategory,OptionListName,Description,
	CodeName)
	SELECT CountryCode,'33',
	'Manual Card Override Password','False','checkbox','0',
	'','','Password required for Manual Card Entry in Payment and Cash and Go Screens',
	'SCManualCardOverride' FROM dbo.Country
END 


DECLARE @taskid INT 
SELECT @taskid =MAX(TaskID) +1 FROM [task]
IF NOT EXISTS (SELECT * FROM [Control] WHERE [Control]='mtb_Cardno' and screen = 'Payment')
BEGIN
    insert into control 
    (TaskID      ,Screen, Control, Visible   ,  Enabled    , ParentMenu)  
    values
    (@taskid,'Payment','mtb_Cardno',0,1,'')

    insert into task
    (taskid,taskname)
    values
    (@taskid,'Payment - Manual Store Card Entry')	
END

GO 
