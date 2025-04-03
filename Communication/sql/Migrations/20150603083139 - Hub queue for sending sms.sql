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

    <xs:element name="SmsMessage">
        <xs:complexType>
            <xs:sequence>
                <xs:element name="Sms" nillable="true" type="ArraySms"/>
            </xs:sequence>
        </xs:complexType>
    </xs:element>

    <xs:complexType name="Sms">
        <xs:sequence>
            <xs:element name="Phone"  minOccurs="1" maxOccurs="1" >
                <xs:simpleType>
                    <xs:restriction base="xs:string">
                        <xs:maxLength value ="26"/>
                    </xs:restriction>
                </xs:simpleType>
            </xs:element>
            <xs:element name="Body"  minOccurs="1" maxOccurs="1" >
                <xs:simpleType>
                    <xs:restriction base="xs:string">
                        <xs:maxLength value ="160"/>
                    </xs:restriction>
                </xs:simpleType>
            </xs:element>
        </xs:sequence>
    </xs:complexType>

    <xs:complexType name="ArraySms">
        <xs:sequence>
            <xs:element minOccurs="0" maxOccurs="unbounded" name="Mail" nillable="true" type="Sms"/>
        </xs:sequence>
    </xs:complexType>
</xs:schema>'

INSERT INTO hub.Queue
	(Id, [Binding], [Schema], SchemaSource, SubscriberHttpUrl, SubscriberHttpMethod)
VALUES
(  
	25,
    N'Cosacs.Communication.SendSms',
	@schema,
	'SmsMessage.xsd',
    '/Communication/api/SendSms',
    'POST'
) 