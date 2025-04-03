-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
DECLARE @schema XML = '<xs:schema xmlns="http://www.bluebridgeltd.com/cosacs/2012/schema.xsd" xmlns:mstns="http://www.bluebridgeltd.com/cosacs/2012/schema.xsd" xmlns:xs="http://www.w3.org/2001/XMLSchema" targetNamespace="http://www.bluebridgeltd.com/cosacs/2012/schema.xsd" elementFormDefault="qualified">
  <xs:element name="WarehouseDeliver">
    <xs:complexType>
      <xs:sequence>
        <xs:element name="Id" minOccurs="1" maxOccurs="1" type="xs:int" />
        <xs:element name="OrigBookingId" minOccurs="1" maxOccurs="1" type="xs:int" />
        <xs:element name="Quantity" minOccurs="1" maxOccurs="1" type="xs:int" />
        <xs:element name="Date" minOccurs="1" maxOccurs="1" type="xs:dateTime" />
        <xs:element name="ConfirmedBy" minOccurs="1" maxOccurs="1" type="xs:int" />
        <xs:element name="CustomerAccount" minOccurs="0" maxOccurs="1">
          <xs:simpleType>
            <xs:restriction base="xs:string">
              <xs:maxLength value="12" />
            </xs:restriction>
          </xs:simpleType>
        </xs:element>
      </xs:sequence>
    </xs:complexType>
  </xs:element>
</xs:schema>'

INSERT INTO [Hub].[Queue]
	(Id, [Binding], [Schema], SubscriberClrAssemblyName, SubscriberClrTypeName, SubscriberSqlConnectionName, SubscriberSqlProcedureName, SchemaSource, SubscriberHttpUrl, SubscriberHttpMethod)
VALUES
	(19,
	'Cosacs.Booking.Deliver',
	@schema,
	NULL,
	NULL,
	NULL,
	NULL,
	'WarehouseDeliver.xsd',
	'/SalesManagement/api/Jobs/CustomerFollowUpCalls',
	'GET')