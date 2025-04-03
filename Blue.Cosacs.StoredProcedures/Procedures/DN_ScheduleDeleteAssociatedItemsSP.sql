SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS OFF
GO

if exists (select * from dbo.sysobjects
where id = object_id('[dbo].[DN_ScheduleDeleteAssociatedItemsSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_ScheduleDeleteAssociatedItemsSP]
GO

CREATE PROCEDURE dbo.DN_ScheduleDeleteAssociatedItemsSP
/***********************************************************************************************************
--
-- Project      : CoSACS .NET
-- File Name    : DN_ScheduleDeleteAssociatedItemsSP.PRC
-- File Type    : MSSQL Server Stored Procedure Script
-- Description  : 
--				
-- Author       : ?
-- Date         : ?
--
--  
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 06/06/11  IP  CR1212 - RI - #3806 - Use ItemID
-- 12/01/12  IP/JC #9440 - Include Installation Items
************************************************************************************************************/
    @acctno         varchar(12),
    @agreementno    int,
    --@itemno         varchar(8),
    @itemID			int,					--IP - 06/06/11 - CR1212 - RI - #3806
    @stocklocn      smallint,
    @changedby      int,
    @return         int OUTPUT

AS

    SET @return = 0

    DELETE FROM schedule
    WHERE EXISTS   (SELECT  1
                    FROM    lineitem l--, stockitem s
                    inner join StockInfo si on l.ItemID = si.ID
                    inner join StockQuantity sq on l.ItemID = sq.ID and l.stocklocn = sq.stocklocn
                    WHERE   schedule.acctno = @acctno
                    AND     schedule.agrmtno = @agreementno
                    --AND     schedule.itemno = l.itemno
                    AND		schedule.ItemID = l.ItemID				--IP - 06/06/11 - CR1212 - RI - #3806
                    AND     schedule.stocklocn = l.stocklocn
                    AND     schedule.contractno = l.contractno
                    AND     l.acctno = schedule.acctno
                    --AND     l.parentitemno = @itemno
                    AND		l.ParentItemID = @itemID				--IP - 06/06/11 - CR1212 - RI - #3806
                    AND     l.parentlocation = @stocklocn
                    --AND     l.itemno = s.itemno
                    --AND     l.stocklocn = s.stocklocn
                    --AND      s.itemtype = 'N'
                    AND		si.itemtype = 'N'
                    --AND     s.category IN(12,82,36,37,38,39,46,47,48,86,87,88)
                    --AND     si.category IN(select distinct code from code where category in ('WAR', 'PCDIS')) --IP - 29/10/09 - CoSACS Improvement - Remove hardcoded discounts & warranties --IP - 06/06/11 - CR1212 - RI - #3806
                    AND	(si.category IN(select distinct code from code where category in ('WAR', 'PCDIS'))		
						or	si.category IN(select distinct reference from code where category in ('INST')))		--IP/JC  12/01/12 - #9440
                    AND     l.quantity != 0)


    SET @return = @@error

GO
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO
