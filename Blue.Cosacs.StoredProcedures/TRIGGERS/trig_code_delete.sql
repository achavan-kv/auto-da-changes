/*

** Created by : John Croft
** Created on : 19 April 2006

    CR776 Transfer Tax Mainenance

    This trigger will delete the codes from Taxitem table and set the Stockitem Taxrate to 
    the default rate from the Countrymaintenance table. 
*/

IF EXISTS(SELECT name FROM sysobjects
	  WHERE name = 'trig_code_delete' 
  	  AND type = 'TR')

DROP TRIGGER trig_code_delete
go

CREATE TRIGGER trig_code_delete
ON code
FOR delete
AS 
  declare @category varchar (3), @code varchar (8), @codedescript varchar(8),@error varchar (500),
            @taxRate money
-- Deleted items
select @category = category, @code = code, @codedescript = codedescript 
        from deleted

IF @@error = 0
    Begin
        if @category = 'TXR'
        Begin
-- Set Tax rate on Stockitems back to default rate
             set @taxrate = (select convert(money,value) from CountryMaintenance 
                            where CodeName = 'taxrate')
             update stockinfo
                    set taxrate=@taxrate
                        where itemno=@code
-- delete item from Taxitem table 
            delete taxitem
                where itemno=@code    

        End
    End


IF @@error != 0
	BEGIN
		set @error =' Taxrate must be numeric: raised by trigger trig_code_update' 
  		RAISERROR(@error, 16, 1) 
	END
  
      
GO

