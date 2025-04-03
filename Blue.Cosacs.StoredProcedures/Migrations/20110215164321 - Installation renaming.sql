UPDATE dbo.Task
SET TaskName = 'Installation - Pending Installations'
WHERE TaskName = 'Pending Installations'

UPDATE dbo.Task
SET TaskName = 'Installation - Rebook Technician'
WHERE TaskName = 'Installation - Reassign Technician'