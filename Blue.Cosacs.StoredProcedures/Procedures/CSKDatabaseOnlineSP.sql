IF EXISTS (SELECT * FROM sysobjects WHERE NAME = 'CSKDatabaseOnlineSP') 
DROP PROCEDURE CSKDatabaseOnlineSP
go
create PROCEDURE CSKDatabaseOnlineSP
@user INT 
AS
DECLARE @return INT 
BEGIN TRY
set @return=0 


IF EXISTS(SELECT 1 FROM Admin.[User]  WHERE id = @user)
begin 
set @return=1   -- user exists

declare @mustdeposit bit
exec DN_CashierMustDepositSP @empeeno=810101,@mustdeposit=@mustdeposit output,@return=0
if @mustdeposit = 1
	set @return = 2

end 
END TRY
BEGIN CATCH
set @return=0 -- due to error
END CATCH
SELECT @return 
RETURN @return 

go 