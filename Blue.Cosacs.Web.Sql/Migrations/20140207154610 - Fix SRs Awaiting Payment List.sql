
UPDATE Service.Request
SET Service.Request.State = 'Awaiting payment',
    Service.Request.IsClosed = 0
    FROM (
        SELECT Service.Request.Id --, ac.outstbal, a.ChargeType, a.AcctNo, re.ChargeTo, r.*, a.*, ac.*, re.*
          FROM Service.Request
          JOIN SR_ServiceRequest r ON Service.Request.Id = r.ServiceRequestNo
          JOIN SR_Resolution re ON r.ServiceRequestNo = re.ServiceRequestNo
          JOIN SR_ChargeAcct a ON r.ServiceRequestNo = a.ServiceRequestNo
          JOIN acct ac ON a.AcctNo = ac.AcctNo
         WHERE r.ServiceType IN ('C','N') AND
               re.ChargeTo IN ('CUS', 'DEL','SUP') AND
               a.ChargeType in ('C', 'D') AND
               ac.outstbal > 0
      --ORDER BY r.ServiceRequestNo
   ) sub
WHERE Service.Request.Id = sub.Id
