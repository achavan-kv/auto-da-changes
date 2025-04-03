-- transaction: true

DELETE warranty.WarrantyReturnFilter
DELETE Warranty.WarrantyReturn


INSERT INTO Warranty.WarrantyReturn (WarrantyLength, ElapsedMonths, PercentageReturn, BranchType, BranchNumber, WarrantyId, [Level_1], FreeWarrantyLength)
    SELECT DISTINCT (WarrantyMonths-ManufacturerMonths), MonthSinceDelivery, refundpercentfromAIG, NULL, NULL, NULL, 'PCE', ManufacturerMonths
    FROM WarrantyReturnCodes 
    WHERE ProductType='E'
    ORDER BY (WarrantyMonths-ManufacturerMonths), ManufacturerMonths, MonthSinceDelivery

INSERT INTO Warranty.WarrantyReturn (WarrantyLength, ElapsedMonths, PercentageReturn, BranchType, BranchNumber, WarrantyId, [Level_1], FreeWarrantyLength)
    SELECT DISTINCT (WarrantyMonths-ManufacturerMonths), MonthSinceDelivery, refundpercentfromAIG, NULL, NULL, NULL, 'PCF', ManufacturerMonths
    FROM WarrantyReturnCodes 
    WHERE ProductType='F'
    ORDER BY (WarrantyMonths-ManufacturerMonths), ManufacturerMonths, MonthSinceDelivery


INSERT INTO Warranty.WarrantyReturnFilter (WarrantyReturnId, TagId)
    SELECT r.id, t.Id
    FROM Warranty.WarrantyReturn r,
        Warranty.[Level] l
        INNER JOIN Warranty.Tag t ON l.id=t.LevelId
    WHERE (l.Name='Department' AND t.Name='Electrical'
        AND r.WarrantyId is NULL AND r.level_1='PCE')
	    AND NOT EXISTS( SELECT * FROM Warranty.WarrantyReturnFilter f WHERE f.WarrantyReturnId=r.id AND f.tagid = t.id )

INSERT INTO Warranty.WarrantyReturnFilter (WarrantyReturnId, TagId)
    SELECT r.id, t.Id
    FROM Warranty.WarrantyReturn r,
        Warranty.[Level] l
        INNER JOIN Warranty.Tag t ON l.id=t.LevelId
    WHERE (l.Name='Department' AND t.Name='Furniture'
        AND r.WarrantyId is NULL AND r.level_1='PCF')
	    AND NOT EXISTS( SELECT * FROM Warranty.WarrantyReturnFilter f WHERE f.WarrantyReturnId=r.id AND f.tagid = t.id )
