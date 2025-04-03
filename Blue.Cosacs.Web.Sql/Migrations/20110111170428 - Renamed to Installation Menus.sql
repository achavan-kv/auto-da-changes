UPDATE dbo.Task
SET TaskName = 'Pending Installations'
WHERE TaskName LIKE '%Installation%Review%'

UPDATE dbo.Control
SET Control = 'menuPendingInstallations'
WHERE Control LIKE 'menuInstallationReview'
