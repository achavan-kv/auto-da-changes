UPDATE dbo.Control 
SET ParentMenu = 'menuCollections'
WHERE Control IN ('menuLetterMerge', 'menuZoneAutomation', 'menuWorkLists', 'menuStrategyConfiguration', 'menuSMS')


INSERT INTO dbo.Control (TaskID, Screen, Control, Visible, Enabled, ParentMenu)
SELECT TaskID, Screen, 'menuCollections', Visible, Enabled, ''
FROM dbo.Control 
WHERE Control IN ('menuLetterMerge', 'menuZoneAutomation', 'menuWorkLists', 'menuStrategyConfiguration', 'menuSMS')