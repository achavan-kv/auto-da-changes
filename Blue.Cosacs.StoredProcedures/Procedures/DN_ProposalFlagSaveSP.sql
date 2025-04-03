SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_ProposalFlagSaveSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_ProposalFlagSaveSP]
GO

/****** Object:  StoredProcedure [dbo].[DN_ProposalFlagSaveSP]    Script Date: 11/05/2007 12:06:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
  
CREATE PROCEDURE  [dbo].[DN_ProposalFlagSaveSP]  
--------------------------------------------------------------------------------
--
-- Project      : CoSACS .NET
-- File Name    : DN_ProposalFlagSaveSP.PRC
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        :
-- Author       : ??
-- Date         : ??
--
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 28/06/11  IP  5.13 - LW73619 - #3751 - If holdprop = 'N' (accounts sanction stage 1 was re-opened and not revised
--				 need to remove entry from delauthorise which would mean it will no longer appear in Incomplete Credit screen
--------------------------------------------------------------------------------
   @origbr smallint,  
   @custid varchar(20),  
   @dateprop smalldatetime,  
   @checktype varchar(4),  
   @datecleared datetime,  
   @empeenopflg int,  
   @acctno char (12),  
   @return int OUTPUT  
  
AS  
  
 SET  @return = 0   --initialise return code  
  
 UPDATE proposalflag  
 SET  origbr = @origbr,  
   custid = @custid,  
   dateprop = @dateprop,  
   checktype = @checktype,  
   datecleared = @datecleared,  
   empeenopflg = @empeenopflg  
 WHERE acctno = @acctno  
 AND  checktype = @checktype  
 AND  custid = @custid 
 AND dateprop = @dateprop  --need all parts of key for insert update in case of duplicate rows.

 IF(@@rowcount = 0)  
 BEGIN  
  INSERT  
  INTO proposalflag  
   (origbr, custid, dateprop, checktype,  
   datecleared, empeenopflg, acctno)  
  VALUES  
   (@origbr, @custid, @dateprop, @checktype,  
   @datecleared, @empeenopflg, @acctno)  
 END
 ELSE		 --28/06/11  IP  5.13 - LW73619 - #3751 
 BEGIN	
	declare @propResult char(1),
			@holdprop char(1) 
			
	select @propResult = propresult from proposal
	where acctno = @acctno
	and dateprop = @dateprop
	and custid = @custid
	
	select @holdprop = holdprop from agreement where acctno = @acctno
	
	IF(@checktype in ('S2', 'DC') and @propResult = 'A' and @holdprop = 'N')
	begin
			exec dbremoveauth @acctno
	end
	
 END  
 -- remove duplicates with incorrect date proposals... 
 delete from proposalflag where acctno= @acctno and 
 checktype = @checktype and 
 (custid !=@custid or dateprop !=@dateprop)

   
   
  --68994 When an account is cleared for checktype S2 or DC, we should set all  
 -- other poposalflags for the same customer and checktype for account type 'R'  
 -- this should also include checktype R  
 if (@checktype = 'S1' OR @checktype = 'S2' or @checktype = 'DC' OR @checktype = 'R') AND exists(select * from acct a  
 inner join proposal p  
  on a.acctno = p.acctno  
  where --custid  =@custid  
   a.acctno = @acctno  
   and a.accttype = 'R')  
 BEGIN  
  update pf  
  set datecleared = @datecleared,  
      empeenopflg = @empeenopflg  
  from proposalflag pf, proposal p , acct a  
  where datecleared is null 
  and checktype = @checktype 
  and a.accttype = 'R' 
  and pf.custid = @custid 
  AND pf.acctno = p.acctno 
  and p.acctno = a.acctno  
 END  
   
 -- Auto DA if set already for CR1034.  
IF EXISTS (SELECT * FROM instalplan
           WHERE acctno = @acctno
           AND AutoDA = 1)
 BEGIN
 	
	
	exec DN_ProposalClearSP @acctno=@acctno,@empeeno=@empeenopflg,@source = 'Auto',@return=@return output
	
		
 END
  
 IF (@@error != 0)  
 BEGIN  
  SET @return = @@error  
 END  
  
