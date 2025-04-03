-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here


UPDATE
    Service.RequestPart
SET
    StockBranch = NULL,
    [Source] = 'External'
FROM 
    Service.RequestPart p
INNER JOIN 
    (
        SELECT 
            rp.Id,
            rp.RequestId 
        FROM
            Service.RequestPart rp
        INNER JOIN
            Service.Request srr on rp.RequestId = srr.Id
        INNER JOIN 
            SR_ServiceRequest s on rp.RequestId = s.ServiceRequestNo
        INNER JOIN 
            SR_PartListResolved p on s.ServiceRequestNo = p.ServiceRequestNo
        WHERE 
            rp.PartNumber = p.PartNo
        AND p.PartID = 0
        AND rp.StockBranch is not null
        AND rp.Source = 'Internal'
        AND srr.[State] != 'Closed'
    ) ReqPartToFix on ReqPartToFix.RequestId = p.RequestId and ReqPartToFix.Id = p.Id