
if exists (select * from sysobjects where type='TR'
		and name = 'tr_interfacecontrol_update')
DROP TRIGGER tr_interfacecontrol_update
GO

CREATE TRIGGER tr_interfacecontrol_update ON interfacecontrol
FOR UPDATE
AS

DECLARE @result varchar (1),
	@interface varchar (12),
	@datestart datetime,
    @runno int

SELECT @result = result, @interface = interface, @datestart = datestart FROM inserted
if @result = 'P' and @interface = 'CHARGES'
begin
	update countrymaintenance
	set    value = convert(varchar,@datestart)
	where  name = 'Last Successful Run Date'
end
GO


