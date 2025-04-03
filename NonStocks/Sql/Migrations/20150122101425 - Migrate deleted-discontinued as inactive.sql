-- transaction: true

UPDATE NonStocks.NonStock
SET Active=0
WHERE ShortDescription like '%DELETED%'
    OR LongDescription like '%DELETED%'
    OR ShortDescription like '%DISCONTINUED%'
    OR LongDescription like '%DISCONTINUED%'

