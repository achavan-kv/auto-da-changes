update Hub.Queue set [Schema] = '
<xs:schema xmlns="http://www.bluebridgeltd.com/cosacs/2012/schema.xsd" xmlns:mstns="http://www.bluebridgeltd.com/cosacs/2012/schema.xsd" xmlns:xs="http://www.w3.org/2001/XMLSchema" targetNamespace="http://www.bluebridgeltd.com/cosacs/2012/schema.xsd" elementFormDefault="qualified">
  <xs:element name="BookingId" type="xs:int" />
  <xs:element name="Type" type="xs:string" />
  <xs:element name="UserId" type="xs:int" />
  <xs:element name="Quantity" type="xs:int" />
  <xs:element name="AverageWeightedCost" type="xs:decimal" />
  <xs:element name="BookingMessage">
    <xs:complexType>
      <xs:sequence>
        <xs:element ref="BookingId" />
        <xs:element ref="Type" />
        <xs:element ref="UserId" />
        <xs:element ref="Quantity" />
        <xs:element ref="AverageWeightedCost" />
      </xs:sequence>
    </xs:complexType>
  </xs:element>
</xs:schema>' WHERE SchemaSource = 'BookingMessage.xsd' 
update Hub.Queue set [Schema] = '
<xs:schema xmlns="http://www.bluebridgeltd.com/cosacs/2012/schema.xsd" xmlns:mstns="http://www.bluebridgeltd.com/cosacs/2012/schema.xsd" xmlns:xs="http://www.w3.org/2001/XMLSchema" targetNamespace="http://www.bluebridgeltd.com/cosacs/2012/schema.xsd" elementFormDefault="qualified">
 
  <xs:element name="CintOrderReceiptMessage">
    <xs:complexType>
      <xs:sequence>
		  <xs:element name="CintOrderId" type="xs:int"/>
		  <xs:element name="Reference" type="xs:string"/>
		  <xs:element name="SaleType" type="xs:string"/>
		  <xs:element name="SaleLocationId" type="xs:string"/>
          <xs:element name="StockLocationId" type="xs:string" nillable="true" minOccurs="0" />
		  <xs:element name="ProductId" type="xs:int"/>
		  <xs:element name="DepartmentCode" type="xs:string"/>
		  <xs:element name="Description" type="xs:string"/>
		  <xs:element name="TotalAWC" type="xs:decimal"/>
		  <xs:element name="FirstYearWarranty" type="xs:decimal" nillable="true" />
		  <xs:element name="ReferenceType" type="xs:string"/>
		  <xs:element name="TransactionDate" type="xs:dateTime"/>
        </xs:sequence>
    </xs:complexType>
  </xs:element>
</xs:schema>' WHERE SchemaSource = 'CintOrderReceiptMessage.xsd' 
update Hub.Queue set [Schema] = '
<xs:schema xmlns="http://www.bluebridgeltd.com/cosacs/2012/schema.xsd" xmlns:mstns="http://www.bluebridgeltd.com/cosacs/2012/schema.xsd" xmlns:xs="http://www.w3.org/2001/XMLSchema" targetNamespace="http://www.bluebridgeltd.com/cosacs/2012/schema.xsd" elementFormDefault="qualified">
  
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
            <xs:maxLength value="18" />
          </xs:restriction>
        </xs:simpleType>
      </xs:element>
      <xs:element name="ProductId" minOccurs="1" maxOccurs="1" type="xs:int" />
	  <xs:element name="RunNo" minOccurs="0" maxOccurs="1" type="xs:int" />
	  <xs:element name="Error" minOccurs="0" maxOccurs="1" type="xs:string" />
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
            <xs:maxLength value="18" />
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
  
  <xs:element name="CintOrderSubmit">
    <xs:complexType>
      <xs:sequence>
       <xs:element name="CintOrder" minOccurs="1" maxOccurs="1" type="CintOrder" />
	    </xs:sequence>
    </xs:complexType>
</xs:element>
</xs:schema>' WHERE SchemaSource = 'Cints.xsd' 
update Hub.Queue set [Schema] = '
<xs:schema xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns="http://www.bluebridgeltd.com/cosacs/2012/schema.xsd" xmlns:mstns="http://www.bluebridgeltd.com/cosacs/2012/schema.xsd" targetNamespace="http://www.bluebridgeltd.com/cosacs/2012/schema.xsd" elementFormDefault="qualified">
<xs:simpleType name="productURLS">
  <xs:restriction base="xs:string" />
</xs:simpleType>


 <xs:element name="GoodsReceiptReview">
      <xs:complexType>
         <xs:sequence>
            <xs:element name="GoodsReceiptId" minOccurs="1" maxOccurs="1" type="xs:int" />
            <xs:element name="FinanceEmailAddress" minOccurs="1" maxOccurs="1">
               <xs:simpleType>
                  <xs:restriction base="xs:string">
                     <xs:maxLength value="300" />
                  </xs:restriction>
               </xs:simpleType>
            </xs:element>
            <xs:element name="AbsoluteReviewUrl" minOccurs="1" maxOccurs="1">
               <xs:simpleType>
                  <xs:restriction base="xs:string">
                     <xs:maxLength value="300" />
                  </xs:restriction>
               </xs:simpleType>
            </xs:element>
            <xs:element name="NumberOfItemsToReview" minOccurs="1" maxOccurs="1" type="xs:int" />
			<xs:element name="ProductURLs" type="productURLS" minOccurs="0" maxOccurs="unbounded" />
         </xs:sequence>
      </xs:complexType>
   </xs:element>
</xs:schema>' WHERE SchemaSource = 'GoodsReceiptEmail.xsd' 
update Hub.Queue set [Schema] = '
<xs:schema xmlns="http://www.bluebridgeltd.com/cosacs/2012/schema.xsd" xmlns:mstns="http://www.bluebridgeltd.com/cosacs/2012/schema.xsd" xmlns:xs="http://www.w3.org/2001/XMLSchema" targetNamespace="http://www.bluebridgeltd.com/cosacs/2012/schema.xsd" elementFormDefault="qualified">
	<xs:element name="GoodsReceiptMessage">
		<xs:complexType>
			<xs:sequence>
				<xs:element name="ReceiptId" type="xs:int"/>
				<xs:element name="ReceiptType" type="xs:string"/>
				<xs:element name="LocationId" type="xs:int"/>
				<xs:element name="SalesLocationId" type="xs:string"/>
				<xs:element name="CreatedDate" type="xs:dateTime"/>
				<xs:element name="VendorId" type="xs:int"/>
				<xs:element name="VendorType" type="xs:string"/>
				<xs:element name="Description" type="xs:string"/>
				<xs:element name="TotalLandedCost" type="xs:decimal"/>
				<xs:element name="Products">
					<xs:complexType>
						<xs:sequence>
							<xs:element name="Product" maxOccurs="unbounded" minOccurs="0">
								<xs:complexType>
									<xs:sequence>
										<xs:element name="Id" type="xs:int"/>
										<xs:element name="Type" type="xs:string"/>
										<xs:element name="DepartmentCode" type="xs:string"/>
										<xs:element name="Cost" type="xs:decimal"/>
										<xs:element name="Units" type="xs:int"/>
									</xs:sequence>
								</xs:complexType>
							</xs:element>
						</xs:sequence>
					</xs:complexType>
				</xs:element>
			</xs:sequence>
		</xs:complexType>
	</xs:element>
