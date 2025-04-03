-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF NOT EXISTS (SELECT * FROM dbo.CountryMaintenance WHERE CodeName = 'STMinBalanceforFee')
BEGIN
	INSERT INTO CountryMaintenance(CountryCode,
								   ParameterCategory,
								   NAME,
								   Value,
								   [Type],
								   [PRECISION],
								   OptionCategory,
								   OptionListName,
								   [Description],
								   CodeName)

	SELECT CountryCode,
		   '33',
		   'Store Card Fee Small Balance Amount',
		   CONVERT(VARCHAR,smallbalance),
		   'numeric',
		   '2',
		   '',
		   '',
		   'Balances below this will not incur Fees for non-payment',
		   'STMinBalanceforFee' 
	FROM dbo.Country
END
 