SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_OracleInteg_UpdateStockInfo]') and OBJECTPROPERTY(id, 'IsProcedure') = 1) 
drop procedure [dbo].[DN_OracleInteg_UpdateStockInfo]
GO
if exists (select countrycode from country where countrycode='M') 
begin
declare @statement nvarchar(max)
set @statement='
-- =============================================
-- Author:		Mohamed Nasmi
-- Create date: 03/03/2009
-- Description:	To update StockInfo table from Oracle inbound CSV files
-- =============================================

CREATE PROCEDURE [dbo].[DN_OracleInteg_UpdateStockInfo] 
    @itemno	varchar(8),
	@itemdescr1	varchar(25),
	@itemdescr2	varchar(40),
	@category smallint,
	@supplier varchar(40),
	@prodstatus	 varchar(1),
	@suppliercode varchar(18),
	@warrantable smallint,
	@itemtype varchar(1),
	@refcode varchar(3),
	@warrantyrenewalflag char(1),
	@leadtime smallint,
	@assemblyrequired char(1),
	@deleted char(1),
	@return INT OUTPUT

AS
	-- SET NOCOUNT ON added to marginally improve performance 
	SET NOCOUNT ON;
    SET @return = 0
	
	if exists (select * from stockquantity where itemno = @itemno)
	BEGIN
		UPDATE StockQuantity
		set refcode=@refcode, deleted = @deleted
		WHERE 
		itemno = @itemno
	END
	ELSE
	BEGIN
		INSERT INTO StockQuantity
			( 
				itemno,	stocklocn, qtyAvailable,	
				stock, stockonorder, stockdamage,
				leadtime, dateupdated, LastOperationSource, refcode, deleted		
			)
			select 
				@itemno, branchno, 0,	
				0, 0, 0,
				@leadtime, getdate(), ''ORACLEINFO''	, @refcode, @deleted
			from branch
	END


	UPDATE StockInfo
	SET 
		itemdescr1	= @itemdescr1,
		itemdescr2	= @itemdescr2,
		category = @category,
		supplier = @supplier, 
		prodstatus	 = @prodstatus,
		suppliercode = @suppliercode,
		warrantable = @warrantable,
		itemtype = @itemtype,
		--refcode = @refcode, -- Now on stockPrice
		warrantyrenewalflag = @warrantyrenewalflag,
		--leadtime = @leadtime,
		assemblyrequired = @assemblyrequired
		--,deleted = @deleted -- Now on stockquantity
	WHERE 
	itemno = @itemno

	IF @@ROWCOUNT = 0

    declare @maxid int

    select @maxid = Max(id) from stockinfo
    where id < 100000
    
    INSERT INTO StockInfo
    ( 
		id,itemno, itemdescr1, itemdescr2,
		category, supplier, prodstatus,
		suppliercode, warrantable, itemtype,
		--refcode,
		 warrantyrenewalflag, leadtime,
		assemblyrequired
		--, deleted 
	)
	VALUES
	( 
		@maxid + 1, @itemno, @itemdescr1, @itemdescr2,
		@category, @supplier, @prodstatus,
		@suppliercode, @warrantable, @itemtype,
		@warrantyrenewalflag, @leadtime,
		@assemblyrequired
		--, @deleted
	) 


	UPDATE StockPrice SET refcode = @refcode WHERE itemno= @itemno 
    UPDATE StockQuantity SET deleted = @deleted  WHERE itemno= @itemno 
	SET @return = @@ERROR

	RETURN @return
	

'


exec sp_executesql @statement
end
go