</xs:schema>
' WHERE SchemaSource = 'GoodsReceiptMessage.xsd' 
update Hub.Queue set [Schema] = '
<xs:schema xmlns="http://www.bluebridgeltd.com/cosacs/2012/schema.xsd" xmlns:mstns="http://www.bluebridgeltd.com/cosacs/2012/schema.xsd" xmlns:xs="http://www.w3.org/2001/XMLSchema" targetNamespace="http://www.bluebridgeltd.com/cosacs/2012/schema.xsd" elementFormDefault="qualified">
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
  <xs:element name="TotalRecords" type="xs:int" />
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
</xs:schema>' WHERE SchemaSource = 'Products.xsd' 
update Hub.Queue set [Schema] = '
<xs:schema xmlns="http://www.bluebridgeltd.com/cosacs/2012/schema.xsd" xmlns:mstns="http://www.bluebridgeltd.com/cosacs/2012/schema.xsd" xmlns:xs="http://www.w3.org/2001/XMLSchema" targetNamespace="http://www.bluebridgeltd.com/cosacs/2012/schema.xsd" elementFormDefault="qualified">
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
  <xs:element name="PreLandedUnitCost" type="xs:decimal"/>
  <xs:element name="SupplierUnitCost" type="xs:decimal"/>
  <xs:element name="PreLandedExtendedCost" type="xs:decimal"/>
  <xs:element name="SupplierExtendedCost" type="xs:decimal"/>
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
  <xs:element name="TotalExtendedPreLandedCost" type="xs:decimal"/>
  <xs:element name="TotalExtendedSupplierCost" type="xs:decimal"/>
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
  <xs:element name="ExchangeRateFactor" type="xs:decimal" nillable="true"/>
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
</xs:schema>' WHERE SchemaSource = 'PurchaseOrder.xsd' 
update Hub.Queue set [Schema] = '
<xs:schema xmlns="http://www.bluebridgeltd.com/cosacs/2012/schema.xsd" xmlns:mstns="http://www.bluebridgeltd.com/cosacs/2012/schema.xsd" xmlns:xs="http://www.w3.org/2001/XMLSchema" targetNamespace="http://www.bluebridgeltd.com/cosacs/2012/schema.xsd" elementFormDefault="qualified">
	<xs:element name="StockAdjustmentMessage">
		<xs:complexType>
			<xs:sequence>
				<xs:element name="AdjustmentId" type="xs:int"/>
				<xs:element name="DateCreated" type="xs:date"/>
				<xs:element name="LocationId" type="xs:int"/>
				<xs:element name="SalesLocationId" type="xs:string"/>
				<xs:element name="CreatedDate" type="xs:dateTime"/>
				<xs:element name="SecondaryReason" type="xs:int"/>
				<xs:element name="Description" type="xs:string"/>
				<xs:element name="AWC" type="xs:decimal"/>
				<xs:element name="Products">
					<xs:complexType>
						<xs:sequence>
							<xs:element name="Product" maxOccurs="unbounded" minOccurs="0">
								<xs:complexType>
									<xs:sequence>
										<xs:element name="Id" type="xs:int"/>
										<xs:element name="Type" type="xs:string"/>
										<xs:element name="DepartmentCode" type="xs:string"/>
										<xs:element name="Cost" type="xs:decimal"/>
										<xs:element name="Units" type="xs:int"/>
									</xs:sequence>
								</xs:complexType>
							</xs:element>
						</xs:sequence>
					</xs:complexType>
				</xs:element>
			</xs:sequence>
		</xs:complexType>
	</xs:element>
</xs:schema>
' WHERE SchemaSource = 'StockAdjustmentMessage.xsd' 
update Hub.Queue set [Schema] = '
<xs:schema xmlns="http://www.bluebridgeltd.com/cosacs/2012/schema.xsd" xmlns:mstns="http://www.bluebridgeltd.com/cosacs/2012/schema.xsd" xmlns:xs="http://www.w3.org/2001/XMLSchema" targetNamespace="http://www.bluebridgeltd.com/cosacs/2012/schema.xsd" elementFormDefault="qualified">
	<xs:element name="TransferMessage">
		<xs:complexType>
			<xs:sequence>
				<xs:element name="Id" type="xs:int"/>
				<xs:element name="Type" type="xs:string"/>
				<xs:element name="WarehouseLocationId" type="xs:int"/>
				<xs:element name="WarehouseLocationSalesId" type="xs:string"/>
				<xs:element name="ReceivingLocationId" type="xs:int"/>
				<xs:element name="ReceivingLocationSalesId" type="xs:string"/>
				<xs:element name="CreatedDate" type="xs:dateTime"/>
				<xs:element name="Description" type="xs:string"/>
				<xs:element name="AWC" type="xs:decimal"/>
				<xs:element name="Products">
					<xs:complexType>
						<xs:sequence>
							<xs:element name="Product" maxOccurs="unbounded" minOccurs="0">
								<xs:complexType>
									<xs:sequence>
										<xs:element name="Id" type="xs:int"/>
										<xs:element name="Type" type="xs:string"/>
										<xs:element name="DepartmentCode" type="xs:string"/>
										<xs:element name="Cost" type="xs:decimal"/>
										<xs:element name="Units" type="xs:int"/>
									</xs:sequence>
								</xs:complexType>
							</xs:element>
						</xs:sequence>
					</xs:complexType>
				</xs:element>
			</xs:sequence>
		</xs:complexType>
	</xs:element>
