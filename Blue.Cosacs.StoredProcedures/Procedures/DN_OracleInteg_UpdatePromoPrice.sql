
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_OracleInteg_UpdatePromoPrice]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_OracleInteg_UpdatePromoPrice]
GO

--Author : Mohamed Nasmi

CREATE PROCEDURE [dbo].[DN_OracleInteg_UpdatePromoPrice] 
	@origbr	smallint,
	@itemno	varchar(8),	
	@stocklocn int,	
	@hporcash char(1),	
	@fromdate datetime,	
	@todate	datetime,	
	@unitprice float,	
	@return INT OUTPUT

AS
	-- SET NOCOUNT ON added to marginally improve performance 
	SET NOCOUNT ON;
    SET @return = 0
    -- make sure promoprice is available the whole day.
    IF ISNULL(@todate ,'1-jan-1900') !='1-jan-1900' AND DATEPART(hour,@todate) =0
		SET @todate=DATEADD(minute,59,DATEADD(hour,23,@todate))
	
	-- terminating any outstanding promotional prices
	UPDATE PromoPrice
	SET 
		--origbr = @origbr,	
		todate = DATEADD(second,-1,@fromdate) -- 
	WHERE 
	itemno = @itemno and stocklocn = @stocklocn and 
	hporcash = @hporcash and todate > @fromdate

	
	
	UPDATE PromoPrice
	SET 
		--origbr = @origbr,	
		todate = @todate, -- 
		unitprice = @unitprice 
	WHERE 
	itemno = @itemno and stocklocn = @stocklocn and 
	hporcash = @hporcash and fromdate = @fromdate

	IF @@ROWCOUNT = 0

    INSERT INTO PromoPrice
    ( 
		origbr, itemno,	stocklocn,	
		hporcash, fromdate,	todate,	
		unitprice		
	)
	VALUES
	( 
		@origbr, @itemno, @stocklocn,	
		@hporcash, @fromdate, @todate,	
		@unitprice	
	) 

	SET @return = @@ERROR

	RETURN @return
	
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO
