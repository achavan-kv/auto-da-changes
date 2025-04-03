
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_BankGetSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_BankGetSP]
GO

CREATE PROCEDURE DN_BankGetSP  
			@return INT OUTPUT AS

	SELECT	bankcode,
			bankname,
			bankaddr1,
		       	ISNULL(bankaddr2,'') as 'bankaddr2',
			ISNULL(bankaddr3,'') as 'bankaddr3',
			ISNULL(bankpocode,'') as 'bankpocode'
	FROM 		bank

	SELECT @return = @@error
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

