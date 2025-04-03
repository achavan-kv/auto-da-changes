insert into admin.Permission
(ID,Name, CategoryId, Description)
select 1807, 'View Warranty Pricing', 18, 'Allow the user to view the warranty pricing screen.' UNION ALL
SELECT 1808, 'Edit Warranty Pricing', 18,'Allow the user to change the warranty pricing.'