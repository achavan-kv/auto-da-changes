
IF NOT EXISTS (SELECT * FROM [Cron].[Endpoint] WHERE [Name]='Update Only Index Products')
BEGIN  
INSERT into [Cron].[Endpoint] (Id,Name,Url,Module)
Values (801,'Update Only Index Products','/cosacs/Merchandising/Products/UpdateOnlyIndexProducts','Merchandising');
Update [Cron].[Endpoint] set Name='Full Index Products' where Module='Merchandising'  and id=206 and name ='Index Products'
END

GO