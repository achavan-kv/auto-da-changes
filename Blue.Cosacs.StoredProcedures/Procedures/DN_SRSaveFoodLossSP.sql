/****** Object:  StoredProcedure [dbo].[DN_SRSaveFoodLossSP]    Script Date: 10/19/2006 15:14:05 ******/
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID('[dbo].[DN_SRSaveFoodLossSP]') AND OBJECTPROPERTY(id,'IsProcedure') = 1)
DROP PROCEDURE [dbo].[DN_SRSaveFoodLossSP]

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Peter Chong
-- Create date: 19-Oct-2006
-- Description:	Saves a food loss item
-- =============================================
CREATE PROCEDURE DN_SRSaveFoodLossSP 
(
	@ServiceRequestNo int , 
	@ItemDescription  varchar(25) ,
	@ItemValue		  money,
	@return			  int output
)
AS
BEGIN
	SET NOCOUNT ON;

    INSERT INTO [dbo].[SR_FoodLoss]
           ([ServiceRequestNo]
           ,[ItemDescription]
           ,[ItemValue])
     VALUES
           (@ServiceRequestNo
           ,@ItemDescription
           ,@ItemValue)
	
	SET @return = @@error
END
GO
