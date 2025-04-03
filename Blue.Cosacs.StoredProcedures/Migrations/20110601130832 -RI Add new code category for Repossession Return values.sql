-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here


if not exists(select * from codecat where category ='RPV')
	insert into dbo.codecat (
		origbr,
		category,
		catdescript,
		codelgth,
		forcenum,
		forcenumdesc,
		usermaint,
		CodeHeaderText,
		DescriptionHeaderText,
		SortOrderHeaderText,
		ReferenceHeaderText,
		AdditionalHeaderText,
		ToolTipText
	) VALUES ( 
		/* origbr - smallint */ 0,
		/* category - varchar(12) */ 'RPV',
		/* catdescript - nvarchar(64) */ N'Repossession Return Values',
		/* codelgth - int */ 5,
		/* forcenum - char(1) */ 'Y',
		/* forcenumdesc - char(1) */ 'N',
		/* usermaint - char(1) */ 'Y',
		/* CodeHeaderText - varchar(30) */ 'Product Category',
		/* DescriptionHeaderText - varchar(30) */ 'Description',
		/* SortOrderHeaderText - varchar(30) */ '',
		/* ReferenceHeaderText - varchar(30) */ 'Repo Cost Price %',
		/* AdditionalHeaderText - varchar(30) */ 'Repo Selling Price %',
		/* ToolTipText - varchar(300) */ 'Enter % of original CP for Repo Cost Price % and % uplift on Repo Cost Price for Repo Selling Price %' ) 
		

