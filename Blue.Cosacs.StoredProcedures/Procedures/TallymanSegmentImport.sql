--alter PROC TallymanSegmentImport

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[TallymanSegmentImport]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[TallymanSegmentImport]
GO
-- Not required in 5.2 - jec 09/02/10 - Malaysia Merge
--alter table Tm_Segments alter column segment_name varchar(32)
--go

Create Procedure [dbo].[TallymanSegmentImport]
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : TallymanSegmentImport.prc
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Tallyman Segment Import
-- Author       : Richard Boyce
-- Date         : March 2008
--
-- This procedure will import the Tallyman Segment details
-- 
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 12/05/08  jec UAT189 Change segment column to 32
-- 06/06/08  jec UAT242 Add Bailiff Allocation procedure.
-- 23/06/08  AA  Error on running import for allocation procedure
-- ================================================
	-- Add the parameters for the stored procedure here
as

-- create temporary table to hold csv data
CREATE TABLE #SegmentTemp
(
	acctNo CHAR(12),
	segment VARCHAR(32)
)

-- import data to table
BULK INSERT #SegmentTemp
	FROM 'd:\cosdata\Tsegment.csv'
	WITH
(
	FieldTerminator = ',',
	RowTerminator = '\n'
)

-- update existing accounts
UPDATE tms
	SET Segment_ID=1,
		Segment_name = segment,
		Date = GetDate()
	FROM Tm_Segments tms
		INNER JOIN #SegmentTemp temp
			ON tms.account_number = temp.acctno


-- insert new accounts
INSERT INTO  Tm_Segments (account_number, segment_name, Date,Segment_ID)
	select acctno, segment,GetDate(),1
	FROM Tm_Segments tms
		RIGHT outer JOIN #SegmentTemp temp
			ON tms.account_number = temp.acctno
		WHERE tms.account_number IS null

-- Bailiff Allocation		-- jec 06/06/08 
	exec BailiffAllocationSP


-- drop temporary table
DROP table #SegmentTemp


GO
-- End End End End End End End End End End End End End End End End End End End End End End End 
