INSERT INTO Admin.Permission
        ( Id, Name, CategoryId, Description )
SELECT '380','Allow Access for Telephone Callers',3,'Allow allocation of accounts for collections in Worklist & Collection Commissions Screens' UNION ALL
SELECT '381','Allow Access for Collectors',3,'Allow allocation of accounts for collections in Zone Automated Allocation Screen'


INSERT INTO Admin.RolePermission
        ( RoleId, PermissionId, [Deny] )
SELECT id,380,0 FROM Admin.Role
WHERE name in ( SELECT codedescript FROM dbo.code
				WHERE reference = 1
				AND category = 'et1')

INSERT INTO Admin.RolePermission
        ( RoleId, PermissionId, [Deny] )
SELECT id,380,0 FROM Admin.Role
WHERE name in (SELECT codedescript FROM dbo.code
				WHERE reference = 3
				AND category = 'et1') UNION ALL
SELECT id,381,0 FROM Admin.Role
WHERE name in (	SELECT codedescript FROM dbo.code
				WHERE reference = 3
				AND category = 'et1')

