-- transaction: true

DECLARE @PaymentsPermissionCategoryId INT = 8; -- this should already exist in Cosacs

IF NOT EXISTS (SELECT 1 FROM [Admin].PermissionCategory WHERE Id = @PaymentsPermissionCategoryId)
    INSERT INTO [Admin].PermissionCategory ([Id], [Name]) VALUES (@PaymentsPermissionCategoryId, 'Payments')
