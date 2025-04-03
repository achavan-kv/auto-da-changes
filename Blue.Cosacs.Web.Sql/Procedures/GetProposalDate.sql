
/****** Object:  StoredProcedure [dbo].[GetProposalDate]    Script Date: 11/19/2018 1:52:23 PM ******/
IF EXISTS (SELECT * FROM sysobjects 
   WHERE NAME = 'GetProposalDate'
   )
BEGIN
DROP PROCEDURE [dbo].[GetProposalDate]
END
GO

/****** Object:  StoredProcedure [dbo].[GetProposalDate]    Script Date: 11/19/2018 1:52:23 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:	<Author,Bhupesh Badwaik>
-- Create date: <Create Date,,>
-- Description:	<Description,,> GetProposalDate '98655111SKdGt','900019728661',''
-- =============================================
CREATE PROCEDURE [dbo].[GetProposalDate]
	    @CustId VARCHAR(20) = N'',
		@AccountNo VARCHAR(12) = N'',
		@Message varchar(MAX) output

AS
BEGIN
	
		if exists (select * from proposal where custid = @CustId and acctno=@AccountNo )		

		 select * from proposal where custid = @CustId and acctno=@AccountNo
		 ELSE 
		 Begin

			Set @Message='Record not found'
			select @Message
			
			
			End


END

GO

