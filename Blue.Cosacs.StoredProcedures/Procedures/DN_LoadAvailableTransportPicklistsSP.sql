SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

if exists (SELECT * FROM dbo.sysobjects WHERE id = object_id('[dbo].[DN_LoadAvailableTransportPicklistsSP]') AND OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_LoadAvailableTransportPicklistsSP]
GO

CREATE PROCEDURE 	dbo.DN_LoadAvailableTransportPicklistsSP 
					@branchno smallint,
					@return integer output
AS
	SET @return = 0
	
	SELECT	picklistnumber 
	FROM	picklist 
	WHERE	branchno = @branchno 
	AND		ordertransport = 'T'
	AND		EXISTS(	SELECT  1 
					FROM	schedule s
							INNER JOIN deliveryload d ON s.buffno = d.buffno
													   AND s.buffbranchno = d.buffbranchno
													   AND s.loadno = d.loadno
							INNER JOIN transptsched t ON d.branchno = t.branchno
													  AND d.datedel = t.datedel
													  AND d.loadno	= t.loadno	
					WHERE	s.transchedno = picklist.picklistnumber  
					AND		s.transchednobranch  = @branchno)
	ORDER BY picklistnumber
     
     SET @return = @@error

GO

