SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_GetLoadContentsSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_GetLoadContentsSP]
GO
CREATE PROCEDURE 	dbo.DN_GetLoadContentsSP
			@loadno smallint, 
			@datedel datetime,
			@branchno smallint,
			@return int OUTPUT

AS


	SET 	@return = 0			--initialise return code

	SELECT 	distinct dl.origbr,
		dl.branchno,
		dl.datedel,
		dl.loadno,
		dl.buffbranchno AS StockLocn,
		dl.buffno,
                sc.acctno,
                sc.Picklistnumber
	FROM  deliveryload dl,  schedule sc 
        WHERE  dl.loadno = @loadno
	      AND  dl.datedel = @dateDel
          AND  dl.branchno  = @branchno
          --AND  sc.datedelplan = dl.datedel
		  AND  sc.datedelplan >= dl.datedel --IP - 07/05/08 - UAT(353) v 5.1
          AND  sc.loadno = dl.loadno
	      AND  dl.buffbranchno = (CASE WHEN ISNULL(sc.retstocklocn,0) = 0 THEN sc.stocklocn ELSE sc.retstocklocn END)
          AND  sc.buffno = dl.buffno
        
       

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO