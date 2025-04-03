SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_AccountConvertRFToHPSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_AccountConvertRFToHPSP]
GO

CREATE PROCEDURE 	dbo.DN_AccountConvertRFToHPSP
			@acctno varchar(12),
			@custid varchar(20),
			@country varchar(2),
			@branch smallint,
			@dateprop datetime,
			@return int OUTPUT

AS
 SET NOCOUNT ON
	SET 	@return = 0			--initialise return code
	DECLARE	@chargeItemId int
	DECLARE @itemNo varchar(18)	
	DECLARE	@stock float
	DECLARE	@stockdamage float
	DECLARE	@itemdecr1 varchar(25)
	DECLARE	@itemdescr2 varchar(40)
	DECLARE	@suppliercode varchar(18)
	DECLARE	@unitprice float
	DECLARE	@cashprice float
	DECLARE	@CostPrice money
	DECLARE	@hpprice float
	DECLARE	@dutyfreeprice float
	DECLARE	@valuecontrolled int
	DECLARE	@kit int
	DECLARE	@isstock int
	DECLARE	@discount int
	DECLARE	@iswarranty int
	DECLARE	@taxrate float
	DECLARE	@isaffinity int
	DECLARE	@isStampDuty smallint
	DECLARE	@isFreeGift smallint
	DECLARE @qtyonorder float
	DECLARE @leadtime smallint
	DECLARE @delnotebranch smallint
	DECLARE @assemblyrequired char(1)
	DECLARE @productcategory VARCHAR(4)
	DECLARE @deleted char(1)
	DECLARE @itemCategory smallint
	DECLARE @sparePartsCategory VARCHAR(4)
	DECLARE @refcode VARCHAR(3)
	DECLARE @isInstallation bit

	UPDATE 	acct
	SET		accttype = 'O'
	WHERE	acctno = @acctno 

	/* remove any sundary lineitems from the account for the 'R' account type
	    and insert sundary charge lineitems for the 'H' type */

	DECLARE	rfcursor 	CURSOR FOR
	SELECT	ItemID
	FROM	sundchgtyp
	WHERE	accttype = 'R'

	OPEN		rfcursor
	
	FETCH NEXT FROM rfcursor 
	INTO	@chargeItemId

	WHILE @@fetch_status = 0
	BEGIN
		DELETE 
		FROM	lineitem 
		WHERE	acctno = @acctno 
		AND		itemId = @chargeItemId
		AND		stocklocn = @branch

		FETCH NEXT FROM rfcursor 
		INTO	@chargeItemId
	END

	CLOSE		rfcursor
	DEALLOCATE	rfcursor

	DECLARE hpcursor CURSOR FOR
	SELECT	ItemID
	FROM	sundchgtyp 
	WHERE	accttype = 'H'

	OPEN		hpcursor

	FETCH NEXT FROM hpcursor 
	INTO	@chargeItemId

	WHILE @@fetch_status = 0
	BEGIN
		EXEC DN_ItemGetDetailsSP 	
					    @itemNo = @itemNo OUT,
						@location = @branch,
						@branch = @branch,
						@accounttype = 'H',
						@country = @country,
						@dutyfree = 0,
						@taxExempt = 0,
						@agrmtno = 1,
						@acctno = @acctno,
						@stock = @stock OUT,
						@stockdamage = @stockdamage OUT,
						@itemdescr1 = @itemdecr1  OUT,
						@itemdescr2 = @itemdescr2 OUT,
						@suppliercode = @suppliercode OUT,
						@unitprice = @unitprice OUT,
						@cashprice = @cashprice OUT,
						@CostPrice = @CostPrice OUT,
						@hpprice = @hpprice OUT,
						@dutyfreeprice = @dutyfreeprice OUT,
						@valueControlled = @valuecontrolled OUT,
						@kit = @kit OUT,
						@isStock = @isstock OUT,
						@discount = @discount OUT,
						@isWarranty = @iswarranty OUT,
						@taxrate = @taxrate OUT,
						@isAffinity = @isaffinity OUT,
						@isStampDuty = @isStampDuty OUT,
						@isFreeGift = @isFreeGift OUT,
						@promobranch = 0,
						@isComponent = 0,
						@qtyonorder = @qtyonorder OUT,
						@leadtime = @leadtime OUT,
						@delnotebranch = @delnotebranch OUT,
						@assemblyrequired = @assemblyrequired OUT,
						@productcategory = @productcategory OUT,
						@deleted = @deleted OUT,
						@itemCategory = @itemCategory OUT,
						@sparePartsCategory = @sparePartsCategory OUT,
						@refcode = @refcode OUT, 
						@isInstallation = @isInstallation OUT,
						@itemId = @chargeItemId,
						@colourName = '',
						@style = '',
						@repoitem = 0,											@return = @return OUT
		INSERT
		INTO		lineitem
				(origbr, acctno, agrmtno, itemno, itemsupptext, quantity, 
				delqty, stocklocn, price, ordval, datereqdel, timereqdel, 
				dateplandel, delnotebranch, qtydiff, itemtype, --hasstring, --IP - 09/03/11 - Removed hasstring
				notes, taxamt, parentitemno, parentlocation, iskit, 
				deliveryaddress, itemId, parentItemId)
		VALUES	(0, @acctno, 1, @itemNo, '', 1,
				0, @branch, @unitprice, @unitprice, null, '', 
				null, null, 'Y', 'N', --'', --IP - 09/03/11 - Removed hasstring
				 '', 0, '', '', 0, 
				 '', @chargeItemId, 0)	
		
		FETCH NEXT FROM hpcursor 
		INTO	@chargeItemId				

	END

	CLOSE		hpcursor
	DEALLOCATE	hpcursor
	
	UPDATE	proposal
	SET		ConvertedFromRF = 'Y'
	WHERE	custid = @custid
	AND		acctno = @acctno
	AND		dateprop = @dateprop

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

