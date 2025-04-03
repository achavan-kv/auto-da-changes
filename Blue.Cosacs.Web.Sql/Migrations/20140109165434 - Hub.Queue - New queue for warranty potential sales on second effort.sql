insert into Hub.Queue ([Id], [Binding], SubscriberClrAssemblyName, SubscriberClrTypeName, [Schema])
values (18, 'Warranty.Potential.Sales', 'Blue.Cosacs.Warranty', 'Blue.Cosacs.Warranty.Subscribers.WarrantyPotentialSales', '<?xml version="1.0" encoding="utf-8"?>
<xs:schema targetNamespace="http://www.bluebridgeltd.com/cosacs/2012/schema.xsd" elementFormDefault="qualified" xmlns="http://www.bluebridgeltd.com/cosacs/2012/schema.xsd" xmlns:mstns="http://www.bluebridgeltd.com/cosacs/2012/schema.xsd" xmlns:xs="http://www.w3.org/2001/XMLSchema">
    <xs:complexType name="Customer">
        <xs:sequence>
            <xs:element name="CustomerId" minOccurs="1" maxOccurs="1">
                <xs:simpleType>
                    <xs:restriction base="xs:string">
                        <xs:maxLength value="50" />
                    </xs:restriction>
                </xs:simpleType>
            </xs:element>
            <xs:element name="AccountNumber" minOccurs="1" maxOccurs="1">
                <xs:simpleType>
                    <xs:restriction base="xs:string">
                        <xs:maxLength value ="12"/>
                    </xs:restriction>
                </xs:simpleType>
            </xs:element>
            <xs:element name="AccountType" minOccurs="1" maxOccurs="1">
                <xs:simpleType>
                    <xs:restriction base="xs:string">
                        <xs:maxLength value ="6"/>
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
            <xs:element name="HomePhone" maxOccurs="1" nillable="true">
                <xs:simpleType>
                    <xs:restriction base="xs:string">
                        <xs:maxLength value="50" />
                    </xs:restriction>
                </xs:simpleType>
            </xs:element>
            <xs:element name="WorkPhone" maxOccurs="1" nillable="true">
                <xs:simpleType>
                    <xs:restriction base="xs:string">
                        <xs:maxLength value="50" />
                    </xs:restriction>
                </xs:simpleType>
            </xs:element>
            <xs:element name="MobilePhone"  maxOccurs="1" nillable="true">
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
            <xs:element name="AddressLine2" maxOccurs="1" nillable="true">
                <xs:simpleType>
                    <xs:restriction base="xs:string">
                        <xs:maxLength value="50" />
                    </xs:restriction>
                </xs:simpleType>
            </xs:element>
            <xs:element name="AddressLine3" maxOccurs="1" nillable="true">
                <xs:simpleType>
                    <xs:restriction base="xs:string">
                        <xs:maxLength value="50" />
                    </xs:restriction>
                </xs:simpleType>
            </xs:element>
            <xs:element name="PostCode"  maxOccurs="1" nillable="true">
                <xs:simpleType>
                    <xs:restriction base="xs:string">
                        <xs:maxLength value="10" />
                    </xs:restriction>
                </xs:simpleType>
            </xs:element>
            <xs:element name="Email"  maxOccurs="1" nillable="true">
                <xs:simpleType>
                    <xs:restriction base="xs:string">
                        <xs:maxLength value="256" />
                    </xs:restriction>
                </xs:simpleType>
            </xs:element>
            <xs:element name="Notes" maxOccurs="1" nillable="true">
                <xs:simpleType>
                    <xs:restriction base="xs:string">
                        <xs:maxLength value="4000" />
                    </xs:restriction>
                </xs:simpleType>
            </xs:element>
        </xs:sequence>
    </xs:complexType>
    <xs:simpleType name="decimalSqlMoney">
        <xs:restriction base="xs:decimal">
            <xs:fractionDigits value="4" />
        </xs:restriction>
    </xs:simpleType>
    <xs:simpleType name="decimalFourTwo">
        <xs:restriction base="xs:decimal">
            <xs:totalDigits value="4"/>
            <xs:fractionDigits value="2" />
        </xs:restriction>
    </xs:simpleType>
    <xs:complexType name="Warranty">
        <xs:sequence>
            <xs:element name="Id" minOccurs="1" maxOccurs="1" type="xs:int"/>
            <xs:element name="Length" minOccurs="1" maxOccurs="1" type="xs:short"/>
            <xs:element name="Number" minOccurs="1" maxOccurs="1">
                <xs:simpleType>
                    <xs:restriction base="xs:string">
                        <xs:maxLength value ="20"/>
                    </xs:restriction>
                </xs:simpleType>
            </xs:element>
            <xs:element name="TaxRate" minOccurs="1" maxOccurs="1" type="decimalFourTwo" />
            <xs:element name="IsFree" minOccurs="1" maxOccurs="1" type="xs:boolean" />
            <xs:element name="CostPrice"  minOccurs="1" maxOccurs="1" type="xs:decimal" />
            <xs:element name="RetailPrice" minOccurs="1" maxOccurs="1" type="xs:decimal" />
            <xs:element name="SalePrice" minOccurs="1" maxOccurs="1" type="xs:decimal"/>
            <xs:element name="ContractNumber" minOccurs="1" maxOccurs="1">
                <xs:simpleType>
                    <xs:restriction base="xs:string">
                        <xs:maxLength value ="10"/>
                    </xs:restriction>
                </xs:simpleType>
            </xs:element>
            <xs:element name="WarrantyGroupId" minOccurs="1" maxOccurs="1" type="xs:int"/>
        </xs:sequence>
    </xs:complexType>
    <xs:complexType name="Item">
        <xs:sequence>
            <xs:element name="Id" minOccurs="1" maxOccurs="1" type="xs:int"/>
            <xs:element name="StockLocation" minOccurs="1" maxOccurs="1" type="xs:short"/>
            <xs:element name="Quantity" minOccurs="1" maxOccurs="1" type="xs:int" />
            <xs:element name="Number" minOccurs="1" maxOccurs="1">
                <xs:simpleType>
                    <xs:restriction base="xs:string">
                        <xs:maxLength value ="25"/>
                    </xs:restriction>
                </xs:simpleType>
            </xs:element>
            <xs:element name="UPC" minOccurs="0" maxOccurs="1">
                <xs:simpleType>
                    <xs:restriction base="xs:string">
                        <xs:maxLength value ="25"/>
                    </xs:restriction>
                </xs:simpleType>
            </xs:element>
            <xs:element name="Price" minOccurs="1" maxOccurs="1" type="decimalSqlMoney"/>
            <xs:element name="CostPrice" minOccurs="0" maxOccurs="1" type="decimalSqlMoney"/>
            <xs:element name="Description" minOccurs="0" maxOccurs="1">
                <xs:simpleType>
                    <xs:restriction base="xs:string">
                        <xs:maxLength value ="100"/>
                    </xs:restriction>
                </xs:simpleType>
            </xs:element>
            <xs:element name="Supplier" minOccurs="0" maxOccurs="1">
                <xs:simpleType>
                    <xs:restriction base="xs:string">
                        <xs:maxLength value ="50"/>
                    </xs:restriction>
                </xs:simpleType>
            </xs:element>
            <xs:element name="Brand" minOccurs="0" maxOccurs="1" nillable="true">
                <xs:simpleType>
                    <xs:restriction base="xs:string">
                        <xs:maxLength value ="50"/>
                    </xs:restriction>
                </xs:simpleType>
            </xs:element>
            <xs:element name="Model" minOccurs="0" maxOccurs="1" nillable="true">
                <xs:simpleType>
                    <xs:restriction base="xs:string">
                        <xs:maxLength value ="50"/>
                    </xs:restriction>
                </xs:simpleType>
            </xs:element>
            <xs:element name="Department" minOccurs="1" maxOccurs="1">
                <xs:simpleType>
                    <xs:restriction base="xs:string">
                        <xs:maxLength value ="3"/>
                    </xs:restriction>
                </xs:simpleType>
            </xs:element>
            <xs:element name="Warranty" minOccurs="0" maxOccurs="unbounded" type="Warranty"/>
        </xs:sequence>
    </xs:complexType>
    <xs:simpleType name="soldByUser">  
        <xs:restriction base="xs:string">  
            <xs:maxLength value="50" />
        </xs:restriction>  
    </xs:simpleType>
    <xs:element name="SalesOrder">
        <xs:complexType>
            <xs:sequence>
                <xs:element name="InvoiceNumber" minOccurs="1" maxOccurs="1">
                    <xs:simpleType>
                        <xs:restriction base="xs:string">
                            <xs:maxLength value ="50"/>
                        </xs:restriction>
                    </xs:simpleType>
                </xs:element>
                <xs:element name="SaleBranch" minOccurs="1" maxOccurs="1" type="xs:short"/>
                <xs:element name="SoldOn" minOccurs="1" maxOccurs="1" type="xs:dateTime" />
                <xs:element name="SoldBy" minOccurs="1" maxOccurs="1"> 
                    <xs:complexType>
                        <xs:simpleContent>  
                            <xs:extension base="soldByUser">  
                                <xs:attribute name="SoldById" type="xs:int" />
                            </xs:extension>  
                        </xs:simpleContent> 
                    </xs:complexType>
                </xs:element>
                <xs:element name="DeliveredOn" minOccurs="1" maxOccurs="1" type="xs:dateTime" />
                <xs:element name="SecondEffort" maxOccurs="1" type="xs:boolean"/>
                <xs:element name="Customer" minOccurs="1" maxOccurs="1" type="Customer"/>
                <xs:element name="Item" minOccurs="1" maxOccurs="1" type="Item"/>
            </xs:sequence>
        </xs:complexType>
    </xs:element>
    <xs:element name="SalesOrderCancel">
        <xs:complexType>
            <xs:sequence>
                <xs:element name="ContractNumber" minOccurs="1" maxOccurs="1">
                    <xs:simpleType>
                        <xs:restriction base="xs:string">
                            <xs:maxLength value ="50"/>
                        </xs:restriction>
                    </xs:simpleType>
                </xs:element>
                <xs:element name="SaleBranch" minOccurs="1" maxOccurs="1" type="xs:short"/>
                <xs:element name="AccountType" minOccurs="1" maxOccurs="1">
                    <xs:simpleType>
                        <xs:restriction base="xs:string">
                            <xs:maxLength value ="6"/>
                        </xs:restriction>
                    </xs:simpleType>
                </xs:element>
                <xs:element name="Department" minOccurs="1" maxOccurs="1">
                    <xs:simpleType>
                        <xs:restriction base="xs:string">
                            <xs:maxLength value ="3"/>
                        </xs:restriction>
                    </xs:simpleType>
                </xs:element>
                <xs:element name="Warranty" minOccurs="0" maxOccurs="unbounded" type="Warranty"/>
            </xs:sequence>
        </xs:complexType>
    </xs:element>
    <xs:element name="SalesOrderCancelItem">
        <xs:complexType>
            <xs:sequence>
                <xs:element name="AccountNumber" minOccurs="1" maxOccurs="1">
                    <xs:simpleType>
                        <xs:restriction base="xs:string">
                            <xs:maxLength value ="12"/>
                        </xs:restriction>
                    </xs:simpleType>
                </xs:element>
                <xs:element name="InvoiceNumber" minOccurs="1" maxOccurs="1">
                    <xs:simpleType>
                        <xs:restriction base="xs:string">
                            <xs:maxLength value ="50"/>
                        </xs:restriction>
                    </xs:simpleType>
                </xs:element>
                <xs:element name="SaleBranch" minOccurs="1" maxOccurs="1" type="xs:short"/>
                <xs:element name="ItemId" minOccurs="1" maxOccurs="1" type="xs:int"/>
                <xs:element name="Quantity" minOccurs="1" maxOccurs="1" type="xs:int" />
            </xs:sequence>
        </xs:complexType>
    </xs:element>
    <xs:element name="PotentialSales">
        <xs:complexType>
            <xs:sequence>
                <xs:element name="InvoiceNumber" minOccurs="1" maxOccurs="1">
                    <xs:simpleType>
                        <xs:restriction base="xs:string">
                            <xs:maxLength value ="50"/>
                        </xs:restriction>
                    </xs:simpleType>
                </xs:element>
                <xs:element name="SaleBranch" minOccurs="1" maxOccurs="1" type="xs:short"/>
                <xs:element name="SoldOn" minOccurs="1" maxOccurs="1" type="xs:dateTime" />
                <xs:element name="SoldBy" minOccurs="1" maxOccurs="1" type="xs:int"/>
                <xs:element name="DeliveredOn" minOccurs="1" maxOccurs="1" type="xs:dateTime" />
                <xs:element name="SecondEffort" maxOccurs="1" type="xs:boolean"/>
                <xs:element name="Customer" minOccurs="1" maxOccurs="1" type="Customer"/>
                <xs:element name="Items" minOccurs="1" maxOccurs="unbounded" type="Item"/>
            </xs:sequence>
        </xs:complexType>
    </xs:element>
</xs:schema>')