

/****** Object:  StoredProcedure [dbo].[CheckAccountDetails]    Script Date: 11/19/2018 2:04:18 PM ******/
IF EXISTS (SELECT * FROM sysobjects 
   WHERE NAME = 'CheckAccountDetails'
   )
BEGIN
DROP PROCEDURE [dbo].[CheckAccountDetails]
END
GO

/****** Object:  StoredProcedure [dbo].[CheckAccountDetails]    Script Date: 11/19/2018 2:04:18 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Bhupesh badwaik
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
Create PROCEDURE [dbo].[CheckAccountDetails]
@CustId As nvarchar(20)
, @StoreId As nvarchar(30)
AS
BEGIN

	--select Count(*) AS TotalCount from custacct where custid=@CustId
	select Count(*) AS TotalCount from Custacct, acct where Custacct.acctno=acct.acctno and Custacct.custid=@CustId and accttype='R'

	--Stock Item
	select Count(*) AS TotalCount from merchandising.supplier where Code=@StoreId

	-- Non stock Item

	--select Count(MS.Code) As TotalCount from Stockinfo As SI
	--Inner join merchandising.product AS MP on MP.SKU=SI.itemno
	--Inner join merchandising.supplier AS MS on MP.PrimaryVendorid=MS.ID
	--where MS.Code=@StoreId and SI.itemtype = 'N'
	
END

GO

