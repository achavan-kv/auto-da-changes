/*
    Author:    John Croft
    Date:      April 2006

    This procedure retrieves the Payment file definitions from the Stordercontrol table
    The table is maintained in the Payment File Definition screen

*/

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_PaymentFileDefnSP]') 
        and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_PaymentFileDefnSP]
GO





CREATE PROCEDURE 	dbo.DN_PaymentFileDefnSP
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : DN_PaymentFileDefnSP
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : DN_PaymentFileDefnSP
-- Author       : John Croft
-- Date         : 2007
--
-- This procedure will retrieve all payment file definitions 
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 13/08/10 ip  CR1092 - COASTER to CoSACS Enhancements - Return new columns added.
-- 20/08/10 ip  CR1092 - COASTER to CoSACS Enhancements - Return new columns added for Batch file processing.
-- 25/08/10 ip  CR1092 - COASTER to CoSACS Enhancements - Return new columns added for a Delimited file.
-- 03/09/10 ip  CR1112 - Tallyman Interest Charges - Return new column added to identify the file as an interest file.
-- 03/09/10 ip  CR1092 - COASTER to CoSACS Enhancements - Return new columns added for Batch file processing.
-- ====================================================

						@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	select bankname,filename,acctnobegin,acctnolength,moneybegin,moneylength,
        moneypoint,headline,datebegin,datelength,dateformat,trailerbegin,
        trailerlength,paymentmethod,hastrailer,
        HeaderIdBegin, HeaderIdLength, HeaderId, TrailerIdBegin, TrailerIdLength, TrailerId,		--IP - 13/08/10 - CR1092 - COASTER to CoSACS Enhancements
        IsBatch, BatchHeaderIdBegin, BatchHeaderIdLength, BatchHeaderId, BatchHeaderHasTotal, BatchHeaderMoneyBegin, BatchHeaderMoneyLength,	--IP - 20/08/10 - CR1092 - COASTER to CoSACS Enhancements
        BatchTrailerIdBegin, BatchTrailerIdLength, BatchTrailerId,			--IP - 03/09/10 - CR1092
        IsDelimited, Delimiter, DelimitedNoOfCols,DelimitedAcctNoColNo,DelimitedDateColNo, DelimitedMoneyColNo,		--IP - 25/08/10 - CR1092 - COASTER to CoSACS Enhancements
        IsInterest	--IP - 03/09/10 - CR1112
    from stordercontrol


	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

