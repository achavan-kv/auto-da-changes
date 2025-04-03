if  exists (select * from sysobjects  where name =  'trig_countrymaintenance' )
drop trigger trig_countrymaintenance
go
create trigger trig_countrymaintenance on countrymaintenance
for update -- of country table from countrymaintenance
as declare @codename varchar (32),@value varchar (256),@type varchar (14), @datatype varchar (14),@intvalue integer,
@statement sqltext, @charvalue varchar(256), @oldvalue varchar (1000),@vardate varchar(30),@name VARCHAR(100),
@error VARCHAR(128)

SET NOCOUNT ON 

if not exists (select * from countryupdated where isnull(dateupdate,'1-jan-1900') > dateadd (second, - 10, getdate()) and updatedby ='OpenROAD')
begin
update countryupdated set dateupdate = getdate(), updatedby ='Net'
select @codename =codename ,--country table column name
@value = value ,
@name = [name],
@type = type from inserted

IF @name ='Tier2 Discount Item Number' AND @value='DS'
BEGIN
       set @error ='Cannot save Loyalty Club- Discount Number as DS as this is reserved for Kit Discounts -raised by trigger trig_countrymaintenance'
        RAISERROR (@error, 16, 1)	
END

-- update branch from Behavioural scorecard  
IF EXISTS (SELECT * FROM inserted WHERE codename ='BehaviouralScorecard' AND value ='S')
BEGIN
	UPDATE branch SET BehaviouralScoring = 1 	
END

IF EXISTS (SELECT * FROM inserted WHERE codename ='BehaviouralScorecard' AND value ='A')
BEGIN
	UPDATE branch SET BehaviouralScoring = 0 	
END


IF EXISTS (SELECT * FROM inserted WHERE codename ='taxrate')
BEGIN -- updating tax rate history table 
	DECLARE @taxvalue MONEY 
	SELECT @taxvalue= value FROM inserted WHERE codename ='taxrate'
    -- set dateto to end of today. 
    DECLARE @currentdate DATETIME 
    SET @currentdate = DATEADD(hh,-5,GETDATE()) -- taking time portion off if after midnight.
    -- now we are going to make sure that date finish is at end of day. 
	UPDATE taxratehistory 
	SET dateto =DATEADD(minute,59,DATEADD(hh,23,DATEADD(dd, 0, DATEDIFF(dd, 0, @currentdate )) ))
	WHERE ISNULL(dateto,'1-jan-1900') ='1-jan-1900' AND  taxrate != @taxvalue 

	DELETE FROM taxratehistory WHERE datefrom > dateto  AND dateto > '1-jan-1980' -- in case change multiple times during one day. 
	
	IF NOT EXISTS (SELECT * FROM taxratehistory t WHERE t.taxrate = @taxvalue AND  ISNULL(dateto,'1-jan-1900') ='1-jan-1900')
	BEGIN 
		INSERT INTO taxratehistory (
			datefrom,
			dateto,
			taxrate
		) VALUES ( 
			DATEADD(dd, 1, DATEDIFF(dd, 0,@currentdate)), -- tax rate will be applied tomorrow or this morning if after midnight
			'1-jan-1900', 
			@taxvalue) 
	END 		
END 

/* Uncomment when behavioural scoring implemented
IF EXISTS (SELECT * FROM inserted WHERE codename ='BehaviouralScorecard' AND value ='S')
BEGIN
	UPDATE branch SET BehaviouralScoring = 1 	
END
-- update branch from Behavioural scorecard  if applicant then set to zero
IF EXISTS (SELECT * FROM inserted WHERE codename ='BehaviouralScorecard' AND value ='A')
BEGIN
	UPDATE branch SET BehaviouralScoring = 0 	
END
*/


set @statement =''
select @oldvalue = value from deleted
if @codename !='' and @value !=@oldvalue
begin

   select @datatype =data_type from information_schema.columns where table_name = 'country' and column_name = @codename
   
   if @datatype = 'int' or @datatype = 'smallint' or @datatype = 'tinyint'
   begin
     if @value ='True' 
	   	set @value='1'
     if @value ='False' 
	   	set @value='0'

    set @statement =' update country set ' + @codename + ' = convert (int,' + @value +') '
        +  ' where ' + @codename + ' != convert (int,' + @value +') '
   end
   if @datatype = 'char'  or @datatype = 'varchar' 
   begin
     if @value = 'False'
	     set @value ='N'
     if @value = 'True' 
       set @value ='Y'

    set @statement= ' update country set ' + @codename +' = convert (varchar(128),' + '''' + @value + '''' +') where ' + @codename
       + ' != convert (varchar(128),' + '''' + @value + '''' +')'
    end
   if @datatype = 'datetime' or @datatype = 'smalldatetime' 
   begin
	 select  @vardate=@value
	 if isdate(@vardate)=0
			 select  @vardate=right(@value,datalength(@value)/2- PATINDEX ( '%,%' , @value )) 
	 if isdate(@vardate)=1
     --right(@value,datalength(@value)/2- PATINDEX ( '%,%' , @value )) 
	    set @statement= ' update country set ' + @codename +' = convert (datetime,' + '''' + @vardate + '''' +') where ' + @codename
       + ' != convert (varchar(128),' + '''' + @vardate + '''' +')'
     --print @statement
    end

   if @datatype = 'money'  or @datatype = 'float' 
   begin
     if @value ='True' 
		 set @value='1'
     if @value ='False' 
		set @value='0'
   set @statement= ' update country set ' + @codename +' = convert (float,' + @value +') where ' +
                 + @codename +' != convert (float,' + @value +') ' 
 
  end
  --select @value,@statement
  if @statement !=''   
	  execute sp_executesql @statement
end
declare @runno integer,@startdate datetime, @existvalue varchar(20)
select @runno = convert (integer,value ) from countrymaintenance where name = 'Last Successful Run week No'
--converting eg. Sunday, April 04, 2004 to  April 04  ,2004 which can be converted using 106 accoring to BOL
select  @vardate=value from countrymaintenance where name =  'Week 1 start date for charges'
 if isdate(@vardate)=0
		 select  @vardate=right(@value,datalength(@value)/2- PATINDEX ( '%,%' , @value )) 

end
	

GO