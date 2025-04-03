-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
DECLARE @xml XML = CAST('<xs:schema xmlns="http://www.bluebridgeltd.com/cosacs/2012/schema.xsd" xmlns:mstns="http://www.bluebridgeltd.com/cosacs/2012/schema.xsd" xmlns:xs="http://www.w3.org/2001/XMLSchema" targetNamespace="http://www.bluebridgeltd.com/cosacs/2012/schema.xsd" elementFormDefault="qualified">
  <xs:element name="SalesOrderCancel">
    <xs:complexType>
      <xs:sequence>
        <xs:element name="ContractNumber" minOccurs="1" maxOccurs="1">
          <xs:simpleType>
            <xs:restriction base="xs:string">
              <xs:maxLength value="50" />
            </xs:restriction>
          </xs:simpleType>
        </xs:element>
        <xs:element name="SaleBranch" minOccurs="0" maxOccurs="1" type="xs:short" />
      </xs:sequence>
    </xs:complexType>
  </xs:element>
</xs:schema>' AS XML)

IF EXISTS(SELECT 1 FROM Hub.[Queue] WHERE id = 17)
	UPDATE Hub.[Queue]
	SET SubscriberSqlConnectionName = 'Default'
	WHERE id = 17
ELSE
	INSERT INTO Hub.[Queue]
		(Id, [Binding], [Schema], SubscriberClrAssemblyName, SubscriberClrTypeName, SubscriberSqlConnectionName, SubscriberSqlProcedureName)
	VALUES
		(17, 'Warranty.Sale.CancelItem', @xml, NULL, NULL, 'Default', 'Financial.ProcessMessageWarrantyCancelation')
