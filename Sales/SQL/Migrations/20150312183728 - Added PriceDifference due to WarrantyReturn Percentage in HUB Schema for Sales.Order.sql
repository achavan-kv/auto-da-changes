-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here


IF EXISTS(SELECT [Binding] FROM Hub.Queue WHERE [Binding]='Sales.Order') 
BEGIN	
		update hub.Queue
		set [Schema] =  '<?xml version="1.0" encoding="utf-8"?>
							<xs:schema targetNamespace="http://www.bluebridgeltd.com/cosacs/Order/2015/schema.xsd" elementFormDefault="qualified"
								xmlns="http://www.bluebridgeltd.com/cosacs/Order/2015/schema.xsd"
								xmlns:mstns="http://www.bluebridgeltd.com/cosacs/Order/2015/schema.xsd"
								xmlns:xs="http://www.w3.org/2001/XMLSchema">

								<xs:element name="Order">
									<xs:complexType>
										<xs:sequence>
											<xs:element name="Id" minOccurs="1" maxOccurs="1" type="xs:int" />
											<xs:element name="TotalAmount" minOccurs="1" maxOccurs="1" type="xs:decimal"/>
											<xs:element name="TotalTaxAmount" minOccurs="1" maxOccurs="1" type="xs:decimal"/>
											<xs:element name="TotalDiscount" minOccurs="1" maxOccurs="1" type="xs:decimal"/>
											<xs:element name="BranchNo" minOccurs="1" maxOccurs="1" type="xs:short"/>
											<xs:element name="CreatedOn" minOccurs="1" maxOccurs="1" type="xs:dateTime"/>
											<xs:element name="CreatedBy" minOccurs="1" maxOccurs="1" type="xs:int"/>
											<xs:element name="OriginalOrderId" minOccurs="0" maxOccurs="1" type="xs:int"/>
											<xs:element name="Comments" minOccurs="0" maxOccurs="1">
												<xs:simpleType>
													<xs:restriction base="xs:string">
														<xs:maxLength value="1024" />
													</xs:restriction>
												</xs:simpleType>
											</xs:element>
											<xs:element name="IsDutyFreeSale" minOccurs="1" maxOccurs="1" type="xs:boolean"/>
											<xs:element name="IsTaxFreeSale" minOccurs="1" maxOccurs="1" type="xs:boolean"/>
											<xs:element name="CustomerAccount" minOccurs="1" maxOccurs="1">
												<xs:simpleType>
													<xs:restriction base="xs:string">
														<xs:maxLength value="12" />
													</xs:restriction>
												</xs:simpleType>
											</xs:element>
											<xs:element name="Customer" minOccurs="0" maxOccurs="1" nillable="true" type="Customer" />      
											<xs:element name="Payments">
												<xs:complexType>
													<xs:sequence>
														<xs:element name="Payment" minOccurs="1" maxOccurs="unbounded" type="Payment"/>
													</xs:sequence>
												</xs:complexType>
											</xs:element>
											<xs:element name="Items">
												<xs:complexType>
													<xs:sequence>
														<xs:element name="Item" minOccurs="1" maxOccurs="unbounded" type="Item"/>
													</xs:sequence>
												</xs:complexType>
											</xs:element>
											<xs:element name="PotentialWarranties">
												<xs:complexType>
													<xs:sequence>
														<xs:element name="PotentialWarranty" minOccurs="0" maxOccurs="unbounded" type="PotentialWarranty"/>
													</xs:sequence>
												</xs:complexType>
											</xs:element>               
										</xs:sequence>
									</xs:complexType>
								</xs:element>
								<xs:complexType name="Item" >
									<xs:sequence>
										<xs:element name="Id" minOccurs="1" maxOccurs="1" type="xs:int" />
										<xs:element name="ParentId" minOccurs="0" maxOccurs="1" type="xs:int" />
										<xs:element name="ItemTypeId" minOccurs="1" maxOccurs="1" type="xs:unsignedByte" />
										<xs:element name="ItemNo" minOccurs="1" maxOccurs="1">
											<xs:simpleType>
												<xs:restriction base="xs:string">
													<xs:maxLength value="32" />
												</xs:restriction>
											</xs:simpleType>
										</xs:element>
										<xs:element name="Description" minOccurs="1" maxOccurs="1">
											<xs:simpleType>
												<xs:restriction base="xs:string">
													<xs:maxLength value="128" />
												</xs:restriction>
											</xs:simpleType>
										</xs:element>
										<xs:element name="Quantity" minOccurs="1" maxOccurs="1" type="xs:short"/>
										<xs:element name="Price" minOccurs="1" maxOccurs="1" type="xs:decimal"/>
										<xs:element name="TaxRate" minOccurs="1" maxOccurs="1" type="xs:decimal"/>

										<xs:element name="TaxAmount" minOccurs="0" maxOccurs="1" type="xs:decimal"/>
										<xs:element name="WarrantyLengthMonths" minOccurs="0" maxOccurs="1" type="xs:unsignedByte"/>
										<xs:element name="WarrantyEffectiveDate" minOccurs="0" maxOccurs="1" type="xs:date"/>
										<xs:element name="WarrantyContractNo" minOccurs="0" maxOccurs="1">
											<xs:simpleType>
												<xs:restriction base="xs:string">
													<xs:maxLength value="20" />
												</xs:restriction>
											</xs:simpleType>
										</xs:element>
										<xs:element name="ManualDiscount" minOccurs="0" maxOccurs="1" type="xs:decimal"/>
										<xs:element name="WarrantyTypeCode" minOccurs="0" maxOccurs="1">
											<xs:simpleType>
												<xs:restriction base="xs:string">
													<xs:maxLength value="1" />
												</xs:restriction>
											</xs:simpleType>
										</xs:element>
										<xs:element name="ProductItemId" minOccurs="0" maxOccurs="1" type="xs:int"/>
										<xs:element name="WarrantyLinkId" minOccurs="0" maxOccurs="1" type="xs:int"/>
										<xs:element name="Returned" minOccurs="1" maxOccurs="1" type="xs:boolean"/>
										<xs:element name="ReturnQuantity" minOccurs="0" maxOccurs="1" type="xs:short"/>
										<xs:element name="ReturnReason" minOccurs="0" maxOccurs="1">
											<xs:simpleType>
												<xs:restriction base="xs:string">
													<xs:maxLength value="32" />
												</xs:restriction>
											</xs:simpleType>
										</xs:element>

										<xs:element name="ItemUPC" minOccurs="0" maxOccurs="1">
											<xs:simpleType>
												<xs:restriction base="xs:string">
													<xs:maxLength value="18" />
												</xs:restriction>
											</xs:simpleType>
										</xs:element>
										<xs:element name="ItemSupplier" minOccurs="0" maxOccurs="1">
											<xs:simpleType>
												<xs:restriction base="xs:string">
													<xs:maxLength value="40" />
												</xs:restriction>
											</xs:simpleType>
										</xs:element>
										<xs:element name="Department" minOccurs="0" maxOccurs="1">
											<xs:simpleType>
												<xs:restriction base="xs:string">
													<xs:maxLength value="12" />
												</xs:restriction>
											</xs:simpleType>
										</xs:element>
										<xs:element name="Category" minOccurs="0" maxOccurs="1" type="xs:short"/>
										<xs:element name="Class" minOccurs="0" maxOccurs="1" >
											<xs:simpleType>
												<xs:restriction base="xs:string">
													<xs:maxLength value="3" />
												</xs:restriction>
											</xs:simpleType>
										</xs:element>
										<xs:element name="CostPrice" minOccurs="0" maxOccurs="1" type="xs:decimal"/>
										<xs:element name="RetailPrice" minOccurs="0" maxOccurs="1" type="xs:decimal"/>
										<xs:element name="ParentItemNo" minOccurs="0" maxOccurs="1" >
											<xs:simpleType>
												<xs:restriction base="xs:string">
													<xs:maxLength value="32" />
												</xs:restriction>
											</xs:simpleType>
										</xs:element>
										<xs:element name="PriceDifference" minOccurs="0" maxOccurs="1" type="xs:decimal"/>
									</xs:sequence>
								</xs:complexType>
								<xs:complexType name="Customer">
									<xs:sequence>
										<xs:element name="CustomerId" minOccurs="0" maxOccurs="1" >
											<xs:simpleType>
												<xs:restriction base="xs:string">
													<xs:maxLength value="20" />
												</xs:restriction>
											</xs:simpleType>
										</xs:element>
										<xs:element name="Title" minOccurs="1" maxOccurs="1">
											<xs:simpleType>
												<xs:restriction base="xs:string">
													<xs:maxLength value="8" />
												</xs:restriction>
											</xs:simpleType>
										</xs:element>
										<xs:element name="FirstName" minOccurs="1" maxOccurs="1">
											<xs:simpleType>
												<xs:restriction base="xs:string">
													<xs:maxLength value="32" />
												</xs:restriction>
											</xs:simpleType>
										</xs:element>
										<xs:element name="LastName" minOccurs="1" maxOccurs="1">
											<xs:simpleType>
												<xs:restriction base="xs:string">
													<xs:maxLength value="32" />
												</xs:restriction>
											</xs:simpleType>
										</xs:element>
										<xs:element name="Email" minOccurs="0" maxOccurs="1">
											<xs:simpleType>
												<xs:restriction base="xs:string">
													<xs:maxLength value="64" />
												</xs:restriction>
											</xs:simpleType>
										</xs:element>
										<xs:element name="HomePhone" minOccurs="0" maxOccurs="1">
											<xs:simpleType>
												<xs:restriction base="xs:string">
													<xs:maxLength value="32" />
												</xs:restriction>
											</xs:simpleType>
										</xs:element>
										<xs:element name="MobilePhone" minOccurs="0" maxOccurs="1">
											<xs:simpleType>
												<xs:restriction base="xs:string">
													<xs:maxLength value="32" />
												</xs:restriction>
											</xs:simpleType>
										</xs:element>
										<xs:element name="AddressLine1" minOccurs="1" maxOccurs="1">
											<xs:simpleType>
												<xs:restriction base="xs:string">
													<xs:maxLength value="64" />
												</xs:restriction>
											</xs:simpleType>
										</xs:element>
										<xs:element name="AddressLine2" minOccurs="0" maxOccurs="1">
											<xs:simpleType>
												<xs:restriction base="xs:string">
													<xs:maxLength value="64" />
												</xs:restriction>
											</xs:simpleType>
										</xs:element>
										<xs:element name="AddressLine3" minOccurs="1" maxOccurs="1">
											<xs:simpleType>
												<xs:restriction base="xs:string">
													<xs:maxLength value="64" />
												</xs:restriction>
											</xs:simpleType>
										</xs:element>
										<xs:element name="PostCode" minOccurs="0" maxOccurs="1">
											<xs:simpleType>
												<xs:restriction base="xs:string">
													<xs:maxLength value="16" />
												</xs:restriction>
											</xs:simpleType>
										</xs:element>
									</xs:sequence>
								</xs:complexType>
								<xs:complexType name="Payment">
									<xs:sequence>
										<xs:element name="MethodId" minOccurs="1" maxOccurs="1" type="xs:unsignedByte"/>
										<xs:element name="Amount" minOccurs="1" maxOccurs="1" type="xs:decimal"/>
										<xs:element name="Bank" minOccurs="0" maxOccurs="1">
											<xs:simpleType>
												<xs:restriction base="xs:string">
													<xs:maxLength value="32" />
												</xs:restriction>
											</xs:simpleType>
										</xs:element>
										<xs:element name="CardType" minOccurs="0" maxOccurs="1">
											<xs:simpleType>
												<xs:restriction base="xs:string">
													<xs:maxLength value="16" />
												</xs:restriction>
											</xs:simpleType>
										</xs:element>
										<xs:element name="CardNo" minOccurs="0" maxOccurs="1" type="xs:short"/>
										<xs:element name="ChequeNo" minOccurs="0" maxOccurs="1" type="xs:short"/>
										<xs:element name="BankAccountNo" minOccurs="0" maxOccurs="1" >
											<xs:simpleType>
												<xs:restriction base="xs:string">
													<xs:maxLength value="32" />
												</xs:restriction>
											</xs:simpleType>
										</xs:element>
										<xs:element name="CurrencyRate" minOccurs="0" maxOccurs="1" type="xs:decimal"/>
										<xs:element name="CurrencyAmount" minOccurs="0" maxOccurs="1" type="xs:decimal"/>
										<xs:element name="StoreCardNo" minOccurs="0" maxOccurs="1" type="xs:long"/>
										<xs:element name="VoucherNo" minOccurs="0" maxOccurs="1">
											<xs:simpleType>
												<xs:restriction base="xs:string">
													<xs:maxLength value="32" />
												</xs:restriction>
											</xs:simpleType>
										</xs:element>
										<xs:element name="CurrencyCode" minOccurs="0" maxOccurs="1">
											<xs:simpleType>
												<xs:restriction base="xs:string">
													<xs:maxLength value="9" />
												</xs:restriction>
											</xs:simpleType>
										</xs:element>
										<xs:element name="VoucherIssuer" minOccurs="0" maxOccurs="1">
											<xs:simpleType>
												<xs:restriction base="xs:string">
													<xs:maxLength value="1" />
												</xs:restriction>
											</xs:simpleType>
										</xs:element>
										<xs:element name="VoucherIssuerCode" minOccurs="0" maxOccurs="1">
											<xs:simpleType>
												<xs:restriction base="xs:string">
													<xs:maxLength value="12" />
												</xs:restriction>
											</xs:simpleType>
										</xs:element>
									</xs:sequence>
								</xs:complexType>
								<xs:complexType name="PotentialWarranty" >
									<xs:sequence>
										<xs:element name="ItemId" minOccurs="1" maxOccurs="1" type="xs:int" />
										<xs:element name="ItemNo" minOccurs="1" maxOccurs="1">
											<xs:simpleType>
												<xs:restriction base="xs:string">
													<xs:maxLength value="32" />
												</xs:restriction>
											</xs:simpleType>
										</xs:element>
										<xs:element name="ItemPrice" minOccurs="1" maxOccurs="1" type="xs:decimal"/>
										<xs:element name="WarrantyId" minOccurs="1" maxOccurs="1" type="xs:int" />
										<xs:element name="WarrantyNumber" minOccurs="1" maxOccurs="1">
											<xs:simpleType>
												<xs:restriction base="xs:string">
													<xs:maxLength value="32" />
												</xs:restriction>
											</xs:simpleType>
										</xs:element>
										<xs:element name="WarrantyLength" minOccurs="1" maxOccurs="1" type="xs:unsignedByte"/>
										<xs:element name="WarrantyTaxRate" minOccurs="1" maxOccurs="1" type="xs:decimal"/>
										<xs:element name="WarrantyCostPrice" minOccurs="1" maxOccurs="1" type="xs:decimal"/>
										<xs:element name="WarrantyRetailPrice" minOccurs="1" maxOccurs="1" type="xs:decimal"/>
										<xs:element name="WarrantySalePrice" minOccurs="1" maxOccurs="1" type="xs:decimal"/> 
										<xs:element name="ItemSerialNo" minOccurs="0" maxOccurs="1" >
											<xs:simpleType>
												<xs:restriction base="xs:string">
													<xs:maxLength value="50" />
												</xs:restriction>
											</xs:simpleType>
										</xs:element>
										<xs:element name="ItemCostPrice" minOccurs="1" maxOccurs="1" type="xs:decimal"/>
										<xs:element name="IsReturned" minOccurs="1" maxOccurs="1" type="xs:boolean"/>
										<xs:element name="SecondEffort" minOccurs="1" maxOccurs="1" type="xs:boolean"/>
										<xs:element name="WarrantyTypeCode" minOccurs="0" maxOccurs="1">
											<xs:simpleType>
												<xs:restriction base="xs:string">
													<xs:maxLength value="1" />
												</xs:restriction>
											</xs:simpleType>
										</xs:element>
										<xs:element name="CustomerAccountNo" minOccurs="1" maxOccurs="1" >
											<xs:simpleType>
												<xs:restriction base="xs:string">
													<xs:maxLength value="12" />
												</xs:restriction>
											</xs:simpleType>
										</xs:element>
										<xs:element name="Quantity" minOccurs="1" maxOccurs="1" type="xs:short"/>
									</xs:sequence>
								</xs:complexType>
							</xs:schema>'
		where [Binding]='Sales.Order'
	END
GO