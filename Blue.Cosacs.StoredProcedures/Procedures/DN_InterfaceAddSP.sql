SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_InterfaceAddSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_InterfaceAddSP]
GO

CREATE PROCEDURE 	dbo.DN_InterfaceAddSP
			@interface varchar(10),
			@runno int OUTPUT,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code
	SET  	@runno = 0
   -- we are chacking the result and for certain functions only generating a new one if the last procedure passed.
   declare @result varchar(1)
   if @interface in ('COS FACT','CHARGES')
      set @result = 'P'
   else
      set @result = '%'
   if @interface ='CHARGES'
      	SELECT	@runno = lastchargesweekno + 1
      	FROM    	country

   if @runno = 0
   	SELECT	@runno = ISNULL(MAX(runno),0) + 1
	   FROM    	interfacecontrol
   	WHERE   	interface = UPPER(@interface) and result like @result;

   update interfacecontrol set datestart =GETDATE(),datefinish = '1-jan-1900'
   where interface = @interface and runno = @runno
   if @@rowcount = 0
   	INSERT INTO interfacecontrol(interface, runno, datestart, datefinish)
      VALUES(upper(@interface), @runno, GETDATE(), '')

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO


