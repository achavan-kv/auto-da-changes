set ANSI_NULLS ON
set QUOTED_IDENTIFIER ON
go

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[GetIsGiftItem]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[GetIsGiftItem]
GO



create procedure [dbo].[GetIsGiftItem]
--------------------------------------------------------------------------------
-- Project      : eCoSACS r 2002 Strategic Thought Ltd.
-- File Name    : GetIsGiftItem.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Checks if item is a gift item
-- Author       : ??
-- Date         : ??
--
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 07/06/11 jec  CR1212 RI Integration use itemid instead of itemno
--------------------------------------------------------------------------------
    -- Parameters
		--@itemno varchar(8),
		@itemId int,		-- RI
		@stocklocn smallint,
		@IsGiftItem bit out,
		@Return int out
as
if exists
(
select * from stockitem
	inner join code
		on stockitem.category = code.code
	where code.category = 'FGC'
	--and itemno = @itemno and stocklocn = @stocklocn
	and itemId = @itemId and stocklocn = @stocklocn		-- RI
)
	set @IsGiftItem = 1
else
	set @IsgiftItem = 0

set @return = 0


-- End End End End End End End End End End End End End End End End End End End End End End End End End End End