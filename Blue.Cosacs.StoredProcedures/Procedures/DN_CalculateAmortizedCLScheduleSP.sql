IF EXISTS (SELECT * FROM sysobjects WHERE TYPE = 'P' AND NAME = 'DN_CalculateAmortizedCLScheduleSP')
DROP PROCEDURE DN_CalculateAmortizedCLScheduleSP
GO


CREATE PROCEDURE [dbo].[DN_CalculateAmortizedCLScheduleSP]

@principal decimal(15,2),
@servicechgpct float,
@term int,
@instalment decimal(15,2),
@totalservicechg decimal(15,2) output,
@finalinstal decimal(15,2) output,
@return int output

as 

declare @openingbal decimal(15,2)
declare @closingbal decimal(15,2)
declare @principalreduced decimal(15,2)
declare @servicechg decimal(15,2)
declare @counter int

SET 	@return = 0

set @counter=1

set @openingbal = @principal

set @servicechgpct = @servicechgpct / 12 

set @totalservicechg = 0

while(@openingbal > 0)
BEGIN

set @servicechg = @openingbal * @servicechgpct
set @totalservicechg = @totalservicechg + @servicechg

if(@counter=@term and @instalment > @openingbal)
begin
	set @instalment = @openingbal + @servicechg
	set @finalinstal = @instalment
end

set @principalreduced = @instalment	- @servicechg
set @closingbal = @openingbal - @principalreduced

set @openingbal = @closingbal
set @counter = @counter + 1

END

IF (@@error != 0)
BEGIN
	SET @return = @@error
END	
                