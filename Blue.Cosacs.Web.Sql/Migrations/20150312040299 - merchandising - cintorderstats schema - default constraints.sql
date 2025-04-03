-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
ALTER TABLE [Merchandising].[CintOrderStats] ADD  DEFAULT ((0)) FOR [QtyOrdered]
ALTER TABLE [Merchandising].[CintOrderStats] ADD  DEFAULT ((0)) FOR [QtyDelivered]
ALTER TABLE [Merchandising].[CintOrderStats] ADD  DEFAULT ((0)) FOR [QtyReturned]
ALTER TABLE [Merchandising].[CintOrderStats] ADD  DEFAULT ((0)) FOR [QtyRepossessed]



