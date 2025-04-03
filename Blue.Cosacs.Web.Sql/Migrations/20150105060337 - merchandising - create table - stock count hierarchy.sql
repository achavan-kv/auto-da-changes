create table Merchandising.[StockCountHierarchy] (
	Id int not null identity(1,1),
	StockCountId int not null,
	HierarchyLevelId int not null,
	HierarchyTagId int null,
	constraint [PK_Merchandising_StockCountHierarchy] primary key clustered (Id asc)
)

alter table Merchandising.[StockCountHierarchy]
with check add constraint FK_Merchandising_StockCountHierarchy_StockCount
foreign key (StockCountId)
references Merchandising.StockCount(Id)

alter table Merchandising.[StockCountHierarchy]
with check add constraint FK_Merchandising_StockCountHierarchy_Level
foreign key (HierarchyLevelId)
references Merchandising.HierarchyLevel(Id)

alter table Merchandising.[StockCountHierarchy]
with check add constraint FK_Merchandising_StockCountHierarchy_Tag
foreign key (HierarchyTagId)
references Merchandising.HierarchyTag(Id)