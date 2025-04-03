-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
  UPDATE [Admin].[Permission]
  SET Name = 'Amend Shipment', Description = 'Allows user to Amend or Reject Shipments and Exceptions'
  WHERE Name = 'Amend Booking' AND CategoryId = (SELECT id FROM [Admin].[PermissionCategory] WHERE Name = 'Warehouse')

  UPDATE [Admin].[Permission]
  SET Name = 'Shipment Search Screen', Description = 'Allows access to the Search Shipment screen'
  WHERE Name = 'Booking Search Screen' AND CategoryId = (SELECT id FROM [Admin].[PermissionCategory] WHERE Name = 'Warehouse')
