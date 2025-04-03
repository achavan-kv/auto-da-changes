SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS OFF
GO


if exists (select * from dbo.sysobjects
where id = object_id(N'[dbo].[DN_SRGetWarrantyLength]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[DN_SRGetWarrantyLength]
GO


CREATE PROCEDURE dbo.DN_SRGetWarrantyLength
	 
 @prodCode varchar(100), @stockLocn int, @return int output    
    
AS    
 SET NOCOUNT ON   
	  --RMCr1051 new procedure to get the MAN warranty length
     set @return = 12 
      
    select top 1 @return =  isnull(convert(int,FirstYearWarPeriod), 12)
			from warrantyband b
			where refcode = (select top 1 refcode
			from stockitem s where s.itemno = @prodCode
			and s.stocklocn = @stockLocn)
			  
  
  --set @return = @@ERROR
  
 RETURN @return 
 GO  
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO


