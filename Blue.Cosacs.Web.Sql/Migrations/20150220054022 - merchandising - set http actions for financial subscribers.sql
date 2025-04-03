update hub.Queue
set  SubscriberHttpMethod = 'GET'
	,SubscriberHttpUrl = 'Merchandising/StockAdjustmentCreated'
	,Binding = 'Merchandising.StockAdjustmentCreated'
where
	Binding = 'Merchandisnig.StockAdjustmentCreated'

update hub.Queue
set  SubscriberHttpMethod = 'GET'
	,SubscriberHttpUrl = 'Merchandising/VendorReturnCreated'
	,Binding = 'Merchandising.VendorReturnCreated'
where
	Binding = 'Merchandisnig.VendorReturnCreated'

update hub.Queue
set  SubscriberHttpMethod = 'GET'
	,SubscriberHttpUrl = 'Merchandising/GoodsReceiptCreated'
	,Binding = 'Merchandising.GoodsReceiptCreated'
where
	Binding = 'Merchandisnig.GoodsReceiptCreated'

update hub.Queue
set  SubscriberHttpMethod = 'GET'
	,SubscriberHttpUrl = 'Merchandising/CintOrderReturned'
where
	Binding = 'Merchandising.CintOrderReturned'

update hub.Queue
set  SubscriberHttpMethod = 'GET'
	,SubscriberHttpUrl = 'Merchandising/CintOrderDelivered'
where
	Binding = 'Merchandising.CintOrderDelivered'