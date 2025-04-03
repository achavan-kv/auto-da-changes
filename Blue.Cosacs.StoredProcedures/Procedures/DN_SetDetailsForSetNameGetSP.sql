SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_SetDetailsForSetNameGetSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_SetDetailsForSetNameGetSP]
GO

CREATE PROCEDURE 	dbo.DN_SetDetailsForSetNameGetSP
			@setname varchar(64),
			@tname varchar(24),
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	DECLARE @setdetails table
	(
	    setname varchar(128),
	    data varchar(64),
	    tname varchar(64),
	    codedescript varchar(64)
	)		

    INSERT INTO @setdetails
    (
        setname,
        data,
        tname,
        codedescript
    )
	SELECT  setname,
	        data,
	        tname,
	        ''
	FROM 	SetDetails 
	WHERE 	setname = @setname
	AND     tname = @tname
	ORDER BY data ASC

    IF @tname = 'ProductCategories' -- getting nicer descriptions for product categories
    BEGIN
        UPDATE  t 
        SET     setname = c.codedescript
        FROM    @setdetails t, code c
        WHERE   t.data = c.code 
        AND     c.category in ('PCE','PCW','PCF')
    END          

    IF @tname = 'Items' 
    BEGIN
        UPDATE  @setdetails 
        SET     setname = 'Invalid product'
        
        UPDATE  t 
        SET     setname =isnull(c.itemdescr1 + ' ' + isnull(itemdescr2,''),'No item found')
        FROM    @setdetails t, stockitem c
        WHERE   t.data = c.itemno
    END
    
    IF @tname = 'deliveryarea'
    BEGIN
        UPDATE  t 
        SET     codedescript = c.codedescript
        FROM    @setdetails t, code c
        WHERE   t.data = c.code 
        AND     c.category = 'DDY'
    END          

    SELECT  data as code, 
            setname as [Description], 
            tname,
            codedescript
    FROM    @setdetails
	
	SELECT b.BranchNo,
	       CASE WHEN sb.BranchNo IS NULL THEN 0 ELSE 1 END AS Selected
	FROM   Branch b
	LEFT OUTER JOIN SetByBranch sb
	ON   sb.BranchNo = b.BranchNo
	AND  sb.tname = @tname
	AND  sb.setname = @setname

	SET @return = @@error
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

