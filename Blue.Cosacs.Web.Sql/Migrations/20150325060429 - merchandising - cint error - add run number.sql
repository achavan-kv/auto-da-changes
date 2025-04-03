if col_length('Merchandising.CintError', 'RunNo') is null
begin

alter table Merchandising.CintError
add RunNo int not null default 0

end