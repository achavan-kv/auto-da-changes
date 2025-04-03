create table Financial.CintOrderReceiptMessage (
	 Id					int				identity not null
	,CintOrderId		int				not null
	,ProductId			int				not null
	,Reference			varchar(max)	null
	,SalesType			varchar(max)	null
	,SalesLocationId	varchar(max)	null
	,TotalAWC			money			not null
	,constraint [PK_Financial_CintOrderReceiptMessage] primary key clustered (Id asc)
)