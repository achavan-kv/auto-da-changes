INSERT [Hub].[Queue] ([Id], [Binding], [SubscriberClrAssemblyName], [SubscriberClrTypeName], [SubscriberSqlConnectionName], [SubscriberSqlProcedureName], [SchemaSource], [SubscriberHttpUrl], [SubscriberHttpMethod], [Schema]) 
VALUES (
	214
	,N'Merchandising.Transfer'
	,NULL, NULL, NULL, NULL
	,N'Merchandising.xsd'
	,N'/cosacs/Financial/Transfer'
	,N'POST'
	,N'<xs:schema xmlns="http://www.bluebridgeltd.com/cosacs/2012/schema.xsd" xmlns:mstns="http://www.bluebridgeltd.com/cosacs/2012/schema.xsd" xmlns:xs="http://www.w3.org/2001/XMLSchema" targetNamespace="http://www.bluebridgeltd.com/cosacs/2012/schema.xsd" elementFormDefault="qualified">
  <xs:element name="Id" type="xs:int"/>
  <xs:element name="Type" type="xs:string"/>
  <xs:element name="DepartmentCode" type="xs:string"/>
  <xs:element name="Cost" type="xs:float"/>
  <xs:element name="Product">
    <xs:complexType>
      <xs:sequence>
        <xs:element ref="Id"/>
        <xs:element ref="Type"/>
        <xs:element ref="DepartmentCode"/>
		<xs:element ref="Cost"/>
      </xs:sequence>
    </xs:complexType>
  </xs:element>
  <xs:element name="LocationId" type="xs:int"/>
  <xs:element name="SalesLocationId" type="xs:string"/>
  <xs:element name="CreatedDate" type="xs:date"/>
  <xs:element name="Products">
    <xs:complexType>
      <xs:sequence>
        <xs:element ref="Product" maxOccurs="unbounded" minOccurs="0"/>
      </xs:sequence>
    </xs:complexType>
  </xs:element>
  <xs:element name="TotalAWC" type="xs:float"/>
  <xs:element name="TransferMessage">
    <xs:complexType>
      <xs:sequence>
        <xs:element ref="LocationId"/>
		<xs:element ref="SalesLocationId"/>
        <xs:element ref="CreatedDate"/>
        <xs:element ref="Products"/>
        <xs:element ref="TotalAWC"/>
      </xs:sequence>
    </xs:complexType>
  </xs:element>
</xs:schema>'
)