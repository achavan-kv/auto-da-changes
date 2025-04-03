UPDATE Admin.Permission
SET Name = 'Manage Warranty Levels', Description = 'Allow the user to add/update warranty levels'
where Id = 1801

UPDATE Admin.Permission
SET Name = 'Manage Warranty Tags', Description = 'Allow the user to add/update warranty tags'
where Id = 1802


delete from Admin.Permission
where Id in (1803, 1804, 1805, 1806)