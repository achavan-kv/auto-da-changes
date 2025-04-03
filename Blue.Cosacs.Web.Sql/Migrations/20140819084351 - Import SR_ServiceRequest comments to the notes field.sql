
ALTER TABLE [Service].[Comment]
ALTER COLUMN [Text] VARCHAR(MAX) NOT NULL
GO

INSERT INTO [Service].[Comment] ([RequestId], [Date], [AddedBy], [Text])
SELECT
    old_r.ServiceRequestNo,
    old_r.DateLogged,
    u.FullName,
    old_r.Comments
FROM Service.Request r
INNER JOIN SR_ServiceRequest old_r
    ON r.Id = old_r.ServiceRequestNo
LEFT JOIN Admin.[User] u
    ON old_r.LoggedBy=u.Id
