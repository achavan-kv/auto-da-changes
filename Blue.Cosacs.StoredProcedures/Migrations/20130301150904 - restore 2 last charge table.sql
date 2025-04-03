INSERT INTO Config.DecisionTable
        ( [Key], CreatedUtc, Value )
select 'SR.DecisionTable.Charge', -- Key - varchar(50)
          GETDATE(), -- CreatedUtc - datetime
          Value FROM Config.DecisionTable
          WHERE [key] = 'SR.DecisionTable.Charge'
		  AND id = (SELECT MAX(id) FROM Config.DecisionTable d
		             WHERE [key] = 'SR.DecisionTable.Charge'
		             AND id !=  (SELECT MAX(id) FROM Config.DecisionTable d2
								 WHERE [key] = 'SR.DecisionTable.Charge'))
								 
