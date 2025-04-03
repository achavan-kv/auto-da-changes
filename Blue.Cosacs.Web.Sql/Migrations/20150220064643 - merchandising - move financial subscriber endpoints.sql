update [Hub].[Queue]
set SubscriberHttpUrl = 'Financial/StockAdjustmentCreated'
where SubscriberHttpUrl = 'Merchandising/StockAdjustmentCreated'

update [Hub].[Queue]
set SubscriberHttpUrl = 'Financial/VendorReturnCreated'
where SubscriberHttpUrl = 'Merchandising/VendorReturnCreated'

update [Hub].[Queue]
set SubscriberHttpUrl = 'Financial/GoodsReceiptCreated'
where SubscriberHttpUrl = 'Merchandising/GoodsReceiptCreated'

update [Hub].[Queue]
set SubscriberHttpUrl = 'Financial/CintOrderDelivered'
where SubscriberHttpUrl = 'Merchandising/CintOrderDelivered'

update [Hub].[Queue]
set SubscriberHttpUrl = 'Financial/CintOrderReturned'
where SubscriberHttpUrl = 'Merchandising/CintOrderReturned'
