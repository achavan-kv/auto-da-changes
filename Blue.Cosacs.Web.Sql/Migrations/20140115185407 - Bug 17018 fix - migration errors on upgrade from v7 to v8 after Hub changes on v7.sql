-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
--  select * from Hub.Queue
-- Put your SQL code here

if not exists(select * from Hub.[queue] where id=1)
begin
 insert into Hub.[Queue] ([Id],[Binding])
 select  1, ''
end
if not exists(select * from Hub.[queue] where id=2)
begin
 insert into Hub.[Queue] ([Id],[Binding])
 select  2, ''
end
if not exists(select * from Hub.[queue] where id=3)
begin
 insert into Hub.[Queue] ([Id],[Binding])
 select  3, ''
end
if not exists(select * from Hub.[queue] where id=4)
begin
 insert into Hub.[Queue] ([Id],[Binding])
 select  4, ''
end
if not exists(select * from Hub.[queue] where id=5)
begin
 insert into Hub.[Queue] ([Id],[Binding])
 select  5, ''
end
if not exists(select * from Hub.[queue] where id=6)
begin
 insert into Hub.[Queue] ([Id],[Binding])
 select  6, ''
end
if not exists(select * from Hub.[queue] where id=7)
begin
 insert into Hub.[Queue] ([Id],[Binding])
 select  7, ''
end
if not exists(select * from Hub.[queue] where id=8)
begin
 insert into Hub.[Queue] ([Id],[Binding])
 select  8, ''
end
if not exists(select * from Hub.[queue] where id=9)
begin
 insert into Hub.[Queue] ([Id],[Binding])
 select  9, ''
end
if not exists(select * from Hub.[queue] where id=10)
begin
 insert into Hub.[Queue] ([Id],[Binding])
 select  10, ''
end
if not exists(select * from Hub.[queue] where id=11)
begin
 insert into Hub.[Queue] ([Id],[Binding])
 select  11, ''
end
if not exists(select * from Hub.[queue] where id=12)
begin
 insert into Hub.[Queue] ([Id],[Binding])
 select  12, ''
end
if not exists(select * from Hub.[queue] where id=14)
begin
 insert into Hub.[Queue] ([Id],[Binding])
 select  14, ''
end
if not exists(select * from Hub.[queue] where id=15)
begin
 insert into Hub.[Queue] ([Id],[Binding])
 select  15, ''
end
if not exists(select * from Hub.[queue] where id=16)
begin
 insert into Hub.[Queue] ([Id],[Binding])
 select  16, ''
end
if not exists(select * from Hub.[queue] where id=17)
begin
 insert into Hub.[Queue] ([Id],[Binding])
 select  17, ''
end
if not exists(select * from Hub.[queue] where id=18)
begin
 insert into Hub.[Queue] ([Id],[Binding])
 select  18, ''
end

UPDATE [Hub].[Queue] SET [Binding]=N'Warehouse.Booking.Submit', SubscriberClrAssemblyName='Blue.Cosacs.Warehouse',
 SubscriberClrTypeName='Blue.Cosacs.Warehouse.Subscribers.BookingSubmit',
 SubscriberSqlConnectionName=NULL, SubscriberSqlProcedureName=NULL
WHERE Id=1
GO
UPDATE [Hub].[Queue] SET [Binding]=N'Warehouse.Booking.Cancel', SubscriberClrAssemblyName='Blue.Cosacs.Warehouse',
 SubscriberClrTypeName='Blue.Cosacs.Warehouse.Subscribers.BookingCancel',
 SubscriberSqlConnectionName=NULL, SubscriberSqlProcedureName=NULL
WHERE Id=2
GO
UPDATE [Hub].[Queue] SET [Binding]=N'Cosacs.Booking.Deliver', SubscriberClrAssemblyName='Blue.Cosacs',
 SubscriberClrTypeName='Blue.Cosacs.Subscribers.WarehouseDeliver',
 SubscriberSqlConnectionName=NULL, SubscriberSqlProcedureName=NULL
WHERE Id=3
GO
UPDATE [Hub].[Queue] SET [Binding]=N'Cosacs.Booking.Cancel', SubscriberClrAssemblyName='Blue.Cosacs',
 SubscriberClrTypeName='Blue.Cosacs.Subscribers.WarehouseCancel',
 SubscriberSqlConnectionName=NULL, SubscriberSqlProcedureName=NULL
WHERE Id=4
GO
UPDATE [Hub].[Queue] SET [Binding]=N'Cosacs.Service.Summary', SubscriberClrAssemblyName='Blue.Cosacs',
 SubscriberClrTypeName='Blue.Cosacs.Subscribers.ServiceSummary',
 SubscriberSqlConnectionName=NULL, SubscriberSqlProcedureName=NULL
