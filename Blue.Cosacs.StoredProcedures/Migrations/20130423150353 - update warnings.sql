update service.Request
set RepairLimitWarning = t.warning
from (
SELECT r.id, CASE WHEN sum(r.ResolutionAdditionalCost) + sum(r.ResolutionTransportCost) + sum(r.ResolutionLabourCost) + value + labour + parts >  s.costprice * 0.500 THEN 1
                  ELSE 0 END AS warning
FROM Service.Request r 
inner join stockitem s on s.itemId = r.ItemId AND s.stocklocn = r.Branch
inner join (select sum(value) value,sum(labour) labour,sum(partsother) parts, RequestId 
            from service.charge
			group by RequestId) as c on c.RequestId = r.id
CROSS JOIN Service.Request r2 
WHERE r.Id != r2.Id 
AND r.ItemNumber = r2.ItemNumber 
AND r.ItemSupplier = r2.ItemSupplier
AND (r.ItemSerialNumber = r2.ItemSerialNumber AND r.ItemSerialNumber IS NOT NULL
	  OR   r.Account = r2.Account 
	)
group by r.id, c.value, c.labour, c.parts, s.costprice) t
where t.Id = service.Request.id