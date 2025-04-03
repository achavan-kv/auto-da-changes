UPDATE dbo.Task
SET TaskName = 'Installation Management Review'
WHERE TaskName LIKE '%Installation Review%'

UPDATE dbo.Control
SET ParentMenu = 'menuService'
WHERE TaskID IN(SELECT TaskID FROM dbo.Task WHERE TaskName LIKE '%Installation%Review%')
