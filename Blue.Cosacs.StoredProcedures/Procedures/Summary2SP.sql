SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS OFF
GO

IF EXISTS (SELECT * FROM SysObjects
           WHERE id = object_id(N'[dbo].Summary2SP') and OBJECTPROPERTY(id, N'IsProcedure') = 1)

BEGIN
    DROP PROCEDURE Summary2SP
END
GO

CREATE PROCEDURE dbo.Summary2SP
/*
** Author	: M. S. Davies, M. A. King (Strategic Thought)
** Created on	: 10-Nov-1999
** Version	: 1.0
** Name		: Summary Table 2
**
** Who  Date      Description
** ---  ----      -----------
** MSD  03/12/99  Update following review by Courts
** MJKC 14/02/00  Select, in addition, the buffbranchno from 
**                schedule table. Also populate buffbranchname
** MJKC 07/03/00  Exclude deferred terms and stamp duty
** CJB  20/06/00  Add countrycode
** CJB  30/06/00  Split out Country code update to speed up processing
** CJB  29/09/00  Improve uniqueness on schedule/lineitem link
** KEF  13/03/02  Added in where clause cancelledflag <> 'Y'
** KEF  16/01/03  FR117634 - Amended unique index 'ix_smrydata' so it also uses 'buffbranchno' column
** KEF  21/01/03  FR121052 - Re-tune indexes: Removed indexes ix_summary2 and ix_summary6
**                                            Created new index ix_summary21 on acctno
** KEF  23/01/03  FR117634 - Removed restriction: stockitem.itemtype	= 'S'
**                as report 3 needs to report all stockitems outstanding
**                Added column to summary2 table for itemtype as report 2 should only report stock items
**                Removed restriction: currstatus <> 'S' as report3 needs to report these accounts
** KEF  23/04/03  FR121052 - Insert and update into temporary table then at end put into 
**                permanent summary1 table.
**                Add restriction currstatus = 's' and outstbal = 0 so these accounts are not 
**                inserted into summary2
**                Add restriction so no P&T accounts are in summary2
**                When calculating datedel only retrieve distinct acctno, only need 1 datedel for each acct
** KEF  27/05/03  Change varchar->nvarchar for all varchar columns.
** KEF  16/06/03  Remove buffbranchno column as it's not being used in any report.
**                Remove countrycode column as not used.
** KEF  25/06/03  Added if statement so nvarchar and nchar changes only affect Thailand
** KEF  29/06/03  Changed index ix_smrydata so it's not unqiue, it can't be unique as settled accts 
**                and non stocks are not in schedule table so causing problems.
** KEF  09/10/03  Changed size of itemdesc2 column for Thailand only as this is larger.
** KEF  06/11/03  Changed Contractno to NVarchar for Thailand.
** AA   02/03/04  Ensure only items with lineitem.iskit = 0 are included
** KEF  15/04/04  CR597 Reports need to be split between securitised and non-securitised accounts
** Sent after 4.0.0.0rpt release **
** KEF  19/06/06  68326 Lineitem and schedule need to be joined on contractno too since delivery in .Net
-- jec  18/10/10  72873 Duplicate key error
-- jec  10/08/11  CR1254 RI Changes
*/

 @Return     int OUTPUT
 
 AS
 
SET NOCOUNT ON

set @Return = 0

print convert (varchar, getdate()) + ' summary2 query started**********************'


/***** Summary2_sec *****/

--MODIFY summary2_sec TABLE TO TRUNCATED TO DELETE ALL ROWS and remove index
TRUNCATE TABLE summary2_sec
--GO
if exists (select * from sysindexes
		   where name = 'ix_smrydata2_sec')
drop index summary2_sec.ix_smrydata2_sec
--GO

if (select countrycode from country) <> 'H'
begin
    CREATE TABLE [dbo].[#summary2_sec] (
/*	[countrycode] [char] (1) NOT NULL ,	KEF 16/06/03 */
	[acctno] [varchar] (12) NOT NULL ,
	[itemno] [varchar] (18) NULL ,					-- RI	
	[itemdescr1] [varchar] (35) NULL ,				-- RI
	[itemdescr2] [varchar] (40) NULL ,
	[stocklocn] [smallint] NULL ,
	[stocklocnname] [varchar] (26) NULL ,
	[loadno] [smallint] NULL ,
	[dateloaded] [datetime] NULL ,
	[buffno] [int] NULL ,
	[buffbranchno] [int] NULL ,
/*	[buffbranchname] [varchar] (26) NULL ,  KEF  16/06/03 */
	[datedel] [datetime] NULL ,
	[itemtype] [varchar] (1) NOT NULL ,
	[contractno] [varchar] (10) not null,
	ItemId   INT,									-- RI
	--[parentitemno] [varchar] (8) NULL   --IZ 14/09/11
	ParentItemID INT					  --IP - 02/11/11 
    ) ON [PRIMARY]
