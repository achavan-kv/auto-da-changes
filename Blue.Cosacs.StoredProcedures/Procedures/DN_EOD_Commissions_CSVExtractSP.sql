
/****** Object:  StoredProcedure [dbo].[DN_EOD_Commissions_CSVExtractSP]    Script Date: 11/01/2006 17:15:46 ******/
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID('[dbo].[DN_EOD_Commissions_CSVExtractSP]') AND OBJECTPROPERTY(id,'IsProcedure') = 1)
DROP PROCEDURE [dbo].[DN_EOD_Commissions_CSVExtractSP]
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Peter Chong
-- Create date: 2006-11-01
-- Description:	CSV Extract for end of day commissions report
-- =============================================
CREATE PROCEDURE DN_EOD_Commissions_CSVExtractSP 
(	
	@RunNo int,
	@return int = 0 output
)
AS
BEGIN
	SET NOCOUNT ON;
	
	--IP - 18/09/08 - UAT5.1 - UAT(539) - Added 'Branch' where the 'Sales Person' is based.
	SELECT 
	u.branchno [Branch],
	Employee [Employee Number],
	u.FullName	[Employee Name],
	Sum(CommissionAmount)  [Commission Value] 	
	FROM salescommission C	
	INNER JOIN interfacecontrol I ON C.RunDate = I.DateStart  	
	LEFT OUTER JOIN Admin.[User] u ON C.Employee  = u.id
	WHERE I.Interface = 'COMMISSIONS'  
	AND I.RunNo = @RunNo
	GROUP BY u.branchno, Employee, u.Id, u.FullName

	SET @Return = @@Error	
END
GO
    



