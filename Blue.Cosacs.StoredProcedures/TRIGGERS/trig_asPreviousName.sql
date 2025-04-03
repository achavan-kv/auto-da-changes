/**********************************************/
/* Database upgrade script                      		*/
/*                                              		*/
/* Pa Njie 13th June 2003			*/
/**********************************************/

-- add a trigger to the customer table.  
-- This trigger stores the previous name in the prevname table.



if exists (select * from dbo.sysobjects where id = object_id('[dbo].[trig_asPreviousName]') and OBJECTPROPERTY(id, 'IsTrigger') = 1)

drop trigger trig_asPreviousName
go

/* Create the trigger trig_asPreviousName */
CREATE TRIGGER trig_asPreviousName
ON dbo.customer
FOR update
AS
   DECLARE @new_custid varchar(20),
	@new_datenamechge datetime,
	@new_empeenoname int,
	@new_name varchar(60),
	@new_firstname varchar(30),
	@old_name varchar(91), --69543 KEF Changed from 60 as needs to be firstname + space + lastname
	@old_firstname varchar(30)

	SELECT @new_custid = custid,
	     @new_datenamechge = isnull(datechange,getdate()),
	     @new_empeenoname = empeenochange,
	     @new_name = name,
	     @new_firstname = firstname	
	FROM   inserted 
	     
	SELECT @old_name = name,
	     @old_firstname = firstname
	FROM   deleted
	
	
	IF(@old_name != @new_name OR @old_firstname != @new_firstname)
	BEGIN
		INSERT INTO prevname(
				     origbr,
				     custid,
				     datenamechge,
				     empeenoname,
				     prevname,  
				     firstname
				  )
		VALUES(
			0,
			@new_custid,
			@new_datenamechge,	
			@new_empeenoname,	
			@old_firstname + ' ' +  @old_name,
			''
		    )
	END


GO