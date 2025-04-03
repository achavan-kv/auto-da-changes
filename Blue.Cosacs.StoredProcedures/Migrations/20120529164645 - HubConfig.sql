DELETE FROM hub.Message
Go

DELETE  from hub.Queue
GO


DECLARE @schema XML
SET @schema = '<?xml version="1.0" encoding="utf-8"?>
<xs:schema targetNamespace="http://www.bluebridgeltd.com/cosacs/2012/schema.xsd"
    elementFormDefault="qualified"
    xmlns="http://www.bluebridgeltd.com/cosacs/2012/schema.xsd"
    xmlns:mstns="http://www.bluebridgeltd.com/cosacs/2012/schema.xsd"
    xmlns:xs="http://www.w3.org/2001/XMLSchema">

    <xs:element name="BookingSubmit">
        <xs:complexType>
            <xs:sequence>
                <xs:element name="Id" minOccurs="1" maxOccurs="1" type="xs:int" />
                <xs:element name="CustomerName" minOccurs="1" maxOccurs="1">
                    <xs:simpleType>
                        <xs:restriction base="xs:string">
                            <xs:maxLength value="90" />
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
                <xs:element name="AddressLine2" minOccurs="1" maxOccurs="1">
                    <xs:simpleType>
                        <xs:restriction base="xs:string">
                            <xs:maxLength value="50" />
                        </xs:restriction>
                    </xs:simpleType>
                </xs:element>
                <xs:element name="AddressLine3" minOccurs="1" maxOccurs="1">
                    <xs:simpleType>
                        <xs:restriction base="xs:string">
                            <xs:maxLength value="50" />
                        </xs:restriction>
                    </xs:simpleType>
                </xs:element>
                <xs:element name="PostCode" minOccurs="1" maxOccurs="1">
                    <xs:simpleType>
                        <xs:restriction base="xs:string">
                            <xs:maxLength value="10" />
                        </xs:restriction>
                    </xs:simpleType>
                </xs:element>
                <xs:element name="StockBranch" minOccurs="1" maxOccurs="1" type="xs:short" />
                <xs:element name="DeliveryBranch" minOccurs="1" maxOccurs="1" type="xs:short" />
                <xs:element name="DeliveryOrCollection" minOccurs="1" maxOccurs="1">
                    <xs:simpleType>
                        <xs:restriction base="xs:string">
                            <xs:maxLength value ="1"/>
                        </xs:restriction>
                    </xs:simpleType>
                </xs:element>
                <xs:element name="DeliveryOrCollectionDate" minOccurs="1" maxOccurs="1" type="xs:dateTime"/>
                <xs:element name="ItemNo" minOccurs="1" maxOccurs="1">
                    <xs:simpleType>
                        <xs:restriction base="xs:string">
                            <xs:maxLength value ="18"/>
                        </xs:restriction>
                    </xs:simpleType>
                </xs:element>
                <xs:element name="ItemId" minOccurs="1" maxOccurs="1" type="xs:int" />
                <xs:element name="ItemUPC" minOccurs="1" maxOccurs="1">
                    <xs:simpleType>
                        <xs:restriction base="xs:string">
                            <xs:maxLength value ="18"/>
                        </xs:restriction>
                    </xs:simpleType>
                </xs:element>
                <xs:element name="ProductDescription" minOccurs="1" maxOccurs="1">
                    <xs:simpleType>
                        <xs:restriction base="xs:string">
                            <xs:maxLength value ="100"/>
                        </xs:restriction>
                    </xs:simpleType>
                </xs:element>
                <xs:element name="ProductBrand" minOccurs="1" maxOccurs="1">
                    <xs:simpleType>
                        <xs:restriction base="xs:string">
                            <xs:maxLength value ="50"/>
                        </xs:restriction>
                    </xs:simpleType>
                </xs:element>
                <xs:element name="ProductModel" minOccurs="1" maxOccurs="1">
                    <xs:simpleType>
                        <xs:restriction base="xs:string">
                            <xs:maxLength value ="50"/>
                        </xs:restriction>
                    </xs:simpleType>
                </xs:element>
                <xs:element name="ProductArea" minOccurs="1" maxOccurs="1">
                    <xs:simpleType>
                        <xs:restriction base="xs:string">
                            <xs:maxLength value ="10"/>
                        </xs:restriction>
                    </xs:simpleType>
                </xs:element>
                <xs:element name="ProductCategory" minOccurs="1" maxOccurs="1">
                    <xs:simpleType>
                        <xs:restriction base="xs:string">
                            <xs:maxLength value ="10"/>
                        </xs:restriction>
                    </xs:simpleType>
                </xs:element>
                <xs:element name="Quantity" minOccurs="1" maxOccurs="1" type="xs:short" />
                <xs:element name="RepoItemId" minOccurs="1" maxOccurs="1" type="xs:int" />
                <xs:element name="Comment" minOccurs="1" maxOccurs="1">
                    <xs:simpleType>
                        <xs:restriction base="xs:string">
                            <xs:maxLength value ="200"/>
                        </xs:restriction>
                    </xs:simpleType>
                </xs:element>
                <xs:element name="DeliveryZone" minOccurs="1" maxOccurs="1">
                    <xs:simpleType>
                        <xs:restriction base="xs:string">
                            <xs:maxLength value ="8"/>
                        </xs:restriction>
                    </xs:simpleType>
                </xs:element>
                <xs:element name="ContactInfo" minOccurs="1" maxOccurs="1">
                    <xs:simpleType>
                        <xs:restriction base="xs:string">
                            <xs:maxLength value ="256"/>
                        </xs:restriction>
                    </xs:simpleType>
                </xs:element>
                <xs:element name="OrderedOn" minOccurs="1" maxOccurs="1" type="xs:dateTime"/>
                <xs:element name="Damaged" minOccurs="1" maxOccurs="1" type="xs:boolean"/>
                <xs:element name="AssemblyReq" minOccurs="1" maxOccurs="1" type="xs:boolean"/>
                <xs:element name="Acctno" minOccurs="1" maxOccurs="1">
                    <xs:simpleType>
                        <xs:restriction base="xs:string">
                            <xs:maxLength value ="12"/>
                        </xs:restriction>
                    </xs:simpleType>
                </xs:element>
                <xs:element name="UnitPrice" minOccurs="1" maxOccurs="1" type="xs:decimal"/>
            </xs:sequence>            
        </xs:complexType>        
    </xs:element>

    <xs:element name="BookingCancel">
        <xs:complexType>
            <xs:sequence>
                <xs:element name="Id" minOccurs="1" maxOccurs="1" type="xs:int" />
                <xs:element name="User" minOccurs="1" maxOccurs="1" type="xs:int" />
                <xs:element name="Quantity" minOccurs="1" maxOccurs="1" type="xs:int" />
                <xs:element name="DateTime" minOccurs="1" maxOccurs="1" type="xs:dateTime" />
                <xs:element name="Comment"  minOccurs="1" maxOccurs="1" >
                    <xs:simpleType>
                        <xs:restriction base="xs:string">
                            <xs:maxLength value ="200"/>
                        </xs:restriction>
                    </xs:simpleType>
                </xs:element>
            </xs:sequence>
        </xs:complexType>
    </xs:element>

    <xs:element name="BookingDeliver">
        <xs:complexType>
            <xs:sequence>
                <xs:element name="Id" minOccurs="1" maxOccurs="1" type="xs:int" />
                <xs:element name="Quantity" minOccurs="1" maxOccurs="1" type="xs:int" />
                <xs:element name="Date"  minOccurs="1" maxOccurs="1" type="xs:dateTime"/>
            </xs:sequence>
        </xs:complexType>
    </xs:element>
