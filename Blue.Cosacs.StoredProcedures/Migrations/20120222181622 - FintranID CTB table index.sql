CREATE NONCLUSTERED INDEX CTB_FintransID
ON [dbo].[CashierTotalsBreakdown] ([FintransId])
INCLUDE ([cashiertotalid])