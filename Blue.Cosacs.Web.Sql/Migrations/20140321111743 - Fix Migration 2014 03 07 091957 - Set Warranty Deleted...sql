UPDATE Warranty.Warranty
SET Deleted = 0 -- Undelete wrongly deleted products
FROM Warranty.Warranty w INNER JOIN (
    -- select products that still exist (at least in one of the branches)
    SELECT DISTINCT itemno FROM warranty.Warranty w
    INNER JOIN stockitem s ON w.number = s.itemno
    WHERE category IN (12, 82) AND s.deleted = 'N'
) Sub ON REPLACE(w.number,'M','') = Sub.itemno
