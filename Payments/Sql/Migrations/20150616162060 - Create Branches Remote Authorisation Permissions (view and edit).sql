-- transaction: true

IF NOT EXISTS(SELECT 1 FROM [Admin].Permission WHERE ID = 801)
	INSERT INTO [Admin].Permission 
		(CategoryId, Id, Name, Description) 
	VALUES
		(8, 801, 'Payments - Payments Search', 'Grants users access to the payments search screeen')
	
IF NOT EXISTS(SELECT 1 FROM [Admin].Permission WHERE ID = 802)
	INSERT INTO [Admin].Permission 
		(CategoryId, Id, Name, Description) 
	VALUES
		(8, 802, 'Payments - Load Payments Screen', 'Grants users access to the payments screeen')
