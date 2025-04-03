GO
/****** Object:  Trigger [trig_audit_address]    Script Date: 07/07/2008 11:09:44 ******/
IF  EXISTS (SELECT * FROM sysobjects WHERE name = 'trig_audit_address' AND type='TR')
DROP TRIGGER [dbo].[trig_audit_address]
GO
/****** Object:  Trigger [trig_audit_address]    Script Date: 07/07/2008 11:10:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE trigger [trig_audit_address]
ON [dbo].[custaddress]
for INSERT,update

AS


BEGIN

DECLARE
    @datechangeprev DATETIME,
	@delcustid			varchar(20), --IP - 11/05/10 - UAT(128) UAT5.2.1.0 log - Changed from Varchar(15)
    @deladdtype			char(1),
    @deldatechange		DATETIME,
	@deldatein			DATETIME,
	@deldatemoved		DATETIME,
	@delcusaddr1		VARCHAR(50),
	@delcusaddr2		VARCHAR(50),
	@delcusaddr3		VARCHAR(50),
	@delcuspocode		VARCHAR(10),
	@delemail			CHAR(60),
	@delempeenochange	INT,
	@inscustid			varchar(20),	 --IP - 11/05/10 - UAT(128) UAT5.2.1.0 log - Changed from Varchar(15)
    @insaddtype			char(1),
    @insdatechange		DATETIME,
	@insdatein			DATETIME,
	@insdatemoved		DATETIME,
	@inscusaddr1		VARCHAR(50),
	@inscusaddr2		VARCHAR(50),
	@inscusaddr3		VARCHAR(50),
	@inscuspocode		VARCHAR(10),
	@insemail			CHAR(60),
	@insempeenochange	INT


SELECT		@delcustid = d.custid,
			@deladdtype = d.addtype,
			@deldatechange = d.datechange,
			@deldatein = d.datein,
			@deldatemoved = d.datemoved,
			@delcusaddr1 = d.cusaddr1,
			@delcusaddr2 = d.cusaddr2,
			@delcusaddr3 = d.cusaddr3,
			@delcuspocode = d.cuspocode,
			@delemail = d.email,
			@delempeenochange = d.empeenochange,
			@inscustid = i.custid,
			@insaddtype = i.addtype,
			@insdatechange = i.datechange,
			@insdatein = i.datein,
			@insdatemoved = i.datemoved,
			@inscusaddr1 = i.cusaddr1,
			@inscusaddr2 = i.cusaddr2,
			@inscusaddr3 = i.cusaddr3,
			@inscuspocode = i.cuspocode,
			@insemail = i.email,
			@insempeenochange = i.empeenochange
	FROM deleted d
		FULL OUTER JOIN inserted i
			ON d.custid = i.custid
			AND d.addtype = i.addtype

IF not @delcustid IS null
begin
	--update
	--check if previous record added
	IF NOT EXISTS(SELECT * FROM custaddressaudit c where
			c.custid = @delcustid AND c.addtype = @deladdtype AND  c.datein = @deldatein  )
	BEGIN
		INSERT INTO custaddressaudit
			(custid,addtype,datechange,datein,datemoved,cusaddr1,cusaddr2,cusaddr3,
				cuspocode,Email,empeenochange,changetype)
		SELECT @delcustid,@deladdtype,@deldatechange,@deldatein,@deldatemoved,@delcusaddr1,@delcusaddr2,@delcusaddr3,
			@delcuspocode,@delEmail,@delempeenochange, 'Original'
	END

	INSERT INTO custaddressaudit
		(custid,addtype,datechange,datein,datemoved,cusaddr1,cusaddr2,cusaddr3,
		cuspocode,Email,empeenochange,changetype)
    SELECT @inscustid,@insaddtype,@insdatechange,@insdatein,@insdatemoved,@inscusaddr1,@inscusaddr2,@inscusaddr3,
		@inscuspocode,@insEmail,@insempeenochange, 'Update'
END
ELSE
BEGIN
   --insert
	SELECT @datechangeprev = datechange
		FROM custaddress
			WHERE custid =@inscustid AND addtype = @insaddtype
				AND datechange < @insdatechange
		ORDER BY datechange DESC

	--if @datechangeprev is null, no previous records, new customer, no need to audit
	-- THIS IS NOT GETTING ADDED SOMETIMES AS RECORDS EXIST WITH DATECHANGE VALUE NULL

	-- if we add a new address and have one previous address record,
	-- then this code would not pick it up as the datechange for that record has been changed
	-- ot same as our value so i add a check for datein
	IF EXISTS(	SELECT * FROM custaddress
			WHERE custid =@inscustid AND addtype = @insaddtype AND 
			((datechange < @insdatechange or datechange is null) OR datein <> @insdatein))
	begin
		INSERT INTO custaddressaudit
			(custid,addtype,datechange,datein,datemoved,cusaddr1,cusaddr2,cusaddr3,
				cuspocode,Email,empeenochange,changetype)
		SELECT @inscustid,@insaddtype,@insdatechange,@insdatein,@insdatemoved,@inscusaddr1,@inscusaddr2,@inscusaddr3,
			@inscuspocode,@insEmail,@insempeenochange, 'Insert'


		--chack if previous record added
		IF NOT EXISTS(SELECT * FROM custaddressaudit WHERE
			@inscustid =custid AND @insaddtype = addtype AND  @datechangeprev = datechange
			AND datein =@insdatein  )
		BEGIN
			INSERT INTO custaddressaudit
				(custid,addtype,datechange,datein,datemoved,cusaddr1,cusaddr2,cusaddr3,
					cuspocode,Email,empeenochange,changetype)
			SELECT custid,addtype,datechange,datein,datemoved,cusaddr1,cusaddr2,cusaddr3,
				cuspocode,Email,empeenochange, 'Original'
			FROM custaddress
			WHERE
				@inscustid =custid AND @insaddtype = addtype AND  @datechangeprev = datechange
    	
		END
	END
END

END
GO