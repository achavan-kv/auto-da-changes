SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_CreditBureauSaveSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_CreditBureauSaveSP]
GO

--PC 28/Nov/2006 //CR 843 Added source parameter  
CREATE PROCEDURE 	dbo.DN_CreditBureauSaveSP
			@custid varchar(20),
			@scoredate datetime,
			@responsexml ntext,
			@lawsuittimesincelast smallint, 
			@bankruptciestimesincelast smallint, 
			@lawsuits smallint,
			@lawsuits12months smallint,
			@lawsuits24months smallint,
			@lawsuitsavgvalue money,
			@lawsuitstotalvalue money,
			@bankruptcies smallint,
			@bankruptcies12months smallint,
			@bankruptcies24months smallint,
			@bankruptciesavgvalue money,
			@bankruptciestotalvalue money,
			@previousenquiries smallint,
			@previousenquiriestotalvalue money,
			@previousenquiriesavgvalue money,
			@previousenquiries12months smallint, 
			@previousenquiriesavgvalue12months money,
			@source char(1), --//CR 843 Added to identify the additional credit burea 
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	UPDATE	CreditBureau
	SET		scoredate = @scoredate,
			responsexml = @responsexml,
			lawsuittimesincelast = @lawsuittimesincelast,
			bankruptciestimesincelast = @bankruptciestimesincelast,
			lawsuits = @lawsuits,
			lawsuits12months = @lawsuits12months,
			lawsuits24months = @lawsuits24months,
			lawsuitsavgvalue = @lawsuitsavgvalue,
			lawsuitstotalvalue = @lawsuitstotalvalue,
			bankruptcies = @bankruptcies,
			bankruptcies12months = @bankruptcies12months,
			bankruptcies24months = @bankruptcies24months,
			bankruptciesavgvalue = @bankruptciesavgvalue,
			bankruptciestotalvalue = @bankruptciestotalvalue,
			previousenquiries = @previousenquiries,
			previousenquiriestotalvalue = @previousenquiriestotalvalue,
			previousenquiriesavgvalue = @previousenquiriesavgvalue,
			previousenquiries12months = @previousenquiries12months,
			previousenquiriesavgvalue12months = @previousenquiriesavgvalue12months,
			source = @source --CR 843
	WHERE	custid = @custid AND source = @Source --CR 843

	IF(@@rowcount = 0)
	BEGIN
		INSERT
		INTO		CreditBureau
				( custid, scoredate, responsexml, lawsuittimesincelast, bankruptciestimesincelast, lawsuits, lawsuits12months, lawsuits24months,  lawsuitsavgvalue,
				lawsuitstotalvalue, bankruptcies, bankruptcies12months, bankruptcies24months, bankruptciesavgvalue, bankruptciestotalvalue, previousenquiries,
				previousenquiriestotalvalue, previousenquiriesavgvalue, previousenquiries12months, previousenquiriesavgvalue12months, source)
		VALUES	( @custid, @scoredate, @responsexml, @lawsuittimesincelast, @bankruptciestimesincelast, @lawsuits, @lawsuits12months, @lawsuits24months, @lawsuitsavgvalue,
				@lawsuitstotalvalue, @bankruptcies, @bankruptcies12months, @bankruptcies24months, @bankruptciesavgvalue, @bankruptciestotalvalue, @previousenquiries,
				@previousenquiriestotalvalue, @previousenquiriesavgvalue, @previousenquiries12months, @previousenquiriesavgvalue12months, @source)
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

