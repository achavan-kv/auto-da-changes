update hub.[Queue] set [schema] = 
'<xs:schema xmlns="http://www.bluebridgeltd.com/cosacs/2012/schema.xsd" xmlns:mstns="http://www.bluebridgeltd.com/cosacs/2012/schema.xsd" xmlns:xs="http://www.w3.org/2001/XMLSchema" targetNamespace="http://www.bluebridgeltd.com/cosacs/2012/schema.xsd" elementFormDefault="qualified">
  <xs:element name="Company" type="xs:string"/>
  <xs:element name="POType" type="xs:string"/>
  <xs:element name="CompanyRec">
    <xs:complexType>
      <xs:sequence>
        <xs:element ref="Company"/>
        <xs:element ref="POType"/>
      </xs:sequence>
    </xs:complexType>
  </xs:element>
  <xs:element name="CategoryId" type="xs:string" nillable="true"/>
  <xs:element name="AttributeId" type="xs:string"/>
  <xs:element name="AttributeValue" type="xs:string" nillable="true"/>
  <xs:element name="AttributeRec">
    <xs:complexType>
      <xs:sequence>
        <xs:element ref="CategoryId" minOccurs="0"/>
        <xs:element ref="AttributeId"/>
        <xs:element ref="AttributeValue" minOccurs="0"/>
      </xs:sequence>
    </xs:complexType>
  </xs:element>
  <xs:element name="LineNumber" type="xs:int"/>
  <xs:element name="SkuNumber" type="xs:string"/>
  <xs:element name="SkuDescription" type="xs:string"/>
  <xs:element name="SkuComments" type="xs:string" nillable="true"/>
  <xs:element name="SkuDeliveryDate" type="xs:date" nillable="true"/>
  <xs:element name="CorporateUPC" type="xs:string"/>
  <xs:element name="OrderedUnits" type="xs:int"/>
  <xs:element name="PreLandedUnitCost" type="xs:float"/>
  <xs:element name="SupplierUnitCost" type="xs:float"/>
  <xs:element name="PreLandedExtendedCost" type="xs:float"/>
  <xs:element name="SupplierExtendedCost" type="xs:float"/>
  <xs:element name="Record">
    <xs:complexType>
      <xs:sequence>
        <xs:element ref="LineNumber"/>
        <xs:element ref="SkuNumber"/>
        <xs:element ref="SkuDescription"/>
        <xs:element ref="SkuComments" minOccurs="0"/>
        <xs:element ref="SkuDeliveryDate" minOccurs="0"/>
        <xs:element ref="CorporateUPC"/>
        <xs:element ref="OrderedUnits"/>
        <xs:element ref="PreLandedUnitCost"/>
        <xs:element ref="SupplierUnitCost"/>
        <xs:element ref="PreLandedExtendedCost"/>
        <xs:element ref="SupplierExtendedCost"/>
      </xs:sequence>
    </xs:complexType>
  </xs:element>
  <xs:element name="TotalUnitsOnOrder" type="xs:int"/>
  <xs:element name="TotalExtendedPreLandedCost" type="xs:float"/>
  <xs:element name="TotalExtendedSupplierCost" type="xs:float"/>
  <xs:element name="OriginSystem" type="xs:string"/>
  <xs:element name="OriginModule" type="xs:string"/>
  <xs:element name="DestinationSystem" type="xs:string"/>
  <xs:element name="PONumber" type="xs:string"/>
  <xs:element name="POStatus" type="xs:string"/>
  <xs:element name="POSource" type="xs:string"/>
  <xs:element name="CreationDate" type="xs:date"/>
  <xs:element name="ShipDate" type="xs:date" nillable="true"/>
  <xs:element name="ExpectedDeliveryDate" type="xs:date"/>
  <xs:element name="Warehouse" type="xs:string"/>
  <xs:element name="VendorCode" type="xs:string"/>
  <xs:element name="VendorName" type="xs:string"/>
  <xs:element name="CommissionChargeFlag" type="xs:string" nillable="true"/>
  <xs:element name="CommissionPercentage" type="xs:string" nillable="true"/>
  <xs:element name="ExchangeRateType" type="xs:string" nillable="true"/>
  <xs:element name="ExchangeRateFactor" type="xs:float" nillable="true"/>
  <xs:element name="CurrencyCode" type="xs:string"/>
  <xs:element name="ForeignCurrencyCode" type="xs:string"/>
  <xs:element name="ShipVia" type="xs:string" nillable="true"/>
  <xs:element name="PortOfLoading" type="xs:string" nillable="true"/>
  <xs:element name="Incoterm" type="xs:string" nillable="true"/>
  <xs:element name="CompanySection">
    <xs:complexType>
      <xs:sequence>
        <xs:element ref="CompanyRec"/>
      </xs:sequence>
    </xs:complexType>
  </xs:element>
  <xs:element name="AttributeSection">
    <xs:complexType>
      <xs:sequence>
        <xs:element ref="AttributeRec" maxOccurs="unbounded" minOccurs="0"/>
      </xs:sequence>
    </xs:complexType>
  </xs:element>
  <xs:element name="OrderDetail">
    <xs:complexType>
      <xs:sequence>
        <xs:element ref="Record" maxOccurs="unbounded" minOccurs="1"/>
      </xs:sequence>
    </xs:complexType>
  </xs:element>
  <xs:element name="SummarySection">
    <xs:complexType>
      <xs:sequence>
        <xs:element ref="TotalUnitsOnOrder"/>
        <xs:element ref="TotalExtendedPreLandedCost"/>
        <xs:element ref="TotalExtendedSupplierCost"/>
      </xs:sequence>
    </xs:complexType>
  </xs:element>
  <xs:element name="POCreation">
    <xs:complexType>
      <xs:sequence>
        <xs:element ref="OriginSystem"/>
        <xs:element ref="OriginModule"/>
        <xs:element ref="DestinationSystem"/>
        <xs:element ref="PONumber"/>
        <xs:element ref="POStatus"/>
        <xs:element ref="POSource"/>
        <xs:element ref="CreationDate"/>
        <xs:element ref="ShipDate" minOccurs="0"/>
        <xs:element ref="ExpectedDeliveryDate"/>
        <xs:element ref="Warehouse"/>
        <xs:element ref="VendorCode"/>
        <xs:element ref="VendorName"/>
        <xs:element ref="CommissionChargeFlag" minOccurs="0"/>
        <xs:element ref="CommissionPercentage" minOccurs="0"/>
        <xs:element ref="ExchangeRateType" minOccurs="0"/>
        <xs:element ref="ExchangeRateFactor" minOccurs="0"/>
        <xs:element ref="CurrencyCode"/>
        <xs:element ref="ForeignCurrencyCode"/>
        <xs:element ref="ShipVia" minOccurs="0"/>
        <xs:element ref="PortOfLoading" minOccurs="0"/>
        <xs:element ref="Incoterm" minOccurs="0"/>
        <xs:element ref="CompanySection"/>
        <xs:element ref="AttributeSection" minOccurs="0"/>
        <xs:element ref="OrderDetail"/>
        <xs:element ref="SummarySection"/>
      </xs:sequence>
    </xs:complexType>
  </xs:element>
</xs:schema>'
where [Binding] = 'Merchandising.PurchaseOrder'