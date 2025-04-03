-- transaction: true

ALTER TABLE [NonStocks].[LinkProduct]
ADD [Order] SMALLINT NULL

ALTER TABLE [NonStocks].[LinkNonStock]
ADD [Order] SMALLINT NULL
