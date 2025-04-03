SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_SUCBGetDelDetailsSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_SUCBGetDelDetailsSP]
GO

CREATE PROCEDURE 	dbo.DN_SUCBGetDelDetailsSP
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : DN_SUCBGetDelDetailsSP.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : 
-- Author       : ??
-- Date         : ??
--
--
-- 
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 20/02/12  IP #9423 - CR8262 - Flag Store Card transactions and order ensuring regular
--			    transactions are displayed first
-- ================================================
			--@runno int,
			@datetrans varchar(20),									--IP - 20/02/12 - #9423 - CR8262
			@branch varchar(5),
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	SELECT 	acctno,
		itemno,
		Quantity,
		BuffNo,
		stocklocn,
		transvalue,
		contractno,
		ISNULL(retitemno,'') as retitemno,
		ISNULL(retstocklocn, 0) as retstocklocn,
		case when substring(acctno, 4,1) = '9' then 'Y'																		--IP - 20/02/12 - #9423 - CR8262
		when substring(acctno, 4,1) = '4' and exists(select * from view_FintranswithTransfers v
															inner join storecard sc on v.acctno = sc.acctno
															where v.transferaccount = d.acctno
															and v.code = 'SCT') then 'Y' else 'N' end as StoreCard
	FROM	delivery d																										--IP - 20/02/12 - #9423 - CR8262																
	--WHERE	runno = @runno
	WHERE	d.datetrans between cast(@datetrans as datetime) AND  dateadd(mi,-1,dateadd(d, 1, cast(@datetrans as datetime)))
	AND	acctno like @branch
	ORDER BY d.acctno, StoreCard																							--IP - 20/02/12 - #9423 - CR8262

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

