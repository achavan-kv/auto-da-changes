SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if  exists (select * from sysobjects  where name =  'DN_GetWarrantyReturnCodes' )
drop procedure DN_GetWarrantyReturnCodes
go

CREATE PROCEDURE DN_GetWarrantyReturnCodes 
--------------------------------------------------------------------------------
--
-- Project      : CoSACS .NET
-- File Name    : DN_GetWarrantyReturnCodes.PRC
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : DN_GetWarrantyReturnCodes.PRC
-- Description	:
-- Author       : ?
-- Date         : ?
--
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 21/06/11  IP  CR1212 - RI - #3939 - RI System Changes
-- 13/09/11  IP  RI - #8112 - Return the ItemId of the warranty return code
--------------------------------------------------------------------------------
				@acctno char(12),
				@stocklocn smallint, 
				@replacement smallint,
				--@parentitemno varchar (10), 
				@parentItemID int,						--IP - 21/06/11 - CR1212 - RI - #3939
				--@warrantyno varchar (10),
				@warrantyItemID int,					--IP - 21/06/11 - CR1212 - RI - #3939
				@contractno varchar(12),
				@returnwarranty varchar (10) OUT, 
				@refund float OUT, 
                @datedel DATETIME OUT,  -- UAT 349 Delivery Date required so that it can be determined whether or not the contract is over 30 days old.
                @warrantyRetCodeItemID int OUT,			--IP - 09/09/11 - RI - #8112	 
				@return int OUTPUT

AS

	SET @return = 0

	DECLARE	@category smallint, 
			@varcategory varchar (10),
			@producttype char(1),
			@months smallint,
			@warrantylength float,
			--@datedel datetime,
			@country varchar(3),
			@refcode varchar(3),
			@warrantyno varchar(18)			--IP - 21/06/11 - CR1212 - RI - #3939

	SELECT @warrantyno = itemno from StockInfo where ID = @warrantyItemID	--IP - 21/06/11 - CR1212 - RI - #3939
	
	SELECT	@category = si.category,
	        @refcode = sp.refcode 
	--FROM	stockitem 
	FROM	StockInfo si inner join StockPrice sp on si.ID = sp.ID		--IP - 21/06/11 - CR1212 - RI - #3939
	WHERE	si.ID = @parentItemID										--IP - 21/06/11 - CR1212 - RI - #3939
	--WHERE	itemno = @parentitemno

	SELECT	@country = countrycode 
	FROM	country

	IF @country = 'S'
		return 0

	--For Mauritius only.  Mobile phone warranties end in  2 excluding furniture warranties which always start in XW
	IF @country = 'M' AND RIGHT (@warrantyno, 1)= '2' AND LEFT(@warrantyno, 2) != 'XW'
	BEGIN
		SET @producttype ='M'
	END
	ELSE
	BEGIN
		SELECT	@varcategory = ISNULL(category,'') 
		FROM	code 
		WHERE	code = convert(varchar,@category)
		AND		category IN ('PCE','PCF','PCW')
	
		SET @producttype = ISNULL(RIGHT(@varcategory, 1),'')

		IF @producttype = 'W' OR @producttype = ''
		BEGIN
			IF @warrantyno like '19%' --electrical
				SET @producttype = 'E'
			IF @warrantyno like 'XW%' --furniture
  				SET @producttype='F'
		END
		
		IF @refcode = 'ZZ'
		BEGIN
		    SET @producttype = 'I'
		END    
	END

	--SELECT	@warrantylength = ISNULL(warrantylength, 0) * 12
	SELECT	@warrantylength = ISNULL(warrantylength, 0)					--IP - 21/06/11 - CR1212 - RI - #3939 - No longer * 12 as WarrantyLength now holds months than previously years
	FROM	warrantyband 
	WHERE   ItemID = @warrantyItemID									--IP - 21/06/11 - CR1212 - RI - #3939
	--WHERE	waritemno=@warrantyno

	IF @warrantylength = 0 OR @warrantylength is null
		IF @producttype ='M' -- mobile phones are always 2.
			--SET @warrantylength = 2
			SET @warrantylength = 24									--IP - 21/06/11 - CR1212 - RI - #3939 - Warrantylength now in months

	SELECT	@datedel = ISNULL(min(datedel),'1-jan-2000') 
	FROM	delivery 
	WHERE	acctno = @acctno 
	--AND		itemno = @warrantyno 
	AND		itemID = @warrantyItemID									--IP - 21/06/11 - CR1212 - RI - #3939
	AND		stocklocn = @stocklocn
	AND		delorcoll !='C' 
	AND		contractno = @contractno 
	AND		transvalue > 0

	SELECT @months = datediff(day,@datedel,getdate())/30.4

	SET @months = @months + 1

	IF @months = 0 
		SET @months = 1
	
	-- if expired last month used or replacement
	--IF (@months > 36 OR @replacement > 0) AND @warrantylength = 3 
	  IF (@months > 36 OR @replacement > 0) AND @warrantylength = 36	--IP - 21/06/11 - CR1212 - RI - #3939 - Warrantylength now in months
		SET @months = 36
	
	--IF (@months > 60 or @replacement > 0) AND @warrantylength = 5 
	IF (@months > 60 or @replacement > 0) AND @warrantylength = 60		--IP - 21/06/11 - CR1212 - RI - #3939 - Warrantylength now in months
		SET @months=60
	
	--IF (@months > 24 or @replacement > 0) AND @warrantylength = 2 
	IF (@months > 24 or @replacement > 0) AND @warrantylength = 24		--IP - 21/06/11 - CR1212 - RI - #3939 - Warrantylength now in months
		SET @months=24

	SELECT	@returnwarranty = ISNULL(returncode ,''),
			@refund = ISNULL(refundpercentfromAIG, 0),
			@warrantyRetCodeItemID = si.ID								--IP - 09/09/11 - RI - #8112
	FROM	WarrantyReturnCodes							
	LEFT JOIN stockinfo si ON si.iupc = WarrantyReturnCodes.returncode	--IP - 09/09/11 - RI - #8112
	AND si.repossesseditem = 0											--IP - 09/09/11 - RI - #8112
	WHERE	warrantymonths = @warrantylength 
	AND		producttype = @producttype 
	AND		MonthSinceDelivery = @months

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO
