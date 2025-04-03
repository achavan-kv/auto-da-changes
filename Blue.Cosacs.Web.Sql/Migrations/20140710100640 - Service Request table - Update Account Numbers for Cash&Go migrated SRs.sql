
UPDATE Service.Request
SET Account=uData.AccountNumber
FROM (
    SELECT DISTINCT InvoiceData.Id RequestId, d.acctno AccountNumber, d.agrmtno Invoice, InvoiceData.OldInvoice
    FROM delivery d
    INNER JOIN (
        SELECT
            r.id,
            r.InvoiceNumber AS OldInvoice,
            SUBSTRING(
                r.InvoiceNumber,
                CHARINDEX('/', r.InvoiceNumber) + 1,
                LEN(r.InvoiceNumber) - CHARINDEX('/', r.InvoiceNumber)
            ) InvoiceNumber
        FROM
            Service.Request r
        WHERE r.InvoiceNumber IS NOT NULL
            AND ISNUMERIC(r.InvoiceNumber)=0
            AND CHARINDEX('/', r.InvoiceNumber)>0
    ) InvoiceData
        ON d.agrmtno=InvoiceData.InvoiceNumber
) AS uData
WHERE Id = uData.RequestId
    AND InvoiceNumber=uData.OldInvoice
