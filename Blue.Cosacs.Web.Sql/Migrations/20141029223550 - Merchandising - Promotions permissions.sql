-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
insert into Admin.Permission
(CategoryId, Id, Name, Description)
VALUES
(21, 2128, 'PromotionView', 'Allows users to view the promotions maintenance screen'),
(21, 2129, 'PromotionEdit', 'Allows users to create and edit promotions')