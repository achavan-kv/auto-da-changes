SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_SaveWarrantyRenewalSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_SaveWarrantyRenewalSP]
GO

CREATE PROCEDURE 	dbo.DN_SaveWarrantyRenewalSP
			@acctno varchar(12),
			@origacctno varchar(12),
			@contractno varchar(10),
			@warrantyId int,
			@location smallint,
			@origcontractno varchar(10),
			@origlocation smallint,
			@user int,	
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	UPDATE	WarrantyRenewalPurchase
	SET	stockitemacctno = @origacctno, 
		contractno= @contractno, 
		itemno = '',
		ItemID = @warrantyId,
		stocklocn = @location, 
		originalcontractno = @origcontractno, 
		originalstocklocn = @origlocation
	WHERE	acctno =  @acctno

	IF(@@rowcount = 0)
	BEGIN
		INSERT INTO WarrantyRenewalPurchase
			(acctno, stockitemacctno, contractno, itemno, ItemID,
			  stocklocn, originalcontractno, originalstocklocn, addedby,
			  dateadded,datedelivered)
		VALUES(@acctno, @origacctno, @contractno, '', @warrantyId, 
				@location, @origcontractno, @origlocation, @user, 
				GETDATE(), null)
	END
	
	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO
