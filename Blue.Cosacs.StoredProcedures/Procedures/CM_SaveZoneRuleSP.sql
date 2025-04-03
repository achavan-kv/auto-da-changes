SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[CM_SaveZoneRuleSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[CM_SaveZoneRuleSP]
GO

-- =============================================
-- Author:		Mohamed Nasmi
-- Create date: 03/02/2009
-- Description:	Saves zones rules
-- =============================================

CREATE PROCEDURE [dbo].[CM_SaveZoneRuleSP] 
	@return INTEGER OUT, 
	@Zone VARCHAR (4)  ,
	@column_name VARCHAR(32)   ,
	@Query VARCHAR(128) ,
	@or_clause VARCHAR(2) ,
	@notlike bit

AS 
	SET @return = 0 
	
	UPDATE CmZoneAllocation
	SET column_name = @column_name,
	Query = @Query,
	Zone  = @Zone,
	or_clause = @or_clause,
	notlike = @notlike
	WHERE ZONE = @ZONE 
	AND column_name= @column_name
	AND Query = @Query

	IF @@ROWCOUNT = 0 
	BEGIN
		INSERT INTO CmZoneAllocation
		(Zone,
			   column_name,
			   Query,
			   or_clause,
				notlike )
		VALUES (@Zone,
		@column_nAme,
		@Query,
		@or_clause,
		@notlike )
	END 
	
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO
