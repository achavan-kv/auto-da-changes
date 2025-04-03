INSERT INTO Service.RequestScriptLookup
        ( Question, Active, [Order] )
SELECT 'Have you read the instruction manual?',1,1 UNION ALL
SELECT 'Is the item under warranty?',1,1 UNION ALL
SELECT 'Is the item in use domestically?',1,1 UNION ALL
SELECT 'Was the item installed by qualified personnel?',1,1 UNION ALL
SELECT 'Has the item been moved from the delivery address?',1,1 

