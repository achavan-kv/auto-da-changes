SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_OracleInteg_UpdateWarrantyBand]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_OracleInteg_UpdateWarrantyBand]
GO

-- =============================================
-- Author:		Mohamed Nasmi
-- Create date: 03/03/2009
-- Description:	To update WarrantyBand table from Oracle inbound CSV files
-- =============================================

CREATE PROCEDURE [dbo].[DN_OracleInteg_UpdateWarrantyBand] 
	@waritemno varchar(8),
	@refcode varchar(3),
	@minprice money,
	@maxprice money,
	@warrantylength float,
	@firstYearWarPeriod smallint,
	@return INT OUTPUT

AS
	-- SET NOCOUNT ON added to marginally improve performance 
	SET NOCOUNT ON;
    SET @return = 0
    
      -- need to remove other refcodes in case the warranty itemno has changed 
	DELETE FROM warrantyband WHERE  waritemno !=@waritemno 
	AND ( (minprice >= @minprice AND minprice < @maxprice ) OR (maxprice<=@maxprice AND maxprice >@minprice)  )
	AND refcode = @refcode AND warrantylength = @warrantylength
	
	UPDATE WarrantyBand
	SET 		
		refcode = @refcode,
		minprice = @minprice,
		maxprice = @maxprice,
		warrantylength = @warrantylength,
		firstYearWarPeriod = @firstYearWarPeriod
	WHERE 
	waritemno = @waritemno
	
	IF @@ROWCOUNT = 0

    INSERT INTO WarrantyBand
    ( 
		waritemno, refcode, minprice,
		maxprice, warrantylength, firstYearWarPeriod		
	)
	VALUES
	( 
		@waritemno, @refcode, @minprice,
		@maxprice, @warrantylength, @firstYearWarPeriod
	) 

	SET @return = @@ERROR

	RETURN @return
	
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO