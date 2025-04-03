update Hub.Queue set [Schema] = N'
<xs:schema xmlns="http://www.bluebridgeltd.com/cosacs/2012/schema.xsd" xmlns:mstns="http://www.bluebridgeltd.com/cosacs/2012/schema.xsd" xmlns:xs="http://www.w3.org/2001/XMLSchema" targetNamespace="http://www.bluebridgeltd.com/cosacs/2012/schema.xsd" elementFormDefault="qualified">
  <xs:element name="BookingId" type="xs:int" />
  <xs:element name="CurrentBookingId" type="xs:int" nillable="true" />
  <xs:element name="Type" type="xs:string" />
  <xs:element name="UserId" type="xs:int" />
  <xs:element name="Quantity" type="xs:int" />
  <xs:element name="AverageWeightedCost" type="xs:decimal" />
  <xs:element name="BookingMessage">
    <xs:complexType>
      <xs:sequence>
        <xs:element ref="BookingId" />
        <xs:element ref="CurrentBookingId" />
        <xs:element ref="Type" />
        <xs:element ref="UserId" />
        <xs:element ref="Quantity" />
        <xs:element ref="AverageWeightedCost" />
      </xs:sequence>
    </xs:complexType>
  </xs:element>
</xs:schema>',
SchemaSource = 'BookingMessage.xsd' 
WHERE Binding = 'Merchandising.Booking.Cancel'