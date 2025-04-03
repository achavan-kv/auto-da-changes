CREATE NONCLUSTERED INDEX [IDX_TaxRate_EffectiveDate] ON [Merchandising].[TaxRate]
(
	ProductId, [EffectiveDate] ASC 
) INCLUDE ([Rate])
GO
