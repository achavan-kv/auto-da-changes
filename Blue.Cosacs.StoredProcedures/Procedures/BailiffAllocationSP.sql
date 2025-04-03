IF EXISTS (select * from dbo.sysobjects where id = object_id(N'[dbo].[BailiffAllocationSP]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[BailiffAllocationSP]
GO
create PROCEDURE [dbo].[BailiffAllocationSP]

-- Tally man import only.
-- This has been commented out because it is rubbish and not used!
-- Uncomment for a good time.


-- ================================================
-- Project      : CoSACS .NET
-- File Name    : BailiffAllocationSP.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Bailiff Allocation from Tallyman 
-- Author       : John Croft
-- Date         : June 2008
--
-- This procedure will allocate accounts to a Dummy Bailiff depending on the segment ID in Tallyman.
-- 
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 08/06/09  AA  using segment name rather than segment id
-- 15/02/10  jec CR1072 Malaysia Merge
-- ================================================
	-- Add the parameters for the stored procedure here

as

----drop table #folluptemp		-- testing only!!!!!
---- create temporary table
--select top 0 * into #folluptemp from follupalloc

---- populate data from Tallyman segment data
--insert into #folluptemp
--(origbr,acctno,allocno,empeeno,
--datealloc,datedealloc,allocarrears,bailfee,
--allocprtflag,empeenoalloc,empeenodealloc)
-- select CAST(LEFT(Account_Number,3) as int),Account_Number,0,0,
--GETDATE(),null,0,0,
--' ',99999,0
--	from TM_Segments T 
--WHERE Segment_Name in ('Accounts In Salvage Actions',
--'Accounts with ECA',
--'Auto Allocation (Coll)',
--'Bailiff Allocated (Auto)',
--'Bailiff Allocated (Manual)',
--'Bailiff Allocation (Auto)',
--'Bailiff Allocation (Manual)',
--'Collector Allocated (Auto)',
--'Collector Allocated (Manual)',
--'ECA Actions',
--'Manual Allocation (Coll)')
----	where Segment_ID in(171,141,203,244,245,226,227,246,247,248,204)
---- and not already allocated to an employee
--	and not exists(select * from follupalloc f 
--			where T.Account_Number=f.acctno and datedealloc is null)
	
---- set allocno and arrears and Dummy Bailiff empeeno
--update #folluptemp
--	set allocno=(select (ISNULL(MAX(allocno),0) +1) from follupalloc
--			where #folluptemp.acctno=follupalloc.acctno),empeeno=9123, 
--					allocarrears=ISNULL((select ISNULL(arrears,0) from acct 
--			where #folluptemp.acctno=acct.acctno),0)

---- Delete accounts less than 4 months in arrears
--delete #folluptemp
--	from #folluptemp f INNER JOIN instalplan i on f.acctno=i.acctno
--	where i.instalamount>0
--		and (f.allocarrears/i.instalamount)<4


------ set Dummy Bailiff empeeno
----update #folluptemp
----	set empeeno=ISNULL((select empeeno from courtsperson where empeename='Dummy Bailiff'),9123)

---- Insert into main table
--insert into follupalloc 
--		select * from #folluptemp

---- update number of accounts allocated
--Update courtsperson
--	set alloccount=alloccount+ (select COUNT(*) from #folluptemp)
--		where empeeno=9123

--drop table #folluptemp
GO
-- End End End End End End End End End End End End End End End End End End End End End End End End 
