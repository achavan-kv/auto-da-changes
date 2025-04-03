IF EXISTS (SELECT * FROM sysobjects WHERE NAME = 'CM_LoadUnallocatedAddressZones')
DROP PROCEDURE CM_LoadUnallocatedAddressZones
GO 

CREATE PROCEDURE CM_LoadUnallocatedAddressZones @return INTEGER OUT
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : CM_LoadUnallocatedAddressZones.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : 
-- Date         : ??
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 16/01/12  ip  #9463 - Time out encountered. Only select for current addresses
-- ================================================
AS 
	SET @return = 0 

	SELECT TOP 200 ca.custid, ca.cusaddr1,ca.cusaddr2,ca.cusaddr3,ca.cuspocode, c.hldorjnt
    FROM custaddress ca
	join custacct c ON c.custid = ca.custid
	WHERE ISNULL(zone,'') = ''
	and datechange is null									--IP - 16/01/12 - #9463
    order by NewID()
GO 
