-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
update hub.Queue
set SubscriberClrTypeName = 'Blue.Cosacs.Merchandising.Subscribers.CINTsSubscriber', [Binding] = 'Merchandising.Cints'
where [binding] = 'Merchandising.Cint'

update hub.Queue
set SubscriberClrTypeName = 'Blue.Cosacs.Merchandising.Subscribers.CINTSubscriber', [Binding] = 'Merchandising.Cint'
where [binding] = 'Merchandising.CintOrder'
