
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_BankDeleteSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_BankDeleteSP]
GO

CREATE PROCEDURE [DN_BankDeleteSP] 
(
	@bankcode VARCHAR(6), 	
	@return int output
)
AS

	DELETE FROM [bank]
	WHERE [bankcode] = @bankcode

	SELECT @return = @@ERROR

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO
