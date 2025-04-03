SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_AcctCodeIsTaxExemptSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_AcctCodeIsTaxExemptSP]
GO



CREATE PROCEDURE [dbo].[DN_AcctCodeIsTaxExemptSP]    
   @acctno varchar(12),  
   @reference VARCHAR(10) = "1",    
   @taxExempt smallint OUT,    
   @return int OUTPUT    
    
AS    

    
 SET  @return = 0   --initialise return code    
    
 IF (@reference IS NULL or @reference = 1)   
 SET @reference = ''  
   
 SELECT @taxExempt = count(*)    
 FROM  acctcode    
 WHERE acctno = @acctno    
 AND  code = 'TE'    
 AND reference = @reference  
 AND datedeleted is null    
 
 if(@taxExempt = 0)
 begin
  select @taxExempt = count(*) from [Sales].[Order] where id = convert(int, @reference) and IsTaxFreeSale =1
 end 
    
 IF (@@error != 0)    
 BEGIN    
  SET @return = @@error    
 END 

GO


