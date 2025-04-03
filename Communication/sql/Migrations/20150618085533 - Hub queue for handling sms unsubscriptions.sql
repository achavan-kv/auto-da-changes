-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
DECLARE @schema XML
SET @schema = '<?xml version="1.0" encoding="utf-8"?>
<xs:schema targetNamespace="http://www.bluebridgeltd.com/cosacs/2012/schema.xsd"
    elementFormDefault="qualified"
    xmlns="http://www.bluebridgeltd.com/cosacs/2012/schema.xsd"
    xmlns:mstns="http://www.bluebridgeltd.com/cosacs/2012/schema.xsd"
    xmlns:xs="http://www.w3.org/2001/XMLSchema">

    <xs:element name="CustomerPhoneNumbers">
        <xs:complexType>
            <xs:sequence>
                <xs:element name="CustomerId"  minOccurs="1" maxOccurs="1" >
                    <xs:simpleType>
                        <xs:restriction base="xs:string">
                            <xs:maxLength value ="20"/>
                        </xs:restriction>
                    </xs:simpleType>
                </xs:element>
                <xs:element name="Unsubscribe" minOccurs="1" maxOccurs="1" type="xs:boolean"></xs:element>
                <xs:element name="PhoneNumbers" nillable="true" type="ArrayPhoneNumbers"/>
            </xs:sequence>
        </xs:complexType>
    </xs:element>
    <xs:complexType name="ArrayPhoneNumbers">
        <xs:sequence>
            <xs:element name="Number"  minOccurs="0" maxOccurs="unbounded" >
                <xs:simpleType>
                    <xs:restriction base="xs:string">
                        <xs:maxLength value ="20"/>
                    </xs:restriction>
                </xs:simpleType>
            </xs:element>
        </xs:sequence>
    </xs:complexType>
</xs:schema>'

INSERT INTO hub.Queue
	(Id, [Binding], [Schema], SchemaSource, SubscriberHttpUrl, SubscriberHttpMethod)
VALUES
(  
	26,
    N'Cosacs.Communication.SmsUnsubscriptions',
	@schema,
	'PhoneNumbers.xsd',
    '/Communication/api/SmsUnsubscriptions',
    'POST'
) 