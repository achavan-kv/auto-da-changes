
UPDATE [Service].[Request]
SET [State]='New'
FROM [Service].[TechnicianBooking] b
WHERE
    [Service].[Request].[State] = 'Awaiting repair'
    AND [Service].[Request].Id NOT IN (SELECT RequestId FROM [Service].[TechnicianBooking])
