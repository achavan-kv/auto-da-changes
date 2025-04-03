-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 

update hub.queue set [schema] = '<?xml version="1.0" encoding="utf-8"?>
<xs:schema targetNamespace="http://www.bluebridgeltd.com/cosacs/2012/schema.xsd"
    elementFormDefault="qualified"
    xmlns="http://www.bluebridgeltd.com/cosacs/2012/schema.xsd"
    xmlns:mstns="http://www.bluebridgeltd.com/cosacs/2012/schema.xsd"
    xmlns:xs="http://www.w3.org/2001/XMLSchema">

    <xs:element name="WarehouseDeliver">
        <xs:complexType>
            <xs:sequence>
                <xs:element name="Id" minOccurs="1" maxOccurs="1" type="xs:int" />
                <xs:element name="OrigBookingId" minOccurs="1" maxOccurs="1" type="xs:int" />
                <xs:element name="Quantity" minOccurs="1" maxOccurs="1" type="xs:int" />
                <xs:element name="Date"  minOccurs="1" maxOccurs="1" type="xs:dateTime"/>
                <xs:element name="ConfirmedBy" minOccurs="1" maxOccurs="1" type="xs:int" />
                <xs:element name="DeliveryOrCollection"  minOccurs="0" maxOccurs="1" >
                    <xs:simpleType>
                        <xs:restriction base="xs:string">
                            <xs:maxLength value ="1"/>
                        </xs:restriction>
                    </xs:simpleType>
                </xs:element>
                <xs:element name="CustomerAccount"  minOccurs="0" maxOccurs="1" >
                    <xs:simpleType>
                        <xs:restriction base="xs:string">
                            <xs:maxLength value ="12"/>
                        </xs:restriction>
                    </xs:simpleType>
                </xs:element>
				<xs:element name="UnitPrice" minOccurs="1" maxOccurs="1" type="xs:decimal" />
            </xs:sequence>
        </xs:complexType>
    </xs:element>
</xs:schema>'
where binding = 'Cosacs.Booking.Deliver'