</xs:schema>
' WHERE SchemaSource = 'TransferMessage.xsd' 
update Hub.Queue set [Schema] = '
<xs:schema xmlns="http://www.bluebridgeltd.com/cosacs/2012/schema.xsd" xmlns:mstns="http://www.bluebridgeltd.com/cosacs/2012/schema.xsd" xmlns:xs="http://www.w3.org/2001/XMLSchema" targetNamespace="http://www.bluebridgeltd.com/cosacs/2012/schema.xsd" elementFormDefault="qualified">
  <xs:element name="VendorPurchaseOrder">
    <xs:complexType>
      <xs:sequence>
        <xs:element name="VendorId" minOccurs="1" maxOccurs="1" type="xs:int" />
        <xs:element name="VendorEmail" minOccurs="1" maxOccurs="1">
          <xs:simpleType>
            <xs:restriction base="xs:string">
              <xs:maxLength value="300" />
            </xs:restriction>
          </xs:simpleType>
        </xs:element>
        <xs:element name="VendorName" minOccurs="1" maxOccurs="1">
          <xs:simpleType>
            <xs:restriction base="xs:string">
              <xs:maxLength value="300" />
            </xs:restriction>
          </xs:simpleType>
        </xs:element>
        <xs:element name="PurchaseOrderId" minOccurs="1" maxOccurs="1" type="xs:int" />
      </xs:sequence>
    </xs:complexType>
  </xs:element>
</xs:schema>' WHERE SchemaSource = 'VendorPurchaseOrder.xsd' 
update Hub.Queue set [Schema] = '
<xs:schema xmlns="http://www.bluebridgeltd.com/cosacs/2012/schema.xsd" xmlns:mstns="http://www.bluebridgeltd.com/cosacs/2012/schema.xsd" xmlns:xs="http://www.w3.org/2001/XMLSchema" targetNamespace="http://www.bluebridgeltd.com/cosacs/2012/schema.xsd" elementFormDefault="qualified">
  <xs:element name="Id" type="xs:int"/>
  <xs:element name="Type" type="xs:string"/>
  <xs:element name="DepartmentCode" type="xs:string"/>
  <xs:element name="Cost" type="xs:float"/>
  <xs:element name="Units" type="xs:int"/>
  <xs:element name="Product">
    <xs:complexType>
      <xs:sequence>
        <xs:element ref="Id"/>
        <xs:element ref="Type"/>
        <xs:element ref="DepartmentCode"/>
		<xs:element ref="Cost"/>
		<xs:element ref="Units"/>
      </xs:sequence>
    </xs:complexType>
  </xs:element>
  <xs:element name="VendorReturnId" type="xs:int"/>
  <xs:element name="ReceiptType" type="xs:string"/>
  <xs:element name="LocationId" type="xs:int"/>
  <xs:element name="SalesLocationId" type="xs:string"/>
  <xs:element name="CreatedDate" type="xs:dateTime"/>
  <xs:element name="VendorId" type="xs:int"/>
  <xs:element name="VendorType" type="xs:string"/>
   <xs:element name="Description" type="xs:string"/>
  <xs:element name="Products">
    <xs:complexType>
      <xs:sequence>
        <xs:element ref="Product" maxOccurs="unbounded" minOccurs="0"/>
      </xs:sequence>
    </xs:complexType>
  </xs:element>
  <xs:element name="TotalLandedCost" type="xs:float"/>
  <xs:element name="VendorReturnMessage">
    <xs:complexType>
      <xs:sequence>
        <xs:element ref="VendorReturnId"/>
        <xs:element ref="ReceiptType"/>
        <xs:element ref="LocationId"/>
		<xs:element ref="SalesLocationId"/>
        <xs:element ref="CreatedDate"/>
        <xs:element ref="VendorId"/>
        <xs:element ref="VendorType"/>
		<xs:element ref="Description"/>
        <xs:element ref="Products"/>
        <xs:element ref="TotalLandedCost"/>
      </xs:sequence>
    </xs:complexType>
  </xs:element>
