SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

if exists (SELECT * FROM dbo.sysobjects WHERE id = object_id('[dbo].[DN_LoadAvailablePicklistsSP]') AND OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_LoadAvailablePicklistsSP]
GO

CREATE PROCEDURE 	dbo.DN_LoadAvailablePicklistsSP 
					@branchno smallint,
					@type char,
					@return integer output
AS
	SET @return = 0
	
	IF (@type = 'P')
	BEGIN
		SELECT	picklistnumber 
		FROM	picklist 
		WHERE	branchno = @branchno 
		AND		datedel IS  NULL 
		AND		ordertransport = 'O'
		AND		EXISTS(	SELECT  * 
						FROM	schedule s
						WHERE	s.picklistnumber = picklist.picklistnumber  
						AND		s.picklistbranchnumber  = @branchno)
		ORDER BY picklistnumber
	END
	
	IF (@type = 'S')
	BEGIN
		SELECT	picklistnumber FROM picklist 
        WHERE	branchno =@branchno 
        AND		datedel IS NULL 
        AND		ordertransport = 'O'
        AND NOT EXISTS (SELECT  * 
						FROM  schedule s
						WHERE  s.loadno>0 
						AND  s.picklistnumber=picklist.picklistnumber  
						AND  s.picklistbranchnumber  = @branchno)
        AND EXISTS(	SELECT  * 
                    FROM  schedule s
                    WHERE  s.picklistnumber=picklist.picklistnumber  
                    AND  s.picklistbranchnumber  = @branchno)
		ORDER BY picklistnumber
	END	
	
	IF (@type = 'T')
	BEGIN
		SELECT	picklistnumber 
		FROM	picklist 
        WHERE	branchno = @branchno 
        AND		datedel IS NULL 
		AND		ordertransport = 'T'
		AND		EXISTS(	SELECT  * 
						FROM	schedule s
						WHERE	s.transchedno=picklist.picklistnumber  
						AND		s.transchednobranch = @branchno)
		ORDER BY picklistnumber
     END 
     
     SET @return = @@error

GO

