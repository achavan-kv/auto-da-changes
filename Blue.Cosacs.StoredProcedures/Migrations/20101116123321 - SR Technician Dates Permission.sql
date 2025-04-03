-- put your SQL code here

INSERT INTO Task (TaskID, TaskName)
SELECT MAX(TaskId) +1, 'Service Request - Technician Unavailable Dates'
FROM Task


INSERT INTO Control
SELECT MAX(t.TaskId), Screen,Control,Visible,Enabled,ParentMenu
FROM Task t, Control c
WHERE c.Control = 'menuTechDiary'
group by Screen,Control,Visible,Enabled,ParentMenu