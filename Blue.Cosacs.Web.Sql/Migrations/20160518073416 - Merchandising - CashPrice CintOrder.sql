DECLARE @SQL NVARCHAR(MAX) = N'';

SELECT @SQL += N'
ALTER TABLE Merchandising.' + OBJECT_NAME(PARENT_OBJECT_ID) + ' DROP CONSTRAINT ' + OBJECT_NAME(OBJECT_ID) + ';' 
from sys.objects
where type = 'D'
and OBJECT_NAME(parent_object_id) = 'CintOrder'

EXECUTE (@SQL)

ALTER TABLE Merchandising.CintOrder
ALTER COLUMN CashPrice DECIMAL(18,4) NULL
GO

ALTER TABLE [Merchandising].[CintOrder] 
ADD constraint DF_CintOrder_Discount  DEFAULT ((0)) FOR [Discount]
GO

ALTER TABLE [Merchandising].[CintOrder] 
ADD constraint DF_CintOrder_CashPrice  DEFAULT ((0)) FOR CashPrice
GO