
IF  EXISTS (SELECT * FROM sysobjects WHERE name = 'CustTelAuditGet' AND type in ('P', 'PC'))
DROP PROCEDURE [dbo].[CustTelAuditGet]

GO

CREATE PROC [dbo].[CustTelAuditGet]
@CustID VARCHAR(12),
@Return INT OUT
AS

SET @Return = 0

SELECT custid,
	   datechange,
	   tellocn,
	   dateteladd,
	   datediscon,
	   telno,
	   extnno,
	   	CASE WHEN tellocn ='M' THEN ''
	   ELSE DialCode END AS DialCode,
	   empeenochange,
	   ChangeType FROM custtelAudit
	WHERE CustID = @CustID
		ORDER BY datechange desc
GO