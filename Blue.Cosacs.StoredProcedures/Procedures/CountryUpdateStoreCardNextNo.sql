-- Issue is need to update country parameter....
-- 

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[CountryUpdateStoreCardNextNo]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[CountryUpdateStoreCardNextNo]
GO

CREATE PROCEDURE [dbo].[CountryUpdateStoreCardNextNo] @latestnumber INT OUT 
AS 
--#log no #4761758 --26/02/2018 fix for Storecard new number generation lock issue.

declare @isCount int
declare @Actuallastnumber int
set @isCount =0
select @isCount = count(lastnumber) from countrymaintenance_Stageing

	if(@isCount = 0)
	begin
		SELECT @Actuallastnumber = CAST(value as int) FROM countrymaintenance
		WHERE  codename  = 'StoreCardNumber'
		
	end 
	else
	begin
		select @Actuallastnumber = max(lastnumber) from countrymaintenance_Stageing
	end 
	set @Actuallastnumber = @Actuallastnumber + 1
	
	
	Insert into countrymaintenance_Stageing values(@Actuallastnumber)
	select * from countrymaintenance_Stageing
	set @latestnumber = @Actuallastnumber

	GO