create table Financial.StockAdjustmentMessage (
	 Id							int				identity not null
	,AdjustmentId				int				not null
	,LocationId					int				not null
	,CreatedDate				datetime		not null
	,SecondaryReason			varchar(max)	null
	,constraint [PK_Financial_StockAdjustmentMessage] primary key clustered (Id asc)
)

create table Financial.StockAdjustmentProductMessage (
	 Id							int				identity not null
	,StockAdjustmentMessageId	int				not null
	,ProductId					int				not null
	,[Type]						varchar(max)	null
	,DepartmentCode				varchar(max)	null
	,constraint [PK_Financial_StockAdjustmentProductMessage] primary key clustered (Id asc)
)

alter table Financial.[StockAdjustmentProductMessage]
with check add constraint FK_Financial_StockAdjustmentMessage_ProductMessage
foreign key (StockAdjustmentMessageId)
references Financial.StockAdjustmentMessage(Id)