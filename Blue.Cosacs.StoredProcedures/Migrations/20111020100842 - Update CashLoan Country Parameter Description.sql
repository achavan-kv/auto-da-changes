-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF EXISTS(select * from countrymaintenance where codename = 'CL_MaxPctRFavail')
BEGIN
	UPDATE countrymaintenance set [Description] = 'This is the percentage of the RF Spend Limit that can be allocated to cash Loans'
	WHERE CodeName = 'CL_MaxPctRFavail'
END