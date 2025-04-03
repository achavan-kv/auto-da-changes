-- transaction: true

UPDATE [Service].[Request]
SET [ResolutionDate] = [FinaliseReturnDate]
WHERE [State] = 'Closed' AND [FinaliseReturnDate] IS NOT NULL -- was finalised
    AND [ResolutionDate] IS NULL                              -- was not resolved
    AND [Resolution] NOT IN ('Save a Call', 'No Fault Found') -- was not resolved
    AND [CreatedOn] >= '20140101' -- don't update anything before 2014
