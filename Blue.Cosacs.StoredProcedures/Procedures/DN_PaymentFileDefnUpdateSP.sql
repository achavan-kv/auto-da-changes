/*
    Author:    John Croft
    Date:      April 2006

    This procedure updates the Payment file definitions
    The table is maintained in the Payment File Definition screen

*/

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_PaymentFileDefnUpdateSP]') 
        and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_PaymentFileDefnUpdateSP]
GO



CREATE PROCEDURE 	dbo.DN_PaymentFileDefnUpdateSP
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : DN_PaymentFileDefnUpdateSP
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : DN_PaymentFileDefnUpdateSP
-- Author       : John Croft
-- Date         : 2007
--
-- This procedure will save a new or update an existing
-- payment file definition for a bank
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 12/08/10 ip  CR1092 - COASTER to CoSACS Enhancements
-- 19/08/10 ip  CR1092 - COASTER to CoSACS - Batch File processing
-- 20/08/10 ip  CR1092 - COASTER to CoSACS - filename will now store the file extension e.g. (txt)
-- 24/08/10 ip  CR1092 - COASTER to CoSACS - Added extra fields for processing a delimited file.
-- 03/09/10 ip  CR1112 - Tallyman Interest Charges - added field to identify file as interest file.
-- 03/09/10 ip  CR1092 - COASTER to CoSACS Enhancements - added fields for batch processing 
-- ====================================================

    @bankname varchar(16),
    @fileExt varchar(32),
    @acctnobegin smallint,
    @acctnolength smallint,
    @moneybegin smallint,
    @moneylength smallint,
    @moneypoint smallint,
    @headline smallint,
    @datebegin smallint,
    @datelength smallint,
    @dateformat varchar(16),
    @trailerbegin smallint,
    @trailerlength smallint,
    @paymentmethod smallint,
    @hastrailer char(1),
    @headerIdBegin smallint,			--IP - 12/08/10 - CR1092 - COASTER to CoSACS Enhancements
    @headerIdLength smallint,			--IP - 12/08/10 - CR1092 - COASTER to CoSACS Enhancements
    @headerId varchar(20),				--IP - 12/08/10 - CR1092 - COASTER to CoSACS Enhancements	
    @trailerIdBegin smallint,			--IP - 12/08/10 - CR1092 - COASTER to CoSACS Enhancements	
    @trailerIdLength smallint,			--IP - 12/08/10 - CR1092 - COASTER to CoSACS Enhancements	
    @trailerId varchar(20),				--IP - 12/08/10 - CR1092 - COASTER to CoSACS Enhancements	
    @isBatch bit,						--IP - 19/08/10 - CR1092 - COASTER to CoSACS Enhancements	
    @batchHeaderIdBegin smallint,		--IP - 19/08/10 - CR1092 - COASTER to CoSACS Enhancements
    @batchHeaderIdLength smallint,		--IP - 19/08/10 - CR1092 - COASTER to CoSACS Enhancements
    @batchHeaderId varchar(20),			--IP - 19/08/10 - CR1092 - COASTER to CoSACS Enhancements
    @batchHeaderHasTotal bit,			--IP - 19/08/10 - CR1092 - COASTER to CoSACS Enhancements
    @batchHeaderMoneyBegin smallint,	--IP - 19/08/10 - CR1092 - COASTER to CoSACS Enhancements
    @batchHeaderMoneyLength smallint,	--IP - 19/08/10 - CR1092 - COASTER to CoSACS Enhancements	
    @batchTrailerIdBegin smallint,		--IP - 03/09/10 - CR1092 - COASTER to CoSACS Enhancements
    @batchTrailerIdLength smallint,		--IP - 03/09/10 - CR1092 - COASTER to CoSACS Enhancements
    @batchTrailerId varchar(20),		--IP - 03/09/10 - CR1092 - COASTER to CoSACS Enhancements
    @isDelimited bit,					--IP - 24/08/10 - CR1092 - COASTER to CoSACS Enhancements
    @delimiter varchar(15),				--IP - 24/08/10 - CR1092 - COASTER to CoSACS Enhancements
    @delimitedNoOfCols int,				--IP - 24/08/10 - CR1092 - COASTER to CoSACS Enhancements
    @delimitedAcctNoColNo varchar(10),	--IP - 24/08/10 - CR1092 - COASTER to CoSACS Enhancements
    @delimitedDateColNo varchar(10),	--IP - 24/08/10 - CR1092 - COASTER to CoSACS Enhancements
    @delimitedMoneyColNo varchar(10),	--IP - 24/08/10 - CR1092 - COASTER to CoSACS Enhancements	
    @isInterest bit,					--IP - 03/09/10 - CR1092 - COASTER to CoSACS Enhancements			
    @return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	if exists (select bankname from stordercontrol where bankname=@bankname)
        Begin
            Update stordercontrol
                set [filename]=@fileExt,
                    acctnobegin=@acctnobegin,
                    acctnolength=@acctnolength,
                    moneybegin=@moneybegin,
                    moneylength=@moneylength,
                    moneypoint=@moneypoint,
                    headline=@headline,
                    datebegin=@datebegin,
                    datelength=@datelength,
                    [dateformat]=@dateformat,
                    trailerbegin=@trailerbegin,
                    trailerlength=@trailerlength,
                    paymentmethod=@paymentmethod,
                    hastrailer=@hastrailer,
                    HeaderIdBegin = @headerIdBegin,						--IP - 12/08/10 - CR1092 
                    HeaderIdLength = @headerIdLength,					--IP - 12/08/10 - CR1092 
                    HeaderId = @headerId,								--IP - 12/08/10 - CR1092 
                    TrailerIdBegin = @trailerIdBegin,					--IP - 12/08/10 - CR1092 
                    TrailerIdLength = @trailerIdLength,					--IP - 12/08/10 - CR1092 
                    TrailerId = @trailerId,								--IP - 12/08/10 - CR1092 
                    IsBatch = @isBatch,									--IP - 19/08/10 - CR1092 
                    BatchHeaderIdBegin = @batchHeaderIdBegin,			--IP - 19/08/10 - CR1092 
                    BatchHeaderIdLength = @batchHeaderIdLength,			--IP - 19/08/10 - CR1092 
                    BatchHeaderId = @batchHeaderId,						--IP - 19/08/10 - CR1092 
                    BatchHeaderHasTotal = @batchHeaderHasTotal,			--IP - 19/08/10 - CR1092 
                    BatchHeaderMoneyBegin = @batchHeaderMoneyBegin,		--IP - 19/08/10 - CR1092 
                    BatchHeaderMoneyLength = @batchHeaderMoneyLength,	--IP - 19/08/10 - CR1092 
                    BatchTrailerIdBegin = @batchTrailerIdBegin,			--IP - 03/09/10 - CR1092 
                    BatchTrailerIdLength = @batchTrailerIdLength,		--IP - 03/09/10 - CR1092
                    BatchTrailerId = @batchTrailerId,					--IP - 03/09/10 - CR1092
                    IsDelimited = @isDelimited,							--IP - 24/08/10 - CR1092 
					Delimiter = @delimiter,								--IP - 24/08/10 - CR1092 
					DelimitedNoOfCols = @delimitedNoOfCols,				--IP - 24/08/10 - CR1092 
					DelimitedAcctNoColNo = @delimitedAcctNoColNo,		--IP - 24/08/10 - CR1092 
					DelimitedDateColNo =  @delimitedDateColNo,			--IP - 24/08/10 - CR1092  
					DelimitedMoneyColNo = @delimitedMoneyColNo,			--IP - 24/08/10 - CR1092 
					IsInterest = @isInterest							--IP - 03/09/10 - CR1112		   

            where bankname=@bankname

        End
        Else
    -- Insert new definition    
        Begin
            
            insert into stordercontrol (bankname,[filename],acctnobegin,acctnolength,moneybegin,moneylength,
                    moneypoint,headline,datebegin,datelength,[dateformat],trailerbegin,
                    trailerlength,hastrailer,paymentmethod,
                    HeaderIdBegin, HeaderIdLength, HeaderId, TrailerIdBegin, TrailerIdLength, TrailerId,--IP - 12/08/10 - CR1092 
                    IsBatch, BatchHeaderIdBegin, BatchHeaderIdLength, BatchHeaderId, BatchHeaderHasTotal,BatchHeaderMoneyBegin, BatchHeaderMoneyLength, --IP - 19/08/10 - CR1092
                    BatchTrailerIdBegin, BatchTrailerIdLength, BatchTrailerId,				--IP - 03/09/10 - CR1092 
                    IsDelimited, Delimiter, DelimitedNoOfCols, DelimitedAcctNoColNo, DelimitedDateColNo, DelimitedMoneyColNo,	--IP - 24/08/10 - CR1092
                    IsInterest)		--IP - 03/09/10 - CR1112

            values (@bankname,@fileExt,@acctnobegin,@acctnolength,@moneybegin,
                    @moneylength,@moneypoint,@headline,@datebegin,@datelength,
                    @dateformat,@trailerbegin,@trailerlength,@hastrailer,@paymentmethod,
                    @headerIdBegin, @headerIdLength, @headerId, @trailerIdBegin, @trailerIdLength, @trailerId,	--IP - 12/08/10 - CR1092 
                    @IsBatch,@BatchHeaderIdBegin,@BatchHeaderIdLength,@BatchHeaderId,@BatchHeaderHasTotal,@BatchHeaderMoneyBegin,@BatchHeaderMoneyLength,
                    @batchTrailerIdBegin, @batchTrailerIdLength, @batchTrailerId,				--IP - 03/09/10 - CR1092
                    @isDelimited, @delimiter, @delimitedNoOfCols, @delimitedAcctNoColNo, @delimitedDateColNo, @delimitedMoneyColNo,	--IP - 24/08/10 - CR1092
                    @isInterest)	--IP - 03/09/10 - CR1112
        
        End          


	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

-- End End End End End End End End End End End End End End End End End End End End End End End End 
