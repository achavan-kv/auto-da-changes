SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_AccountGetAccountNoByBuffNoSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_AccountGetAccountNoByBuffNoSP]
GO
CREATE PROCEDURE 	dbo.DN_AccountGetAccountNoByBuffNoSP
			@stockLocn int ,
                        @buffNo int ,
                        @acctNo varchar(12)  OUT ,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code
	SET     @acctNo =  '000000000000'
	SELECT 	DISTINCT @acctNo = acctno 
	  FROM  schedule
	 WHERE  @stockLocn = (CASE WHEN ISNULL(retstocklocn,0) = 0 THEN stocklocn ELSE retstocklocn END)
           AND  buffNo = @buffNo
	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END 

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO