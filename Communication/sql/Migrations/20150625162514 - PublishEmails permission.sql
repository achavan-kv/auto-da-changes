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

    <xs:element name="MailMessage">
        <xs:complexType>
            <xs:sequence>
                <xs:element name="From"  minOccurs="0" maxOccurs="1" type="From" ></xs:element>
                <xs:element name="TemplateId"  maxOccurs="1" nillable="true" type="xs:short" ></xs:element>
                <xs:element name="OverrideUnsubscribe" maxOccurs="1" nillable="true" type="xs:boolean"></xs:element>
                <xs:element name="Mails" nillable="true" type="ArrayMail"/>
            </xs:sequence>
        </xs:complexType>
    </xs:element>

    <xs:complexType name="Mail">
        <xs:sequence>
            <xs:element name="From"  minOccurs="0" maxOccurs="1" type="From" ></xs:element>
            <xs:element name="To"  minOccurs="1" maxOccurs="1" >
                <xs:simpleType>
                    <xs:restriction base="xs:string">
                        <xs:maxLength value ="128"/>
                    </xs:restriction>
                </xs:simpleType>
            </xs:element>
            <xs:element name="CustomerId"  minOccurs="1" maxOccurs="1" >
                <xs:simpleType>
                    <xs:restriction base="xs:string">
                        <xs:maxLength value ="20"/>
                    </xs:restriction>
                </xs:simpleType>
            </xs:element>
            <xs:element name="TemplateId"  maxOccurs="1" nillable="true" type="xs:short" ></xs:element>
            <xs:element name="OverrideUnsubscribe" maxOccurs="1" nillable="true" type="xs:boolean"></xs:element>
            <xs:element name="ArrayOfTags" nillable="true" type="ArrayKeyValuePair"/>
            <xs:element name="Subject"  minOccurs="1" maxOccurs="1" >
                <xs:simpleType>
                    <xs:restriction base="xs:string">
                        <xs:maxLength value ="128"/>
                    </xs:restriction>
                </xs:simpleType>
            </xs:element>
        </xs:sequence>
    </xs:complexType>

    <xs:complexType name="From">
        <xs:sequence>
            <xs:element name="FromMail"  minOccurs="1" maxOccurs="1" >
                <xs:simpleType>
                    <xs:restriction base="xs:string">
                        <xs:maxLength value ="128"/>
                    </xs:restriction>
                </xs:simpleType>
            </xs:element>
            <xs:element name="FromName"  minOccurs="0" maxOccurs="1" >
                <xs:simpleType>
                    <xs:restriction base="xs:string">
                        <xs:maxLength value ="128"/>
                    </xs:restriction>
                </xs:simpleType>
            </xs:element>
        </xs:sequence>
    </xs:complexType>

    <xs:complexType name="KeyValuePair">
        <xs:sequence>
            <xs:element name="Key"  minOccurs="1" maxOccurs="1" >
                <xs:simpleType>
                    <xs:restriction base="xs:string">
                        <xs:maxLength value ="128"/>
                    </xs:restriction>
                </xs:simpleType>
            </xs:element>
            <xs:element name="Value"  minOccurs="1" maxOccurs="1" >
                <xs:simpleType>
                    <xs:restriction base="xs:string">
                        <xs:maxLength value ="1024"/>
                    </xs:restriction>
                </xs:simpleType>
            </xs:element>
        </xs:sequence>
    </xs:complexType>

    <xs:complexType name="ArrayKeyValuePair">
        <xs:sequence>
            <xs:element minOccurs="0" maxOccurs="unbounded" name="KeyValuePair" nillable="true" type="KeyValuePair"/>
        </xs:sequence>
    </xs:complexType>

    <xs:complexType name="ArrayMail">
        <xs:sequence>
            <xs:element minOccurs="0" maxOccurs="unbounded" name="Mail" nillable="true" type="Mail"/>
        </xs:sequence>
    </xs:complexType>
</xs:schema>'



UPDATE hub.Queue
SET [Schema] = @schema
WHERE id = 24