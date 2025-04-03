UPDATE dbo.Control
SET ParentMenu = 'menuCollections'
WHERE TaskID IN (SELECT TaskID FROM dbo.Control WHERE Control = 'menuCommissionMaint') AND 
	  [Control] = 'menuCommissionMaint';

UPDATE dbo.Control
SET [Control] = 'menuCollections'
WHERE TaskID IN (SELECT TaskID FROM dbo.Control WHERE Control = 'menuCommissionMaint') AND 
	  [Control] = 'menuSysMaint';
	  
	  
-------------------------------------------------------------------------------------------

  
UPDATE dbo.Control 
SET ParentMenu = 'menuAllEODTasks'
WHERE Control IN ('menuEOD', 'menuTallymanExtract', 'menuEODInterface');

DELETE FROM dbo.Control 
WHERE Control = 'menuSysMaint' AND 
      TaskID IN (SELECT TaskID FROM dbo.Control WHERE Control IN ('menuEOD', 'menuTallymanExtract', 'menuEODInterface'));

INSERT INTO dbo.Control (TaskID, Screen, Control, Visible, Enabled, ParentMenu)
SELECT TaskID, Screen, 'menuAllEODTasks', Visible, Enabled, 'menuSysMaint'
FROM dbo.Control 
WHERE Control IN ('menuEOD', 'menuTallymanExtract', 'menuEODInterface');

INSERT INTO dbo.Control (TaskID, Screen, Control, Visible, Enabled, ParentMenu)
SELECT TaskID, Screen, 'menuSysMaint', Visible, Enabled, ''
FROM dbo.Control 
WHERE Control IN ('menuEOD', 'menuTallymanExtract', 'menuEODInterface');


-------------------------------------------------------------------------------------------


UPDATE dbo.Control 
SET ParentMenu = 'menuScoring'
WHERE Control IN ('menuScoringRules', 'menuScoringMatrix', 'menuTTMatrix');

DELETE FROM dbo.Control 
WHERE Control = 'menuSysMaint' AND 
      TaskID IN (SELECT TaskID FROM dbo.Control WHERE Control IN ('menuScoringRules', 'menuScoringMatrix', 'menuTTMatrix'));

INSERT INTO dbo.Control (TaskID, Screen, Control, Visible, Enabled, ParentMenu)
SELECT TaskID, Screen, 'menuScoring', Visible, Enabled, 'menuSysMaint'
FROM dbo.Control 
WHERE Control IN ('menuScoringRules', 'menuScoringMatrix', 'menuTTMatrix');

INSERT INTO dbo.Control (TaskID, Screen, Control, Visible, Enabled, ParentMenu)
SELECT TaskID, Screen, 'menuSysMaint', Visible, Enabled, ''
FROM dbo.Control 
WHERE Control IN ('menuScoringRules', 'menuScoringMatrix', 'menuTTMatrix');