WHERE Id=5
GO
UPDATE [Hub].[Queue] SET [Binding]=N'Service.Request.Submit', SubscriberClrAssemblyName='Blue.Cosacs.Service',
 SubscriberClrTypeName='Blue.Cosacs.Service.Subscribers.RequestSubmit',
 SubscriberSqlConnectionName=NULL, SubscriberSqlProcedureName=NULL
WHERE Id=6
GO
UPDATE [Hub].[Queue] SET [Binding]=N'Cosacs.Service.Payment', SubscriberClrAssemblyName='Blue.Cosacs',
 SubscriberClrTypeName='Blue.Cosacs.Subscribers.ServicePayment',
 SubscriberSqlConnectionName=NULL, SubscriberSqlProcedureName=NULL
WHERE Id=7
GO
UPDATE [Hub].[Queue] SET [Binding]=N'Cosacs.Service.Charges', SubscriberClrAssemblyName='Blue.Cosacs',
 SubscriberClrTypeName='Blue.Cosacs.Subscribers.ServiceCharges',
 SubscriberSqlConnectionName=NULL, SubscriberSqlProcedureName=NULL
WHERE Id=8
GO
UPDATE [Hub].[Queue] SET [Binding]=N'Cosacs.Service.Parts', SubscriberClrAssemblyName='Blue.Cosacs',
 SubscriberClrTypeName='Blue.Cosacs.Subscribers.ServiceParts',
 SubscriberSqlConnectionName=NULL, SubscriberSqlProcedureName=NULL
WHERE Id=9
GO
UPDATE [Hub].[Queue] SET [Binding]=N'Warranty.Sale.Submit', SubscriberClrAssemblyName='Blue.Cosacs.Warranty',
 SubscriberClrTypeName='Blue.Cosacs.Warranty.Subscribers.WarrantySaleSubmit',
 SubscriberSqlConnectionName=NULL, SubscriberSqlProcedureName=NULL
WHERE Id=10
GO
UPDATE [Hub].[Queue] SET [Binding]=N'Cosacs.Service.WarrantyServiceCompleted', SubscriberClrAssemblyName='Blue.Cosacs.Warranty',
 SubscriberClrTypeName='Blue.Cosacs.Warranty.Subscribers.WarrantyServiceCompletion',
 SubscriberSqlConnectionName=NULL, SubscriberSqlProcedureName=NULL
WHERE Id=11
GO
UPDATE [Hub].[Queue] SET [Binding]=N'Warranty.Sale.Cancel', SubscriberClrAssemblyName='Blue.Cosacs.Warranty',
 SubscriberClrTypeName='Blue.Cosacs.Warranty.Subscribers.WarrantySaleCancel',
 SubscriberSqlConnectionName=NULL, SubscriberSqlProcedureName=NULL
WHERE Id=12
GO
UPDATE [Hub].[Queue] SET [Binding]=N'Warranty.Sale.CancelItem', SubscriberClrAssemblyName='Blue.Cosacs.Warranty',
 SubscriberClrTypeName='Blue.Cosacs.Warranty.Subscribers.WarrantySaleCancelItem',
 SubscriberSqlConnectionName=NULL, SubscriberSqlProcedureName=NULL
WHERE Id=14
GO
UPDATE [Hub].[Queue] SET [Binding]=N'Cosacs.Service.Detail', SubscriberClrAssemblyName=NULL,
 SubscriberClrTypeName=NULL,
 SubscriberSqlConnectionName='Default', SubscriberSqlProcedureName='Financial.ProcessMessageService'
WHERE Id=15
GO
UPDATE [Hub].[Queue] SET [Binding]=N'Warranty.Sale.Submit', SubscriberClrAssemblyName=NULL,
 SubscriberClrTypeName=NULL,
 SubscriberSqlConnectionName='Default', SubscriberSqlProcedureName='Financial.ProcessMessageWarranty'
WHERE Id=16
GO
UPDATE [Hub].[Queue] SET [Binding]=N'Warranty.Sale.Cancel', SubscriberClrAssemblyName=NULL,
 SubscriberClrTypeName=NULL,
 SubscriberSqlConnectionName='Default', SubscriberSqlProcedureName='Financial.ProcessMessageWarrantyCancelation'
WHERE Id=17
GO
UPDATE [Hub].[Queue] SET [Binding]=N'Warranty.Potential.Sales', SubscriberClrAssemblyName='Blue.Cosacs.Warranty',
 SubscriberClrTypeName='Blue.Cosacs.Warranty.Subscribers.WarrantyPotentialSales',
 SubscriberSqlConnectionName=NULL, SubscriberSqlProcedureName=NULL
WHERE Id=18
GO