</xs:schema>
' WHERE SchemaSource = 'VendorReturnMessage.xsd' 
update Hub.Queue set [Schema] = '
<xs:schema xmlns="http://www.bluebridgeltd.com/cosacs/2012/schema.xsd" xmlns:mstns="http://www.bluebridgeltd.com/cosacs/2012/schema.xsd" xmlns:xs="http://www.w3.org/2001/XMLSchema" targetNamespace="http://www.bluebridgeltd.com/cosacs/2012/schema.xsd" elementFormDefault="qualified">
  <xs:element name="Company" type="xs:string"/>
  <xs:element name="VendorAction" type="xs:string"/>
  <xs:element name="VendorStatus" type="xs:string"/>
  <xs:element name="VendorType" type="xs:string"/>
  <xs:element name="CompanyRec">
    <xs:complexType>
      <xs:sequence>
        <xs:element ref="Company"/>
        <xs:element ref="VendorAction"/>
        <xs:element ref="VendorStatus"/>
        <xs:element ref="VendorType"/>
      </xs:sequence>
    </xs:complexType>
  </xs:element>
  <xs:element name="PaymentTerms" type="xs:string"/>
  <xs:element name="ContactName" type="xs:string"/>
  <xs:element name="ContactEmail" type="xs:string"/>
  <xs:element name="ContactPhone" type="xs:string"/>
  <xs:element name="CompanyPhone" type="xs:string"/>
  <xs:element name="AddressLine1" type="xs:string"/>
  <xs:element name="AddressLine2" type="xs:string"/>
  <xs:element name="City" type="xs:string"/>
  <xs:element name="VendorCountry" type="xs:string"/>
  <xs:element name="State" type="xs:string"/>
  <xs:element name="PostalCode" type="xs:string"/>
  <xs:element name="Record">
    <xs:complexType>
      <xs:sequence>
        <xs:element ref="PaymentTerms"/>
        <xs:element ref="ContactName"/>
        <xs:element ref="ContactEmail"/>
        <xs:element ref="ContactPhone"/>
        <xs:element ref="CompanyPhone"/>
        <xs:element ref="AddressLine1"/>
        <xs:element ref="AddressLine2"/>
        <xs:element ref="City"/>
        <xs:element ref="VendorCountry"/>
        <xs:element ref="State"/>
        <xs:element ref="PostalCode"/>
      </xs:sequence>
    </xs:complexType>
  </xs:element>
  <xs:element name="VendorCode" type="xs:string"/>
  <xs:element name="VendorName" type="xs:string"/>
  <xs:element name="CompanySection">
    <xs:complexType>
      <xs:sequence>
        <xs:element ref="CompanyRec"/>
      </xs:sequence>
    </xs:complexType>
  </xs:element>
  <xs:element name="VendorDetail">
    <xs:complexType>
      <xs:sequence>
        <xs:element ref="Record"/>
      </xs:sequence>
    </xs:complexType>
  </xs:element>
  <xs:element name="TotalRecords" type="xs:int"/>
  <xs:element name="VendorRecordHeader">
    <xs:complexType>
      <xs:sequence>
        <xs:element ref="VendorCode"/>
        <xs:element ref="VendorName"/>
        <xs:element ref="CompanySection"/>
        <xs:element ref="VendorDetail"/>
      </xs:sequence>
    </xs:complexType>
  </xs:element>
  <xs:element name="SummarySection">
    <xs:complexType>
      <xs:sequence>
        <xs:element ref="TotalRecords"/>
      </xs:sequence>
    </xs:complexType>
  </xs:element>
  <xs:element name="VendorInfoSending">
    <xs:complexType>
      <xs:sequence>
        <xs:element ref="VendorRecordHeader" maxOccurs="unbounded" minOccurs="0"/>
        <xs:element ref="SummarySection"/>
      </xs:sequence>
    </xs:complexType>
  </xs:element>
</xs:schema>
' WHERE SchemaSource = 'Vendors.xsd' 
