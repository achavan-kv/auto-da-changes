UPDATE cs
SET QtyOrdered = 0, QtyDelivered = 0, QtyRepossessed = 0, QtyReturned = 0
FROM Merchandising.CintOrderStats cs
INNER JOIN  cancellation c on c.acctno = cs.PrimaryReference
