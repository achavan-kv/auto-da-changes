sp_rename 'Warranty.Category', 'Level', 'OBJECT'
go

sp_rename 'Warranty.Tag.CategoryId', 'LevelId', 'COLUMN'
go