insert into [Admin].Permission (
	 CategoryId
	,Id
	,Name
	,[Description])
values (
	 21
	,2157
	,'Transaction Type View'
	,'Allows the user to view the transaction types screen'
), (
	 21
	,2158
	,'Transaction Type Edit'
	,'Allows the user to edit the transaction types screen'
)

update [Admin].Permission
set name = 'Incoterm View'
where name = 'IncotermView'