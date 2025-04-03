-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

-- Contacts
    INSERT INTO Service.Requestcontact 
        (Requestid, Type, Value)
    SELECT
        r.Id,
        'HomePhone',
        LTRIM(REPLACE((c.DialCode + ' ' + c.telno), '  ', ' '))
    FROM
        service.Request AS R 
        INNER JOIN custtel c
        on c.custid = r.CustomerId
    WHERE 
        c.tellocn = 'H'
    AND c.datediscon is null
    AND not exists (select 'a' from Service.Requestcontact where RequestId = r.Id and [type] = 'HomePhone')


	INSERT INTO Service.Requestcontact 
        (Requestid, Type, Value)
    SELECT
        r.Id,
        'MobilePhone',
        LTRIM(REPLACE((c.DialCode + ' ' + c.telno), '  ', ' '))
    FROM
        service.Request AS R 
        INNER JOIN custtel c
        on c.custid = r.CustomerId
    WHERE 
        c.tellocn = 'M'
    AND c.datediscon is null
    AND not exists (select 'a' from Service.Requestcontact where RequestId = r.Id and [type] = 'MobilePhone')


	INSERT INTO Service.Requestcontact 
        (Requestid, Type, Value)
    SELECT
        r.Id,
        'WorkPhone',
        LTRIM(REPLACE((c.DialCode + ' ' + c.telno), '  ', ' '))
    FROM
        service.Request AS R 
        INNER JOIN custtel c
        on c.custid = r.CustomerId
    WHERE 
        c.tellocn = 'W'
    AND c.datediscon is null
    AND not exists (select 'a' from Service.Requestcontact where RequestId = r.Id and [type] = 'WorkPhone')