</xs:schema>
'




INSERT INTO hub.Queue
        ( Id ,
          Name ,
          [Schema] ,
          SubscriberAssemblyName ,
          SubscriberTypeName
        )
SELECT  1 , -- Id - int
          N'Warehouse.Booking.Submit' , -- Name - nvarchar(100)
         @schema , -- Schema - xml
          N'Blue.Cosacs.Warehouse' , -- SubscriberAssemblyName - nvarchar(200)
          N'Blue.Cosacs.Warehouse.Subscribers.BookingSubmit'  -- SubscriberTypeName - nvarchar(200)
          
          UNION ALL
          
 SELECT  2 , -- Id - int
          N'Warehouse.Booking.Cancel' , -- Name - nvarchar(100)
           @schema , -- Schema - xml
          N'Blue.Cosacs.Warehouse' , -- SubscriberAssemblyName - nvarchar(200)
          N'Blue.Cosacs.Warehouse.Subscribers.BookingCancel'  -- SubscriberTypeName - nvarchar(200)
          UNION ALL
     
      SELECT  3 , -- Id - int
          N'Cosacs.Booking.Deliver' , -- Name - nvarchar(100)
          @schema , -- Schema - xml
          N'Blue.Cosacs' , -- SubscriberAssemblyName - nvarchar(200)
          N'Cosacs.Booking.Deliver'  -- SubscriberTypeName - nvarchar(200)
UNION ALL 

      SELECT  4 , -- Id - int
            N'Cosacs.Booking.Cancel', -- Name - nvarchar(100)
          @schema , -- Schema - xml
          N'Blue.Cosacs' , -- SubscriberAssemblyName - nvarchar(200)
          N'Cosacs.Booking.Cancel'  -- SubscriberTypeName - nvarchar(200)

     
GO

