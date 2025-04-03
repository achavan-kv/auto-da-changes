SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_CODReprintCalculationSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_CODReprintCalculationSP]
GO

CREATE procedure DN_CODReprintCalculationSP
			@acctno char(12),
			@buffno integer,
			@agrmtno integer,
			@totalamountdue money OUT,
			@nonstocktotal money OUT,
			@cod smallint OUT,
			@dateReqDel DateTime,
			@timeReqDel varchar (12),  
			@addtype varchar (2),
			@return integer OUT
as 

   	declare 	@hasdelivery smallint, 
		@hasschedule smallint,
    		@amountdue money, 
		@paidamount money
   
	set 	@return = 0
	set	@totalamountdue = 0
	set	@nonstocktotal = 0

	/* make sure it's a COD account first */
	SELECT	@cod = count(*)
	FROM		agreement 
	WHERE	acctno = @acctno
	AND		agrmtno = @agrmtno
	AND		codflag = 'Y'

	IF(@cod > 0)
	BEGIN	  
		 set nocount on   
		
		if exists ( select * from schedule where 
		   buffno != @buffno and
		   acctno = @acctno		and agrmtno =@agrmtno)
	    		
			set @hasschedule = 1
	   	else 
			set @hasschedule = 0
	      
	   	if exists (select * 
			from delivery 
			where 
			--buffno =@buffno and
	     	acctno =@acctno 
			and agrmtno =@agrmtno)
	     
			set @hasdelivery=1
	   	else
	     		set @hasdelivery=0
	     
	   	if @hasschedule = 0 and @hasdelivery = 0 	/* stockitems have not yet been included on the delivery note
	   							so will need to be calculated and paid for*/    
	   	begin
        		select 	@nonstocktotal = isnull(sum(ordval),0) 
   			from 	lineitem, stockitem 
   			where 	acctno =@acctno
   	      		and 	agrmtno =@agrmtno 
   			      and  	stockitem.itemno = lineitem.itemno
   	      		and 	lineitem.stocklocn = stockitem.stocklocn
   	      		and 	stockitem.itemtype != 'S'
   	      		
   			select 	@paidamount = isnull(sum(transvalue),0) 
   			from 	fintrans
   			where	acctno = @acctno 
   			and	transtypecode in ('PAY','COR','REF','RET','SCX','REB','XFR','DDN','DDR','DDE')
   	  	end
	   	else /* these would already have appeared on another delivery note */
	   	begin
	   	 	set @nonstocktotal= 0
	   	 	set @paidamount = 0
	   	end   	 
	    /* amountdue is the amount which will be displayed on the delivery note */
	   	select 	@amountdue = isnull(s.quantity * Price,0)
	    	from 	lineitem l, schedule s
		where 	l.acctno = @acctno 
		and	l.acctno = s.acctno
		and 	l.agrmtno = @agrmtno
        		and 	dateReqDel =@dateReqDel
		and	timeReqDel = @timeReqDel
		and 	deliveryaddress = @addtype
		and	s.buffno = @buffno

    	SET  @totalamountdue =@amountdue + @paidamount
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

