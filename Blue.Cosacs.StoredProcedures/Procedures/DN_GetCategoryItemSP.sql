GO
/****** Object:  StoredProcedure [dbo].[DN_GetCategoryItemSP]    Script Date: 07/03/2008 12:30:34 ******/
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID('[dbo].[DN_GetCategoryItemSP]') AND OBJECTPROPERTY(id,'IsProcedure') = 1)
DROP PROCEDURE [dbo].[DN_GetCategoryItemSP]
Go
CREATE PROC DN_GetCategoryItemSP
@code VARCHAR(12),
@return INT OUT
AS
	SET @return = 0
	SELECT * FROM code WHERE category in ('PCE','PCW','PCF')
	AND code = @code
	Go