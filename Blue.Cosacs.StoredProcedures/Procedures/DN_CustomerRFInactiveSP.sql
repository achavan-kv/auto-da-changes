SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_CustomerRFInactiveSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_CustomerRFInactiveSP]
GO

CREATE PROCEDURE 	dbo.DN_CustomerRFInactiveSP
			@custid varchar(20),
			@inactive bit OUT,
			@return int OUTPUT

AS

	DECLARE	@total money
	--RM 72254 inactive should be based on 
	--no rescore for X months and status parameters
	 Declare @daterescore datetime
	 declare @maxstatus varchar(3)
	 declare @rescoremonths int
	 declare @rescorestatus int
 
	SET 		@return = 0			--initialise return code

	SET		@inactive = 0
	SET		@total = 0
	
	SELECT 	@inactive = count(*)
	FROM		custcatcode
	WHERE	custid = @custid
	AND		code = 'REX' 
	AND		datedeleted is null 

	if (@inactive = 0)
	 BEGIN
 
		select @rescoremonths = value
		from CountryMaintenance
		where CodeName = 'rescoremonths'
	
		select @rescorestatus = convert(int, value)
		from CountryMaintenance
		where CodeName = 'rescorestatus'

		select @daterescore = max(datelastscored)
		from customer
			where custid = @custid
	
		select @maxstatus = MAX(convert(int, currstatus))
		from acct a
		inner join custacct c on c.custid = @custid
				and c.acctno = a.acctno
				and c.hldorjnt = 'H'
		where ISNUMERIC(currstatus) = 1
	
		
		if(datediff(month, @daterescore, GETDATE()) > @rescoremonths and @maxstatus > @rescorestatus)
			set @inactive = 1
		else
			set @inactive = 0
	
 
	 END


	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

