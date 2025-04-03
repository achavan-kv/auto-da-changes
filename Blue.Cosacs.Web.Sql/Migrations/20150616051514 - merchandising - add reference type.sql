-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
update hub.Queue
set [Schema] = '<xs:schema xmlns="http://www.bluebridgeltd.com/cosacs/2012/schema.xsd" xmlns:mstns="http://www.bluebridgeltd.com/cosacs/2012/schema.xsd" xmlns:xs="http://www.w3.org/2001/XMLSchema" targetNamespace="http://www.bluebridgeltd.com/cosacs/2012/schema.xsd" elementFormDefault="qualified">
  <xs:element name="CintOrderId" type="xs:int"/>
  <xs:element name="Reference" type="xs:string"/>
  <xs:element name="SaleType" type="xs:string"/>
  <xs:element name="SaleLocationId" type="xs:string"/>
  <xs:element name="ProductId" type="xs:int"/>
  <xs:element name="DepartmentCode" type="xs:string"/>
  <xs:element name="Description" type="xs:string"/>
  <xs:element name="TotalAWC" type="xs:decimal"/>
  <xs:element name="FirstYearWarranty" type="xs:decimal" nillable="true" />
  <xs:element name="ReferenceType" type="xs:string"/>
  
  <xs:element name="CintOrderReceiptMessage">
    <xs:complexType>
      <xs:sequence>
        <xs:element ref="CintOrderId"/>
        <xs:element ref="Reference"/>
        <xs:element ref="SaleType"/>
        <xs:element ref="SaleLocationId"/>
        <xs:element ref="ProductId"/>
		<xs:element ref="DepartmentCode"/>		
		<xs:element ref="Description"/>
        <xs:element ref="TotalAWC"/>
        <xs:element ref="FirstYearWarranty"/>
		<xs:element ref="ReferenceType"/>
      </xs:sequence>
    </xs:complexType>
  </xs:element>
</xs:schema>'
where [binding] in ('Merchandising.CintOrderDelivered','Merchandising.CintOrderReturned')