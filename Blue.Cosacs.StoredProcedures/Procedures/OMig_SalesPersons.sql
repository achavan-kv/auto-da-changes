SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS OFF
GO

IF EXISTS (SELECT * FROM SysObjects
           WHERE id = object_id('[dbo].OMig_SalesPersons') and OBJECTPROPERTY(id, 'IsProcedure') = 1)

BEGIN
    DROP PROCEDURE OMig_SalesPersons
END
GO

CREATE PROCEDURE dbo.OMig_SalesPersons 

-- ================================================
-- Project      : CoSACS .NET
-- File Name    : OMig_SalesPersons.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Migrate SalesPerson Details
-- Author       : John Croft
-- Date         : 16 July 2008
--
-- This procedure will create csv file for the SalesPerson Interface into Oracle
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 
-- ================================================
	-- Add the parameters for the stored procedure here

as

IF EXISTS (SELECT table_name FROM INFORMATION_SCHEMA.TABLES
	   WHERE  table_schema = 'dbo' and table_name = 'SalesPersonInterface')
	drop table SalesPersonInterface

Select FullName AS EmployeeName,Login
Into SalesPersonInterface	
From Admin.[User]

declare @path varchar(200)

set @path = '"c:\program files\microsoft sql server\80\tools\binn\BCP" ' + db_name()+'..SalesPersonInterface' + ' out ' +
'd:\users\default\SalesPersonInterface.csv ' + '-c -t, -q -T'

exec master.dbo.xp_cmdshell @path

-- End End End End End End End End End End End End End End End End End End End End End End End End 
