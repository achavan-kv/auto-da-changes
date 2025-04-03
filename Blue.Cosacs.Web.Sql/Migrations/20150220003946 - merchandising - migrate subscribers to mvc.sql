-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
UPDATE Hub.Queue set [binding] = 'Merchandisnig.GoodsReceiptCreated' Where ID = 211
UPDATE Hub.Queue set [binding] = 'Merchandisnig.StockAdjustmentCreated' Where ID = 209
UPDATE Hub.Queue set [binding] = 'Merchandisnig.VendorReturnCreated' Where ID = 210

UPDATE [Hub].[Queue] 
SET [Schema]= N'<xs:schema xmlns="http://www.bluebridgeltd.com/cosacs/2012/schema.xsd" xmlns:mstns="http://www.bluebridgeltd.com/cosacs/2012/schema.xsd" xmlns:xs="http://www.w3.org/2001/XMLSchema" targetNamespace="http://www.bluebridgeltd.com/cosacs/2012/schema.xsd" elementFormDefault="qualified"><xs:element name="GoodsReceiptReview"><xs:complexType><xs:sequence><xs:element name="GoodsReceiptId" minOccurs="1" maxOccurs="1" type="xs:int" /><xs:element name="FinanceEmailAddress" minOccurs="1" maxOccurs="1"><xs:simpleType><xs:restriction base="xs:string"><xs:maxLength value="300" /></xs:restriction></xs:simpleType></xs:element><xs:element name="AbsoluteReviewUrl" minOccurs="1" maxOccurs="1"><xs:simpleType><xs:restriction base="xs:string"><xs:maxLength value="300" /></xs:restriction></xs:simpleType></xs:element><xs:element name="NumberOfItemsToReview" minOccurs="1" maxOccurs="1" type="xs:int" /></xs:sequence></xs:complexType></xs:element></xs:schema>'
WHERE [Binding]='Merchandising.GoodsReceipt'



UPDATE Hub.Queue set subscriberclrassemblyname = null, subscriberclrtypename = null, subscriberhttpurl = '/cosacs/Merchandising/CINTSubscriber', subscriberhttpmethod = 'POST' where [Binding] = 'Merchandising.Cint'
UPDATE Hub.Queue set subscriberclrassemblyname = null, subscriberclrtypename = null, subscriberhttpurl = '/cosacs/Merchandising/CINTsSubscriber', subscriberhttpmethod = 'POST' where [Binding] = 'Merchandising.Cints'
UPDATE Hub.Queue set subscriberclrassemblyname = null, subscriberclrtypename = null, subscriberhttpurl = '/cosacs/Merchandising/VendorMailSubscriber', subscriberhttpmethod = 'POST' where [Binding] = 'Merchandising.VendorMail'
UPDATE Hub.Queue set subscriberclrassemblyname = null, subscriberclrtypename = null, subscriberhttpurl = '/cosacs/Merchandising/GoodsReceiptReviewSubscriber', subscriberhttpmethod = 'POST' where [Binding] = 'Merchandising.GoodsReceipt'
UPDATE Hub.Queue set subscriberclrassemblyname = null, subscriberclrtypename = null, subscriberhttpurl = '/cosacs/Merchandising/VendorSubscriber', subscriberhttpmethod = 'POST' where [Binding] = 'Merchandising.Vendor'
UPDATE Hub.Queue set subscriberclrassemblyname = null, subscriberclrtypename = null, subscriberhttpurl = '/cosacs/Merchandising/VendorsSubscriber', subscriberhttpmethod = 'POST' where [Binding] = 'Merchandising.Vendors'
UPDATE Hub.Queue set subscriberclrassemblyname = null, subscriberclrtypename = null, subscriberhttpurl = '/cosacs/Merchandising/ProductSubscriber', subscriberhttpmethod = 'POST' where [Binding] = 'Merchandising.Product'
UPDATE Hub.Queue set subscriberclrassemblyname = null, subscriberclrtypename = null, subscriberhttpurl = '/cosacs/Merchandising/ProductsSubscriber', subscriberhttpmethod = 'POST' where [Binding] = 'Merchandising.Products'
UPDATE Hub.Queue set subscriberclrassemblyname = null, subscriberclrtypename = null, subscriberhttpurl = '/cosacs/Merchandising/PurchaseOrderSubscriber', subscriberhttpmethod = 'POST' where [Binding] = 'Merchandising.PurchaseOrder'
