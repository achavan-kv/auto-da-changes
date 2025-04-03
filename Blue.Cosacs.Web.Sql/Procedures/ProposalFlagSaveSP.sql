

/****** Object:  StoredProcedure [dbo].[ProposalFlagSaveSP]    Script Date: 11/19/2018 1:51:26 PM ******/
IF EXISTS (SELECT * FROM sysobjects 
   WHERE NAME = 'ProposalFlagSaveSP'
   )
BEGIN
DROP PROCEDURE [dbo].[ProposalFlagSaveSP]
END
GO

/****** Object:  StoredProcedure [dbo].[ProposalFlagSaveSP]    Script Date: 11/19/2018 1:51:26 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:	<Author,Bhupesh Badwaik>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[ProposalFlagSaveSP]
   @origbr smallint,  
   @custid varchar(20),  
   @dateprop smalldatetime,  
   @checktype varchar(4),  
   @datecleared datetime,  
   @empeenopflg int,  
   @acctno char (12),  
   @returnFlag int OUTPUT 
AS
BEGIN
	exec DN_ProposalFlagSaveSP @origbr=@origbr,@custid=@custid,@dateprop = @dateprop,@checktype=@checktype,
	@datecleared=@datecleared,@empeenopflg=@empeenopflg,@acctno=@acctno,@return=@returnFlag output
END

GO

