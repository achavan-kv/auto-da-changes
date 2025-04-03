-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

update [hub].[queue]
set [schema] = '<xs:schema xmlns="http://www.bluebridgeltd.com/cosacs/2012/schema.xsd" xmlns:mstns="http://www.bluebridgeltd.com/cosacs/2012/schema.xsd" xmlns:xs="http://www.w3.org/2001/XMLSchema" targetNamespace="http://www.bluebridgeltd.com/cosacs/2012/schema.xsd" elementFormDefault="qualified">
  <xs:element name="CintSubmit">
    <xs:complexType>
      <xs:sequence>
        <xs:element name="RunNo" minOccurs="1" maxOccurs="1" type="xs:int" />
        <xs:element name="CintOrder" minOccurs="0" maxOccurs="unbounded" type="CintOrder" />
        <xs:element name="OrdersDeliveriesTotal" minOccurs="1" maxOccurs="1" type="xs:decimal" />
        <xs:element name="DeliveriesTotal" minOccurs="1" maxOccurs="1" type="xs:decimal" />
      </xs:sequence>
    </xs:complexType>
  </xs:element>
  <xs:complexType name="CintOrder">
    <xs:sequence>
      <xs:element name="Type" minOccurs="1" maxOccurs="1">
        <xs:simpleType>
          <xs:restriction base="xs:string">
            <xs:maxLength value="16" />
          </xs:restriction>
        </xs:simpleType>
      </xs:element>
      <xs:element name="PrimaryReference" minOccurs="1" maxOccurs="1">
        <xs:simpleType>
          <xs:restriction base="xs:string">
            <xs:maxLength value="20" />
          </xs:restriction>
        </xs:simpleType>
      </xs:element>
	   <xs:element name="SecondaryReference" minOccurs="1" maxOccurs="1">
        <xs:simpleType>
          <xs:restriction base="xs:string">
            <xs:maxLength value="20" />
          </xs:restriction>
        </xs:simpleType>
      </xs:element>
	  <xs:element name="ReferenceType" minOccurs="1" maxOccurs="1">
        <xs:simpleType>
          <xs:restriction base="xs:string">
            <xs:maxLength value="20" />
          </xs:restriction>
        </xs:simpleType>
      </xs:element>
      <xs:element name="SaleType" minOccurs="1" maxOccurs="1">
        <xs:simpleType>
          <xs:restriction base="xs:string">
            <xs:maxLength value="6" />
          </xs:restriction>
        </xs:simpleType>
      </xs:element>
      <xs:element name="SaleLocation" minOccurs="1" maxOccurs="1">
        <xs:simpleType>
          <xs:restriction base="xs:string">
            <xs:maxLength value="3" />
          </xs:restriction>
        </xs:simpleType>
      </xs:element>
      <xs:element name="Sku" minOccurs="1" maxOccurs="1">
        <xs:simpleType>
          <xs:restriction base="xs:string">
            <xs:maxLength value="8" />
          </xs:restriction>
        </xs:simpleType>
      </xs:element>
      <xs:element name="ProductId" minOccurs="1" maxOccurs="1" type="xs:int" nillable="true" />
      <xs:element name="StockLocation" minOccurs="1" maxOccurs="1">
        <xs:simpleType>
          <xs:restriction base="xs:string">
            <xs:maxLength value="3" />
          </xs:restriction>
        </xs:simpleType>
      </xs:element>
      <xs:element name="ParentSku" minOccurs="1" maxOccurs="1">
        <xs:simpleType>
          <xs:restriction base="xs:string">
            <xs:maxLength value="8" />
          </xs:restriction>
        </xs:simpleType>
      </xs:element>
      <xs:element name="ParentId" minOccurs="0" maxOccurs="1" type="xs:int" nillable="true" />
      <xs:element name="TransactionDate" minOccurs="1" maxOccurs="1" type="xs:dateTime" />
      <xs:element name="Quantity" minOccurs="1" maxOccurs="1" type="xs:int" />
      <xs:element name="Price" minOccurs="1" maxOccurs="1" type="xs:decimal" nillable="true" />	  
      <xs:element name="Discount" minOccurs="1" maxOccurs="1" type="xs:decimal" nillable="true" />
	  <xs:element name="CashPrice" minOccurs="1" maxOccurs="1" type="xs:decimal" nillable="true" />
      <xs:element name="Tax" minOccurs="1" maxOccurs="1" type="xs:decimal" nillable="true" />
	  <xs:element name="PromotionId" minOccurs="0" maxOccurs="1" type="xs:int" nillable="true" />
    </xs:sequence>
  </xs:complexType>
