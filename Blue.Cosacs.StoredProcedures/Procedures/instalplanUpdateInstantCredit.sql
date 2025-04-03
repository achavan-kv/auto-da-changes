IF EXISTS (SELECT * FROM sysobjects WHERE NAME = 	'instalplanUpdateInstantCredit')
DROP PROCEDURE instalplanUpdateInstantCredit
GO 
CREATE PROCEDURE instalplanUpdateInstantCredit @acctno CHAR(12),@InstantCredit VARCHAR(3), @instalmentWaived bit
AS

UPDATE instalplan SET InstantCredit = @instantcredit ,
instalmentWaived =@instalmentWaived
WHERE acctno= @acctno 
GO 