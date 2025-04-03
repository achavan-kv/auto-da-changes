-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

if not exists(select * from admin.Permission where id =395)
Begin

INSERT INTO Admin.Permission
        ( Id, Name, CategoryId, Description )
VALUES  ( 395, -- Id - int
          'Failed Delivery and Collection - View all CSR', -- Name - varchar(100)
          14, -- CategoryId - int
          'Allows users to view all CSR ' 
          )

insert into [Control]
select 395,'FailedDeliveriesCollections','drpSalesperson',1,1,''

insert into [Control]
select 395,'FailedDeliveriesCollections','drpBranch',1,1,''

End

