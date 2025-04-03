SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_AcctIsRejectedSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_AcctIsRejectedSP]
GO

CREATE PROCEDURE dbo.DN_AcctIsRejectedSP
		 @acctno varchar(12),
		 @isrejected int OUT,
   		 @return INT OUTPUT
 
AS
	DECLARE @latest datetime 	

	SET  @return = 0 --initialise return code
	SET  @isrejected = 0

        SELECT  @latest = MAX(dateprop)
        FROM    proposal
        WHERE   acctno = @acctno

	SELECT	@isrejected = COUNT(*)
	FROM	proposal
	WHERE	acctno = @acctno
	AND	dateprop = @latest
	AND	propresult in ('D','X')
	AND NOT EXISTS (SELECT * FROM Delivery										  -- Issue 68994
				    INNER JOIN Stockitem ON Stockitem.itemno = Delivery.itemno	  -- 
					WHERE Stockitem.itemtype = 'S'								  --
				    AND Delivery.acctno = Proposal.acctno)						  --

 	IF (@@error != 0)
 	BEGIN
  		SET @return = @@error
 	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

