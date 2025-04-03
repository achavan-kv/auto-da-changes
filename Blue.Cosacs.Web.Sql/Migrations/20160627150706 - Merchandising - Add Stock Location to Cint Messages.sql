-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

UPDATE Hub.[Queue]
SET [Schema] = 
'<xs:schema xmlns="http://www.bluebridgeltd.com/cosacs/2012/schema.xsd" xmlns:mstns="http://www.bluebridgeltd.com/cosacs/2012/schema.xsd" xmlns:xs="http://www.w3.org/2001/XMLSchema" targetNamespace="http://www.bluebridgeltd.com/cosacs/2012/schema.xsd" elementFormDefault="qualified">
  <xs:element name="CintOrderReceiptMessage">
    <xs:complexType>
      <xs:sequence>
        <xs:element name="CintOrderId" type="xs:int" />
        <xs:element name="Reference" type="xs:string" />
        <xs:element name="SaleType" type="xs:string" />
        <xs:element name="SaleLocationId" type="xs:string" />
        <xs:element name="StockLocationId" type="xs:string" nillable="true" minOccurs="0"/>
        <xs:element name="ProductId" type="xs:int" />
        <xs:element name="DepartmentCode" type="xs:string" />
        <xs:element name="Description" type="xs:string" />
        <xs:element name="TotalAWC" type="xs:decimal" />
        <xs:element name="FirstYearWarranty" type="xs:decimal" nillable="true" />
        <xs:element name="ReferenceType" type="xs:string" />
        <xs:element name="TransactionDate" type="xs:dateTime" />
      </xs:sequence>
    </xs:complexType>
  </xs:element>
</xs:schema>'
WHERE Id IN (212, 213)