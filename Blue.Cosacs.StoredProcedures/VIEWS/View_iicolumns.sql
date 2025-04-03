drop view iicolumns
go
/****** Object:  View iicolumns    Script Date: 08/03/00 13:09:29 ******/
CREATE VIEW iicolumns(		
table_name,
table_owner,
column_name,
column_datatype,
column_length,
column_scale,
column_nulls,
column_defaults,
column_sequence,
key_sequence,
sort_direction,
column_ingdatatype,
column_internal_da,
column_internal_le,
column_internal_in,
column_system_main,
column_updateable,
column_has_default,
column_default_val)
AS SELECT
lower (o.name),
u.name,
lower (c.name),
m.gw_dt_name,
case
when m.gw_dt_name in ('MONEY', 'LONG VARCH','LONG BYTE', 'DATE')

then convert(int, 0)
when m.gw_dt_name in ('DECIMAL ')
then convert(int, c.prec)
else 
convert(int, c.length)
end,	

isnull(convert(int, c.scale), 0),substring('NY', (((0x08 & c.status)/8) + 1), 1),' ',convert(int, c.colid),0,' ',-1,	t.name,	convert(int, c.length),-1,
' ',' ',' ',' '							    


FROM
dbo.sysobjects o,
dbo.sysusers u,
dbo.syscolumns c,
dbo.systypes t,
iigw_datatype_map m	  
WHERE
o.type in ('S','U','V')
and o.id = c.id
and o.uid = u.uid
and c.usertype = t.usertype
and c.usertype = m.ss_usertype
and (t.usertype != 0 or (t.usertype = 0 AND t.name = 'varchar'))


GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO