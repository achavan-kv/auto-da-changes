if  exists (select * from sys.views where object_id = object_id(N'[Merchandising].[LocationByWarehouseView]'))
drop view  [Merchandising].[LocationByWarehouseView]
go