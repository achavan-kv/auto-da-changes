

ALTER TABLE Service.Request
ALTER COLUMN Evaluation BIT NULL
GO

sp_rename 'Service.Request.Evaluation', 'EvaluationClaimFoodLoss','COLUMN'
GO