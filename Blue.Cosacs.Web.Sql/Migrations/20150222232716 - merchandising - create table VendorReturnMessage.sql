create table Financial.VendorReturnMessage (
	 Id						int				identity not null
	,VendorReturnId			int				not null
	,LocationId				int				not null
	,VendorId				int				not null
	,ReceiptType			varchar(max)	null
	,CreatedDate			datetime		not null
	,VendorType				varchar(max)	null
	,constraint [PK_Financial_VendorReturnMessage] primary key clustered (Id asc)
)

create table Financial.VendorReturnProductMessage (
	 Id						int				identity not null
	,VendorReturnMessageId	int				not null
	,ProductId				int				not null
	,[Type]					varchar(max)	null
	,DepartmentCode			varchar(max)	null
	,constraint [PK_Financial_VendorReturnProductMessage] primary key clustered (Id asc)
)

alter table Financial.[VendorReturnProductMessage]
with check add constraint FK_Financial_VendorReturnMessage_ProductMessage
foreign key (VendorReturnMessageId)
references Financial.VendorReturnMessage(Id)