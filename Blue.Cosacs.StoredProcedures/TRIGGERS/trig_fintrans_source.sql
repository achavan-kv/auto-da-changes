--
-- Trigger to update source to upper case when not in fintrans table
--  
-- Created for	: Issue 66680
-- Created By	: Rupal Desai
-- Created On	: 30 June 2005
--
--Modified On	:
-- Kirsty Fernandez 25/01/06 67878 Trigger not working as Singapore still have source <> COSACS
--                           Also changing so rolls back rather than translating source to COSACS as need to find scripts doing this 
-- AA 25/04/06 Removing from update as don't want to stop system working if update incorrect
-- All this now in INSTEAD of trigger
IF EXISTS(SELECT name FROM sysobjects WHERE name = 'trig_fintrans_source' 
  	  		AND type = 'TR')
BEGIN
	DROP TRIGGER trig_fintrans_source
END
GO

-- RD 66680 Trigger to update source to Uppercase
/*CREATE TRIGGER trig_fintrans_source
ON  fintrans 
FOR INSERT, UPDATE
AS
   DECLARE @new_source varchar(10),
   	   @new_acctno   char(12),
	   @new_transrefno int,
	   @error varchar (256)

   SELECT  @new_source = source,
           @new_acctno   = acctno,
	   @new_transrefno = transrefno
   FROM    inserted

   --Check if source is not equal to COSACS or COASTER (all upper case)
   IF  	not (  (@new_source = 'COSACS' and ascii(right(left(@new_source,2),1)) = 79)
    	    or (@new_source = 'COASTER'))
   BEGIN 
         ROLLBACK
         SET @error = 'Please report this error to CoSACS Support Centre, ensure you supply the script that has just been run. ' +
		      'Source must be COSACS or the SUCR breakdown will be incorrect. Raised by trigger: trig_fintrans_source.'
 	 RAISERROR(@error, 16, 1) 
   END
   */
