update Hub.Queue
set [Schema] = 
'<xs:schema xmlns="http://www.bluebridgeltd.com/cosacs/2012/schema.xsd" xmlns:mstns="http://www.bluebridgeltd.com/cosacs/2012/schema.xsd" xmlns:xs="http://www.w3.org/2001/XMLSchema" targetNamespace="http://www.bluebridgeltd.com/cosacs/2012/schema.xsd" elementFormDefault="qualified">
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
  <xs:element name="AdjustmentId" type="xs:int"/>
  <xs:element name="LocationId" type="xs:int"/>
  <xs:element name="SalesLocationId" type="xs:string"/>
  <xs:element name="CreatedDate" type="xs:date"/>
  <xs:element name="SecondaryReason" type="xs:string"/>
  <xs:element name="Description" type="xs:string"/>
  <xs:element name="Products">
    <xs:complexType>
      <xs:sequence>
        <xs:element ref="Product" maxOccurs="unbounded" minOccurs="0"/>
      </xs:sequence>
    </xs:complexType>
  </xs:element>
  <xs:element name="TotalAWC" type="xs:float"/>
  <xs:element name="StockAdjustmentMessage">
    <xs:complexType>
      <xs:sequence>
        <xs:element ref="AdjustmentId"/>
        <xs:element ref="LocationId"/>
		<xs:element ref="SalesLocationId"/>
        <xs:element ref="CreatedDate"/>
        <xs:element ref="SecondaryReason"/>
		<xs:element ref="Description"/>
        <xs:element ref="Products"/>
        <xs:element ref="TotalAWC"/>
      </xs:sequence>
    </xs:complexType>
  </xs:element>
</xs:schema>'
where binding = 'Merchandising.StockAdjustmentCreated'

update Hub.Queue
set [Schema] = 
'<xs:schema xmlns="http://www.bluebridgeltd.com/cosacs/2012/schema.xsd" xmlns:mstns="http://www.bluebridgeltd.com/cosacs/2012/schema.xsd" xmlns:xs="http://www.w3.org/2001/XMLSchema" targetNamespace="http://www.bluebridgeltd.com/cosacs/2012/schema.xsd" elementFormDefault="qualified">
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
  <xs:element name="CreatedDate" type="xs:date"/>
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
</xs:schema>'
where binding = 'Merchandising.VendorReturnCreated'

update Hub.Queue
set [Schema] = 
'<xs:schema xmlns="http://www.bluebridgeltd.com/cosacs/2012/schema.xsd" xmlns:mstns="http://www.bluebridgeltd.com/cosacs/2012/schema.xsd" xmlns:xs="http://www.w3.org/2001/XMLSchema" targetNamespace="http://www.bluebridgeltd.com/cosacs/2012/schema.xsd" elementFormDefault="qualified">
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
  <xs:element name="ReceiptId" type="xs:int"/>
  <xs:element name="ReceiptType" type="xs:string"/>
  <xs:element name="LocationId" type="xs:int"/>
  <xs:element name="SalesLocationId" type="xs:string"/>
  <xs:element name="CreatedDate" type="xs:date"/>
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
  <xs:element name="GoodsReceiptMessage">
    <xs:complexType>
      <xs:sequence>
        <xs:element ref="ReceiptId"/>
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
</xs:schema>'
where binding = 'Merchandising.GoodsReceiptCreated'

update Hub.Queue
set [Schema] = 
'<xs:schema xmlns="http://www.bluebridgeltd.com/cosacs/2012/schema.xsd" xmlns:mstns="http://www.bluebridgeltd.com/cosacs/2012/schema.xsd" xmlns:xs="http://www.w3.org/2001/XMLSchema" targetNamespace="http://www.bluebridgeltd.com/cosacs/2012/schema.xsd" elementFormDefault="qualified">
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
  <xs:element name="WarehouseLocationId" type="xs:int"/>
  <xs:element name="WarehouseLocationSalesId" type="xs:string"/>
  <xs:element name="ReceivingLocationId" type="xs:int"/>
  <xs:element name="ReceivingLocationSalesId" type="xs:string"/>
  <xs:element name="CreatedDate" type="xs:date"/>
   <xs:element name="Description" type="xs:string"/>
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
	    <xs:element name="Id" type="xs:int"/>
		<xs:element name="Type" type="xs:string"/>
        <xs:element ref="WarehouseLocationId"/>
		<xs:element ref="WarehouseLocationSalesId"/>
        <xs:element ref="ReceivingLocationId"/>
		<xs:element ref="ReceivingLocationSalesId"/>
        <xs:element ref="CreatedDate"/>
		<xs:element ref="Description"/>
        <xs:element ref="Products"/>
        <xs:element ref="TotalAWC"/>
      </xs:sequence>
    </xs:complexType>
  </xs:element>
</xs:schema>'
where binding = 'Merchandising.Transfer'