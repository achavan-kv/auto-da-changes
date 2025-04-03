
IF  EXISTS (SELECT * FROM sysobjects WHERE name = 'CustTelGet' AND type in ('P', 'PC'))
DROP PROCEDURE [dbo].[CustTelGet]

GO
/****** Object:  StoredProcedure [dbo].[CustTelGet]    Script Date: 07/07/2008 11:25:36 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[CustTelGet]
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : CustTelGet.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Get Telephone details
-- Author       : ??
-- Date         : ??
--
-- This procedure will retreive the Telephone details.
-- 
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 20/05/09  jec UAT576 The ?Date To? column should NOT display (null) and should display ?1/1/1900? consistent. 
-- 23/06/20  Snehalata Tilekar Address Standardization CR2019 - 025 -Added Multiple delivery mobile to keep dilacode value empty. 
-- =================================================================================
	-- Add the parameters for the stored procedure here
@CustID VARCHAR(12),
@Return INT OUT
AS
SET NOCOUNT ON --Address Standardization CR2019 - 025			
SET @Return = 0

SELECT origbr,
	   custid,
	   tellocn,
	   dateteladd,
	   ISNULL(DateDiscon, CONVERT(DATETIME,'1 Jan 1900',106)) AS DateDiscon,
	   telno,
	   extnno,
	   CASE
	  WHEN tellocn IN ('M','DM','D1M','D2M','D3M') THEN ' '  --Address Standardization CR2019 - 025			
	   ELSE DialCode END AS DialCode,
	   empeenochange,
	   datechange FROM custtel
	WHERE CustID = @CustID
		ORDER BY datechange DESC



GO


-- End End End End End End End End End End End End End End End End End End End End End End End End End End End End