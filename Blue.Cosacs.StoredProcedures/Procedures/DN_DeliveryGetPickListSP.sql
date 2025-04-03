SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_DeliveryGetPickListSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_DeliveryGetPickListSP]
GO

CREATE PROCEDURE 	dbo.DN_DeliveryGetPickListSP
-- ===================================================================
-- Author:		?
-- Create date: ?
-- Description:	
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 22/09/11  IP  RI - #8224 - CR8201 - Picklist printout
--				 1)description needs to be: descr+brand+vendor style long - dependent on country parameter
--				 2)select category description from code table rather than category from stockitem and change heading category/department
--				 dependent on country parameter.
--				 3)select Courts Code dependent on Country Parameter
-- ================================================================
			@picklistno int,
			@branch smallint,
			@reprint bit,
			@amendment bit,
			@return int OUTPUT

AS

	DECLARE @statement sqltext,
			@dispCourtsCode bit																		--IP - 22/09/11 - RI - #8222 - CR8201										
	


select @dispCourtsCode = value from countrymaintenance where codename = 'RIDispCourtsCode'			--IP - 22/09/11 - RI - #8222 - CR8201	

	SET 	@return = 0			--initialise return code

	SET @statement = ' SELECT sc.buffno,' +
		 	 --' ISNULL(st.category, 0) as category,' +
		 	 ' c.codedescript as category,' +														--IP - 22/09/11 - RI - #8224 - CR8201						
		 	 ' st.IUPC as itemno,' +
		 	 ' sc.stocklocn,' +
		 	 ' st.itemdescr1 + ' + ''' ''' + ' + st.itemdescr2 as itemdescr1,' +
		 	 ' sc.quantity,' +
		 	 ' ISNULL(sc.loadno, 0) as loadno,' +
			 ' acctno,' +
			 ' sc.ItemId,' +
			 ' rtrim(ltrim(isnull(st.VendorLongStyle,''''))) as Style,' +							--IP - 22/09/11 - RI - #8224 - CR8201
			 ' rtrim(ltrim(isnull(st.Brand,''''))) as Brand,' +										--IP - 22/09/11 - RI - #8224 - CR8201
			 ' case when ' + convert(varchar,@dispCourtsCode) + ' = 0 then '''' ' +					--IP - 22/09/11 - RI - #8224 - CR8201
			 '		else case when st.ItemNo != '''' then ' + '''(''' + '+st.ItemNo+' + ''')''' +
			 '		else '''' end' +
			 '		 end as CourtsCode' +
	         	 --' FROM schedule sc, stockitem st' +
	         	 ' FROM schedule sc, stockitem st, code c' +										--IP - 22/09/11 - RI - #8224 - CR8201
	         	 ' WHERE sc.picklistnumber = ' + convert(varchar, @picklistno) +
	         	 ' AND	sc.picklistbranchnumber = ' + convert(varchar, @branch) +
	         	 ' AND	sc.itemId = st.itemId' +
	         	 ' AND	sc.stocklocn = st.stocklocn' +
	         	 ' AND  st.category = c.code' +														--IP - 22/09/11 - RI - #8224 - CR8201
	         	 ' AND  c.category in (''PCE'', ''PCF'', ''PCW'', ''PCO'')'							--IP - 22/09/11 - RI - #8224 - CR8201

	IF(@amendment = 0)
	BEGIN
		IF(@reprint = 1)
			SET @statement = @statement + '	AND sc.datepicklistprinted IS NOT NULL'
		ELSE
			SET @statement = @statement + '	AND sc.datepicklistprinted IS NULL'
	END

	SET @statement = @statement + '	ORDER BY sc.itemno'

	EXECUTE sp_executesql @statement

	SET @return = @@error
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

