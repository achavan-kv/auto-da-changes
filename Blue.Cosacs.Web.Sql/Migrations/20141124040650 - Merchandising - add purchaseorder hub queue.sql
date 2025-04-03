-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
INSERT INTO Hub.Queue
(
    Hub.Queue.Id,
    Hub.Queue.[Binding],
    Hub.Queue.[Schema],
    Hub.Queue.SubscriberClrAssemblyName,
    Hub.Queue.SubscriberClrTypeName,
    Hub.Queue.SubscriberSqlConnectionName,
    Hub.Queue.SubscriberSqlProcedureName,
    Hub.Queue.SchemaSource
)
VALUES
(   
	210,
    'Merchandising.PurchaseOrder',
    '<?xml version="1.0" encoding="utf-8"?>
		<xs:schema targetNamespace="http://www.bluebridgeltd.com/cosacs/2012/schema.xsd" elementFormDefault="qualified" xmlns="http://www.bluebridgeltd.com/cosacs/2012/schema.xsd" xmlns:mstns="http://www.bluebridgeltd.com/cosacs/2012/schema.xsd" xmlns:xs="http://www.w3.org/2001/XMLSchema">   
			<xs:element name="VendorPurchaseOrder">
				<xs:complexType>
					<xs:sequence>
						<xs:element name="VendorId" minOccurs="1" maxOccurs="1" type="xs:int"/>
						<xs:element name="VendorEmail" minOccurs="1" maxOccurs="1">
							<xs:simpleType>
								<xs:restriction base="xs:string">
									<xs:maxLength value ="300"/>
								</xs:restriction>
							</xs:simpleType>
						</xs:element>
						<xs:element name="VendorName" minOccurs="1" maxOccurs="1">
							<xs:simpleType>
								<xs:restriction base="xs:string">
									<xs:maxLength value ="300"/>
								</xs:restriction>
							</xs:simpleType>
						</xs:element>
						<xs:element name="PurchaseOrderId" minOccurs="1" maxOccurs="1" type="xs:int"/>                     
					</xs:sequence>
				</xs:complexType>
			</xs:element>
		</xs:schema>',
    'Blue.Cosacs.Merchandising', 
    'Blue.Cosacs.Merchandising.Subscribers.VendorMailSubscriber',
    NULL,
    NULL,
    'Merchandising.xsd'
)