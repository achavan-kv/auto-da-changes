update hub.queue
set [Schema]='<xs:schema xmlns="http://www.bluebridgeltd.com/cosacs/2012/schema.xsd" xmlns:mstns="http://www.bluebridgeltd.com/cosacs/2012/schema.xsd" xmlns:xs="http://www.w3.org/2001/XMLSchema" targetNamespace="http://www.bluebridgeltd.com/cosacs/2012/schema.xsd" elementFormDefault="qualified">
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
  <xs:element name="WarehouseLocationId" type="xs:int"/>
  <xs:element name="WarehouseLocationSalesId" type="xs:string"/>
  <xs:element name="ReceivingLocationId" type="xs:int"/>
  <xs:element name="ReceivingLocationSalesId" type="xs:string"/>
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
        <xs:element ref="WarehouseLocationId"/>
		<xs:element ref="WarehouseLocationSalesId"/>
        <xs:element ref="ReceivingLocationId"/>
		<xs:element ref="ReceivingLocationSalesId"/>
        <xs:element ref="CreatedDate"/>
        <xs:element ref="Products"/>
        <xs:element ref="TotalAWC"/>
      </xs:sequence>
    </xs:complexType>
  </xs:element>
</xs:schema>'
where [Binding]='Merchandising.Transfer'