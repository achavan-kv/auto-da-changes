SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[DN_ScheduleSetDHLDeliveryPrintedSP]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[DN_ScheduleSetDHLDeliveryPrintedSP]
GO


CREATE PROCEDURE dbo.DN_ScheduleSetDHLDeliveryPrintedSP
   @acctno varchar(12),  
   @currentBranch int,  
   @user int,  
   @buffbranchno int,
   @return int OUTPUT  
  
AS  
 SET  @return = 0   --initialise return code  
  
 UPDATE schedule   
 SET printedby = @user,  
  dateprinted = GETDATE()  
 WHERE acctno = @acctno  
 AND  (stocklocn = @currentBranch OR (delorcoll='R' AND retstocklocn = @currentBranch)) -- redelivery after repo also needs to be updated.   
 AND buffbranchno = @buffbranchno 
 AND EXISTS (SELECT * FROM branch
             WHERE dotnetforwarehouse = 'Y'
             AND buffbranchno = schedule.buffbranchno) -- redelivery after repo also needs to be updated. 
	--AND     vanno = 'DHL'			UAT 35 Immediate deliveries need to be updated

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

