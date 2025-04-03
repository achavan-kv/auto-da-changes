SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_DeliveryGetTransportPickListSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_DeliveryGetTransportPickListSP]
GO

CREATE PROCEDURE 	dbo.DN_DeliveryGetTransportPickListSP
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : DN_DeliveryGetTransportPickListSP.prc
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : DN_DeliveryGetTransportPickListSP 
-- Author       : ??
-- Date         : ??
--
-- This procedure will retreive the detaill to be printed on the Transport Picklist
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 20/12/07  jec Error when printing. acctno not being retrieved.
-- ================================================
	-- Add the parameters for the stored procedure here
			@picklistno int,
			@branch smallint,
			@reprint bit,
			@amendment bit,
			@return int OUTPUT

AS

	DECLARE @statement sqltext

	SET 	@return = 0			--initialise return code

	SET @statement = ' SELECT sc.buffno,' +
		 	 ' ISNULL(st.category, 0) as category,' +
		 	 ' st.IUPC as itemno,' +
		 	 ' sc.stocklocn,' +
		 	 ' st.itemdescr1 + ' + ''' ''' + ' + st.itemdescr2 as itemdescr1,' +
		 	 ' sc.quantity,' +
		 	 ' ISNULL(sc.loadno, 0) as loadno,' +
			 ' acctno,' +		-- jec 20/12/07 uat 327
			 ' sc.ItemId ' +
	         	 ' FROM schedule sc, stockitem st' +
	         	 ' WHERE sc.transchedno = ' + convert(varchar, @picklistno) +
	         	 ' AND	sc.transchednobranch = ' + convert(varchar, @branch) +
	         	 ' AND	sc.itemId = st.itemId' +
	         	 ' AND	sc.stocklocn = st.stocklocn'

	IF(@amendment = 0)
	BEGIN
		IF(@reprint = 1)
			SET @statement = @statement + '	AND sc.datetranschednoprinted IS NOT NULL'
		ELSE
			SET @statement = @statement + '	AND sc.datetranschednoprinted IS NULL'
	END

	SET @statement = @statement + '	ORDER BY sc.itemno'

	EXECUTE sp_executesql @statement

	SET @return = @@error
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

