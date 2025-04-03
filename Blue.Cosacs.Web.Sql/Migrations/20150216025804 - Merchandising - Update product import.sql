-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
update hub.Queue
set [schema]= '<xs:schema xmlns="http://www.bluebridgeltd.com/cosacs/2012/schema.xsd" xmlns:mstns="http://www.bluebridgeltd.com/cosacs/2012/schema.xsd" xmlns:xs="http://www.w3.org/2001/XMLSchema" targetNamespace="http://www.bluebridgeltd.com/cosacs/2012/schema.xsd" elementFormDefault="qualified">
  <xs:element name="Company" type="xs:string" />
  <xs:element name="SkuAction" type="xs:string" />
  <xs:element name="SKUType" type="xs:string" />
  <xs:element name="CompanyRec">
    <xs:complexType>
      <xs:sequence>
        <xs:element ref="Company" />
        <xs:element ref="SkuAction" />
        <xs:element ref="SKUType" />
      </xs:sequence>
    </xs:complexType>
  </xs:element>
  <xs:element name="DivisionCode" type="xs:string" />
  <xs:element name="DivisionName" type="xs:string" />
  <xs:element name="DepartmentCode" type="xs:string" />
  <xs:element name="DepartmentName" type="xs:string" />
  <xs:element name="ClassCode" type="xs:string" />
  <xs:element name="ClassName" type="xs:string" />
  <xs:element name="HierarchyRec">
    <xs:complexType>
      <xs:sequence>
        <xs:element ref="DivisionCode" />
        <xs:element ref="DivisionName" />
        <xs:element ref="DepartmentCode" />
        <xs:element ref="DepartmentName" />
        <xs:element ref="ClassCode" />
        <xs:element ref="ClassName" />
      </xs:sequence>
    </xs:complexType>
  </xs:element>
  <xs:element name="CategoryId" type="xs:string" nillable="true" />
  <xs:element name="AttributeId" type="xs:string" nillable="true" />
  <xs:element name="AttributeValue" type="xs:string" nillable="true" />
  <xs:element name="AttributeRec" nillable="true">
    <xs:complexType>
      <xs:sequence>
        <xs:element ref="CategoryId" />
        <xs:element ref="AttributeId" />
        <xs:element ref="AttributeValue" />
      </xs:sequence>
    </xs:complexType>
  </xs:element>
  <xs:element name="IncotermName" type="xs:string" nillable="true" />
  <xs:element name="CurrencyType" type="xs:string" nillable="true" />
  <xs:element name="SupplierUnitCost" type="xs:decimal" nillable="true" />
  <xs:element name="CountryOfDispatch" type="xs:string" nillable="true" />
  <xs:element name="LeadTime" type="xs:string" nillable="true" />
  <xs:element name="IncotermsRec" nillable="true">
    <xs:complexType>
      <xs:sequence>
        <xs:element ref="IncotermName" />
        <xs:element ref="CurrencyType" />
        <xs:element ref="SupplierUnitCost" />
        <xs:element ref="CountryOfDispatch" />
        <xs:element ref="LeadTime" />
      </xs:sequence>
    </xs:complexType>
  </xs:element>
  <xs:element name="SkuNumber" type="xs:string" />
  <xs:element name="LongDescription" type="xs:string" />
  <xs:element name="POSDescription" type="xs:string" />
  <xs:element name="SKUStatusCode" type="xs:string" />
  <xs:element name="CorporateUPC" type="xs:string" />
  <xs:element name="VendorUPC" type="xs:string" nillable="true" />
  <xs:element name="VendorCode" type="xs:string" />
  <xs:element name="VendorName" type="xs:string" />
  <xs:element name="BrandCode" type="xs:string" />
  <xs:element name="BrandName" type="xs:string" />
  <xs:element name="VendorStyleLong" type="xs:string" />
  <xs:element name="CountryOfOrigin" type="xs:string" />
  <xs:element name="VendorWarranty" type="xs:int" nillable="true" />
  <xs:element name="CreationDate" type="xs:string" />
  <xs:element name="ReplacingTo" type="xs:string" nillable="true" />
  <xs:element name="ProductType" type="xs:string" />
  <xs:element name="CompanySection">
    <xs:complexType>
      <xs:sequence>
        <xs:element ref="CompanyRec" />
      </xs:sequence>
    </xs:complexType>
  </xs:element>
  <xs:element name="HierarchySection">
    <xs:complexType>
      <xs:sequence>
        <xs:element ref="HierarchyRec" />
      </xs:sequence>
    </xs:complexType>
  </xs:element>
  <xs:element name="AttributeSection" nillable="true">
    <xs:complexType>
      <xs:sequence>
        <xs:element ref="AttributeRec" maxOccurs="unbounded" minOccurs="0" />
      </xs:sequence>
    </xs:complexType>
  </xs:element>
  <xs:element name="BasicCostSection" nillable="true">
    <xs:complexType>
      <xs:sequence>
        <xs:element ref="IncotermsRec" maxOccurs="unbounded" minOccurs="0" />
      </xs:sequence>
    </xs:complexType>
  </xs:element>
  <xs:element name="TotalRecords" type="xs:byte" />
  <xs:element name="ItemRecord">
    <xs:complexType>
      <xs:sequence>
        <xs:element ref="SkuNumber" />
        <xs:element ref="LongDescription" />
        <xs:element ref="POSDescription" />
        <xs:element ref="SKUStatusCode" />
        <xs:element ref="CorporateUPC" />
        <xs:element ref="VendorUPC" />
        <xs:element ref="VendorCode" />
        <xs:element ref="VendorName" />
        <xs:element ref="BrandCode" />
        <xs:element ref="BrandName" />
        <xs:element ref="VendorStyleLong" />
        <xs:element ref="CountryOfOrigin" />
        <xs:element ref="VendorWarranty" minOccurs="0" />
        <xs:element ref="CreationDate" />
        <xs:element ref="ReplacingTo" />
        <xs:element ref="ProductType" />
        <xs:element ref="CompanySection" />
        <xs:element ref="HierarchySection" />
        <xs:element ref="AttributeSection" />
        <xs:element ref="BasicCostSection" />
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
  <xs:element name="ItemInfoSending">
    <xs:complexType>
      <xs:sequence>
        <xs:element ref="ItemRecord" maxOccurs="unbounded" minOccurs="0" />
        <xs:element ref="SummarySection" />
      </xs:sequence>
    </xs:complexType>
  </xs:element>
