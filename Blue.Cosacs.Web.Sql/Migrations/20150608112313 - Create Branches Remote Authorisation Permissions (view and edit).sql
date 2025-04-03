
-- A quick analysis of (the file) '\src\Blue.Cosacs.Web.Sql\Migrations\20120808144941 - Set all permissions.sql', shows that, all permissions categories,
-- will most probably have deterministic values, so I'm pretty sure that catid '12' will always be the 'System Administration' category.
DECLARE @catid INT = 12

EXEC [Admin].[AddPermission] 1208, 'View Remote Authorisation Branches', @catid, 'System Administration - Allows users to view the configured branch relations for remote authorisations.'
EXEC [Admin].[AddPermission] 1209, 'Edit Remote Authorisation Branches', @catid, 'System Administration - Allows users to edit the configured branch relations for remote authorisations.'
