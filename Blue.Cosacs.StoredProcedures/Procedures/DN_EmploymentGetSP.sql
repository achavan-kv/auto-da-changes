SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_EmploymentGetSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_EmploymentGetSP]
GO

--[27-11-2006] CR 866 Added additional fields for CR 866 [PC]
--[4-1-2010] CR 1066 Added additional address fields [FA]

CREATE PROCEDURE 	[dbo].[DN_EmploymentGetSP]
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : DN_EmploymentGetSP.prc
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Get Employment Details
-- Author       : ??
-- Date         : ??
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 07/04/10  jec UAT16 - Allow staffno 20 characters
-- 19/04/10  jec UAT16 - staffno varchar
-- ================================================
	-- Add the parameters for the stored procedure here
			@custid varchar(20),
			@origbr smallint OUT,
			@dateemployed datetime OUT,
			@empyrno varchar(6) OUT,
			@worktype varchar(2) OUT,
			@empmtstatus char(1) OUT,
			@fullorpart char(1) OUT,
			@temporperm char(1) OUT,
			@custempeeno varchar(12) OUT,
			@payfreq char(1) OUT, 
			@annualgross float OUT,
			@dateleft datetime OUT,
			@persdialcode char(8) OUT,
			@perstel char(13) OUT,		
			@pdateemployed datetime OUT,
			@pdateleft datetime OUT,
			@staffno varchar(20) OUT,			-- UAT16 jec
			@jobtitle Tjobtitle OUT,
			@employer varchar(20) OUT,
			@department varchar(20) OUT,
			@occupation varchar(20) OUT, -- CR 866
			@industry Tindustry OUT,   -- CR 866
			@organisation Torganisation OUT,  --CR 866
			@educationLevel TeducationLevel OUT, --CR 866 
			@employeradd1 varchar(26) OUT, --CR1066
			@employeradd2 varchar(26) OUT, --CR1066
			@employeradd3 varchar(26) OUT, --CR1066
			@employerpocode varchar(26) OUT, --CR1066
			@return int OUTPUT
AS

	SET 	@return = 0			--initialise return code

	SELECT	TOP 1
			@origbr 	=	origbr,
			@dateemployed = 	dateemployed,
			@empyrno	=	empyrno,
			@worktype	=	worktype,
			@empmtstatus	=	empmtstatus,
			@fullorpart 	=	fullorpart,
			@temporperm	=	temporperm,
			@custempeeno	=	custempeeno,
			@payfreq	=	payfreq,
			@annualgross	=	annualgross,
			@dateleft	=	dateleft,
			@persdialcode	= 	PersDialCode,
			@perstel	=	PersTel,
			@staffno	=	StaffNo,
			@department	=	department,
			@jobtitle = jobtitle, -- CR 866
			@industry	= industry,   -- CR 866
			@organisation = organisation,  --CR 866
			@educationLevel = educationLevel  --CR 866
	FROM		employment
	WHERE	custid = @custid
	AND		dateleft is null
	ORDER BY	dateemployed DESC

	SELECT	TOP 1
			@pdateemployed = dateemployed,
			@pdateleft = dateleft
	FROM		employment
	WHERE	custid = @custid
	AND		dateleft is not null
	AND     dateemployed <= @dateemployed  -- Must not be after current employment
	ORDER BY	dateemployed DESC

	SELECT TOP 1 	@occupation = codedescript --CR 866
	FROM 		code
	WHERE 	category LIKE 'WT%'
	AND 		code = @worktype

	SELECT 	@employer = empyrname, @employeradd1 = empyraddr1, @employeradd2 = empyraddr2, 
	@employeradd3 = empyraddr3, @employerpocode=empyrpocode
		FROM 		employer
	WHERE 	empyrno = @empyrno

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