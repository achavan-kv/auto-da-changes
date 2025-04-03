IF EXISTS(SELECT name FROM sysobjects
          WHERE name = 'trig_agr_authupd' AND type = 'TR')
BEGIN
    DROP TRIGGER trig_agr_authupd
END
GO

-- 
-- Trigger to insert an agreement audit record when any of
-- Agreement Total, Service Charge or Deposit are changed.
--
-- Modified on    Modified By    Comment
-- 20 May 2005    Rupal Desai    Modify days to Month when deleting old data
-- 20 Jun 2005    DSR            UAT 88 - No longer delete records on the same day, but
--                               instead update to maintain the correct OLD values
-- 04 Feb 2008    Jez Hemans     COD changes in agreement to be recorded in agreementaudit

CREATE TRIGGER trig_agr_authupd
ON  agreement 
FOR update
AS
    DECLARE @new_holdprop       char(1)
    DECLARE @new_AcctNo         char(12)
    DECLARE @new_AgrmtNo        integer
    DECLARE @auditdataperiod    integer
    DECLARE @FirstDateChange    DATETIME
    DECLARE @LastDateChange     DATETIME
    DECLARE @CurDateChange      DATETIME
    DECLARE @newCODflag         CHAR(1)
    DECLARE @oldCODflag         CHAR(1)
    DECLARE @empeenoauth        int --IP - 17/02/10 - CR1072 - LW 70250 - Delivery Fixes from 4.3
	DECLARE @oldempeenoauth     int --IP - 17/02/10 - CR1072 - LW 70964 - General Fixes from 4.3
    DECLARE @oldtermstype       VARCHAR(4) --14236
    DECLARE @newtermstype       VARCHAR(4) --14236
	DECLARE @datechange			DATETIME   --#14392

    -- Assume this was not triggered by a set query and pick one account
    SELECT  @new_holdprop = holdprop,
            @new_AcctNo   = acctno,
            @new_AgrmtNo  = agrmtno,
            @newCODflag   = codflag,
            @empeenoauth = empeenoauth, --IP - 17/02/10 - CR1072 - LW 70250 - Delivery Fixes from 4.3
			@datechange = datechange	--#14392
    FROM    inserted

    SELECT @oldCODflag = codflag FROM DELETED
    
    --IP - 17/02/10 - CR1072 - LW 70964 - General Fixes from 4.3
    select @oldempeenoauth=empeenoauth from agreement where acctno=@new_acctno
	if (@oldempeenoauth not in ('0','') and @oldempeenoauth is not null)
	BEGIN
		set @empeenoauth=@oldempeenoauth
	END

	--IP - 17/02/10 - CR1072 - LW 70964 - General Fixes from 4.3
	update agreement
	set empeenoauth=@empeenoauth
	where acctno=@new_AcctNo
	and agrmtno=@new_agrmtno
	
    --IF @new_holdprop = 'Y'
    IF @new_holdprop = 'Y' and @empeenoauth<>'0' --IP - 17/02/10 - CR1072 - LW 70964 - General Fixes from 4.3
    BEGIN
        EXECUTE dbnewauth @acctno = @new_AcctNo
    END

    IF right(left(@new_AcctNo, 4), 1) != '5'
    BEGIN

        -- Get value for Number of months Audit data to be stored from country maintenance
        select @auditdataperiod = convert (integer, value) from countrymaintenance where codename = 'auditdataperiod'

        -- deleting old data from agreementaudit where number of months over the value set in the auditdataperiod
        -- modified to be months as the country paramater indicates months but trigger was checking for days
        delete from agreementaudit where datechange < dateadd(month,-@auditdataperiod,getdate()) and acctno = @new_AcctNo

        --14236 Insert an Agreement Audit Record each time

        SET @CurDateChange = GETDATE()
		
        SELECT top 1 @oldtermstype= newtermstype  
        FROM  agreementAudit 
		WHERE acctno = @new_acctno 
		order by datechange DESC 

        select @newtermstype = TermsType
        from acct where acctno = @new_AcctNo

        -- protect against violation of PrimaryKey constraint in agreementAudit
        IF EXISTS (SELECT 1 FROM agreementAudit WHERE acctno=@new_AcctNo AND agrmtno=@new_AgrmtNo AND datechange=@datechange)
        BEGIN
            SET @datechange = GETDATE()
        END

               -- Insert an agreement audit record

        insert into agreementaudit (acctno, agrmtno, OldAgreementTotal, NewAgreementTotal,
                                    OldServiceCharge, NewServiceCharge, Olddeposit, Newdeposit,
		    						empeenochange, datechange, systemusername, source, OldCODflag, NewCODflag,
			    					OldTermsType,NewTermsType)
        select distinct inserted.acctno, inserted.agrmtno,deleted.agrmttotal, inserted.agrmttotal,
               deleted.servicechg,inserted.servicechg, deleted.deposit, inserted.deposit, 
			   inserted.empeenochange, @datechange, '', inserted.source, deleted.codflag, inserted.codflag,	--#14392	-- #8621 
			   @oldtermstype,@newtermstype
        from   inserted, deleted 
        where  inserted.acctno  = @new_AcctNo
                and    inserted.agrmtno = @new_AgrmtNo
                and    deleted.acctno   = @new_AcctNo
                and    deleted.agrmtno  = @new_AgrmtNo
				and (deleted.agrmttotal <> inserted.agrmttotal
				    or deleted.servicechg <> inserted.servicechg
					or deleted.deposit <> inserted.deposit
					or deleted.codflag <> inserted.codflag
					or @oldtermstype <> @newtermstype
				)

    END
GO   
