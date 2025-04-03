-- transaction: true


IF NOT EXISTS(SELECT 1 FROM [Admin].Permission WHERE ID = 803)
    INSERT INTO [Admin].Permission 
		(CategoryId, Id, Name, Description) 
	VALUES
		(8, 803, 'Payments - Bank Maintenance', 'Grants users access to the Bank Maintenance screeen')

IF NOT EXISTS(SELECT 1 FROM [Admin].Permission WHERE ID = 804)
    INSERT INTO [Admin].Permission 
		(CategoryId, Id, Name, Description) 		
	VALUES
		(8, 804, 'Payments - Payment Methods Setup', 'Grants users access to the Payment Methods Setup maintenance screeen')

IF NOT EXISTS(SELECT 1 FROM [Admin].Permission WHERE ID = 805)
    INSERT INTO [Admin].Permission 
		(CategoryId, Id, Name, Description) 	
    VALUES 
		(8, 805, 'Payments - Exchange Rate', 'Grants users access to the Exchange Rate maintenance screeen')
