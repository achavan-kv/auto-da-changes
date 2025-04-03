SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_InstantCreditFlagSaveSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_InstantCreditFlagSaveSP]
GO

/****** Object:  StoredProcedure [dbo].[DN_InstantCreditFlagSaveSP]    Script Date: 11/05/2007 12:06:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
  
CREATE PROCEDURE  [dbo].[DN_InstantCreditFlagSaveSP]  
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : DN_InstantCreditFlagSaveSP.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : DN_InstantCreditFlagSaveSP
-- Author       : Alex Ayscough
-- Date         : Feb 2011
-- 
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 03/03/11  ip  Sprint 5.11 - #3255 - If the date has not been cleared then set the empeeno to null
-- ================================================
	-- Add the parameters for the stored procedure here
   @origbr smallint,  
   @custid varchar(20),  
   @checktype varchar(4),  
   @datecleared datetime,  
   @empeenopflg int,  
   @acctno char (12),  
   @return int OUTPUT  
  
AS  
  
 SET  @return = 0   --initialise return code  
  
  IF @datecleared is NULL --IP - 03/03/11 - #3255
	SET @empeenopflg = NULL --IP - 03/03/11 - #3255
  
 UPDATE instantcreditflag  
 SET  origbr = @origbr,  
   custid = @custid,    
   checktype = @checktype,
   datecleared = @datecleared,
   empeenopflg =@empeenopflg  
 WHERE acctno = @acctno  
 AND  checktype = @checktype  
  
 IF(@@rowcount = 0)  
 BEGIN  
  INSERT  
  INTO instantcreditflag  
   (origbr, custid, checktype,  
    acctno)  
  VALUES  
   (@origbr, @custid, @checktype,  
    @acctno)  
 END  

  
 IF (@@error != 0)  
 BEGIN  
  SET @return = @@error  
 END  
 GO 
 