-- ******************  [Admin].[Permission]  *****************************
IF NOT EXISTS (SELECT 1 FROM  [Admin].[Permission] WHERE ID = 2172 AND CategoryId = 21)
BEGIN
	INSERT INTO [Admin].[Permission] (Id, Name, CategoryId, Description, IsDelegate)
	  VALUES (2172, 'View Product Attributes', 21, 'Allows User View the Product Attributes', 0);
	-- View (Read Only) Product Attributes view 
END

IF NOT EXISTS (SELECT 1 FROM  [Admin].[Permission] WHERE ID = 2173 AND CategoryId = 21)
BEGIN
	INSERT INTO [Admin].[Permission] (Id, Name, CategoryId, Description, IsDelegate)
	  VALUES (2173, 'Create/Edit Product Attributes', 21, 'Allows User Create/Edit the Product Attributes', 0);
	-- Crrate/Edit (Edit) Product Attributes Create/Edit 
END

IF NOT EXISTS (SELECT 1 FROM  [Admin].[Permission] WHERE ID = 2174 AND CategoryId = 21)
BEGIN
	INSERT INTO [Admin].[Permission] (Id, Name, CategoryId, Description, IsDelegate)
	  VALUES (2174, 'View the Min. & Max. range value for Stock', 21, 'Allow User View the Min. & Max. range value for Stock', 0);
	--  View (Read Only)  Min & Max Value View   
END

IF NOT EXISTS (SELECT 1 FROM  [Admin].[Permission] WHERE ID = 2175 AND CategoryId = 21)
BEGIN
	INSERT INTO [Admin].[Permission] (Id, Name, CategoryId, Description, IsDelegate)
	  VALUES (2175, 'Create/Edit the Min. & Max. range value for Stock', 21, 'Allow User Create/Edit the Min. & Max. range value for Stock', 0);
	-- Create/Edit  (Edit)  Min & Max range value stock 
END
IF NOT EXISTS (SELECT 1 FROM  [Admin].[Permission] WHERE ID = 2176 AND CategoryId = 21)
BEGIN
	INSERT INTO [Admin].[Permission] (id, Name, categoryId, Description, IsDelegate)
	  VALUES (2176, 'PO First Level Approval/Rejection', 21, 'First Level Approval/Rejection for Ashley PO', 0);
END
IF NOT EXISTS (SELECT 1 FROM  [Admin].[Permission] WHERE ID = 2177 AND CategoryId = 21)
BEGIN
	INSERT INTO [Admin].[Permission] (id, Name, categoryId, Description, IsDelegate)
	  VALUES (2177, 'PO Second Level Approval/Rejection', 21, 'Second Level Approval/Rejection for Ashley PO', 0);
END



