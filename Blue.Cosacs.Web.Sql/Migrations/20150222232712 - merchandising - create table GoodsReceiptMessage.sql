create table Financial.GoodsReceiptMessage (
	 Id						int				identity not null
	,ReceiptId				int				not null
	,LocationId				int				not null
	,VendorId				int				not null
	,ReceiptType			varchar(max)	null
	,CreatedDate			datetime		not null
	,VendorType				varchar(max)	null
	,constraint [PK_Financial_GoodsReceiptMessage] primary key clustered (Id asc)
)

create table Financial.GoodsReceiptProductMessage (
	 Id						int				identity not null
	,GoodsReceiptMessageId	int				not null
	,ProductId				int				not null
	,[Type]					varchar(max)	null
	,DepartmentCode			varchar(max)	null
	,constraint [PK_Financial_GoodsReceiptProductMessage] primary key clustered (Id asc)
)

alter table Financial.[GoodsReceiptProductMessage]
with check add constraint FK_Financial_GoodsReceiptMessage_ProductMessage
foreign key (GoodsReceiptMessageId)
references Financial.GoodsReceiptMessage(Id)
