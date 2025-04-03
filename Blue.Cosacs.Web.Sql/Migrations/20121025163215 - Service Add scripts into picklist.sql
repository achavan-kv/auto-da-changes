INSERT INTO Config.PickList
        ( Id, Name )
VALUES  ( 'ServiceQuestions', -- Id - varchar(100)
          'Service Questions'  -- Name - varchar(100)
          )

INSERT INTO Config.PickRow
        ( ListId, [Order], String )
SELECT 'ServiceQuestions',  sortorder, codedescript FROM dbo.code
WHERE category = 'SRSCRIPT'
ORDER BY sortorder