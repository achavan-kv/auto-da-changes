-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
exec sp_rename 'Merchandising.Product.FeatureBenefit', 'Features', 'COLUMN'