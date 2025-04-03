IF EXISTS (SELECT * FROM sys.objects so
           INNER JOIN sys.schemas ss
               ON so.schema_id = ss.schema_id
           WHERE so.type = 'P'
               AND so.name = 'BookingDetails'
               AND ss.name = 'Warehouse')
BEGIN
    DROP PROCEDURE Warehouse.BookingDetails
END
GO

CREATE PROCEDURE Warehouse.BookingDetails
    @id int
AS
BEGIN
    DECLARE @parentid VARCHAR(10)

    SELECT @parentid = SUBSTRING(b1.path, 0, CHARINDEX('.', b1.path))
    FROM Warehouse.booking b1
    WHERE b1.Id = @id

    SELECT b.*, c.UserId AS CancelUser, c.date AS CancelDate, c.Reason AS CancelReason, p.ConfirmedOn AS PickingConfirmedOn 
    FROM Warehouse.booking b
    LEFT OUTER JOIN Warehouse.Picking p
        ON p.id = b.pickingId
    LEFT OUTER JOIN Warehouse.Cancellation c
        ON b.id = c.id 
    WHERE b.Path like @parentid + '.%'
END
GO