-- Reset all schemas...
update [Hub].[Queue] -- Warehouse
set [schema]=N'<xs:schema targetNamespace="http://www.bluebridgeltd.com/cosacs/2012/schema.xsd"
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
                            <xs:maxLength value="95" />
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
                <xs:element name="Express" minOccurs="1" maxOccurs="1" type="xs:boolean"/>
                <xs:element name="Acctno" minOccurs="1" maxOccurs="1">
                    <xs:simpleType>
                        <xs:restriction base="xs:string">
                            <xs:maxLength value ="12"/>
                        </xs:restriction>
                    </xs:simpleType>
                </xs:element>
                <xs:element name="UnitPrice" minOccurs="1" maxOccurs="1" type="xs:decimal"/>
                <xs:element name="CreatedBy" minOccurs="1" maxOccurs="1" type="xs:int" />
                <xs:element name="AddressNotes" minOccurs="1" maxOccurs="1">
                  <xs:simpleType>
                    <xs:restriction base="xs:string">
                      <xs:maxLength value ="1000"/>
                    </xs:restriction>
                  </xs:simpleType>
              </xs:element>
                <xs:element name="Fascia" minOccurs="1" maxOccurs="1">
                    <xs:simpleType>
                        <xs:restriction base="xs:string">
                            <xs:maxLength value ="1"/>
                        </xs:restriction>
                    </xs:simpleType>
                </xs:element>
                <xs:element name="PickUp" minOccurs="1" maxOccurs="1" type="xs:boolean"/>
            </xs:sequence>            
        </xs:complexType>        
    </xs:element>


    <xs:element name="BookingCancel">
        <xs:complexType>
            <xs:sequence>
                <xs:element name="Id" minOccurs="1" maxOccurs="1" type="xs:int" />
                <xs:element name="User" minOccurs="1" maxOccurs="1" type="xs:int" />
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

    <xs:element name="WarehouseCancel">
        <xs:complexType>
            <xs:sequence>
                <xs:element name="Id" minOccurs="1" maxOccurs="1" type="xs:int" />
                <xs:element name="OrigBookingId" minOccurs="1" maxOccurs="1" type="xs:int" />
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

    <xs:element name="WarehouseDeliver">
        <xs:complexType>
            <xs:sequence>
                <xs:element name="Id" minOccurs="1" maxOccurs="1" type="xs:int" />
                <xs:element name="OrigBookingId" minOccurs="1" maxOccurs="1" type="xs:int" />
                <xs:element name="Quantity" minOccurs="1" maxOccurs="1" type="xs:int" />
                <xs:element name="Date"  minOccurs="1" maxOccurs="1" type="xs:dateTime"/>
                <xs:element name="ConfirmedBy" minOccurs="1" maxOccurs="1" type="xs:int" />
            </xs:sequence>
        </xs:complexType>
    </xs:element>
</xs:schema>
'
where id in (1,2,3,4)
go

update [Hub].[Queue] -- Service
set [schema]=N'<xs:schema targetNamespace="http://www.bluebridgeltd.com/cosacs/2012/schema.xsd"
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
            <xs:element name="PostCode" minOccurs="0" maxOccurs="1">
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
    <xs:complexType name="Hierarchy">
        <xs:sequence>
            <xs:element name="Level" minOccurs="0" maxOccurs="unbounded">
                <xs:complexType>
                    <xs:simpleContent>
                        <xs:extension base="xs:string">
                            <xs:attribute name="name" type="xs:string" use="required"/>
                        </xs:extension>
                    </xs:simpleContent>
                </xs:complexType>
            </xs:element>
        </xs:sequence>
    </xs:complexType>
    <xs:complexType name="Item">
        <xs:sequence>
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
            <xs:element name="Hierarchy" minOccurs="0" maxOccurs="1" type="Hierarchy"/>
        </xs:sequence>
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
                <xs:element name="StockLocation" minOccurs="1" maxOccurs="1" type="xs:short" />
                <xs:element name="Supplier" minOccurs="1" maxOccurs="1">
                    <xs:simpleType>
                        <xs:restriction base="xs:string">
                            <xs:maxLength value="40" />
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
                <xs:element name="ServiceType" minOccurs="0" maxOccurs="1">
                    <xs:simpleType>
                        <xs:restriction base="xs:string">
                            <xs:maxLength value ="2"/>
                        </xs:restriction>
                    </xs:simpleType>
                </xs:element>
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
</xs:schema>
'
where id in (5,6,7,8,9,11,15)
go

update [Hub].[Queue] -- Warranty
set [schema]=N'<xs:schema targetNamespace="http://www.bluebridgeltd.com/cosacs/2012/schema.xsd" elementFormDefault="qualified" xmlns="http://www.bluebridgeltd.com/cosacs/2012/schema.xsd" xmlns:mstns="http://www.bluebridgeltd.com/cosacs/2012/schema.xsd" xmlns:xs="http://www.w3.org/2001/XMLSchema">
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
</xs:schema>
'
where id in (10,12,14,16,17,18)
go
