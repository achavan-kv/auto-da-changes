
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID('[dbo].[DN_CustomerAdditionalDetailsResidentialGetSP]') AND OBJECTPROPERTY(id,'IsProcedure') = 1)
DROP PROCEDURE [dbo].[DN_CustomerAdditionalDetailsResidentialGetSP]

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Peter Chong
-- Create date: 10-Oct-2006
-- Description:	Gets Customer residential and financial details from the additional details table if a record exists otherwise from the proposal
-- =============================================
CREATE PROCEDURE DN_CustomerAdditionalDetailsResidentialGetSP
	@CustId varchar(20),
	@return int out
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	IF EXISTS (SELECT * FROM CustomerAdditionalDetails WHERE CustId = @CustID AND DateResidentialUpdate IS NOT NULL)
		SELECT 
				CustId
			  ,  DateInCurrentAddress	[DateIn]
 			  , DateinPreviousAddress	[PrevDateIn]
			  ,[ResidentialStatus] 
			  ,[PrevResidentialStatus]
			  ,MonthlyRent				[MonthlyRent]
			  ,PropertyType				[PropertyType]
			  
		FROM [CustomerAdditionalDetails]  
		WHERE CustId = @CustID 
		
	ELSE
	  SELECT CurrAD.custid, CurrAD.datein, PrevAD.Datein AS PrevDateIN, ISNULL(CurrAD.resstatus,'') AS ResidentialStatus,  
      ISNULL(PrevAD.resstatus,'') AS PrevResidentialStatus, CurrAD.mthlyrent AS Monthlyrent, CurrAD.PropType AS PropertyType  
	  FROM CustAddress CurrAD                         
	  LEFT OUTER JOIN Custaddress PrevAD ON  CurrAD.Custid = PrevAD.CustID   
											  AND PrevAD.datein = (SELECT MAX(datein) FROM Custaddress CA2  
																   WHERE CA2.custid = CurrAD.custid  
																   AND CA2.datein < CurrAD.datein  
																   AND CA2.addtype = 'H')  
	  WHERE CurrAD.addtype = 'H' 
	  AND CurrAD.custid = @custid
	  AND CurrAD.datein = (SELECT MAX(datein) FROM Custaddress CA  
							WHERE CA.Custid = CurrAD.custid  
							AND CA.addtype = 'H')  

	SET @return = @@Error

END
GO



SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


