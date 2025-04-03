DECLARE @UserFullName VARCHAR(101) = ''

SELECT @UserFullName = FullName
FROM [Admin].[User] u
WHERE Id = 0

UPDATE Warranty.WarrantySale
SET SoldBy = @UserFullName
WHERE SoldById = 0
