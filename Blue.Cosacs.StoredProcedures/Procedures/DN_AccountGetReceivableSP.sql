SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_AccountGetReceivableSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_AccountGetReceivableSP]
GO

CREATE PROCEDURE 	dbo.DN_AccountGetReceivableSP
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : DN_AccountGetReceivableSP.prc
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Get Account Receivable
-- Author       : ??
-- Date         : ??
--
-- This procedure will get the Receivable account for Shortages
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 08/07/08  jec UAT473 Due to rule change(sometime) firstname must not be blank when creating New customer record. 
-- ================================================
	-- Add the parameters for the stored procedure here
			@empeeno int,
			@acctno varchar(12) OUT,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code
	DECLARE	@empname varchar(20)

	IF EXISTS 	(SELECT 	1
			FROM 		customer 
			WHERE	custid = 'SHORTAGE' + convert ( varchar, @empeeno ) )
	BEGIN
		SELECT	@acctno = CA.acctno 
		FROM		custacct CA  
		INNER JOIN	acct A
		ON		CA.acctno = A.acctno
		WHERE	CA.custid = 'SHORTAGE' + convert ( varchar, @empeeno ) 
		AND		A.currstatus != 'S'
	END
	ELSE		/* if the customer doesn't exist creat a customer record */
	BEGIN

		INSERT 
		INTO 		customer 
				(origbr, custid, otherid, branchnohdle, name, 
				firstname, title, alias, addrsort, namesort, 
				dateborn, sex, ethnicity, morerewardsno, 
				effectivedate, iDNumber, IdType, RFCreditLimit, 
				RFCardSeqNo, RFCardPrinted, creditblocked, 
				datelastscored, RFDateReminded, empeenochange, 
				datechange, maidenname)
		SELECT 	null, 'SHORTAGE' + convert ( varchar, @empeeno ),'',0,
				N'SHORTAGE' + fullname, 'Short','MR','','','SHORTAGE',		-- jec UAT473 
				dateadd(year, -20, getdate()),'M','X','',NULL,'','',0,0,'N',0,NULL,
				NULL,0,NULL,NULL
		FROM Admin.[User] 
		WHERE id = @empeeno

		/* we still need to create the shortage account and tie it to the new customer 
		    but this will be done in the BL layer to make use of existing code */

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

