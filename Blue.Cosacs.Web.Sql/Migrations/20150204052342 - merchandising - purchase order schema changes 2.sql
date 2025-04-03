alter table Merchandising.PurchaseOrder
add
	 [OriginSystem]			varchar(10)		null
	,[CorporatePoNumber]	varchar(20)		null
	,[ShipDate]				date			null
	,[ShipVia]				varchar(60)		null
	,[PortOfLoading]		varchar(60)		null
	,[Attributes]			varchar(max)	null
	,[CommissionChargeFlag]	char(1)			null
	,[CommissionPercentage]	varchar(10)		null