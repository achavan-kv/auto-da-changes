
-- additional stored procs for BasicCustomerDetails
IF  EXISTS (SELECT * FROM sysobjects WHERE name = 'CustomerAddressAuditGet' AND type in ('P', 'PC'))
DROP PROCEDURE [dbo].[CustomerAddressAuditGet]
GO
CREATE PROC [dbo].[CustomerAddressAuditGet]
@CustID VARCHAR(12),
@Return INT out
AS

SET @return = 0

SELECT custid,
	   addtype,
	   datechange,
	   datein,
	   ISNULL(DateMoved, CONVERT(DATETIME,'1 Jan 1900',106)) AS DateMoved,
	   cusaddr1,
	   cusaddr2,
	   cusaddr3,
	   cuspocode,
	   Email,
	   empeenochange,
	   ChangeType FROM Custaddressaudit
	WHERE custID  =@CustID
		ORDER BY datechange DESC, datein desc
GO
