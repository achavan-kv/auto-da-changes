/*
	Author : Rahul D
	Date   : 13/06/2019
	Details: The SQL - Function fn_CLAmortizationReturnAdminFee will accepts account no as a parameter and 
			 returns total admin fee due for that date.
*/
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[fn_CLAmortizationReturnAdminFee]') AND xtype in (N'FN', N'IF', N'TF'))
DROP FUNCTION [dbo].fn_CLAmortizationReturnAdminFee
go 
 

CREATE FUNCTION fn_CLAmortizationReturnAdminFee(@acctno varchar(12))
RETURNS MONEY  
AS
BEGIN  
	-- Variable Declaration
	DECLARE @admin MONEY =0

	-- Sum up the Admin fee for missed installment
	SELECT @admin = SUM(adminfee) 
	FROM CLAmortizationPaymentHistory 
	WHERE acctno = @acctno and CAST(instalduedate AS DATE) < CAST (GETDATE() AS DATE) and isPaid = 0
	
	RETURN(SELECT isnull(@admin,0))  
END