-- transaction: true
DECLARE @PermId INT

SELECT @PermId = [Id] 
  FROM [Admin].[Permission]
 WHERE [Name] = 'Transactions - Overages and Shortages'

SELECT @PermId

IF (@PermId > 0)
BEGIN
  UPDATE [Admin].[Permission]
  SET [Description] =
    'Overages and Shortages - Allows access to the Overages and ' +
    'Shortages (Cashiers by branch) screen via the Transactions menu'
  WHERE [Id] = @PermId
END
