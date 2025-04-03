-- transaction: true

UPDATE [Admin].[Permission]
SET [Name] = 'Service Request - Deposit Update Allowed',
           -- was: Service Request - Estimates Update Allowed
    [CategoryId] = 16,
	       -- was: 11
    [Description] = 'Service Request - Allows the user to update automatic entered Deposit values'
	       -- was:   Service request - Allows the user to update previously entered repair estimates
WHERE Name = 'Service Request - Estimates Update Allowed'

-- update permissions 1614 - 'Enable Estimate Update'
UPDATE [Admin].[Permission]
SET [Name] = 'Service Request - Enable Deposit Update',
      -- was: Enable Estimate Update
    [Description] = 'Service Request - Allows the user to change Deposits after save in Service Request Screen'
	  -- was:                           Allow user to change Estimates after save in Service Request Screen.
WHERE Name = 'Enable Estimate Update'
