SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[dbagrdate]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[dbagrdate]
GO


CREATE procedure dbagrdate
-- ================================================      
-- Project      : CoSACS .NET      
-- File Name    : dbagrdate.sql      
-- File Type    : MSSQL Server Stored Procedure Script      
-- Title        : 
-- Author       : ??      
-- Date         : ??      
--        
--       
-- Change Control      
-- --------------      
-- Date      By  Description      
-- ----      --  ---------- 
-- 12/10/11  jec CR1232 #3291 new disbursement screen - include CLD transtypecode
-- =================================================================================   
	@acctno   char(12) = ' ', 
    @agrmtno    int = 0, 
    @agrmttotal money = 0, 
    @countpcent   float = 0.0, 
    @origbr   smallint = 0, 
    @datefirst DATETIME = '1900-01-01'  
as  
/* AA procedure dbagrdate Version 2.0 14/06/01
putting asc in the sort order for the calculation of delivery totals
 AA FR 1309 procedure dbagrdate Version 2.1 11/07/01 Was not setting delivery
dates correct for accounts taken on through Coaster to Cosacs
It is important 
KEF 68490 added @continue paramter so can break out of cursor checking for when delivery met threshold or GRT's have datedel messed up
*/
declare
    @i   integer,     @deltot   money,     @vtot   money,     @datetrans DATETIME, 
    @datelast DATETIME,     @instalno   smallint,     @vardate   varchar(12),     @trefno   integer, 
    @transvalue   money,     @updateacct   smallint,     @olddatefirst DATETIME,     @holdstring   varchar(64), 
    @as400bal   money,     @branchno   smallint,     @transrefno   integer,     @ct   smallint, 
    @delhold    money,     @status  integer,    @fixeddatefirst tinyint , @datedel datetime ,@datefullydelivered datetime ,
    @continue bit

    set @i = 0     set @deltot = 0     set @vtot = 0 
    set @datetrans = '1900-01-01'     set @datelast = '1900-01-01'     set @instalno = 0 
    set @trefno = 0     set @transvalue = 0     set @updateacct = 0     set @olddatefirst = '1900-01-01' 
    set @holdstring = ' '     set @as400bal = 0     set @branchno = 0     set @transrefno = 0 
    set @ct = 0     set @delhold = 0     set @olddatefirst = @datefirst  
    if @countpcent = 0
    BEGIN
      select @countpcent =globdelpcent from country

    END
    /* AA SA change - don't want datefirst linked to delivery date*/
    select @fixeddatefirst = fixeddatefirst from country

    if @agrmttotal = 0 
    and substring (@acctno, 4, 1) ! = '4'
    BEGIN
      return  
    END

    select @deltot = isnull(sum(transvalue), 0) 
    from  fintrans 
    where acctno = @acctno 
    and transtypecode in ('DEL', 'GRT', 'ADD','CLD')		-- jec CR1232 #3291

    -- Agrmt Total is now reduced by Goods Return before collection is delivered
    --if @deltot > @agrmttotal 
    --and substring (@acctno, 4, 1) = '4'
    --BEGIN
     --   set @updateacct = 1  
     --   set @delhold = @deltot  
     --   if @agrmttotal = 0
     --   BEGIN
     --       set @agrmttotal = @deltot  
     --   END
    --END
    
    if @deltot = 0 /* no deliveries */
    BEGIN
      return  
    END

	/* possible that the agreement total is zero at this point which 
	    will cause a divide by zero error so just return if the agreement
	    total is zero */
	IF(@agrmttotal = 0)
	BEGIN
		RETURN
	END

    /* if delivery amount less than the required percentage of agreement 
       total then return */
