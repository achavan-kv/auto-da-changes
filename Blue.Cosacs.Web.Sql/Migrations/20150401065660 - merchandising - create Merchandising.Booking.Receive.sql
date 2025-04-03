INSERT [Hub].[Queue] ([Id], [Binding], [SubscriberClrAssemblyName], [SubscriberClrTypeName], [SubscriberSqlConnectionName], [SubscriberSqlProcedureName], [SchemaSource], [SubscriberHttpUrl], [SubscriberHttpMethod], [Schema]) 
VALUES (
	216
	,N'Merchandising.Booking.Receive'
	,NULL, NULL, NULL, NULL
	,N'Merchandising.xsd'
	,N'/cosacs/Merchandising/BookingReceiveStock'
	,N'POST'
	,null
)