INSERT [Hub].[Queue] ([Id],[Binding],[Schema],[SubscriberClrAssemblyName],[SubscriberClrTypeName],[SubscriberSqlConnectionName],[SubscriberSqlProcedureName],[SchemaSource],[SubscriberHttpUrl],[SubscriberHttpMethod]) 
VALUES 
	(211, N'Merchandising.GoodsReceipt','', N'Blue.Cosacs.Merchandising', N'Blue.Cosacs.Merchandising.Subscribers.GoodsReceiptSubscriber', NULL, NULL, N'Merchandising.xsd', NULL, NULL)

UPDATE [Hub].[Queue] 
SET [Schema]= N'<xs:schema xmlns="http://www.bluebridgeltd.com/cosacs/2012/schema.xsd" xmlns:mstns="http://www.bluebridgeltd.com/cosacs/2012/schema.xsd" xmlns:xs="http://www.w3.org/2001/XMLSchema" targetNamespace="http://www.bluebridgeltd.com/cosacs/2012/schema.xsd" elementFormDefault="qualified">
  <xs:element name="Id" type="xs:int"/>
  <xs:element name="Type" type="xs:string"/>
  <xs:element name="DepartmentCode" type="xs:string"/>
  <xs:element name="Product">
    <xs:complexType>
      <xs:sequence>
        <xs:element ref="Id"/>
        <xs:element ref="Type"/>
        <xs:element ref="DepartmentCode"/>
      </xs:sequence>
    </xs:complexType>
  </xs:element>
  <xs:element name="ReceiptId" type="xs:int"/>
  <xs:element name="ReceiptType" type="xs:string"/>
  <xs:element name="LocationId" type="xs:int"/>
  <xs:element name="CreatedDate" type="xs:date"/>
  <xs:element name="VendorId" type="xs:int"/>
  <xs:element name="VendorType" type="xs:string"/>
  <xs:element name="Products">
    <xs:complexType>
      <xs:sequence>
        <xs:element ref="Product" maxOccurs="unbounded" minOccurs="0"/>
      </xs:sequence>
    </xs:complexType>
  </xs:element>
  <xs:element name="TotalLandedCost" type="xs:float"/>
  <xs:element name="GoodsReceiptMessage">
    <xs:complexType>
      <xs:sequence>
        <xs:element ref="ReceiptId"/>
        <xs:element ref="ReceiptType"/>
        <xs:element ref="LocationId"/>
        <xs:element ref="CreatedDate"/>
        <xs:element ref="VendorId"/>
        <xs:element ref="VendorType"/>
        <xs:element ref="Products"/>
        <xs:element ref="TotalLandedCost"/>
      </xs:sequence>
    </xs:complexType>
  </xs:element>
</xs:schema>'
WHERE [Binding]='Merchandising.GoodsReceipt'