</xs:schema>'
where [binding] = 'Merchandising.Cints'

update [hub].[queue]
set [schema] = '<xs:schema xmlns="http://www.bluebridgeltd.com/cosacs/2012/schema.xsd" xmlns:mstns="http://www.bluebridgeltd.com/cosacs/2012/schema.xsd" xmlns:xs="http://www.w3.org/2001/XMLSchema" targetNamespace="http://www.bluebridgeltd.com/cosacs/2012/schema.xsd" elementFormDefault="qualified">
  <xs:element name="CintSubmit">
    <xs:complexType>
      <xs:sequence>
        <xs:element name="RunNo" minOccurs="1" maxOccurs="1" type="xs:int" />
        <xs:element name="CintOrder" minOccurs="0" maxOccurs="1" type="CintOrder" />
      </xs:sequence>
    </xs:complexType>
  </xs:element>
  <xs:complexType name="CintOrder">
    <xs:sequence>
      <xs:element name="Type" minOccurs="1" maxOccurs="1">
        <xs:simpleType>
          <xs:restriction base="xs:string">
            <xs:maxLength value="16" />
          </xs:restriction>
        </xs:simpleType>
      </xs:element>
      <xs:element name="PrimaryReference" minOccurs="1" maxOccurs="1">
        <xs:simpleType>
          <xs:restriction base="xs:string">
            <xs:maxLength value="20" />
          </xs:restriction>
        </xs:simpleType>
      </xs:element>
	   <xs:element name="SecondaryReference" minOccurs="1" maxOccurs="1">
        <xs:simpleType>
          <xs:restriction base="xs:string">
            <xs:maxLength value="20" />
          </xs:restriction>
        </xs:simpleType>
      </xs:element>
	  <xs:element name="ReferenceType" minOccurs="1" maxOccurs="1">
        <xs:simpleType>
          <xs:restriction base="xs:string">
            <xs:maxLength value="20" />
          </xs:restriction>
        </xs:simpleType>
      </xs:element>
      <xs:element name="SaleType" minOccurs="1" maxOccurs="1">
        <xs:simpleType>
          <xs:restriction base="xs:string">
            <xs:maxLength value="6" />
          </xs:restriction>
        </xs:simpleType>
      </xs:element>
      <xs:element name="SaleLocation" minOccurs="1" maxOccurs="1">
        <xs:simpleType>
          <xs:restriction base="xs:string">
            <xs:maxLength value="3" />
          </xs:restriction>
        </xs:simpleType>
      </xs:element>
      <xs:element name="Sku" minOccurs="1" maxOccurs="1">
        <xs:simpleType>
          <xs:restriction base="xs:string">
            <xs:maxLength value="8" />
          </xs:restriction>
        </xs:simpleType>
      </xs:element>
      <xs:element name="ProductId" minOccurs="1" maxOccurs="1" type="xs:int" />
      <xs:element name="StockLocation" minOccurs="1" maxOccurs="1">
        <xs:simpleType>
          <xs:restriction base="xs:string">
            <xs:maxLength value="3" />
          </xs:restriction>
        </xs:simpleType>
      </xs:element>
      <xs:element name="ParentSku" minOccurs="0" maxOccurs="1" nillable="true">
        <xs:simpleType>
          <xs:restriction base="xs:string">
            <xs:maxLength value="8" />
          </xs:restriction>
        </xs:simpleType>
      </xs:element>
      <xs:element name="ParentId" minOccurs="0" maxOccurs="1" type="xs:int" nillable="true" />
      <xs:element name="TransactionDate" minOccurs="1" maxOccurs="1" type="xs:dateTime" />
      <xs:element name="Quantity" minOccurs="1" maxOccurs="1" type="xs:int" />
      <xs:element name="Price" minOccurs="1" maxOccurs="1" type="xs:decimal" nillable="true" />
      <xs:element name="Discount" minOccurs="1" maxOccurs="1" type="xs:decimal" nillable="true" /> 
      <xs:element name="CashPrice" minOccurs="1" maxOccurs="1" type="xs:decimal" nillable="true" />
      <xs:element name="Tax" minOccurs="1" maxOccurs="1" type="xs:decimal" nillable="true" />
	  <xs:element name="PromotionId" minOccurs="0" maxOccurs="1" type="xs:int" nillable="true" />
    </xs:sequence>
  </xs:complexType>
</xs:schema>'
where [binding] = 'Merchandising.Cint'
