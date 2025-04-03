-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here


SELECT qm.Id AS QueueMessageId, 
       qm.Exception AS Error, 
       m.Id AS MessageId, 
       CAST(m.Body AS VARCHAR(MAX)) AS MessageBody,
       m.CorrelationId AS RunNo
INTO #temp
FROM Hub.QueueMessage qm
INNER JOIN Hub.[Message] m
ON qm.MessageId = m.Id
WHERE qm.QueueId = 200 
    AND [State] = 'P'

UPDATE #temp
SET MessageBody = REPLACE(MessageBody, 'CintSubmit', 'CintOrderSubmit')

UPDATE #temp
SET MessageBody = REPLACE(MessageBody, '<RunNo>' + CAST(RunNo AS VARCHAR) + '</RunNo>', '')

UPDATE #temp
SET MessageBody = LEFT(MessageBody, CHARINDEX('<StockLocation>', MessageBody, 0) - 1) +
                  CHAR(13) + CHAR(10) + '<RunNo>' + CAST(RunNo AS VARCHAR) + '</RunNo>' +
                  CHAR(13) + CHAR(10) + '<Error>Old style error</Error>' +
                  CHAR(13) + CHAR(10) + RIGHT(MessageBody, LEN(MessageBody) - CHARINDEX('<StockLocation>', MessageBody, 0) + 1)

UPDATE m
SET m.Body = CAST(t.MessageBody AS XML)
FROM Hub.Message m
INNER JOIN #temp t
ON m.Id = t.MessageId

UPDATE Hub.QueueMessage
SET RunCount = 0,
    Exception = NULL,
    DispatchedOn = NULL,
    [State] = 'I'
WHERE Id IN (SELECT QueueMessageId FROM #temp)

DROP TABLE #temp