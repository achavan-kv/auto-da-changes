UPDATE dbo.Installation
SET Status = 'Booked'
WHERE Status = 'Allocated'
