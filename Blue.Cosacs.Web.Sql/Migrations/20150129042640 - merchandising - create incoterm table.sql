create table Merchandising.Incoterm (
	 Id					int				identity not null
	,ProductId			int				not null
	,Name				varchar(240)	null -- International trade terms such as EXW, CIF, FOB, etc
	,CurrencyType		varchar(3)		null -- It indicates the currency code in EBS11i. i.e. USD
	,SupplierUnitCost	money			null -- Value of supplier unit cost for the product code.
	,CountryOfDispatch	varchar(2)		null -- ISO code of 2 characters of the country of dispatch of the product.
	,LeadTime			varchar(20)		null -- Period of time since the request is made by purchasing department until the product arrives at country,
	,constraint PK_Merchandising_Incoterm primary key clustered (Id)
)

alter table Merchandising.Incoterm with check
add constraint FK_Merchandising_Incoterm_ProductId
foreign key (ProductId)
references Merchandising.Product(Id)