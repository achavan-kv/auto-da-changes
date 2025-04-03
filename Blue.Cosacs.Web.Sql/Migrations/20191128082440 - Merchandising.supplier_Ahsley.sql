--IF OBJECT_ID ('TR_Incoterm_Supplier', 'TR') IS NOT NULL 

IF EXISTS (SELECT * FROM sys.triggers WHERE object_id = OBJECT_ID(N'[dbo].[TR_Incoterm_Supplier]'))
BEGIN
	drop trigger [dbo].[TR_Incoterm_Supplier]
END
go


CREATE TRIGGER TR_Incoterm_Supplier ON [Merchandising].[Supplier] 
    FOR INSERT, UPDATE 
AS
BEGIN 
	SET NOCOUNT ON;
	DECLARE @ID NVARCHAR(50) 
	--set @id=(select top 1 id from [Merchandising].[Supplier] )
	SELECT @Id=id FROM Inserted 
	UPDATE pp SET pp.LeadTime=t.LeadTime
	FROM	 [Merchandising].[Supplier] t  inner join [Merchandising].[AdditionalCostPrices] o ON 
	t.Id=o.VendorId  inner join  [Merchandising].[Incoterm] pp ON o.ProductId=pp.ProductId  
	WHERE t.id =@Id    
 END
GO