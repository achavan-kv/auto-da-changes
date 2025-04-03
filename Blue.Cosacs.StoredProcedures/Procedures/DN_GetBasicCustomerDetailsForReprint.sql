 if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_GetBasicCustomerDetailsForReprint]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
DROP PROCEDURE [dbo].[DN_GetBasicCustomerDetailsForReprint] 
GO

/****** Object:  StoredProcedure [dbo].[DN_GetBasicCustomerDetailsForReprint]]    Script Date: 26-04-2019 11:19:11 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO
 
 -- ===============================================================
-- Author:		Raj Kishore
-- Create date: 26-04-2019
-- Description:	This procedure will search  Customer Details data as per given inputs. 
-- ================================================================
 --DN_GetBasicCustomerDetailsForReprint 1383616,0
  CREATE PROCEDURE DN_GetBasicCustomerDetailsForReprint   
  @Orderid int,
  @return int OUTPUT  
  AS
  BEGIN
  SET @return = 0   --initialise return code  
  select sa.FirstName,
	 sa.Lastname ,
	sa.AddressLine1,
	sa.AddressLine2,
	sa.AddressLine3,
	sa.PostCode, ISNULL(cust.RFCreditLimit, 0) as RFCreditLimit, ISNULL(cust.AvailableSpend, 0) as AvailableSpend  from [Sales].[OrderCustomer] sa 
	LEFT JOIN customer cust on sa.CustomerId=cust.custid  
	 where orderid=@Orderid
	END

	
	 IF (@@error != 0)  
 BEGIN  
  SET @return = @@error  
 END 


