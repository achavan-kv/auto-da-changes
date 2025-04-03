/*

** Created by : John Croft
** Created on : 19 April 2006

    CR776 Transfer Tax Mainenance

    This trigger will update the Taxitem and Stockitem tables with Taxrates entered
    into the code table in category TXR via Code Maintenance
 
 Change Control
 --------------
 Date      By  Description
 ----      --  -----------   
10/05/11 jec  CR1212 RI Integration - use ItemID

*/

IF EXISTS(SELECT name FROM sysobjects
	  WHERE name = 'trig_code_update' 
  	  AND type = 'TR')

DROP TRIGGER trig_code_update
go

CREATE TRIGGER trig_code_update
ON code
FOR insert,update, delete
 AS 
     declare @category varchar (3), @code varchar (18), @codedescript varchar(8),@error varchar (500),
            @taxRate money, @reference varchar(12), @delete bit 

	set @category = 'XXX' set @delete=0
	select @category = category, @code = code, @codedescript = codedescript, @reference = reference 
        from inserted

  if (@category ='XXX')
	select @category = category, @code = code, @codedescript = codedescript, @reference = reference, @delete = 1 
        from deleted
 
  if @category ='FUA'
		 UPDATE cmactionrights SET MinNotesLength  =CONVERT(INT,@reference )
		 WHERE  [Action] = @code AND ISNUMERIC(@reference) = 1
		 
IF @@error = 0
    Begin
	--if WRF is updated, update the first year war period on warranty band table
    if @category = 'WRF'
    begin
		if @delete = 0	
			update warrantyband
			set firstYearWarPeriod = @reference
			where refcode = @code
		else
			update warrantyband
			set firstYearWarPeriod = 1
			where refcode = @code
	end
End	 
	IF EXISTS (SELECT * FROM inserted WHERE category = 'TXR')
	OR EXISTS (SELECT * FROM deleted WHERE category = 'TXR')
	BEGIN -- update tax rates
	
	IF @@error = 0 and not exists(select * from deleted d where d.code=@code)
					and not exists(select * from Stockinfo s where s.IUPC=@code)
					
	BEGIN
		set @error =' Item No ' + @code + ' must exist on StockInfo table: raised by trigger trig_code_update' 
  		RAISERROR(@error, 16, 1) 
	END
	
		INSERT INTO taxitem ( -- note there is a trigger on taxitem which inserts to taxitemhist.
			ItemNo,
			TaxApplied,
			SpecialRate,
			ItemId
		) 
		SELECT code, 
		3,
		CONVERT(float,codedescript),s.Id
		 
		FROM inserted i INNER JOIN StockInfo s on i.code=s.IUPC
		WHERE NOT EXISTS (SELECT * FROM taxitem t 
		WHERE t.itemno= i.code )
		AND i.category = 'TXR'
		
		UPDATE taxitem SET SpecialRate= CONVERT(float,codedescript)
		FROM inserted i
		WHERE  taxitem.ItemNo = i.code AND taxitem.TaxApplied != CONVERT(float,codedescript)
		
		DELETE FROM taxitem WHERE EXISTS
		(SELECT * FROM deleted d WHERE 
		taxitem.itemno= d.code AND taxitem.SpecialRate= CONVERT(float,d.codedescript)
		AND d.category = 'TXR')
		AND NOT EXISTS 
		(SELECT * FROM inserted i WHERE 
		taxitem.itemno= i.code AND taxitem.SpecialRate= CONVERT(float,i.codedescript)
		AND i.category = 'TXR')
	END
	
	
 


 
IF @@error != 0
	BEGIN
		set @error =' Taxrate must be numeric: raised by trigger trig_code_update' 
  		RAISERROR(@error, 16, 1) 
	END
	
    if exists (select * from deleted  where category =  'SCH' )
	or exists (select * from inserted  where category =  'SCH' )
    begin
		delete from sundchgtyp 

         insert into sundchgtyp(
           origbr,
           chargecode,
           accttype,
           amount,
           ItemID		-- RI jec 10/05/11
           )
         select 0,
           codedescript,		-- #10013 correction jec 02/05/12
           --'',		-- RI jec 10/05/11
	       code,
           convert (money, Reference),S.ID
        from code c INNER JOIN stockinfo s on c.codedescript=s.iupc		-- RI jec 10/05/11
        where statusflag !='D' and c.category = 'SCH'
    end

  
GO 
-- End End End End End End End End End End End End End End End End End End End End End End End End End