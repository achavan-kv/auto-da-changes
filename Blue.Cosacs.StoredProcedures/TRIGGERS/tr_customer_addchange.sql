IF EXISTS (SELECT NAME FROM SYSOBJECTS WHERE NAME = 'tr_customer_addchange')

DROP TRIGGER tr_customer_addchange 
GO


CREATE TRIGGER tr_customer_addchange ON customer
FOR INSERT, UPDATE
AS

DECLARE @countrycode varchar (1),
	@custid varchar (20),
	@age smallint,
	@original_age int, 
	@rfcreditlimit money, 
	@OldRFCreditLimit money,
	@minhpage int, 
	@maxhpage int

SELECT @countrycode = countrycode, @minhpage = minhpage, @maxhpage = maxhpage FROM country

-- Thailand so use varchar for CustomerID makes this too complicated to run.
IF @countrycode != 'H'
BEGIN
	SELECT @custid= custid,@age = age FROM inserted

	SELECT @original_age =isnull(age,0) FROM deleted
	
	-- Setting Original age to -555 for new customer 
        IF @original_age IS NULL
	BEGIN
              SET @original_age = -555
	END

	-- Do this check IF the customer has any hire purchase accounts
   	IF exists (SELECT * FROM custacct WHERE custid = @custid AND acctno like '___0%' AND hldorjnt ='H')
   	and @custid not like 'OVER%'  and @custid not like 'SHO%' -- PREVENT if overage account
        BEGIN
		IF (@age < @minhpage or @age > @maxhpage ) AND (@age != @original_age )
			RAISERROR ('Customer age outside acceptable range for credit', 16, 1)
		RETURN
        END

/*#############################################################################

    Modified by:            Jez Hemans
    Modified date:          28/02/2007
    Modified Description:   IF NOT EXISTS check not required since date of birth is now checked at the front-end 
                            and once a RF acct has been created any change to the DOB will be picked up by the query above.
                            This query was also preventing customers from being created without a date of birth.

#############################################################################*/

/*
  	-- Also do this check IF the customer is a ready finance customer who may not yet have any accounts
   	IF not exists (SELECT * FROM custacct WHERE custid = @custid 
		       AND (acctno like '___4%' or acctno like '___5%') AND hldorjnt ='H')
   	and @custid not like 'OVER%' and @custid not like 'SHO%'-- PREVENT if overage account
        BEGIN 
		IF (@age < @minhpage  or @age > @maxhpage ) AND (@age != @original_age )
			RAISERROR ('Customer age outside acceptable range for credit', 16, 1)
		RETURN
        END
*/
  	
END

SELECT @rfcreditlimit = rfcreditlimit,@custid = custid FROM inserted

SELECT @OldRFCreditLimit = rfcreditlimit FROM deleted

IF @rfcreditlimit <>@OldRFCreditLimit
	UPDATE customer SET oldrfcreditlimit = @OldRFCreditLimit WHERE custid =@custid
GO

