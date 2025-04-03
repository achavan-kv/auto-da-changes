update Hub.[Queue]
set [Schema] = '<xs:schema targetNamespace="http://www.bluebridgeltd.com/cosacs/2012/schema.xsd"
    elementFormDefault="qualified"
    xmlns="http://www.bluebridgeltd.com/cosacs/2012/schema.xsd"
    xmlns:mstns="http://www.bluebridgeltd.com/cosacs/2012/schema.xsd"
    xmlns:xs="http://www.w3.org/2001/XMLSchema">
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
            <xs:element name="HomePhone" minOccurs="0" maxOccurs="1">
                <xs:simpleType>
                    <xs:restriction base="xs:string">
                        <xs:maxLength value="50" />
                    </xs:restriction>
                </xs:simpleType>
            </xs:element>
            <xs:element name="WorkPhone" minOccurs="0" maxOccurs="1">
                <xs:simpleType>
                    <xs:restriction base="xs:string">
                        <xs:maxLength value="50" />
                    </xs:restriction>
                </xs:simpleType>
            </xs:element>
            <xs:element name="MobilePhone" minOccurs="0" maxOccurs="1">
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
        <xs:all>
            <xs:element name="LineItemIdentifier" minOccurs="1" maxOccurs="1">
                <xs:simpleType>
                    <xs:restriction base="xs:string">
                        <xs:maxLength value ="50"/>
                    </xs:restriction>
                </xs:simpleType>
            </xs:element>
            <xs:element name="Id" minOccurs="1" maxOccurs="1" type="xs:int"/>
            <xs:element name="Number" minOccurs="1" maxOccurs="1">
                <xs:simpleType>
                    <xs:restriction base="xs:string">
                        <xs:maxLength value ="25"/>
                    </xs:restriction>
                </xs:simpleType>
            </xs:element>
            <xs:element name="StockLocn" minOccurs="1" maxOccurs="1" type="xs:int"/>
            <xs:element name="DeliveredOn" minOccurs="1" maxOccurs="1" type="xs:dateTime" />
            <xs:element name="Model" minOccurs="0" maxOccurs="1">
                <xs:simpleType>
                    <xs:restriction base="xs:string">
                        <xs:maxLength value ="50"/>
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
            <xs:element name="SerialNumber" minOccurs="0" maxOccurs="1">
                <xs:simpleType>
                    <xs:restriction base="xs:string">
                        <xs:maxLength value ="50"/>
                    </xs:restriction>
                </xs:simpleType>
            </xs:element>
        </xs:all>
    </xs:complexType>
    <xs:complexType name="Warranty">
        <xs:all>
            <xs:element name="Length" minOccurs="1" maxOccurs="1" type="xs:int"/>
            <xs:element name="Number" minOccurs="1" maxOccurs="1">
                <xs:simpleType>
                    <xs:restriction base="xs:string">
                        <xs:maxLength value ="20"/>
                    </xs:restriction>
                </xs:simpleType>
            </xs:element>
            <xs:element name="ContractNumber" minOccurs="1" maxOccurs="1">
                <xs:simpleType>
                    <xs:restriction base="xs:string">
                        <xs:maxLength value ="10"/>
                    </xs:restriction>
                </xs:simpleType>
            </xs:element>
        </xs:all>
    </xs:complexType>
    <xs:complexType name="Charge">
        <xs:all>
            <xs:element name="Account" minOccurs="0" maxOccurs="1" nillable="true">
                <xs:simpleType>
                    <xs:restriction base="xs:string">
                        <xs:maxLength value ="12"/>
                    </xs:restriction>
                </xs:simpleType>
            </xs:element>
            <xs:element name="ItemNumber" minOccurs="0" maxOccurs="1">
                <xs:simpleType>
                    <xs:restriction base="xs:string">
                        <xs:maxLength value ="18"/>
                    </xs:restriction>
                </xs:simpleType>
            </xs:element>
            <xs:element name="Value" minOccurs="1" maxOccurs="1" type="xs:decimal" />
            <xs:element name="Cost" minOccurs="1" maxOccurs="1" type="xs:decimal" />
            <xs:element name="Tax" minOccurs="1" maxOccurs="1" type="xs:decimal" nillable="true"/>
            <xs:element name="ChargeType" minOccurs="1" maxOccurs="1">
                <xs:simpleType>
                    <xs:restriction base="xs:string">
                        <xs:maxLength value ="30"/>
                    </xs:restriction>
                </xs:simpleType>
            </xs:element>
            <xs:element name="Label" minOccurs="1" maxOccurs="1" nillable="true">
                <xs:simpleType>
                    <xs:restriction base="xs:string">
                        <xs:maxLength value="50" />
                    </xs:restriction>
                </xs:simpleType>
            </xs:element>
            <xs:element name="IsExternal" minOccurs="1" maxOccurs="1" type="xs:boolean" nillable="true"/>
        </xs:all>
    </xs:complexType>
    <xs:complexType name="ServiceCharge">
        <xs:all>
            <xs:element name="ServiceRequestNo" minOccurs="1" maxOccurs="1" type="xs:int" />
            <xs:element name="Branch" minOccurs="1" maxOccurs="1" type="xs:int"/>
            <xs:element name="CustomerId" minOccurs="1" maxOccurs="1" nillable="true">
                <xs:simpleType>
                    <xs:restriction base="xs:string">
                        <xs:maxLength value="50" />
                    </xs:restriction>
                </xs:simpleType>
            </xs:element>
            <xs:element name="Account" minOccurs="0" maxOccurs="1" nillable="true">
                <xs:simpleType>
                    <xs:restriction base="xs:string">
                        <xs:maxLength value ="12"/>
                    </xs:restriction>
                </xs:simpleType>
            </xs:element>
            <xs:element name="ItemNumber" minOccurs="0" maxOccurs="1">
                <xs:simpleType>
                    <xs:restriction base="xs:string">
                        <xs:maxLength value ="18"/>
                    </xs:restriction>
                </xs:simpleType>
            </xs:element>
            <xs:element name="Value" minOccurs="1" maxOccurs="1" type="xs:decimal" />
            <xs:element name="Cost" minOccurs="1" maxOccurs="1" type="xs:decimal" />
            <xs:element name="Tax" minOccurs="1" maxOccurs="1" type="xs:decimal" nillable="true"/>
            <xs:element name="ChargeType" minOccurs="1" maxOccurs="1">
                <xs:simpleType>
                    <xs:restriction base="xs:string">
                        <xs:maxLength value ="30"/>
                    </xs:restriction>
                </xs:simpleType>
            </xs:element>
            <xs:element name="Label" minOccurs="1" maxOccurs="1" nillable="true">
                <xs:simpleType>
                    <xs:restriction base="xs:string">
                        <xs:maxLength value="50" />
                    </xs:restriction>
                </xs:simpleType>
            </xs:element>
            <xs:element name="IsExternal" minOccurs="1" maxOccurs="1" type="xs:boolean" nillable="true"/>
        </xs:all>
    </xs:complexType>
    <xs:complexType name="ServicePart">
        <xs:sequence>
            <xs:element name="ItemNumber" minOccurs="1" maxOccurs="1">
                <xs:simpleType>
                    <xs:restriction base="xs:string">
                        <xs:maxLength value ="18"/>
                    </xs:restriction>
                </xs:simpleType>
            </xs:element>
            <xs:element name="StockLocn" minOccurs="1" maxOccurs="1" type="xs:int" nillable="true"/>
            <xs:element name="Quantity" minOccurs="1" maxOccurs="1" type="xs:int"/>
            <xs:element name="Price" minOccurs="1" maxOccurs="1" type="xs:decimal" nillable="true"/>
        </xs:sequence>
    </xs:complexType>
    <xs:complexType name="WarrantyServiceCharge">
        <xs:all>
            <xs:element name="Type" minOccurs="1" maxOccurs="1">
                <xs:simpleType>
                    <xs:restriction base="xs:string">
                        <xs:maxLength value ="30"/>
                    </xs:restriction>
                </xs:simpleType>
            </xs:element>
            <xs:element name="TotalCost" minOccurs="1" maxOccurs="1" type="decimalSqlMoney"/>
            <xs:element name="LabourCost" minOccurs="1" maxOccurs="1" type="decimalSqlMoney"/>
            <xs:element name="AdditionalPartsCost" minOccurs="1" maxOccurs="1" type="decimalSqlMoney"/>
        </xs:all>
    </xs:complexType>
    <xs:element name="ServiceSummary">
        <xs:complexType>
            <xs:all>
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
                <xs:element name="ReplacementIssued" minOccurs="1" maxOccurs="1" type="xs:boolean"/>
            </xs:all>
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
    <xs:element name="ServicePayment">
        <xs:complexType>
            <xs:all>
                <xs:element name="ServiceRequestNo" minOccurs="1" maxOccurs="1" type="xs:int" />
                <xs:element name="Branch" minOccurs="1" maxOccurs="1" type="xs:int"/>
                <xs:element name="CustomerId" minOccurs="0" maxOccurs="1">
                    <xs:simpleType>
                        <xs:restriction base="xs:string">
                            <xs:maxLength value="20" />
                        </xs:restriction>
                    </xs:simpleType>
                </xs:element>
                <xs:element name="Amount" minOccurs="1" maxOccurs="1" type="xs:decimal" />
                <xs:element name="PayMethod" minOccurs="1" maxOccurs="1" type="xs:int" />
                <xs:element name="ChequeNumber" minOccurs="0" maxOccurs="1">
                    <xs:simpleType>
                        <xs:restriction base="xs:string">
                            <xs:maxLength value="16" />
                        </xs:restriction>
                    </xs:simpleType>
                </xs:element>
                <xs:element name="Bank" minOccurs="0" maxOccurs="1" >
                    <xs:simpleType>
                        <xs:restriction base="xs:string">
                            <xs:maxLength value="6" />
                        </xs:restriction>
                    </xs:simpleType>
                </xs:element>
                <xs:element name="BankAccountNumber" minOccurs="0" maxOccurs="1" >
                    <xs:simpleType>
                        <xs:restriction base="xs:string">
                            <xs:maxLength value="20" />
                        </xs:restriction>
                    </xs:simpleType>
                </xs:element>
                <xs:element name="EmpeeNo" minOccurs="1" maxOccurs="1" type="xs:int" />
                <xs:element name="ChargeType" minOccurs="0" maxOccurs="1">
                    <xs:simpleType>
                        <xs:restriction base="xs:string">
                            <xs:maxLength value ="30"/>
                        </xs:restriction>
                    </xs:simpleType>
                </xs:element>
            </xs:all>
        </xs:complexType>
    </xs:element>
    <xs:element name="ServiceCharges">
        <xs:complexType>
            <xs:sequence>
                <xs:element name="EmpeeNo" minOccurs="1" maxOccurs="1" type="xs:int" />
                <xs:element name="ServiceCharge" minOccurs="1" maxOccurs="unbounded" type="ServiceCharge"/>
            </xs:sequence>
        </xs:complexType>
    </xs:element>
    <xs:element name="ServiceParts">
        <xs:complexType>
            <xs:sequence>
                <xs:element name="ServiceRequestNo" minOccurs="1" maxOccurs="1" type="xs:int" />
                <xs:element name="Branch" minOccurs="1" maxOccurs="1" type="xs:int"/>
                <xs:element name="Account" minOccurs="1" maxOccurs="1">
                    <xs:simpleType>
                        <xs:restriction base="xs:string">
                            <xs:maxLength value ="12"/>
                        </xs:restriction>
                    </xs:simpleType>
                </xs:element>
                <xs:element name="ServicePart" minOccurs="1" maxOccurs="unbounded" type="ServicePart"/>
            </xs:sequence>
        </xs:complexType>
    </xs:element>
    <xs:element name="WarrantyServiceDetail">
        <xs:complexType>
            <xs:sequence>
                <xs:element name="AccountNumber" minOccurs="1" maxOccurs="1">
                    <xs:simpleType>
                        <xs:restriction base="xs:string">
                            <xs:maxLength value ="12"/>
                        </xs:restriction>
                    </xs:simpleType>
                </xs:element>
                <xs:element name="ServiceRequestNo" minOccurs="1" maxOccurs="1" type="xs:int" />
                <xs:element name="ServiceBranch" minOccurs="1" maxOccurs="1" type="xs:int"/>
                <xs:element name="Item" minOccurs="1" maxOccurs="1" type="Item" />
                <xs:element name="Warranty" minOccurs="1" maxOccurs="1" type="Warranty" />
                <xs:element name="DateLogged" minOccurs="1" maxOccurs="1" type="xs:dateTime"/>
                <xs:element name="DateClosed" minOccurs="1" maxOccurs="1" type="xs:dateTime"/>
                <xs:element name="ReplacementIssued" minOccurs="1" maxOccurs="1" type="xs:boolean"/>
                <xs:element name="Resolution" minOccurs="1" maxOccurs="1">
                    <xs:simpleType>
                        <xs:restriction base="xs:string">
                            <xs:maxLength value ="100"/>
                        </xs:restriction>
                    </xs:simpleType>
                </xs:element>
                <xs:element name="Charges" type="WarrantyServiceCharge" minOccurs="1" maxOccurs="unbounded" />
            </xs:sequence>
        </xs:complexType>
    </xs:element>
    <xs:element name="ServiceDetail">
        <xs:complexType>
            <xs:all>
                <xs:element name="ServiceRequestNo" minOccurs="1" maxOccurs="1" type="xs:int" />
                <xs:element name="RequestType" minOccurs="1" maxOccurs="1">
                    <xs:simpleType>
                        <xs:restriction base="xs:string">
                            <xs:maxLength value ="2"/>
                        </xs:restriction>
                    </xs:simpleType>
                </xs:element>
                <xs:element name="Customer" minOccurs="1" maxOccurs="1" nillable="true" type="Customer" />
                <xs:element name="AccountNumber" minOccurs="0" maxOccurs="1">
                    <xs:simpleType>
                        <xs:restriction base="xs:string">
                            <xs:maxLength value ="12"/>
                        </xs:restriction>
                    </xs:simpleType>
                </xs:element>
                <xs:element name="Item" minOccurs="1" maxOccurs="1" type="Item" />
                <xs:element name="Branch" minOccurs="1" maxOccurs="1" type="xs:int"/>
                <xs:element name="CountryCode" minOccurs="1" maxOccurs="1" type="xs:string"/>
                <xs:element name="StockLocation" minOccurs="1" maxOccurs="1" type="xs:int"/>
                <xs:element name="CreatedBy" minOccurs="1" maxOccurs="1" type="xs:int" />
                <xs:element name="LastUpdatedBy" minOccurs="1" maxOccurs="1" type="xs:int" />
                <xs:element name="DateLogged" minOccurs="1" maxOccurs="1" type="xs:dateTime"/>
                <xs:element name="DateClosed" minOccurs="1" maxOccurs="1" type="xs:dateTime"/>
                <xs:element name="ReplacementIssued" minOccurs="1" maxOccurs="1" type="xs:boolean"/>
                <xs:element name="Charges">
                    <xs:complexType>
                        <xs:sequence>
                            <xs:element name="Charge" minOccurs="0" maxOccurs="unbounded" type="Charge"/>
                        </xs:sequence>
                    </xs:complexType>
                </xs:element>
                <xs:element name="Parts">
                    <xs:complexType>
                        <xs:sequence>
                            <xs:element name="Part" minOccurs="0" maxOccurs="unbounded" type="ServicePart"/>
                        </xs:sequence>
                    </xs:complexType>
                </xs:element>
            </xs:all>
        </xs:complexType>
    </xs:element>
</xs:schema>'
where Id = 15