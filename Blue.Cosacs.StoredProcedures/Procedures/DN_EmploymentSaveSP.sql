SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_EmploymentSaveSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_EmploymentSaveSP]
GO


--[29-11-2006] //CR 866 Added additional fields for CR 866 [PC]
CREATE PROCEDURE 	dbo.DN_EmploymentSaveSP

-- ================================================
-- Project      : CoSACS .NET
-- File Name    : DN_EmploymentSaveSP.prc
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Save Employment Details
-- Author       : ??
-- Date         : ??
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 13/07/08  jec CR907 - update date changed
-- 07/04/10  jec UAT16 - Allow staffno 20 characters
-- ================================================
	-- Add the parameters for the stored procedure here

			@custid varchar(20),
			@dateemployed datetime ,
			@empyrno varchar(6) ,
			@worktype varchar(2) ,
			@empmtstatus char(1) ,
			@fullorpart char(1) ,
			@temporperm char(1) ,
			@custempeeno varchar(12) ,
			@payfreq char(1) , 
			@annualgross float ,
			@dateleft datetime ,
			@persdialcode char(8) ,
			@perstel char(13) ,		
			@pdateemployed datetime ,
			@pdateleft datetime ,
			@staffno char(20),			-- UAT16 jec
			@department varchar(20),
			@jobtitle Tjobtitle ,		--CR 866 
			@industry Tindustry,			--CR 866
			@organisation Torganisation ,	--CR 866
			@educationLevel TeducationLevel,	--CR 866 
			@return int OUTPUT
AS

	SET 	@return = 0			--initialise return code

	DECLARE 	@pworktype varchar(2) 
	DECLARE 	@pempmtstatus char(1) 
	DECLARE 	@pfullorpart char(1) 
	DECLARE 	@ptemporperm char(1) 
	DECLARE 	@pcustempeeno varchar(12) 
	DECLARE 	@ppayfreq char(1) 
	DECLARE 	@pannualgross float 
	DECLARE 	@ppersdialcode char(8) 
	DECLARE 	@pperstel char(13) 	
	DECLARE 	@pstaffno char(20)			-- UAT16 jec
	DECLARE 	@pdepartment varchar(20)	--CR 866
	DECLARE 	@pjobtitle Tjobtitle		--CR 866
	DECLARE 	@pindustry Tindustry		--CR 866
	DECLARE 	@porganisation Torganisation --CR 866
	DECLARE		@peducationLevel TeducationLevel --CR 866
	Declare		@datechange datetime		--CR907 jec
	Declare		@Pdatechanged datetime		--CR907 jec
	Declare		@Cdatechanged datetime		--CR907 jec
	Declare		@Cdateemployed datetime		--CR907 jec
	DECLARE 	@Cworktype varchar(2)		--CR907 jec
	DECLARE 	@Cempmtstatus char(1)		--CR907 jec
	DECLARE 	@Cpayfreq char(1)			--CR907 jec
	
	--store the details of the previous employment record
	SELECT	TOP 1
			@pworktype = worktype,
			@pempmtstatus = empmtstatus,
			@pfullorpart = fullorpart,
			@ptemporperm = temporperm,
			@pcustempeeno = custempeeno,
			@ppayfreq = payfreq,
			@pannualgross = annualgross,
			@ppersdialcode = persdialcode,
			@pperstel = perstel,
			@pstaffno = staffno,
			@pdepartment = department,				--CR 866
			@pjobtitle = jobtitle,					--CR 866
			@pindustry = industry,					--CR 866
			@porganisation = organisation,			--CR 866
			@peducationLevel = educationLevel,		--CR 866
			@pdatechanged = datechanged				--CR907 jec			

	FROM		employment
	WHERE	custid = @custid
	AND		dateleft is not null
	ORDER BY	dateemployed DESC

	--store the details of the current employment record	CR907 jec 
	SELECT	TOP 1
			@Cworktype = worktype,
			@Cdateemployed = dateemployed, 		
			@Cempmtstatus = empmtstatus,			
			@Cpayfreq = payfreq,
			@Cdatechanged = datechanged 					

	FROM		employment
	WHERE	custid = @custid
	AND		dateleft is null


	--delete all non-current records for this customer
	DELETE	
	FROM		employment
	WHERE	custid = @custid

	-- if none of the following have changed;
	-- (date of employment, employment status, occupation, or pay frequency )
	-- the datechange will be set back to the previous datechange 

			if	@Cdateemployed    = @dateemployed				
                and @Cempmtstatus = @empmtstatus
                and @Cworktype = @worktype
                and @Cpayfreq  = @payfreq					
			
				set @datechange = @Cdatechanged
			else
				set @datechange = getdate()

	INSERT 
	INTO		employment
			(origbr,	custid,	dateemployed, empyrno, worktype, 
			empmtstatus, fullorpart, temporperm, custempeeno, 
			payfreq, annualgross, dateleft, persdialcode, perstel,staffno, department, 
			jobtitle, industry, organisation, educationlevel, datechanged) --CR 866	/ CR907 jec
	VALUES	(0, @custid, @dateemployed, @empyrno, @worktype,
			@empmtstatus, @fullorpart, @temporperm, @custempeeno,
			@payfreq, @annualgross, null, @persdialcode, @perstel,@staffno, @department,
			@jobtitle, @industry, @organisation, @educationlevel, @datechange) --CR 866 / CR907 jec	

    -- Only record previous employment if it is not after the current employment
	IF(ISNULL(@pdateemployed,'') <= @dateemployed AND @pdateemployed IS NOT NULL)
	BEGIN
		INSERT 
		INTO		employment
				(origbr,	custid,	dateemployed, empyrno, worktype, 
				empmtstatus, fullorpart, temporperm, custempeeno, 
				payfreq, annualgross, dateleft, persdialcode, perstel, staffno , department, 
				jobtitle, industry, organisation, educationlevel, datechanged) --CR 866 / CR907 jec
		
		VALUES	(0, @custid, @pdateemployed, '', isnull(@pworktype, ''),
				isnull(@pempmtstatus,''), isnull(@pfullorpart, ''), isnull(@ptemporperm,''), @pcustempeeno,
				isnull(@ppayfreq, ''), @pannualgross, isnull(@pdateleft, @dateemployed), isnull(@ppersdialcode, ''),
				isnull(@pperstel, ''), isnull(@pstaffno, ''),  isnull(@pdepartment, ''),
				isnull(@pjobtitle, ''), isnull(@pindustry, ''),  isnull(@porganisation, ''),
				isnull(@peducationLevel, ''), @Pdatechanged)	--CR 866  /CR907 jec @Pdatechanged
	END

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

-- End End End End End End End End End End End End End End End End End End End End End End End End End End