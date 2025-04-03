SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_CodeGetAllSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_CodeGetAllSP]
GO

CREATE PROCEDURE [dbo].[DN_CodeGetAllSP] @return int OUTPUT
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : DN_CodeGetAllSP.prc
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : 
-- Date         : ??
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 07/12/11  IP  CR1234 - Retrieve column Additional2
-- ================================================
AS
	SELECT @return = 0
	
	--IP - 30/10/09 - CoSACS Improvement - Code Maintenance
	--Create table to store the max lengths for each column from the Code table for each category which will be used
	--to set the column widths on the Code Maintenance screen.
	DECLARE @CatLengths TABLE
	(
		Category varchar(12),
		CodeLength int,
		DescriptionLength int,
		SortOrderLength int,
		ReferenceLength int,
		AdditionalLength int,
		Additional2Length int,			--IP - 07/12/11 - CR1234
		MmiApplicable BIT
	)
	
	INSERT into @CatLengths (Category, CodeLength, DescriptionLength, SortOrderLength, ReferenceLength, AdditionalLength, Additional2Length, MmiApplicable)	--IP - 07/12/11 - CR1234
	SELECT DISTINCT c.category,
	MAX(LEN(ISNULL(c.code,0))),
	MAX(LEN(ISNULL(c.codedescript,0))),
	MAX(LEN(ISNULL(c.sortorder,0))),
	MAX(LEN(ISNULL(c.reference,0))),
	MAX(LEN(ISNULL(c.additional,0))),
	MAX(LEN(ISNULL(c.Additional2,0))),	--IP - 07/12/11 - CR1234
	1
	FROM	code c 
	GROUP BY c.category
	
	SELECT c.category, 
		   c.code, 
		   c.codedescript, 
		   c.sortorder, 
		   c.category as oldcategory, 
		   c.code as oldcode, 
		   c.reference,
		   c.additional,
		   c.Additional2,				--IP - 07/12/11 - CR1234
		   ISNULL(CO.IsMmiApplicable, 0) AS [MmiApplicable]
	FROM	code c 
			LEFT OUTER JOIN CodeConfiguration AS CO
			ON c.category = CO.Category
			AND	c.code = CO.Code
	WHERE	c.statusflag = 'L'
	ORDER BY c.category, c.code


	SELECT @return = @@error

	IF (@return = 0)	
	BEGIN
	
	--IP - 30/10/09 - CoSACS Improvement - Code Maintenance 
	--Return the HeaderTexts for each column which will be used to dynamically change the headers on the Code Maintenance screen
	--depending on the category selected.
	SELECT cc.category, 
		   cc.catdescript,
		   cc.CodeHeaderText,
		   cc.DescriptionHeaderText,
		   cc.SortOrderHeaderText,	
		   cc.ReferenceHeaderText,
		   cc.AdditionalHeaderText,
		   cc.ToolTipText,
		   cc.Additional2HeaderText,			--IP - 07/12/11 - CR1234
		   CASE WHEN cl.CodeLength > LEN(ISNULL(cc.CodeHeaderText, 'Code')) THEN cl.CodeLength ELSE LEN(ISNULL(cc.CodeHeaderText, 'Code'))END AS CodeLength,
		   CASE WHEN cl.DescriptionLength > LEN(ISNULL(cc.DescriptionHeaderText, 'Description')) THEN cl.DescriptionLength ELSE LEN(ISNULL(cc.DescriptionHeaderText, 'Description'))END AS DescriptionLength,
		   CASE WHEN cl.SortOrderLength > LEN(ISNULL(cc.SortOrderHeaderText, 'Sort Order')) THEN cl.SortOrderLength ELSE LEN(ISNULL(cc.SortOrderHeaderText, 'Sort Order'))END AS SortOrderLength,
		   CASE WHEN cl.ReferenceLength > LEN(ISNULL(cc.ReferenceHeaderText, 'Reference')) THEN cl.ReferenceLength ELSE LEN(ISNULL(cc.ReferenceHeaderText, 'Reference'))END AS ReferenceLength,
		   CASE WHEN cl.AdditionalLength> LEN(ISNULL(cc.AdditionalHeaderText, 'Additional')) THEN cl.AdditionalLength ELSE LEN(ISNULL(cc.AdditionalHeaderText, 'Additional'))END AS AdditionalLength,
		   CASE WHEN cl.Additional2Length> LEN(ISNULL(cc.Additional2HeaderText, 'Additional2')) THEN cl.Additional2Length ELSE LEN(ISNULL(cc.Additional2HeaderText, 'Additional2'))END AS Additional2Length,		--IP - 07/12/11 - CR1234
		   'MMI Applicable' AS MMIApplicableText,
		   LEN('MMI Applicable') AS MMIApplicableLength
		FROM codecat cc
		LEFT JOIN @CatLengths cl ON cc.category = cl.Category --IP - 22/12/09 - UAT(946)
		WHERE cc.usermaint = 'Y'
		ORDER BY cc.category
		

		SELECT @return = @@error
	END

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

