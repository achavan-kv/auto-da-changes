
SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_CustomerDeletePrizeVouchersSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_CustomerDeletePrizeVouchersSP]
GO


CREATE PROCEDURE 	dbo.DN_CustomerDeletePrizeVouchersSP
					@enddate datetime,
					@acctno varchar(12),
					@iscancellation bit,
					@return int OUTPUT

AS
	SET @return = 0		--initialise return code

	SET @enddate = CONVERT(DATETIME,CONVERT(VARCHAR(20),@enddate,120),120)

	IF (@iscancellation = 1)
	BEGIN
	    DELETE 
	    FROM	PrizeVoucherMaster 
	    WHERE	AcctNo = @acctno
	END    
	ELSE
	BEGIN
	    DELETE 
	    FROM	PrizeVoucherMaster 
	    WHERE	DateIssued <= @enddate
	END    


	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