end
--GO

ALTER TABLE [dbo].[#summary2_sec] WITH NOCHECK ADD 
/*	CONSTRAINT [tsummary2_sec__countr__31D75E8D] DEFAULT (' ') FOR [countrycode],	KEF 16/06/03 */
	CONSTRAINT [tsummary2_sec__loadno__379037E3] DEFAULT (0) FOR [loadno],
	CONSTRAINT [tsummary2_sec__itemty__30B918F3] DEFAULT ('S') FOR [itemtype],
	CONSTRAINT [tsummary2_sec__contra__31052BF8] DEFAULT ('') FOR [contractno]
--GO


print convert (varchar, getdate()) + ' create new temp table ended**********************'


--INSERT REQUIRED LINEITEM TABLE DATA
INSERT INTO #summary2_sec(
	acctno,	
	itemno,
	stocklocn,
	dateloaded,
	loadno,
	buffno,
	buffbranchno,					/* MJKC 14/02/00 */
	itemtype,					/* FR117634 23/01/03 KEF Added new column */
	contractno,
	ItemId,				-- RI
	--parentitemno   --IZ 14/09/11 Added a new column
	ParentItemID	 --IP - 02/11/11
	)
SELECT Distinct		--LW72873 
	lineitem.acctno,  
	lineitem.itemno,
	lineitem.stocklocn,
	schedule.datedelplan,
	isnull(schedule.loadno,0),			 /* Added isnull */
	schedule.buffno,
	schedule.buffbranchno,				/* MJKC 14/02/00 */
	isnull(stockitem.itemtype,'S'),			/* FR117634 23/01/03 KEF Added new column */ /* Added isnull */
	lineitem.contractno,
	lineitem.ItemId,					-- RI
	--lineitem.parentitemno   --IZ 14/09/11 Added a new column	
	lineitem.ParentItemID	  --IP - 02/11/11
FROM summary1 
inner join lineitem 
	ON	summary1.acctno = lineitem.acctno
	and summary1.custid not like 'PAID%' 
	AND	lineitem.iskit = 0
	AND	lineitem.agrmtno = 1
inner join stockitem 
	on stockitem.stocklocn  	= lineitem.stocklocn 	/* CJB 05/06/00 */
	and stockitem.ItemId 	= lineitem.ItemId	
left join schedule 
	ON lineitem.acctno  = schedule.acctno
	AND	lineitem.ItemId  = schedule.ItemId				-- RI
	AND	lineitem.stocklocn = schedule.stocklocn		/* CB 28/09/00 force a bit more uniqueness */
	AND lineitem.contractno = schedule.contractno /* KEF 19/06/06 Need to join contractno to avoid duplicates */
	AND	schedule.agrmtno = 1			/* MSD 03/12/99 */
WHERE	securitised = 'Y'					/* KEF 15/04/04 CR597 */
AND NOT (currstatus = 'S' AND outstbal = 0)             /* FR121052 KEF 23/04/03 */
and     cancelledflag           <> 'Y'			/* KEF 13/03/02 */
and not exists (
				select * from fintrans f 
				where f.acctno = summary1.acctno
				and f.transtypecode = 'SCT'
				)
UNION /* RM - add SCT transactions with lineitems from transfer account*/
SELECT Distinct		
	lineitem.acctno,  
	lineitem.itemno,
	lineitem.stocklocn,
	schedule.datedelplan,
	isnull(schedule.loadno,0),			
	schedule.buffno,
	schedule.buffbranchno,			
	isnull(stockitem.itemtype,'S'),			
	lineitem.contractno,
	lineitem.ItemId,					
	lineitem.ParentItemID	 
FROM summary1 
inner join fintrans 
	on fintrans.acctno = summary1.acctno
	and fintrans.transtypecode = 'SCT'	
	and fintrans.acctno like '___9%'
	and summary1.custid not like 'PAID%' 
inner join lineitem 
	ON	fintrans.chequeno = lineitem.acctno
	AND	lineitem.iskit = 0
	AND	lineitem.agrmtno = 1