--KEF 68490 this is wrong so removing
    if (@deltot/@agrmttotal*100) < @countpcent
    BEGIN
        --KEF ensure datedel and datefullydelivered are null
        update agreement 
        set    datedel = null,
               datefullydelivered = null
        where  acctno = @acctno 
        and    agrmtno = @agrmtno  

        --KEF ensure datefirst and datelast are 01-jan-1900
        update instalplan 
        set    datefirst = '01-jan-1900',
               datelast = '01-jan-1900'
        where  acctno = @acctno 
        and    agrmtno = @agrmtno  

        return  
    END
    ELSE
    BEGIN
        if substring (@acctno, 4, 1) = '4'
        BEGIN
            set @updateacct = 2  
        END
    END

    set  @DateFullyDelivered= NULL
    declare @isdelivered char(1) -- 68513 recording whether delivered as holding date of first delivery so if collected and re-delivered original delivery date stands
    if  @deltot/@agrmttotal*100 > = @countpcent
	set @isdelivered='Y'
    else
	set @isdelivered='N'


   set @deltot = 0  
   set @continue = 1
   DECLARE delivery_cursor CURSOR 
  	FOR SELECT transvalue, datetrans
   from fintrans
   where acctno = @acctno 
        and transtypecode in ('DEL', 'GRT', 'ADD','CLD')		-- jec CR1232 #3291 
   order by datetrans 
   OPEN delivery_cursor
   FETCH NEXT FROM delivery_cursor INTO @transvalue,@datetrans

   WHILE (@@fetch_status <> -1)
   BEGIN
	   IF (@@fetch_status <> -2)
       begin
           if @continue = 1 --KEF 68490
           begin
              set @deltot = @deltot + @transvalue  
	          if  @deltot/@agrmttotal*100 > = @countpcent --and @datedel is null
   	          BEGIN
                 set @datedel =@datetrans
                 set @continue = 0 --KEF 68490
              END
	          if  @deltot/@agrmttotal*100 < @countpcent and @datedel is not null and @isdelivered ='N' -- only set it to null if not delivered in the first place.
   	          BEGIN
                  set @datedel =null
              END
           end
           if @deltot >=@agrmttotal and @DateFullyDelivered= NULL -- set the delivery date
           begin
              set @DateFullyDelivered =@datetrans
		   end
		   if @deltot <@agrmttotal and @DateFullyDelivered is not NULL -- if goods return then reset it to blank
           begin
 		      set @DateFullyDelivered =NULL
           end
              --select @mincontract,@minacctno,@maxcontract,@maxacctno
	    END
        FETCH NEXT FROM delivery_cursor INTO @transvalue,@datetrans

   END

   CLOSE delivery_cursor
   DEALLOCATE delivery_cursor


    if @updateacct = 1
    BEGIN
        select @agrmttotal = isnull(agrmttotal, 0) 
        from agreement 
        where acctno = @acctno  
        if @agrmttotal = 0
        BEGIN
            set @agrmttotal = @delhold  
            update acct 
            set agrmttotal = @agrmttotal 
            where acctno = @acctno  
        END
    END
    ELSE 
    BEGIN
        IF @updateacct = 2  
        BEGIN
	    /* if more than 1 month after delivery set as400bal =deltot for cash accounts only*/
            if datediff(Month, getdate(), @datetrans) > 1
            BEGIN
                set @as400bal = @deltot 
            END
        END
        else
            BEGIN /* @updateacct !=2 */
             set @as400bal = 0  
   END
        select @agrmttotal = isnull(agrmttotal, 0) 
        from  agreement 
        where acctno = @acctno  

        if @agrmttotal = 0
        BEGIN
            set @agrmttotal = @delhold  
        END

        update   acct 
        set agrmttotal = @agrmttotal, 
            as400bal = @as400bal 
        where acctno = @acctno  
    END  /* above for cash accounts only */

   /* HP Accounts */    
    update  agreement 
    set datedel = @datedel, 
        datefullydelivered=@datefullydelivered,
        agrmttotal = @agrmttotal, 
        origbr = @origbr 
    where acctno = @acctno 
    and agrmtno = @agrmtno  

    select @datefirst = isnull(datefirst, ''),
           @datelast = isnull(datelast, ''),
        @instalno = instalno 
    from instalplan 
    where acctno = @acctno  


    /* Date of first instalment should be 1 month after delivery */
    if @fixeddatefirst = 0 or 
       @fixeddatefirst = 2 or
       @datefirst =N'' or @datefirst =N'1-jan-1900'  /* south africa change - datefirst not related to delivery date */
    BEGIN
	    exec dbdatefirst @acctno = @acctno,
        	              @datefirst = @datefirst OUTPUT,
                	      @datedel = @datedel
	    set @datelast = dateadd(month,@instalno-1,@datefirst) 

	    update instalplan 
	    set origbr = @origbr, 
        	datefirst = isnull (@datefirst,'1-jan-1900'), 
	        datelast = @datelast,
	        dueday = cast(datepart(day, isnull (@datefirst,'1-jan-1900')) AS smallint) 
	    where acctno = @acctno 
	    and agrmtno = @agrmtno 
	    and datefirst = @olddatefirst  
    END      
RETURN
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

-- End End End End End End End End End End End End End End End End End End End End End End End End End End End End End End End End