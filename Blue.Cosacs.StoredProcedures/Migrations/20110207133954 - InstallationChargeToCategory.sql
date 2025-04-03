INSERT INTO dbo.codecat
( category, catdescript, codelgth ,
  forcenum, forcenumdesc, usermaint 
)        
VALUES
( 'INSTCHARGE' , -- category - varchar(12)
  'Installation Primary Charge To' , -- catdescript - nvarchar(64)
  3 , -- codelgth - int
  'N' , -- forcenum - char(1)
  'N' , -- forcenumdesc - char(1)
  'N'  -- usermaint - char(1)
)