inner join stockitem 
	on stockitem.stocklocn  	= lineitem.stocklocn 	
	and stockitem.ItemId 	= lineitem.ItemId
left join schedule 
	ON lineitem.acctno  = schedule.acctno
	AND	lineitem.ItemId  = schedule.ItemId				
	AND	lineitem.stocklocn = schedule.stocklocn		
	AND lineitem.contractno = schedule.contractno 
	AND	schedule.agrmtno = 1			
WHERE	securitised = 'Y'					
AND NOT (currstatus = 'S' AND outstbal = 0)           
and     cancelledflag           <> 'Y'			
--GO

print convert (varchar, getdate()) + ' insert into temp table ended**********************'


--CREATE UNIQUE INDEX ix_smrydata2_sec	-- KEF 29/09/03
CREATE INDEX ix_smrydata2_sec
ON #summary2_sec(
	acctno,
	--itemno,
	ItemId,				-- RI
	stocklocn,
	loadno,
	buffno,
	buffbranchno,
    contractno,
    ParentItemID)	  --IP - 02/11/11
    --parentitemno)   --IZ 14/09/11
--GO

--Create index for acctno
CREATE CLUSTERED INDEX [ix_temp_summary21_sec] ON [dbo].[#summary2_sec] ([acctno] )

print convert (varchar, getdate()) + ' create index ended**********************'
--GO


--UPDATE REMAINING COLUMNS
SELECT	distinct s2.acctno, isnull(min(f.datetrans),'') AS datedel /* FR121052 KEF 23/04/03 */
INTO	#fintrans_datedel_sec
FROM	fintrans f, #summary2_sec s2
WHERE	f.transtypecode in ('SCT', 'DEL','CLD')					-- #10138
AND	f.acctno = s2.acctno
GROUP BY s2.acctno
--GO

UPDATE	#summary2_sec
SET	datedel = #fintrans_datedel_sec.datedel
FROM    #fintrans_datedel_sec
WHERE	#summary2_sec.acctno = #fintrans_datedel_sec.acctno
--GO

print convert (varchar, getdate()) + ' update datedel column ended**********************'
--GO

UPDATE	#summary2_sec
SET	itemdescr1         = stockitem.itemdescr1,
	itemdescr2         = stockitem.itemdescr2
FROM	stockitem
--WHERE	#summary2_sec.itemno    = stockitem.itemno
WHERE	#summary2_sec.ItemId    = stockitem.ItemId				-- RI
AND	#summary2_sec.stocklocn = stockitem.stocklocn;
--GO

print convert (varchar, getdate()) + ' update itemdescr columns ended**********************'
--GO

UPDATE	#summary2_sec
SET	stocklocnname      = branch.branchname
FROM	branch
WHERE	#summary2_sec.stocklocn = branch.branchno;
--GO


print convert (varchar, getdate()) + ' update buffbranchname column ended**********************'


/* Insert data from the temporary table into the permanent summary2_sec table */
insert into summary2_sec select * from #summary2_sec
--go
if exists (select * from sysindexes
		   where name = 'ix_summary21_sec')
DROP INDEX [dbo].[summary2_sec].[ix_summary21_sec]
--go
print convert (varchar, getdate()) + ' insert into summary2_sec ended**********************'
--GO

--INDEXES
SET QUOTED_IDENTIFIER ON 
SET ARITHABORT ON 
SET CONCAT_NULL_YIELDS_NULL ON 
SET ANSI_NULLS ON 
SET ANSI_PADDING ON 
SET ANSI_WARNINGS ON 
SET NUMERIC_ROUNDABORT OFF 
--go


--MODIFY TABLE STRUCTURE TO ISAM UNIQUE
CREATE UNIQUE INDEX ix_smrydata2_sec
ON summary2_sec(
	acctno,
	--itemno,
	ItemId,					-- RI
	stocklocn,
	loadno,
	buffno,
	buffbranchno,
    contractno,	/* AA 22/07/03 */
    ParentItemID)   --IP - 02/11/11
   	--parentitemno)   --IZ 14/09/11
--GO



/***** Summary2_non *****/


--MODIFY summary2_non TABLE TO TRUNCATED TO DELETE ALL ROWS and remove index
TRUNCATE TABLE summary2_non
--GO
if exists (select * from sysindexes
where name = 'ix_smrydata2_non')
drop index summary2_non.ix_smrydata2_non
--GO

if (select countrycode from country) <> 'H'
begin
    CREATE TABLE [dbo].[#summary2_non] (
/*	[countrycode] [char] (1) NOT NULL ,	KEF 16/06/03 */
	[acctno] [varchar] (12) NOT NULL ,
	[itemno] [varchar] (18) NULL ,					-- RI
	[itemdescr1] [varchar] (35) NULL ,				-- RI
	[itemdescr2] [varchar] (40) NULL ,
	[stocklocn] [smallint] NULL ,
	[stocklocnname] [varchar] (26) NULL ,
	[loadno] [smallint] NULL ,
	[dateloaded] [datetime] NULL ,
	[buffno] [int] NULL ,
	[buffbranchno] [int] NULL ,
/*	[buffbranchname] [varchar] (26) NULL ,  KEF  16/06/03 */
	[datedel] [datetime] NULL ,
	[itemtype] [varchar] (1) NOT NULL ,
	[contractno] [varchar] (10) not null,
	ItemId   INT,									-- RI
	ParentItemID INT					  --IP - 02/11/11
	--[parentitemno] [varchar] (8) NULL   --IZ 14/09/11
    ) ON [PRIMARY]
end
--GO

ALTER TABLE [dbo].[#summary2_non] WITH NOCHECK ADD 
	CONSTRAINT [tsummary2_non__loadno__379037E3] DEFAULT (0) FOR [loadno],
	CONSTRAINT [tsummary2_non__itemty__30B918F3] DEFAULT ('S') FOR [itemtype],
	CONSTRAINT [tsummary2_non__contra__31052BF8] DEFAULT ('') FOR [contractno]
--GO
print convert (varchar, getdate()) + ' create new temp table ended**********************'


--INSERT REQUIRED LINEITEM TABLE DATA

--INSERT REQUIRED LINEITEM TABLE DATA
INSERT INTO #summary2_non(
	acctno,	
	itemno,
	stocklocn,
	dateloaded,
	loadno,
	buffno,
	buffbranchno,					/* MJKC 14/02/00 */
	itemtype,					/* FR117634 23/01/03 KEF Added new column */
	contractno,
	ItemId,				-- RI
	--parentitemno   --IZ 14/09/11 Added a new column
	ParentItemID	 --IP - 02/11/11
	)
SELECT Distinct		--LW72873 
	lineitem.acctno,  
	lineitem.itemno,
	lineitem.stocklocn,
	schedule.datedelplan,
	isnull(schedule.loadno,0),			 /* Added isnull */
	schedule.buffno,
	schedule.buffbranchno,				/* MJKC 14/02/00 */
	isnull(stockitem.itemtype,'S'),			/* FR117634 23/01/03 KEF Added new column */ /* Added isnull */
	lineitem.contractno,
	lineitem.ItemId,					-- RI
	--lineitem.parentitemno   --IZ 14/09/11 Added a new column	
	lineitem.ParentItemID	  --IP - 02/11/11
FROM summary1 
inner join lineitem 
	ON	summary1.acctno = lineitem.acctno
	and summary1.custid not like 'PAID%' 
	AND	lineitem.iskit = 0
	AND	lineitem.agrmtno = 1
inner join stockitem 
	on stockitem.stocklocn  	= lineitem.stocklocn 	/* CJB 05/06/00 */
	and stockitem.ItemId 	= lineitem.ItemId	
left join schedule 
	ON lineitem.acctno  = schedule.acctno
	AND	lineitem.ItemId  = schedule.ItemId				-- RI
	AND	lineitem.stocklocn = schedule.stocklocn		/* CB 28/09/00 force a bit more uniqueness */
	AND lineitem.contractno = schedule.contractno /* KEF 19/06/06 Need to join contractno to avoid duplicates */
	AND	schedule.agrmtno = 1			/* MSD 03/12/99 */
WHERE	securitised <> 'Y'					/* KEF 15/04/04 CR597 */
AND NOT (currstatus = 'S' AND outstbal = 0)             /* FR121052 KEF 23/04/03 */
and     cancelledflag           <> 'Y'			/* KEF 13/03/02 */
and not exists (
				select * from fintrans f 
				where f.acctno = summary1.acctno
				and f.transtypecode = 'SCT'
				)
UNION /* RM - add SCT transactions with lineitems from transfer account*/
SELECT Distinct		
	lineitem.acctno,  
	lineitem.itemno,
	lineitem.stocklocn,
	schedule.datedelplan,
	isnull(schedule.loadno,0),			
	schedule.buffno,
	schedule.buffbranchno,			
	isnull(stockitem.itemtype,'S'),			
	lineitem.contractno,
	lineitem.ItemId,					
	lineitem.ParentItemID	 
FROM summary1 
inner join fintrans 
	on fintrans.acctno = summary1.acctno
	and fintrans.transtypecode = 'SCT'	
	and fintrans.acctno like '___9%'
	and summary1.custid not like 'PAID%' 
inner join lineitem 
	ON	fintrans.chequeno = lineitem.acctno
	AND	lineitem.iskit = 0
	AND	lineitem.agrmtno = 1
inner join stockitem 
	on stockitem.stocklocn  	= lineitem.stocklocn 	
	and stockitem.ItemId 	= lineitem.ItemId
left join schedule 
	ON lineitem.acctno  = schedule.acctno
	AND	lineitem.ItemId  = schedule.ItemId				
	AND	lineitem.stocklocn = schedule.stocklocn		
	AND lineitem.contractno = schedule.contractno 
	AND	schedule.agrmtno = 1			
WHERE	securitised <> 'Y'					
AND NOT (currstatus = 'S' AND outstbal = 0)           
and     cancelledflag           <> 'Y'			
--GO

print convert (varchar, getdate()) + ' insert into temp table ended**********************'


--CREATE UNIQUE INDEX ix_smrydata2_non	-- KEF 29/09/03
CREATE INDEX ix_smrydata2_non
ON #summary2_non(
	acctno,
	--itemno,
	ItemId,							-- RI
	stocklocn,
	loadno,
	buffno,
	buffbranchno,
	contractno,
	ParentItemID)	  --IP - 02/11/11
	--parentitemno)   --IZ 14/09/11
--GO

--Create index for acctno
CREATE CLUSTERED INDEX [ix_temp_summary21_non] ON [dbo].[#summary2_non] ([acctno] )

print convert (varchar, getdate()) + ' create index ended**********************'
--GO

	/*
	** UPDATE REMAINING COLUMNS
	*/
SELECT	distinct s2.acctno, isnull(min(f.datetrans),'') AS datedel /* FR121052 KEF 23/04/03 */
INTO	#fintrans_datedel_non
FROM	fintrans f, #summary2_non s2
WHERE	f.transtypecode in ('SCT', 'DEL','CLD')					-- #10138
AND	f.acctno = s2.acctno
GROUP BY s2.acctno
--GO

UPDATE	#summary2_non
SET	datedel = #fintrans_datedel_non.datedel
FROM    #fintrans_datedel_non
WHERE	#summary2_non.acctno = #fintrans_datedel_non.acctno
--GO

print convert (varchar, getdate()) + ' update datedel column ended**********************'
--GO

UPDATE	#summary2_non
SET	itemdescr1         = stockitem.itemdescr1,
	itemdescr2         = stockitem.itemdescr2
FROM	stockitem
--WHERE	#summary2_non.itemno    = stockitem.itemno
WHERE	#summary2_non.ItemId    = stockitem.ItemId				-- RI
AND	#summary2_non.stocklocn = stockitem.stocklocn;
--GO

print convert (varchar, getdate()) + ' update itemdescr columns ended**********************'
--GO

UPDATE	#summary2_non
SET	stocklocnname      = branch.branchname
FROM	branch
WHERE	#summary2_non.stocklocn = branch.branchno;
--GO

print convert (varchar, getdate()) + ' update buffbranchname column ended**********************'


/* Insert data from the temporary table into the permanent summary2_non table */
insert into summary2_non select * from #summary2_non
--go
if exists (select * from sysindexes
	   where name = 'ix_summary21_non')
DROP INDEX [dbo].[summary2_non].[ix_summary21_non]
--go
print convert (varchar, getdate()) + ' insert into summary2_non ended**********************'
--GO

--INDEXES
SET QUOTED_IDENTIFIER ON 
SET ARITHABORT ON 
SET CONCAT_NULL_YIELDS_NULL ON 
SET ANSI_NULLS ON 
SET ANSI_PADDING ON 
SET ANSI_WARNINGS ON 
SET NUMERIC_ROUNDABORT OFF 
--go


--MODIFY TABLE STRUCTURE TO ISAM UNIQUE
CREATE UNIQUE INDEX ix_smrydata2_non
ON summary2_non(
	acctno,
	ItemId,							-- RI
	stocklocn,
	loadno,
	buffno,
	buffbranchno,
	contractno,	/* AA 22/07/03 */
	ParentItemID)	  --IP - 02/11/11
--GO


print convert (varchar, getdate()) + ' summary2 query ended**********************'
--go


SET @Return = @@ERROR
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO
