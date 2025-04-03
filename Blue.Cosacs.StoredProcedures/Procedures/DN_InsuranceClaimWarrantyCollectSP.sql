
/****** Object:  StoredProcedure [dbo].[DN_InsuranceClaimWarrantyCollectSP]    Script Date: 09/29/2006 16:35:04 ******/
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID('[dbo].[DN_InsuranceClaimWarrantyCollectSP]') AND OBJECTPROPERTY(id,'IsProcedure') = 1)
DROP PROCEDURE [dbo].[DN_InsuranceClaimWarrantyCollectSP]


SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Peter Chong
-- Create date: 29-Sep-2006
-- Description:	Puts an entry in the delivery table to indicate a warranty has been collected
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 08/06/11  IP  CR1212 - RI changes. Passed in @warrantyID.
-- =============================================
CREATE PROCEDURE DN_InsuranceClaimWarrantyCollectSP 
(		
	@acctno			varchar(12),
	@agreementno	int,
	--@itemno			varchar(10),
	@warrantyID		int,								--IP - 08/06/11 - CR1212 - RI
	@stocklocn		smallint,
	@buffno			int,
	@contractno		varchar(10),
	@empeeno		int, 
	@returncode		varchar(10),
	@newbuffno		int,
	@return int		OUTPUT
)
AS
BEGIN
	SET NOCOUNT ON;
    
	INSERT INTO [delivery]
           ([origbr]
           ,[acctno]
           ,[agrmtno]
           ,[datedel]
           ,[delorcoll]
           ,[itemno]
           ,[stocklocn]
           ,[quantity]
           ,[retitemno]
           ,[retstocklocn]
           ,[retval]
           ,[buffno]
           ,[buffbranchno]
           ,[datetrans]
           ,[branchno]
           ,[transrefno]
           ,[transvalue]
           ,[runno]
           ,[contractno]
           ,[ReplacementMarker]
           ,[NotifiedBy]
           ,[ftnotes]
           ,[ItemID])												--IP - 08/06/11 - CR1212 - RI
     
	SELECT [origbr]
			,[acctno]
			,[agrmtno]
			,[datedel]
			, 'C'
			,[itemno]
			,[stocklocn]
			,-quantity	
			,@returnCode
			,@stocklocn
			,0 --[retval] Should be zero according to the spec
			,@newbuffno
			,[buffbranchno]
			,getdate()
			,[branchno]
			,[transrefno]
			,0 --[transvalue] Should also be zero
			,[runno]
			,[contractno]
			,[ReplacementMarker]
			,@empeeno
			,'CWRT'  -- warranty return
			,[ItemID]												--IP - 08/06/11 - CR1212 - RI
	FROM [delivery]
	WHERE	acctno	= @acctno
		AND		agrmtno			= @agreementno
		--AND		itemno			= @itemno
		AND		ItemID	  =		@warrantyID							--IP - 08/06/11 - CR1212 - RI			
		AND		stocklocn =		@stocklocn		
		AND		buffno =		@buffno			
		AND		contractno = 	@contractno
	

		--Update Lineitem
	UPDATE lineitem
	SET delqty = 0, quantity = 0, ordval = 0
	WHERE acctno = @acctno
		AND		agrmtno			= @agreementno
		--AND		itemno			= @itemno	
		AND		ItemID	  =		@warrantyID							--IP - 08/06/11 - CR1212 - RI		
		AND		stocklocn =		@stocklocn		
		AND		contractno = 	@contractno
	
	


	--Add a collection reason
	IF @@ROWCOUNT > 0 
		INSERT INTO [CollectionReason]
           ([AcctNo]
           ,[ItemNo]
           ,[CollectionReason]
           ,[DateAuthorised]
           ,[EmpeenoAuthorised]
		   ,[StockLocn]
		   , CollectType
		   ,[ItemID]												--IP - 08/06/11 - CR1212 - RI
           )
     VALUES
           (@acctno
           --,@ItemNo 
           ,''														--IP - 08/06/11 - CR1212 - RI
           ,'INW'
           ,GetDate()
           ,@empeeno
		   ,@stocklocn
		   ,'C'
		   ,@warrantyID												--IP - 08/06/11 - CR1212 - RI
           )

SET @Return = @@Error
END
GO




