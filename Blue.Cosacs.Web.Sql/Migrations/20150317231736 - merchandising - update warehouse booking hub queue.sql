update hub.Queue
set [Schema] =
'<xs:schema xmlns="http://www.bluebridgeltd.com/cosacs/2012/schema.xsd" xmlns:mstns="http://www.bluebridgeltd.com/cosacs/2012/schema.xsd" xmlns:xs="http://www.w3.org/2001/XMLSchema" targetNamespace="http://www.bluebridgeltd.com/cosacs/2012/schema.xsd" elementFormDefault="qualified">
  <xs:element name="BookingSubmit">
    <xs:complexType>
      <xs:sequence>
        <xs:element name="Id" minOccurs="1" maxOccurs="1" type="xs:int" />
        <xs:element name="Recipient" minOccurs="1" maxOccurs="1">
          <xs:simpleType>
            <xs:restriction base="xs:string">
              <xs:maxLength value="95" />
            </xs:restriction>
          </xs:simpleType>
        </xs:element>
        <xs:element name="AddressLine1" minOccurs="1" maxOccurs="1">
          <xs:simpleType>
            <xs:restriction base="xs:string">
              <xs:maxLength value="50" />
            </xs:restriction>
          </xs:simpleType>
        </xs:element>
        <xs:element name="AddressLine2" minOccurs="0" maxOccurs="1">
          <xs:simpleType>
            <xs:restriction base="xs:string">
              <xs:maxLength value="50" />
            </xs:restriction>
          </xs:simpleType>
        </xs:element>
        <xs:element name="AddressLine3" minOccurs="0" maxOccurs="1">
          <xs:simpleType>
            <xs:restriction base="xs:string">
              <xs:maxLength value="50" />
            </xs:restriction>
          </xs:simpleType>
        </xs:element>
        <xs:element name="PostCode" minOccurs="0" maxOccurs="1">
          <xs:simpleType>
            <xs:restriction base="xs:string">
              <xs:maxLength value="10" />
            </xs:restriction>
          </xs:simpleType>
        </xs:element>
        <xs:element name="StockBranch" minOccurs="1" maxOccurs="1" type="xs:short" />
        <xs:element name="DeliveryBranch" minOccurs="1" maxOccurs="1" type="xs:short" />
        <xs:element name="Type" minOccurs="1" maxOccurs="1">
          <xs:simpleType>
            <xs:restriction base="xs:string">
              <xs:maxLength value="50" />
            </xs:restriction>
          </xs:simpleType>
        </xs:element>
        <xs:element name="RequestedDate" minOccurs="1" maxOccurs="1" type="xs:dateTime" />
        <xs:element name="SKU" minOccurs="1" maxOccurs="1">
          <xs:simpleType>
            <xs:restriction base="xs:string">
              <xs:maxLength value="18" />
            </xs:restriction>
          </xs:simpleType>
        </xs:element>
        <xs:element name="ItemId" minOccurs="1" maxOccurs="1" type="xs:int" />
        <xs:element name="ItemUPC" minOccurs="1" maxOccurs="1">
          <xs:simpleType>
            <xs:restriction base="xs:string">
              <xs:maxLength value="18" />
            </xs:restriction>
          </xs:simpleType>
        </xs:element>
        <xs:element name="ProductDescription" minOccurs="0" maxOccurs="1">
          <xs:simpleType>
            <xs:restriction base="xs:string">
              <xs:maxLength value="240" />
            </xs:restriction>
          </xs:simpleType>
        </xs:element>
        <xs:element name="ProductBrand" minOccurs="1" maxOccurs="1">
          <xs:simpleType>
            <xs:restriction base="xs:string">
              <xs:maxLength value="50" />
            </xs:restriction>
          </xs:simpleType>
        </xs:element>
        <xs:element name="ProductModel" minOccurs="1" maxOccurs="1">
          <xs:simpleType>
            <xs:restriction base="xs:string">
              <xs:maxLength value="50" />
            </xs:restriction>
          </xs:simpleType>
        </xs:element>
        <xs:element name="ProductArea" minOccurs="0" maxOccurs="1">
          <xs:simpleType>
            <xs:restriction base="xs:string">
              <xs:maxLength value="10" />
            </xs:restriction>
          </xs:simpleType>
        </xs:element>
        <xs:element name="ProductCategory" minOccurs="1" maxOccurs="1">
          <xs:simpleType>
            <xs:restriction base="xs:string">
              <xs:maxLength value="100" />
            </xs:restriction>
          </xs:simpleType>
        </xs:element>
        <xs:element name="Quantity" minOccurs="1" maxOccurs="1" type="xs:short" />
        <xs:element name="RepoItemId" minOccurs="1" maxOccurs="1" type="xs:int" />
        <xs:element name="Comment" minOccurs="0" maxOccurs="1">
          <xs:simpleType>
            <xs:restriction base="xs:string">
              <xs:maxLength value="200" />
            </xs:restriction>
          </xs:simpleType>
        </xs:element>
        <xs:element name="DeliveryZone" minOccurs="0" maxOccurs="1">
          <xs:simpleType>
            <xs:restriction base="xs:string">
              <xs:maxLength value="8" />
            </xs:restriction>
          </xs:simpleType>
        </xs:element>
        <xs:element name="ContactInfo" minOccurs="0" maxOccurs="1">
          <xs:simpleType>
            <xs:restriction base="xs:string">
              <xs:maxLength value="256" />
            </xs:restriction>
          </xs:simpleType>
        </xs:element>
        <xs:element name="OrderedOn" minOccurs="1" maxOccurs="1" type="xs:dateTime" />
        <xs:element name="Damaged" minOccurs="1" maxOccurs="1" type="xs:boolean" />
        <xs:element name="AssemblyReq" minOccurs="1" maxOccurs="1" type="xs:boolean" />
        <xs:element name="Express" minOccurs="1" maxOccurs="1" type="xs:boolean" />
        <xs:element name="Reference" minOccurs="1" maxOccurs="1">
          <xs:simpleType>
            <xs:restriction base="xs:string">
              <xs:maxLength value="12" />
            </xs:restriction>
          </xs:simpleType>
        </xs:element>
        <xs:element name="UnitPrice" minOccurs="1" maxOccurs="1" type="xs:decimal" />
        <xs:element name="CreatedBy" minOccurs="1" maxOccurs="1" type="xs:int" />
        <xs:element name="AddressNotes" minOccurs="0" maxOccurs="1">
          <xs:simpleType>
            <xs:restriction base="xs:string">
              <xs:maxLength value="1000" />
            </xs:restriction>
          </xs:simpleType>
        </xs:element>
        <xs:element name="Fascia" minOccurs="1" maxOccurs="1">
          <xs:simpleType>
            <xs:restriction base="xs:string">
              <xs:maxLength value="100" />
            </xs:restriction>
          </xs:simpleType>
        </xs:element>
        <xs:element name="PickUp" minOccurs="1" maxOccurs="1" type="xs:boolean" />
        <xs:element name="SalesBranch" minOccurs="1" maxOccurs="1" nillable="true" type="xs:short" />
        <xs:element name="NonStockServiceType" maxOccurs="1" nillable="true">
          <xs:simpleType>
            <xs:restriction base="xs:string">
              <xs:maxLength value="8" />
            </xs:restriction>
          </xs:simpleType>
        </xs:element>
        <xs:element name="NonStockServiceItemNo" maxOccurs="1" nillable="true">
          <xs:simpleType>
            <xs:restriction base="xs:string">
              <xs:maxLength value="18" />
            </xs:restriction>
          </xs:simpleType>
        </xs:element>
        <xs:element name="NonStockServiceDescription" maxOccurs="1" nillable="true">
          <xs:simpleType>
            <xs:restriction base="xs:string">
              <xs:maxLength value="75" />
            </xs:restriction>
          </xs:simpleType>
        </xs:element>
      </xs:sequence>
    </xs:complexType>
  </xs:element>
</xs:schema>'
where [Binding] = 'Warehouse.Booking.Submit'