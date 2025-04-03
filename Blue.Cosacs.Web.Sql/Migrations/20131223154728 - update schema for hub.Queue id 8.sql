-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
UPDATE hub.Queue
set [Schema] = '<xs:schema xmlns="http://www.bluebridgeltd.com/cosacs/2012/schema.xsd" xmlns:mstns="http://www.bluebridgeltd.com/cosacs/2012/schema.xsd" xmlns:xs="http://www.w3.org/2001/XMLSchema" targetNamespace="http://www.bluebridgeltd.com/cosacs/2012/schema.xsd" elementFormDefault="qualified">
  <xs:complexType name="ServiceCharge">
    <xs:all>
      <xs:element name="ServiceRequestNo" minOccurs="1" maxOccurs="1" type="xs:int" />
      <xs:element name="Branch" minOccurs="1" maxOccurs="1" type="xs:int" />
      <xs:element name="CustomerId" minOccurs="1" maxOccurs="1" nillable="true">
        <xs:simpleType>
          <xs:restriction base="xs:string">
            <xs:maxLength value="50" />
          </xs:restriction>
        </xs:simpleType>
      </xs:element>
      <xs:element name="Account" minOccurs="0" maxOccurs="1" nillable="true">
        <xs:simpleType>
          <xs:restriction base="xs:string">
            <xs:maxLength value="12" />
          </xs:restriction>
        </xs:simpleType>
      </xs:element>
      <xs:element name="ItemNumber" minOccurs="0" maxOccurs="1">
        <xs:simpleType>
          <xs:restriction base="xs:string">
            <xs:maxLength value="18" />
          </xs:restriction>
        </xs:simpleType>
      </xs:element>
      <xs:element name="Value" minOccurs="1" maxOccurs="1" type="xs:decimal" />
      <xs:element name="Cost" minOccurs="1" maxOccurs="1" type="xs:decimal" />
      <xs:element name="Tax" minOccurs="1" maxOccurs="1" type="xs:decimal" nillable="true" />
      <xs:element name="ChargeType" minOccurs="1" maxOccurs="1">
        <xs:simpleType>
          <xs:restriction base="xs:string">
            <xs:maxLength value="30" />
          </xs:restriction>
        </xs:simpleType>
      </xs:element>
      <xs:element name="Label" minOccurs="1" maxOccurs="1" nillable="true">
        <xs:simpleType>
          <xs:restriction base="xs:string">
            <xs:maxLength value="50" />
          </xs:restriction>
        </xs:simpleType>
      </xs:element>
      <xs:element name="IsExternal" minOccurs="1" maxOccurs="1" type="xs:boolean" nillable="true" />
    </xs:all>
  </xs:complexType>
  <xs:element name="ServiceCharges">
    <xs:complexType>
      <xs:sequence>
        <xs:element name="EmpeeNo" minOccurs="1" maxOccurs="1" type="xs:int" />
        <xs:element name="ServiceCharge" minOccurs="1" maxOccurs="unbounded" type="ServiceCharge" />
		<xs:element name="ServiceType" minOccurs="0" maxOccurs="1">
			<xs:simpleType>
				<xs:restriction base="xs:string">
					<xs:maxLength value ="2"/>
				</xs:restriction>
			</xs:simpleType>
		</xs:element>
      </xs:sequence>
    </xs:complexType>
  </xs:element>
</xs:schema>'
WHERE 
	id = 8