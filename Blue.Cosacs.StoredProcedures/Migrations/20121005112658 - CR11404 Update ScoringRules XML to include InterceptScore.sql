-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

INSERT INTO dbo.ScoringRules
        ( CountryCode ,
          RulesXML ,
          DateImported ,
          ImportedBy ,
          region ,
          filename
        )
select   CountryCode,
          cast(replace(cast(RulesXML as nvarchar(max)),'Region','InterceptScore="0" Region') as ntext) ,
          GETDATE() , 
          ImportedBy, 
          region,
          filename
        
FROM dbo.ScoringRules
WHERE DateImported = (SELECT MAX(dateimported) FROM dbo.ScoringRules)