-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
insert into dbo.HiLo
VALUES
('Merchandising.Combo', 1, 1),
('Merchandising.Set', 1, 1)