GO
SET QUOTED_IDENTIFIER ON
GO
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID('[dbo].[CM_SPAWriteArrangementScheduleSP]') AND OBJECTPROPERTY(id,'IsProcedure') = 1)
DROP PROCEDURE [dbo].[CM_InstalplanAgreementWriteRefinanceSP]
GO

CREATE PROCEDURE [dbo].[CM_InstalplanAgreementWriteRefinanceSP] 
-- ============================================================================================
-- Author:		Ilyas Parker/John Croft
-- Create date: 21/01/2009
-- Description:	The procedure will write the Refinance Details			
-- ============================================================================================
		        @acctno varchar(12),
				@period char(1),
				@arrangementAmt money, 
				@numberOfInstalments int,
				@instalmentAmt money,
				@oddPaymentAmt money,
				@firstPaymentDate datetime,
				@finalPaymentDate datetime,
				@empeeno int,
				@reason varchar(2),
				@servicechg money,
				@freezeind bit,
				@return	int	OUTPUT
as
begin
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	set nocount on;

	set @return = 0    --initialise return code
	
	--Declare counters
	declare @index int, --Counter for 'Instalment Amount'
			@maxagrmtno int,
			@NextDateDue datetime, -- Will hold the next Date Due for the 'Arrangement Schedule'
			@interest money,
   			@admin money
   			
			
--	declare @acctno varchar(12)		-- testing
--	set @acctno ='123456789012'
	-- get max agrmtno from history (account may be refinanced more than once)
	select @maxagrmtno=isnull(max(agrmtno),0)+1
	from instalplanHistory
	Where acctno=@acctno
	
--	select @maxagrmtno

	-- save existing plan to history (increment agrmtno in history)
	insert into instalplanHistory (origbr,acctno,agrmtno,datefirst,instalno	,instalfreq,datelast,instalamount,
								fininstalamt,instaltot,mthsintfree,empeenochange,dueday,InstantCredit)
	select origbr,acctno,@maxagrmtno,datefirst,instalno	,instalfreq,datelast,instalamount,
								fininstalamt,instaltot,mthsintfree,empeenochange,dueday,InstantCredit
	from instalplan 
	Where acctno=@acctno
	
--	-- update values for refinance
--	update instalplan
--		set datefirst=@firstPaymentDate, instalno=@numberOfInstalments,
--				datelast=@finalPaymentDate,instalamount=@instalmentAmt,
--				fininstalamt=@oddPaymentAmt,instaltot=@arrangementAmt,empeenochange=@empeeno,
--				dueday=datepart(d,@firstPaymentDate)
--	Where acctno=@acctno
	
	-- save existing agreement to history (increment agrmtno in history)
	insert into AgreementHistory (origbr,acctno,agrmtno,dateagrmt,empeenosale,datedepchqclr,holdmerch,
				holdprop,datedel,datenextdue,oldagrmtbal,cashprice,discount,pxallowed,servicechg,
				sdrychgtot,agrmttotal,deposit,codflag,soa,paymethod,unpaidflag,deliveryflag,
				fulldelflag,PaymentMethod,empeenoauth,dateauth,empeenochange,datechange,AdminFee,
				InsCharge,datefullydelivered,createdby,paymentcardline,paymentholidays,
				AgreementPrinted,TaxInvoicePrinted,WarrantyPrinted,source)				
	
	select origbr,acctno,@maxagrmtno,dateagrmt,empeenosale,datedepchqclr,holdmerch,
				holdprop,datedel,datenextdue,oldagrmtbal,cashprice,discount,pxallowed,servicechg,
				sdrychgtot,agrmttotal,deposit,codflag,soa,paymethod,unpaidflag,deliveryflag,
				fulldelflag,PaymentMethod,empeenoauth,dateauth,empeenochange,datechange,AdminFee,
				InsCharge,datefullydelivered,createdby,paymentcardline,paymentholidays,
				AgreementPrinted,TaxInvoicePrinted,WarrantyPrinted,source
	from agreement
	Where acctno=@acctno
	
	-- get any charges
	exec FintransGetChargesSP @acctno,@interest out,@admin out,0
	
	-- update values for refinance
--	update agreement
--		set deposit=agrmttotal-(@arrangementAmt-@interest-@admin),		-- amount paid on previous agreement
--			servicechg=@servicechg
--	Where acctno=@acctno	
--	
	
	
	

    if (@@error != 0)
    begin
        set @return = @@error
    end
end

GO

-- End End End End End End End End End End End End End End End End End End End End End End End End End End End End End
