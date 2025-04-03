-- transaction: true

UPDATE [Service].[Request]
SET ItemSerialNumber = NULL
WHERE
    ItemSerialNumber IN (
        'nil', -- 10632
        'nill', -- 1091
        'nilll', -- 30
        'nillll', -- 29
        'nilllll', -- 24
        'nillllll') -- 14
    OR ItemSerialNumber LIKE
        'nillllll%' -- 10632
