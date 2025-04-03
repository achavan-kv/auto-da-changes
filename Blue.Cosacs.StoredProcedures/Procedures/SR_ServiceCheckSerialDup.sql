IF EXISTS (SELECT *
           FROM sysobjects
           WHERE xtype = 'P'
           AND name = 'SR_ServiceCheckSerialDup')
BEGIN
DROP PROCEDURE SR_ServiceCheckSerialDup
END
GO


CREATE PROCEDURE SR_ServiceCheckSerialDup
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : SR_ServiceCheckSerialDup.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Checks for duplicate Serial Numbers 
-- Author       : Peter Chong
-- Date         : 19-Oct-2006
--
-- This procedure will check to see if a duplicate serial number has been entered.

-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 15/11/10 jec CR1030 Check duplicates using account number
-- 26/07/11 ip  RI - System Integration
-- ================================================
	-- Add the parameters for the stored procedure here
--@itemno varchar(8),
@itemno varchar(18),		--IP - 26/07/11 - RI
@serialno VARCHAR(30),
@modelno VARCHAR(30),
@accountno CHAR(12),		--CR1030 
@return int output 
AS
BEGIN
    IF EXISTS ( SELECT * 
                FROM SR_ServiceRequest
                WHERE RTRIM(LTRIM(productcode)) = @itemno
                AND RTRIM(LTRIM(SerialNo)) = @serialno
                AND RTRIM(LTRIM(ModelNo)) = @modelno
                and AcctNo!=@accountno)	--CR1030 jec	
    BEGIN
        SELECT 1 
    END
    ELSE
    BEGIN
        SELECT 0
    END
END
GO

-- End End End End End End End End End End End End End End End End End End End End End End End End End End

