SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_FintransGetWarrantyAdjustmentSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_FintransGetWarrantyAdjustmentSP]
GO

CREATE PROCEDURE 	dbo.DN_FintransGetWarrantyAdjustmentSP
			@acctno varchar(12),
			@amount money OUT,
			@return int OUTPUT

AS
	SET 	@return = 0

	SELECT 	@amount =  isnull(sum (transvalue),0) 
	FROM 		fintrans 
	WHERE 	acctno = @acctno
	AND 		transtypecode IN ('CRF','CRE')

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

