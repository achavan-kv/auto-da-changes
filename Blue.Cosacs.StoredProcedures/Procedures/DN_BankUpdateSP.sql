
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_BankUpdateSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_BankUpdateSP]
GO

CREATE PROCEDURE [DN_BankUpdateSP] 
(
	@bankcode VARCHAR(6), 
	@bankname VARCHAR(20),
	@bankaddr1 VARCHAR(26),
	@bankaddr2 VARCHAR(26),
	@bankaddr3 VARCHAR(26),
	@bankpocode VARCHAR(10),	
	@return int output
)
AS

	IF EXISTS (SELECT 1 FROM [bank] WHERE [bankcode] = @bankcode)
	BEGIN
		UPDATE [bank]
		SET [bankname]=@bankname,
		    [bankaddr1]=@bankaddr1,
	            [bankaddr2]=@bankaddr2,
	            [bankaddr3]=@bankaddr3,
	            [bankpocode]=@bankpocode
	            
		WHERE [bankcode] = @bankcode
	END
	ELSE
	BEGIN
		INSERT INTO [bank] ([bankcode], [bankname], [bankaddr1],[bankaddr2], [bankaddr3], [bankpocode])
		VALUES (@bankcode, @bankname, @bankaddr1, @bankaddr2, @bankaddr3, @bankpocode)
	END
	
/*
	IF @@error = 0
	BEGIN
		IF EXISTS (SELECT 1 FROM [branchregion] WHERE [branchno] = @branchno)
		BEGIN
			UPDATE [branchregion]
			SET [region]= @warehouseregion
			WHERE [branchno] = @branchno
		END
		ELSE
		BEGIN
			INSERT INTO [branchregion] ([branchno], [region])
			VALUES (@branchno, @warehouseregion)
		END
	END	
*/

	SELECT @return = @@error
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO
