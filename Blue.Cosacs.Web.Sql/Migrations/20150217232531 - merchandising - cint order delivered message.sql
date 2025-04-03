INSERT [Hub].[Queue] ([Id],[Binding],[Schema],[SubscriberClrAssemblyName],[SubscriberClrTypeName],[SubscriberSqlConnectionName],[SubscriberSqlProcedureName],[SchemaSource],[SubscriberHttpUrl],[SubscriberHttpMethod]) 
VALUES 
	(212, N'Merchandising.CintOrderDelivered','', N'Blue.Cosacs.Merchandising', N'Blue.Cosacs.Merchandising.Subscribers.CintOrderDelivered', NULL, NULL, N'Merchandising.xsd', NULL, NULL)

UPDATE [Hub].[Queue] 
SET [Schema]= N'<xs:schema xmlns="http://www.bluebridgeltd.com/cosacs/2012/schema.xsd" xmlns:mstns="http://www.bluebridgeltd.com/cosacs/2012/schema.xsd" xmlns:xs="http://www.w3.org/2001/XMLSchema" targetNamespace="http://www.bluebridgeltd.com/cosacs/2012/schema.xsd" elementFormDefault="qualified">
  <xs:element name="CintOrderId" type="xs:int"/>
  <xs:element name="Reference" type="xs:string"/>
  <xs:element name="SaleType" type="xs:string"/>
  <xs:element name="SaleLocationId" type="xs:string"/>
  <xs:element name="ProductId" type="xs:int"/>
  <xs:element name="TotalAWC" type="xs:float"/>
  <xs:element name="CintOrderReceiptMessage">
    <xs:complexType>
      <xs:sequence>
        <xs:element ref="CintOrderId"/>
        <xs:element ref="Reference"/>
        <xs:element ref="SaleType"/>
        <xs:element ref="SaleLocationId"/>
        <xs:element ref="ProductId"/>
        <xs:element ref="TotalAWC"/>
      </xs:sequence>
    </xs:complexType>
  </xs:element>
</xs:schema>'
WHERE [Binding]='Merchandising.CintOrderDelivered'