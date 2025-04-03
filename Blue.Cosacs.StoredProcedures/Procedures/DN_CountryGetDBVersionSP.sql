SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_CountryGetDBVersionSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_CountryGetDBVersionSP]
GO

CREATE PROCEDURE  dbo.DN_CountryGetDBVersionSP
   			@dbversion varchar(20) OUTPUT,
   			@return int OUTPUT
 
AS
 
 	SET  @return = 0   --initialise return code
 	
 	-- 68293 RD 27/06/06 Modified to get correct database version AA 15 Sep 2010 Changing to get correct version just exclude the Netv...
    SELECT	@dbversion = RIGHT(REPLACE(version,'_','.' ),DATALENGTH(version)-4) 
   	FROM 	dbversion 
	WHERE 	version LIKE 'netv%' 
	AND 	version NOT LIKE '%rpt'
	AND		upgrade_date = 
				(SELECT	MAX(upgrade_date)
				 FROM dbversion
				 WHERE version LIKE 'netv%' 
				 AND version NOT LIKE '%rpt')


 	IF (@@error != 0)
 	BEGIN
  		SET @return = @@error
 	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

