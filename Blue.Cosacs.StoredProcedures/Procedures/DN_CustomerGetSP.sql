SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_CustomerGetSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_CustomerGetSP]
GO

CREATE PROCEDURE 	dbo.DN_CustomerGetSP
			@custid varchar (20),
			@origbr smallint OUT,
			@otherid varchar(15) OUT,
			@branchnohdle smallint OUT,
			@name varchar(60) OUT,
			@firstname varchar(30) OUT,
			@title varchar(25) OUT,
			@alias varchar(25) OUT,
			@addrsort varchar(20) OUT,
			@namesort varchar(20) OUT,
			@datebornstr varchar(10) OUT,
			@sex varchar(1) OUT,
			@ethnicity varchar(1) OUT,
			@morerewardsno varchar(16) OUT,
			@effectivedate datetime OUT,
			@idtype varchar(4) OUT,
			@idnumber varchar(30) OUT,
			@dateborn datetime OUT,
			@maidenname varchar(30) OUT,
			@dependants int = null OUT ,
			@maritalStat char(1) = null OUT  ,
			@nationality char(4) = null OUT ,
			@storetype char (2) OUT, -- 69395 SC 03/12/07
			@scoringband VARCHAR(1) OUT ,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	SELECT 	@origbr = origbr,
			@custid = custid,
			@otherid = otherid,
			@branchnohdle = branchnohdle,
			@name = name,
			@firstname = firstname,
			@title = title,
			@alias = alias,
			@addrsort = addrsort,
			@namesort = namesort,
			@datebornstr =  dateborn,
--			@datebornstr = CONVERT(varchar(10), dateborn, 105),
			@sex = sex,
			@ethnicity = ethnicity,
			@morerewardsno = morerewardsno,
			@effectivedate = effectivedate,
			@idtype = Idtype,
			@idnumber = Idnumber,
			@dateborn = dateborn,
			@maidenname = maidenname,
			@dependants = dependants,	--Added for CR 835 [PC]
			@maritalstat = maritalstat, --Added for CR 835 [PC]
			@nationality = nationality,	--Added for CR 835 [PC]
			@storetype = storetype,	   -- 69395 SC 03/12/07
			@scoringband = ScoringBand
	FROM		customer
	WHERE	custid = @custid

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

	
	
	
	
	
