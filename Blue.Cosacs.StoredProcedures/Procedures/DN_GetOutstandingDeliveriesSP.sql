SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO

IF EXISTS (SELECT * FROM SysObjects
           WHERE Name = 'DN_GetOutstandingDeliveriesSP' AND Type = 'P')
BEGIN
    DROP PROCEDURE DN_GetOutstandingDeliveriesSP
END
GO


CREATE PROCEDURE DN_GetOutstandingDeliveriesSP

--------------------------------------------------------------------------------
--
-- Project      : CoSACS .NET
-- File Name    : DN_GetOutstandingDeliveriesSP.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Monitor Outstanding Deliveries
-- Author       : G Johnson (based on SQL from A Ayscough)
-- Date         : 20 June 2005
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 22/03/06  RD  Modified to correct issues 1,3 & 4 for livewire issue 68009
-- 07/07/11  IP  CR1254 - RI - #4018 - Display IUPC and CourtsCode
--------------------------------------------------------------------------------

-- Parameters
	@bufferno integer = 0,
	@warehouseno integer = 0,
	@datefrom datetime,
	@dateto datetime,
	@includesecuritised smallint,
	@includenonsecuritised smallint,
	@operand char(1),
	@return integer output
AS

	set nocount on  -- do this to prevent unneccessary messages coming back from sql

	declare @statement sqltext
	declare @totalvalue money

	create table #osdeliveries
		(acctno char(12),
		delorcoll char(1),
		DateAuthorised datetime,
		buffno int,
		stocklocn smallint,
		--itemno varchar(10),
		itemno varchar(18),							--IP - 07/07/11 - CR1254 - RI - #4018
		CourtsCode varchar(18),						--IP - 07/07/11 - CR1254 - RI - #4018
		ordval money,
		DateDNprinted datetime,
		printedby int)

	SET @datefrom = CONVERT(SMALLDATETIME, CONVERT(VARCHAR(10), @datefrom, 105), 105)
	SET @dateto = CONVERT(SMALLDATETIME, CONVERT(VARCHAR(10), @dateto, 105), 105)

	--set @statement = 'insert into  #osdeliveries ' +
	--		' (acctno, delorcoll ' +
	--                ' ,DateAuthorised, buffno ' +
	--                ' ,stocklocn, itemno ' +
	--                ' ,ordval, dateDNprinted ' +
	--                ' ,printedby) ' +
	--                ' select s.acctno, s.delorcoll ' +
	--                ' , g.dateauth , s.buffno ' + 
	--                ' , s.stocklocn , s.itemno ' + 
	--                ' ,s.quantity * l.price, s.dateprinted ' +
	--                ' ,s.printedby ' + 
	--                ' FROM schedule s, lineitem l , agreement g, acct a ' +
	--                ' WHERE s.acctno = a.acctno and s.acctno = g.acctno and s.acctno = l.acctno ' +
	--                ' AND a.currstatus != ''S'' and l.itemno = s.itemno and l.stocklocn = s.stocklocn ' +
	--                ' and (g.datefullydelivered is null or g.datefullydelivered < ''1-jan-1910'')  ' +
	--                ' and s.stocklocn = ' + convert(varchar,@warehouseno) +
	--                ' and a.acctno not like ''___5%'' ' +
	--                ' and CONVERT(SMALLDATETIME, CONVERT(VARCHAR(10), g.dateauth, 105), 105) between ' +  '''' +  convert (varchar,@datefrom ) + '''' +
	--                ' and ''' + convert (varchar,@dateto) + ''''
	
	set @statement = 'insert into  #osdeliveries ' +
			' (acctno, delorcoll ' +
	                ' ,DateAuthorised, buffno ' +
	                ' ,stocklocn, itemno ' +
	                ' ,CourtsCode ' +
	                ' ,ordval, dateDNprinted ' +
	                ' ,printedby) ' +
	                ' select s.acctno, s.delorcoll ' +
	                ' , g.dateauth , s.buffno ' + 
	                ' , s.stocklocn , isnull(si.iupc,'''') as itemno, isnull(si.itemno,'''') as CourtsCode ' + 
	                ' ,s.quantity * l.price, s.dateprinted ' +
	                ' ,s.printedby ' + 
	                ' FROM schedule s, lineitem l , agreement g, acct a, stockinfo si' +
	                ' WHERE s.acctno = a.acctno and s.acctno = g.acctno and s.acctno = l.acctno ' +
	                ' and l.ItemID = s.ItemID and l.stocklocn = s.stocklocn ' +
	                ' AND l.ItemID = si.ID ' +
	                ' AND a.currstatus != ''S''' +
	                ' and (g.datefullydelivered is null or g.datefullydelivered < ''1-jan-1910'')  ' +
	                ' and s.stocklocn = ' + convert(varchar,@warehouseno) +
	                ' and a.acctno not like ''___5%'' ' +
	                ' and CONVERT(SMALLDATETIME, CONVERT(VARCHAR(10), g.dateauth, 105), 105) between ' +  '''' +  convert (varchar,@datefrom ) + '''' +
	                ' and ''' + convert (varchar,@dateto) + ''''

	if @includesecuritised = 0 
	begin
	    set @statement = @statement + ' and a.securitised != ''Y'' ' 
	end                

	if @includenonsecuritised = 0  
	begin
	    set @statement = @statement + ' and a.securitised = ''Y'' ' 
	end                
	
	-- 68009 RD 22/03/2006 Replaced to load data by Load Number as per original CR
	set @statement = @statement + ' and s.loadno ' + @operand + convert(varchar(12), @bufferno )
	
	execute sp_executesql @statement

	create clustered index ix_osdeliveries2345 on #osdeliveries(acctno)
	
	select @totalvalue = sum(ordval) from #osdeliveries
	
	set @return = @@error
	
	-- 68009 RD 22/03/06 o.stocklocn removed as not required by user and not in CR.
	select  o.acctno ,o.delorcoll ,o.DateAuthorised ,o.buffno ,
		    o.itemno, o.CourtsCode ,o.ordval,@totalvalue as total,o.DateDNprinted,				--IP - 07/07/11 - CR1254 - RI - #4018
		      --o.itemno,o.ordval,@totalvalue as total,o.DateDNprinted,				--IP - 07/07/11 - CR1254 - RI - #4018
		    ISNULL(convert (varchar(9),o.printedby) + ' ' + c.empeename,' ') as PrintedBy
	from 	#osdeliveries o	
	left outer join courtsperson c 
	on o.printedby = c.empeeno

GO

GRANT EXECUTE ON DN_GetBookingsSP TO PUBLIC
GO
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO
