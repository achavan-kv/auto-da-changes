
IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[Warehouse].[ScheduleView]'))
DROP VIEW [Warehouse].[ScheduleView]
GO

-- =======================================================================================
-- Version :  002

-- Change Control
-- --------------
-- Date			By			Description
-- ----			--			-----------
-- 18 Oc 2019  	Raj kishore #Added 'Distinct' Keyword to select  unique records only.
-- 23 Jun 2020 	Tosif		Selected customer accounts with hldorjnt = 'H'
-- =======================================================================================
CREATE VIEW [Warehouse].[ScheduleView]  
AS  
SELECT  DISTINCT      
        B.ScheduleID ,  
        B.Id AS BookingId ,  
        B.PickingComment,          
        B.PickingId AS PickingId,  
        B.CustomerName ,  
		CASE
			WHEN dbo.custaddress.addtype IN ('W','D') THEN ' ' 
			ELSE ISNULL(dbo.custaddress.DELTitleC,'') + ' ' + ISNULL(dbo.custaddress.DELFirstName,'') + ' ' + ISNULL(dbo.custaddress.DELLastName,'')   
		END as DName,  
        B.AddressLine1 ,  
        B.AddressLine2 ,  
        B.AddressLine3 ,  
        B.PostCode ,  
        B.StockBranch ,  
        B.DeliveryBranch ,  
        B.DeliveryOrCollection ,  
        B.DeliveryOrCollectionDate ,  
        B.ItemNo ,  
        B.ItemId ,  
        B.ItemUPC ,  
        B.ProductArea ,  
        B.ProductCategory ,  
        B.ProductDescription ,  
        B.ProductBrand ,  
        B.ProductModel ,  
        B.CurrentQuantity,  
        B.RepoItemId,  
        B.AssemblyReq,  
        B.Damaged,  
        B.ContactInfo,  
        B.OrderedOn,  
        BR1.branchname AS DeliveryBranchName,
        T.Id AS TruckId,  
        T.Name AS TruckName,
        b.DeliveryZone,
        b.ScheduleQuantity											--    #11484  
FROM Warehouse.Booking B 
INNER JOIN branch BR1 ON BR1.branchno = B.DeliveryBranch  
INNER JOIN Warehouse.Truck T ON T.Id = B.TruckId  
LEFT JOIN dbo.custacct AS custadd ON custadd.acctno = B.AcctNo AND custadd.hldorjnt = 'H' LEFT OUTER JOIN
dbo.custaddress ON custadd.custid = dbo.custaddress.custid 
AND (dbo.custaddress.cusaddr1 = B.AddressLine1 
     AND dbo.custaddress.cusaddr2 = B.AddressLine2
	 AND dbo.custaddress.cusaddr3 = B.AddressLine3)

GO

