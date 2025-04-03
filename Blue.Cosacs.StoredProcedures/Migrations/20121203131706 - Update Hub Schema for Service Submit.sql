-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

UPDATE hub.[Queue] 
	SET [Schema] = '<xs:schema targetNamespace="http://www.bluebridgeltd.com/cosacs/2012/schema.xsd"
    elementFormDefault="qualified"
    xmlns="http://www.bluebridgeltd.com/cosacs/2012/schema.xsd"
    xmlns:mstns="http://www.bluebridgeltd.com/cosacs/2012/schema.xsd"
    xmlns:xs="http://www.w3.org/2001/XMLSchema">

    <xs:complexType name="Customer">
        <xs:sequence>
            <xs:element name="CustomerId" minOccurs="0" maxOccurs="1">
                <xs:simpleType>
                    <xs:restriction base="xs:string">
                        <xs:maxLength value="50" />
                    </xs:restriction>
                </xs:simpleType>
            </xs:element>
            <xs:element name="Title" minOccurs="1" maxOccurs="1">
                <xs:simpleType>
                    <xs:restriction base="xs:string">
                        <xs:maxLength value="25" />
                    </xs:restriction>
                </xs:simpleType>
            </xs:element>
            <xs:element name="FirstName" minOccurs="1" maxOccurs="1">
                <xs:simpleType>
                    <xs:restriction base="xs:string">
                        <xs:maxLength value="50" />
                    </xs:restriction>
                </xs:simpleType>
            </xs:element>
            <xs:element name="LastName" minOccurs="1" maxOccurs="1">
                <xs:simpleType>
                    <xs:restriction base="xs:string">
                        <xs:maxLength value="50" />
                    </xs:restriction>
                </xs:simpleType>
            </xs:element>
            <xs:element name="HomePhone" minOccurs="1" maxOccurs="1">
                <xs:simpleType>
                    <xs:restriction base="xs:string">
                        <xs:maxLength value="50" />
                    </xs:restriction>
                </xs:simpleType>
            </xs:element>
            <xs:element name="WorkPhone" minOccurs="1" maxOccurs="1">
                <xs:simpleType>
                    <xs:restriction base="xs:string">
                        <xs:maxLength value="50" />
                    </xs:restriction>
                </xs:simpleType>
            </xs:element>
            <xs:element name="MobilePhone" minOccurs="1" maxOccurs="1">
                <xs:simpleType>
                    <xs:restriction base="xs:string">
                        <xs:maxLength value="50" />
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
            <xs:element name="Email" minOccurs="0" maxOccurs="1">
                <xs:simpleType>
                    <xs:restriction base="xs:string">
                        <xs:maxLength value="256" />
                    </xs:restriction>
                </xs:simpleType>
            </xs:element>
            <xs:element name="Notes" minOccurs="0" maxOccurs="1">
                <xs:simpleType>
                    <xs:restriction base="xs:string">
                        <xs:maxLength value="4000" />
                    </xs:restriction>
                </xs:simpleType>
            </xs:element>
        </xs:sequence>
    </xs:complexType>

    <xs:complexType name="Item">
        <xs:sequence>
            <xs:element name="Id" minOccurs="1" maxOccurs="1" type="xs:int"/>
            <xs:element name="Quantity" minOccurs="1" maxOccurs="1" type="xs:int"/>
            <xs:element name="StockLocn" minOccurs="1" maxOccurs="1" type="xs:int"/>
            <xs:element name="Amount" minOccurs="1" maxOccurs="1" type="xs:decimal" />
            <xs:element name="SoldOn" minOccurs="0" maxOccurs="1" type="xs:dateTime" />
            <xs:element name="DeliveredOn" minOccurs="0" maxOccurs="1" type="xs:dateTime" />
            <xs:element name="Product" minOccurs="1" maxOccurs="1">
                <xs:simpleType>
                    <xs:restriction base="xs:string">
                        <xs:maxLength value ="100"/>
                    </xs:restriction>
                </xs:simpleType>
            </xs:element>
            <xs:element name="Supplier" minOccurs="1" maxOccurs="1">
                <xs:simpleType>
                    <xs:restriction base="xs:string">
                        <xs:maxLength value ="50"/>
                    </xs:restriction>
                </xs:simpleType>
            </xs:element>
        </xs:sequence>
    </xs:complexType>

    <xs:complexType name="Warranty">
        <xs:sequence>
            <xs:element name="Id" minOccurs="1" maxOccurs="1" type="xs:int"/>
            <xs:element name="Length" minOccurs="1" maxOccurs="1" type="xs:int"/>
            <xs:element name="ContractId" minOccurs="1" maxOccurs="1">
                <xs:simpleType>
                    <xs:restriction base="xs:string">
                        <xs:maxLength value ="20"/>
                    </xs:restriction>
                </xs:simpleType>
            </xs:element>
        </xs:sequence>
    </xs:complexType>

    <xs:element name="SalesOrder">
        <xs:complexType>
            <xs:sequence>
                <xs:element name="InvoiceNumber" minOccurs="1" maxOccurs="1" type="xs:long"/>
                <xs:element name="Customer" minOccurs="0" maxOccurs="1" type="Customer"/>
                <xs:element name="Item" minOccurs="1" maxOccurs="1" type="Item"/>
                <xs:element name="Warranty" minOccurs="0" maxOccurs="1" type="Warranty"/>
            </xs:sequence>
        </xs:complexType>
    </xs:element>

    <xs:element name="ServiceSummary">
        <xs:complexType>
            <xs:sequence>
                <xs:element name="Acctno" minOccurs="1" maxOccurs="1">
                    <xs:simpleType>
                        <xs:restriction base="xs:string">
                            <xs:maxLength value ="12"/>
                        </xs:restriction>
                    </xs:simpleType>
                </xs:element>
                <xs:element name="ServiceRequestNo" minOccurs="1" maxOccurs="1" type="xs:int" />
                <xs:element name="Branch" minOccurs="1" maxOccurs="1" type="xs:int"/>
                <xs:element name="ItemId" minOccurs="1" maxOccurs="1" type="xs:int" />
                <xs:element name="StockLocn" minOccurs="1" maxOccurs="1" type="xs:int"/>
                <xs:element name="DateLogged" minOccurs="1" maxOccurs="1" type="xs:dateTime"/>
                <xs:element name="DateClosed" minOccurs="1" maxOccurs="1" type="xs:dateTime"/>
            </xs:sequence>
        </xs:complexType>
    </xs:element>

    <xs:element name="RequestSubmit">
        <xs:complexType>
            <xs:sequence>
                <xs:element name="Id" minOccurs="1" maxOccurs="1" type="xs:int" />
                <xs:element name="Account" minOccurs="1" maxOccurs="1">
                    <xs:simpleType>
                        <xs:restriction base="xs:string">
                            <xs:maxLength value ="12"/>
                        </xs:restriction>
                    </xs:simpleType>
                </xs:element>
                <xs:element name="Branch" minOccurs="1" maxOccurs="1" type="xs:short" />
                <xs:element name="CreatedBy" minOccurs="1" maxOccurs="1">
                    <xs:simpleType>
                        <xs:restriction base="xs:string">
                            <xs:maxLength value="50" />
                        </xs:restriction>
                    </xs:simpleType>
                </xs:element>
                <xs:element name="CreatedById" minOccurs="1" maxOccurs="1" type="xs:int" />
                <xs:element name="CustomerId" minOccurs="0" maxOccurs="1">
                    <xs:simpleType>
                        <xs:restriction base="xs:string">
                            <xs:maxLength value="50" />
                        </xs:restriction>
                    </xs:simpleType>
                </xs:element>
                <xs:element name="Title" minOccurs="1" maxOccurs="1">
                    <xs:simpleType>
                        <xs:restriction base="xs:string">
                            <xs:maxLength value="25" />
                        </xs:restriction>
                    </xs:simpleType>
                </xs:element>
                <xs:element name="FirstName" minOccurs="1" maxOccurs="1">
                    <xs:simpleType>
                        <xs:restriction base="xs:string">
                            <xs:maxLength value="50" />
                        </xs:restriction>
                    </xs:simpleType>
                </xs:element>
                <xs:element name="LastName" minOccurs="1" maxOccurs="1">
                    <xs:simpleType>
                        <xs:restriction base="xs:string">
                            <xs:maxLength value="50" />
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
                <xs:element name="HomePhone" minOccurs="1" maxOccurs="1">
                    <xs:simpleType>
                        <xs:restriction base="xs:string">
                            <xs:maxLength value="50" />
                        </xs:restriction>
                    </xs:simpleType>
                </xs:element>
                <xs:element name="WorkPhone" minOccurs="1" maxOccurs="1">
                    <xs:simpleType>
                        <xs:restriction base="xs:string">
                            <xs:maxLength value="50" />
                        </xs:restriction>
                    </xs:simpleType>
                </xs:element>
                <xs:element name="MobilePhone" minOccurs="1" maxOccurs="1">
                    <xs:simpleType>
                        <xs:restriction base="xs:string">
                            <xs:maxLength value="50" />
                        </xs:restriction>
                    </xs:simpleType>
                </xs:element>
                <xs:element name="Email" minOccurs="0" maxOccurs="1">
                    <xs:simpleType>
                        <xs:restriction base="xs:string">
                            <xs:maxLength value="256" />
                        </xs:restriction>
                    </xs:simpleType>
                </xs:element>
                <xs:element name="Notes" minOccurs="0" maxOccurs="1">
                    <xs:simpleType>
                        <xs:restriction base="xs:string">
                            <xs:maxLength value="4000" />
                        </xs:restriction>
                    </xs:simpleType>
                </xs:element>
                <xs:element name="ItemId" minOccurs="1" maxOccurs="1" type="xs:int" />
                <xs:element name="ItemNumber" minOccurs="1" maxOccurs="1">
                    <xs:simpleType>
                        <xs:restriction base="xs:string">
                            <xs:maxLength value ="18"/>
                        </xs:restriction>
                    </xs:simpleType>
                </xs:element>
                <xs:element name="ItemValue" minOccurs="1" maxOccurs="1" type="xs:decimal" />  
                <xs:element name="ItemSoldOn" minOccurs="1" maxOccurs="1" type="xs:dateTime" />
                <xs:element name="ItemSoldBy" minOccurs="1" maxOccurs="1">
                    <xs:simpleType>
                        <xs:restriction base="xs:string">
                            <xs:maxLength value="50" />
                        </xs:restriction>
                    </xs:simpleType>
                </xs:element>
                <xs:element name="Product" minOccurs="1" maxOccurs="1">
                    <xs:simpleType>
                        <xs:restriction base="xs:string">
                            <xs:maxLength value ="100"/>
                        </xs:restriction>
                    </xs:simpleType>
                </xs:element>
            </xs:sequence>
        </xs:complexType>
    </xs:element>
</xs:schema>'

WHERE id=6

IF NOT EXISTS(select * from syscolumns where name = 'ItemNumber' and object_name(id) = 'Request')
BEGIN
	alter table service.Request add ItemNumber VARCHAR(18) 
	
END