</xs:schema>'
where [binding] = 'merchandising.products'

update hub.Queue
set [schema] = '<xs:schema xmlns="http://www.bluebridgeltd.com/cosacs/2012/schema.xsd" xmlns:mstns="http://www.bluebridgeltd.com/cosacs/2012/schema.xsd" xmlns:xs="http://www.w3.org/2001/XMLSchema" targetNamespace="http://www.bluebridgeltd.com/cosacs/2012/schema.xsd" elementFormDefault="qualified">
  <xs:element name="Company" type="xs:string" />
  <xs:element name="SkuAction" type="xs:string" />
  <xs:element name="SKUType" type="xs:string" />
  <xs:element name="CompanyRec">
    <xs:complexType>
      <xs:sequence>
        <xs:element ref="Company" />
        <xs:element ref="SkuAction" />
        <xs:element ref="SKUType" />
      </xs:sequence>
    </xs:complexType>
  </xs:element>
  <xs:element name="DivisionCode" type="xs:string" />
  <xs:element name="DivisionName" type="xs:string" />
  <xs:element name="DepartmentCode" type="xs:string" />
  <xs:element name="DepartmentName" type="xs:string" />
  <xs:element name="ClassCode" type="xs:string" />
  <xs:element name="ClassName" type="xs:string" />
  <xs:element name="HierarchyRec">
    <xs:complexType>
      <xs:sequence>
        <xs:element ref="DivisionCode" />
        <xs:element ref="DivisionName" />
        <xs:element ref="DepartmentCode" />
        <xs:element ref="DepartmentName" />
        <xs:element ref="ClassCode" />
        <xs:element ref="ClassName" />
      </xs:sequence>
    </xs:complexType>
  </xs:element>
  <xs:element name="CategoryId" type="xs:string" />
  <xs:element name="AttributeId" type="xs:string" />
  <xs:element name="AttributeValue" type="xs:string" />
  <xs:element name="AttributeRec">
    <xs:complexType>
      <xs:sequence>
        <xs:element ref="CategoryId" />
        <xs:element ref="AttributeId" />
        <xs:element ref="AttributeValue" />
      </xs:sequence>
    </xs:complexType>
  </xs:element>
  <xs:element name="IncotermName" type="xs:string" />
  <xs:element name="CurrencyType" type="xs:string" />
  <xs:element name="SupplierUnitCost" type="xs:decimal" />
  <xs:element name="CountryOfDispatch" type="xs:string" />
  <xs:element name="LeadTime" type="xs:string" />
  <xs:element name="IncotermsRec">
    <xs:complexType>
      <xs:sequence>
        <xs:element ref="IncotermName" />
        <xs:element ref="CurrencyType" />
        <xs:element ref="SupplierUnitCost" />
        <xs:element ref="CountryOfDispatch" />
        <xs:element ref="LeadTime" />
      </xs:sequence>
    </xs:complexType>
  </xs:element>
  <xs:element name="SkuNumber" type="xs:string" />
  <xs:element name="LongDescription" type="xs:string" />
  <xs:element name="POSDescription" type="xs:string" />
  <xs:element name="SKUStatusCode" type="xs:string" />
  <xs:element name="CorporateUPC" type="xs:string" />
  <xs:element name="VendorUPC" type="xs:string" />
  <xs:element name="VendorCode" type="xs:string" />
  <xs:element name="VendorName" type="xs:string" />
  <xs:element name="BrandCode" type="xs:string" />
  <xs:element name="BrandName" type="xs:string" />
  <xs:element name="VendorStyleLong" type="xs:string" />
  <xs:element name="CountryOfOrigin" type="xs:string" />
  <xs:element name="VendorWarranty" type="xs:int" />
  <xs:element name="CreationDate" type="xs:string" />
  <xs:element name="ReplacingTo" type="xs:string" />
  <xs:element name="ProductType" type="xs:string" />
  <xs:element name="CompanySection">
    <xs:complexType>
      <xs:sequence>
        <xs:element ref="CompanyRec" />
      </xs:sequence>
    </xs:complexType>
  </xs:element>
  <xs:element name="HierarchySection">
    <xs:complexType>
      <xs:sequence>
        <xs:element ref="HierarchyRec" />
      </xs:sequence>
    </xs:complexType>
  </xs:element>
  <xs:element name="AttributeSection">
    <xs:complexType>
      <xs:sequence>
        <xs:element ref="AttributeRec" maxOccurs="unbounded" minOccurs="0" />
      </xs:sequence>
    </xs:complexType>
  </xs:element>
  <xs:element name="BasicCostSection">
    <xs:complexType>
      <xs:sequence>
        <xs:element ref="IncotermsRec" maxOccurs="unbounded" minOccurs="0" />
      </xs:sequence>
    </xs:complexType>
  </xs:element>
  <xs:element name="ItemRecord">
    <xs:complexType>
      <xs:sequence>
        <xs:element ref="SkuNumber" />
        <xs:element ref="LongDescription" />
        <xs:element ref="POSDescription" />
        <xs:element ref="SKUStatusCode" />
        <xs:element ref="CorporateUPC" />
        <xs:element ref="VendorUPC" />
        <xs:element ref="VendorCode" />
        <xs:element ref="VendorName" />
        <xs:element ref="BrandCode" />
        <xs:element ref="BrandName" />
        <xs:element ref="VendorStyleLong" />
        <xs:element ref="CountryOfOrigin" />
        <xs:element ref="VendorWarranty" minOccurs="0" />
        <xs:element ref="CreationDate" />
        <xs:element ref="ReplacingTo" />
        <xs:element ref="ProductType" />
        <xs:element ref="CompanySection" />
        <xs:element ref="HierarchySection" />
        <xs:element ref="AttributeSection" />
        <xs:element ref="BasicCostSection" />
      </xs:sequence>
    </xs:complexType>
  </xs:element>
</xs:schema>'
where [binding] = 'merchandising.product'

