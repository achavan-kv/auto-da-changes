IF EXISTS (SELECT * FROM SYSOBJECTS 
           WHERE NAME = 'SplitFN'
            AND xtype = 'TF')
BEGIN 
DROP FUNCTION dbo.SplitFN
END
GO

CREATE FUNCTION dbo.SplitFN
-- =============================================
-- Author:		????
-- Create date: 7th October 2009
-- Description:	Function: Split
--
--	This Function will split a variable on delimiter and returns a table
-- 
-- Change Control
-----------------
--
-- =============================================
	-- Add the parameters for the function here
		(@String varchar(max), @Delimiter char(1))         
		returns @temptable TABLE (items varchar(max))         
as         
begin         
    declare @idx int         
    declare @slice varchar(max)
    set @string=RTRIM(@string)         -- jec 25/02/13 trailing spaces causes erroneous results
        
    select @idx = 1         
        if len(@String)<1 or @String is null  return         
        
    while @idx!= 0         
    begin         
        set @idx = charindex(@Delimiter,@String)         
        if @idx!=0         
            set @slice = left(@String,@idx - 1)         
        else         
            set @slice = @String         
            
        if(len(@slice)>0)    
            insert into @temptable(Items) values(@slice)         
    
        set @String = right(@String,len(@String) - @idx)         
        if len(@String) = 0 break         
    end     
return         
end 

-- End End End End End End End End End End End End End End End End End End End End End End End End End End End End 
GO 