-- transaction: true

INSERT INTO [Admin].[PermissionCategory]
VALUES(25, 'Non Stocks')

IF NOT EXISTS(SELECT * FROM [Admin].[Permission] WHERE CategoryId = 25 AND id BETWEEN 2500 AND 2508)
BEGIN
	INSERT INTO [Admin].[Permission]
		([Id], [Name], [CategoryId], [Description])
	VALUES
		(2500, 'Non Stocks - View Items', 25, 'Allows users to see non stock items.'),
		(2501, 'Non Stocks - Edit Items', 25, 'Allows users to edit non stock items.'),
		(2502, 'Non Stocks - View Price Items', 25, 'Allows users to see non stock price items.'),
		(2503, 'Non Stocks - Edit Price Items', 25, 'Allows users to edit non stock price items.'),
		(2504, 'Non Stocks - View Promotion Items', 25, 'Allows users to see non stock promotion items.'),
		(2505, 'Non Stocks - Edit Promotion Items', 25, 'Allows users to edit non stock promotion items.'),
		(2506, 'Non Stocks - View Product Link Items', 25, 'Allows users to see non stock product link items.'),
		(2507, 'Non Stocks - Edit Product Link Items', 25, 'Allows users to edit non stock product link items.'),
		(2508, 'Non Stocks - Perform Export', 25, 'Allows users to perform the export task.')
END
