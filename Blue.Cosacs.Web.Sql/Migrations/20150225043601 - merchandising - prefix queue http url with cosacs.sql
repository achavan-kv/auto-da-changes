update hub.Queue
set SubscriberHttpUrl = '/cosacs/Financial/StockAdjustmentCreated'
,SubscriberHttpMethod='POST'
where SubscriberHttpUrl = 'Financial/StockAdjustmentCreated'

update hub.Queue
set SubscriberHttpUrl = '/cosacs/Financial/VendorReturnCreated'
,SubscriberHttpMethod='POST'
where SubscriberHttpUrl = 'Financial/VendorReturnCreated'

update hub.Queue
set SubscriberHttpUrl = '/cosacs/Financial/GoodsReceiptCreated'
,SubscriberHttpMethod='POST'
where SubscriberHttpUrl = 'Financial/GoodsReceiptCreated'

update hub.Queue
set SubscriberHttpUrl = '/cosacs/Financial/CintOrderDelivered'
,SubscriberHttpMethod='POST'
where SubscriberHttpUrl = 'Financial/CintOrderDelivered'

update hub.Queue
set SubscriberHttpUrl = '/cosacs/Financial/CintOrderReturned'
,SubscriberHttpMethod='POST'
where SubscriberHttpUrl = 'Financial/CintOrderReturned'