
IF  EXISTS (SELECT * FROM sysobjects WHERE name = 'trig_audit_tel' AND type='TR')
DROP TRIGGER [dbo].[trig_audit_tel]

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE trigger [trig_audit_tel]
ON [dbo].[custtel]
for INSERT,update

AS

BEGIN

DECLARE
	@datechangeprev		DATETIME,
    @delcustid			varchar(20),
    @deltellocn			char(2),
    @deldatechange		DATETIME,
	@deldateteladd		DATETIME,
	@deldatediscon		DATETIME,
	@deltelno			VARCHAR(20),
	@delextnno			VARCHAR(6),
	@delDialCode		CHAR(8),
	@delempeenochange	INT,
	@inscustid			varchar(20),
    @instellocn			char(2),
    @insdatechange		DATETIME,
	@insdateteladd		DATETIME,
	@insdatediscon		DATETIME,
	@instelno			VARCHAR(20),
	@insextnno			VARCHAR(6),
	@insDialCode		CHAR(8),
	@insempeenochange	INT


SELECT  @delcustid = d.custid,
		@deltellocn	= d.tellocn,
		@deldatechange = d.datechange,
		@deldateteladd = d.dateteladd,
		@deldatediscon = d.datediscon,
		@deltelno = d.telno,
		@delextnno = d.extnno,
		@delDialCode = d.DialCode,
		@delempeenochange = d.empeenochange,
		@inscustid = i.custid,
		@instellocn	= i.tellocn,
		@insdatechange = i.datechange,
		@insdateteladd = i.dateteladd,
		@insdatediscon = i.datediscon,
		@instelno = i.telno,
		@insextnno = i.extnno,
		@insDialCode = i.DialCode,
		@insempeenochange = i.empeenochange
	FROM deleted d
		FULL OUTER JOIN inserted i
			ON d.custid = i.custid
			AND d.tellocn = i.tellocn

IF not @delcustid IS null
begin
	--update
--check if previous record added
IF NOT EXISTS(SELECT * FROM custtelaudit c where
			c.custid = @delcustid AND c.tellocn = @deltellocn AND  c.dateteladd = @deldateteladd  )
	BEGIN
		IF @delcustid !=NULL 
		INSERT INTO custtelaudit
			(custid,tellocn,datechange,dateteladd,datediscon,telno,extnno,DialCode,
			empeenochange,changetype)
		SELECT @delcustid,@deltellocn,@deldatechange,@deldateteladd,@deldatediscon,@deltelno,@delextnno,@delDialCode,
			--@delempeenochange, 'Original'
			ISNULL(@delempeenochange,ISNULL(@insempeenochange,0)), 'Original'	-- UAT561 jec 09/10/08
	END
   IF @inscustid IS NOT NULL 
   INSERT INTO custtelaudit
		(custid,tellocn,datechange,dateteladd,datediscon,telno,extnno,DialCode,
		empeenochange,changetype)
	SELECT @inscustid,@instellocn,@insdatechange,@insdateteladd,@insdatediscon,@instelno,@insextnno,@insDialCode,
		ISNULL(@insempeenochange,0), 'Update'			-- UAT561 jec 09/10/08

END
ELSE
BEGIN

--insert
SELECT @datechangeprev = datechange
	FROM custtel
		WHERE custid =@inscustid AND tellocn = @instellocn
			AND datechange < @insdatechange
	ORDER BY datechange DESC

	IF EXISTS(	SELECT * FROM custtel
			WHERE custid =@inscustid AND 
					tellocn = @instellocn AND 
					((datechange < @insdatechange or datechange is null) OR dateteladd <> @insdateteladd) AND 
					tellocn = @instellocn )
	BEGIN
		IF @inscustid IS NOT NULL 
			INSERT INTO custtelaudit
				(custid,tellocn,datechange,dateteladd,datediscon,telno,extnno,DialCode,
				empeenochange,changetype)
			SELECT @inscustid,@instellocn,@insdatechange,@insdateteladd,@insdatediscon,@instelno,@insextnno,@insDialCode,
				ISNULL(@insempeenochange,0), 'Insert'			-- UAT561 jec 09/10/08

		--chack if previous record added
		IF NOT EXISTS(SELECT * FROM custtelaudit WHERE
			@inscustid =custid AND @instellocn = tellocn AND  @datechangeprev = datechange  
			AND dateteladd =@insdateteladd  )
		BEGIN
		INSERT INTO custtelaudit
			(custid,tellocn,datechange,dateteladd,datediscon,telno,extnno,DialCode,
			empeenochange,changetype)
		SELECT custid,tellocn,datechange,dateteladd,datediscon,telno,extnno,DialCode,
			ISNULL(empeenochange,0), 'Update'				-- UAT561 jec 09/10/08
			FROM custtel
			WHERE
				@inscustid =custid AND @instellocn = tellocn AND  @datechangeprev = datechange
				AND @inscustid IS NOT NULL 
		END
	END
END
END
GO
