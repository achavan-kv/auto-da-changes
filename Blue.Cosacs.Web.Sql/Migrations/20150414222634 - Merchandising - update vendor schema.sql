-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here



update [hub].[queue] set [schema] = '<xs:schema xmlns="http://www.bluebridgeltd.com/cosacs/2012/schema.xsd" xmlns:mstns="http://www.bluebridgeltd.com/cosacs/2012/schema.xsd" xmlns:xs="http://www.w3.org/2001/XMLSchema" targetNamespace="http://www.bluebridgeltd.com/cosacs/2012/schema.xsd" elementFormDefault="qualified">
  <xs:element name="Company" type="xs:string" />
  <xs:element name="VendorAction" type="xs:string" />
  <xs:element name="VendorStatus" type="xs:string" />
  <xs:element name="VendorType" type="xs:string" />
  <xs:element name="CompanyRec">
    <xs:complexType>
      <xs:sequence>
        <xs:element ref="Company" />
        <xs:element ref="VendorAction" />
        <xs:element ref="VendorStatus" />
        <xs:element ref="VendorType" />
      </xs:sequence>
    </xs:complexType>
  </xs:element>
  <xs:element name="PaymentTerms" type="xs:string" />
  <xs:element name="ContactName" type="xs:string" nillable="true" />
  <xs:element name="ContactEmail" type="xs:string" nillable="true" />
  <xs:element name="ContactPhone" type="xs:string" nillable="true" />
  <xs:element name="CompanyPhone" type="xs:string" nillable="true" />
  <xs:element name="AddressLine1" type="xs:string" />
  <xs:element name="AddressLine2" type="xs:string" />
  <xs:element name="City" type="xs:string" />
  <xs:element name="VendorCountry" type="xs:string" />
  <xs:element name="State" type="xs:string" nillable="true" />
  <xs:element name="PostalCode" type="xs:string" nillable="true" />
  <xs:element name="Record">
    <xs:complexType>
      <xs:sequence>
        <xs:element ref="PaymentTerms" />
        <xs:element ref="ContactName" minOccurs="0" />
        <xs:element ref="ContactEmail" minOccurs="0" />
        <xs:element ref="ContactPhone" minOccurs="0"  />
        <xs:element ref="CompanyPhone" minOccurs="0" />
        <xs:element ref="AddressLine1" />
        <xs:element ref="AddressLine2" />
        <xs:element ref="City" />
        <xs:element ref="VendorCountry" />
        <xs:element ref="State" minOccurs="0" />
        <xs:element ref="PostalCode" minOccurs="0" />
      </xs:sequence>
    </xs:complexType>
  </xs:element>
  <xs:element name="VendorCode" type="xs:string" />
  <xs:element name="VendorName" type="xs:string" />
  <xs:element name="CompanySection">
    <xs:complexType>
      <xs:sequence>
        <xs:element ref="CompanyRec" />
      </xs:sequence>
    </xs:complexType>
  </xs:element>
  <xs:element name="VendorDetail">
    <xs:complexType>
      <xs:sequence>
        <xs:element ref="Record" />
      </xs:sequence>
    </xs:complexType>
  </xs:element>
  <xs:element name="TotalRecords" type="xs:int" />
  <xs:element name="VendorRecordHeader">
    <xs:complexType>
      <xs:sequence>
        <xs:element ref="VendorCode" />
        <xs:element ref="VendorName" />
        <xs:element ref="CompanySection" />
        <xs:element ref="VendorDetail" />
      </xs:sequence>
    </xs:complexType>
  </xs:element>
  <xs:element name="SummarySection">
    <xs:complexType>
      <xs:sequence>
        <xs:element ref="TotalRecords" />
      </xs:sequence>
    </xs:complexType>
  </xs:element>
  <xs:element name="VendorInfoSending">
    <xs:complexType>
      <xs:sequence>
        <xs:element ref="VendorRecordHeader" maxOccurs="unbounded" minOccurs="0" />
        <xs:element ref="SummarySection" />
      </xs:sequence>
    </xs:complexType>
  </xs:element>
</xs:schema>'
where [binding] = 'Merchandising.Vendors'


update [hub].[queue] set [schema] = '<xs:schema xmlns="http://www.bluebridgeltd.com/cosacs/2012/schema.xsd" xmlns:mstns="http://www.bluebridgeltd.com/cosacs/2012/schema.xsd" xmlns:xs="http://www.w3.org/2001/XMLSchema" targetNamespace="http://www.bluebridgeltd.com/cosacs/2012/schema.xsd" elementFormDefault="qualified">
  <xs:element name="Company" type="xs:string" />
  <xs:element name="VendorAction" type="xs:string" />
  <xs:element name="VendorStatus" type="xs:string" />
  <xs:element name="VendorType" type="xs:string" />
  <xs:element name="CompanyRec">
    <xs:complexType>
      <xs:sequence>
        <xs:element ref="Company" />
        <xs:element ref="VendorAction" />
        <xs:element ref="VendorStatus" />
        <xs:element ref="VendorType" />
      </xs:sequence>
    </xs:complexType>
  </xs:element>
  <xs:element name="PaymentTerms" type="xs:string" />
  <xs:element name="ContactName" type="xs:string" nillable="true" />
  <xs:element name="ContactEmail" type="xs:string" nillable="true" />
  <xs:element name="ContactPhone" type="xs:string" nillable="true" />
  <xs:element name="CompanyPhone" type="xs:string" nillable="true" />
  <xs:element name="AddressLine1" type="xs:string" />
  <xs:element name="AddressLine2" type="xs:string" />
  <xs:element name="City" type="xs:string" />
  <xs:element name="VendorCountry" type="xs:string" />
  <xs:element name="State" type="xs:string" nillable="true" />
  <xs:element name="PostalCode" type="xs:string" nillable="true" />
  <xs:element name="Record">
    <xs:complexType>
      <xs:sequence>
        <xs:element ref="PaymentTerms" />
        <xs:element ref="ContactName" minOccurs="0" />
        <xs:element ref="ContactEmail" minOccurs="0" />
        <xs:element ref="ContactPhone" minOccurs="0"  />
        <xs:element ref="CompanyPhone" minOccurs="0" />
        <xs:element ref="AddressLine1" />
        <xs:element ref="AddressLine2" />
        <xs:element ref="City" />
        <xs:element ref="VendorCountry" />
        <xs:element ref="State" minOccurs="0" />
        <xs:element ref="PostalCode" minOccurs="0" />
      </xs:sequence>
    </xs:complexType>
  </xs:element>
  <xs:element name="VendorCode" type="xs:string" />
  <xs:element name="VendorName" type="xs:string" />
  <xs:element name="CompanySection">
    <xs:complexType>
      <xs:sequence>
        <xs:element ref="CompanyRec" />
      </xs:sequence>
    </xs:complexType>
  </xs:element>
  <xs:element name="VendorDetail">
    <xs:complexType>
      <xs:sequence>
        <xs:element ref="Record" />
      </xs:sequence>
    </xs:complexType>
  </xs:element>
  <xs:element name="VendorRecordHeader">
    <xs:complexType>
      <xs:sequence>
        <xs:element ref="VendorCode" />
        <xs:element ref="VendorName" />
        <xs:element ref="CompanySection" />
        <xs:element ref="VendorDetail" />
      </xs:sequence>
    </xs:complexType>
  </xs:element>
</xs:schema>
'
where [binding] = 'Merchandising.Vendor'

