create table Merchandising.CintError (
	[Id]				int				not null identity(1,1),
	[ProductCode]		varchar(max)	null,
	[ReferenceNumber]	varchar(max)	null,
	[Type]				varchar(max)	null,
	[Location]			varchar(max)	null,
	[ErrorMessage]		varchar(max)	null,
	[Date]				datetime		not null,
	constraint PK_Merchandising_CintError primary key clustered (Id asc)
)