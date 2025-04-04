-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

UPDATE Hub.[Queue]
SET [Schema] = 
'<xs:schema xmlns="http://www.bluebridgeltd.com/cosacs/2012/schema.xsd" xmlns:mstns="http://www.bluebridgeltd.com/cosacs/2012/schema.xsd" xmlns:xs="http://www.w3.org/2001/XMLSchema" targetNamespace="http://www.bluebridgeltd.com/cosacs/2012/schema.xsd" elementFormDefault="qualified">
  <xs:element name="TransferMessage">
    <xs:complexType>
      <xs:sequence>
        <xs:element name="Id" type="xs:int" />
        <xs:element name="Type" type="xs:string" />
        <xs:element name="WarehouseLocationId" type="xs:int" />
        <xs:element name="WarehouseLocationSalesId" type="xs:string" />
        <xs:element name="ReceivingLocationId" type="xs:int" />
        <xs:element name="ReceivingLocationSalesId" type="xs:string" />
        <xs:element name="CreatedDate" type="xs:dateTime" />
        <xs:element name="Description" type="xs:string" />
        <xs:element name="AWC" type="xs:decimal" />
        <xs:element name="Products">
          <xs:complexType>
            <xs:sequence>
              <xs:element name="Product" maxOccurs="unbounded" minOccurs="0">
                <xs:complexType>
                  <xs:sequence>
                    <xs:element name="Id" type="xs:int" />
                    <xs:element name="Type" type="xs:string" />
                    <xs:element name="DepartmentCode" type="xs:string" />
                    <xs:element name="Cost" type="xs:decimal" />
                    <xs:element name="Units" type="xs:int" />
                  </xs:sequence>
                </xs:complexType>
              </xs:element>
            </xs:sequence>
          </xs:complexType>
        </xs:element>
      </xs:sequence>
    </xs:complexType>
  </xs:element>
</xs:schema>'
WHERE Id = 214