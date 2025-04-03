SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_CourtspersonSaveSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_CourtspersonSaveSP]
GO

CREATE PROCEDURE DN_CourtspersonSaveSP
as
BEGIN
	RAISERROR('Not Implemented', 16, 1)
END
--CREATE PROCEDURE DN_CourtspersonSaveSP
---- =============================================
---- Author:??
---- Create date: ??
---- Title:	DN_CourtspersonSaveSP
----
----	This procedure will save courtsperson details
---- 
---- Change Control
-------------------
---- 22/03/11 ip #3299 - LW73340 - Change made to allow UpliftCommissionRate to be saved with decimals
---- 13/12/11 jec #8882 correction to #3299
---- 05/03/12 ip #9732 - LW74686 - EmpeeType not updated on tables prevented worklist and actionrights from 
----				being deleted.
---- 10/05/12 jec #9782 CR9439 - new uplift calculation
---- =============================================
--   @branchno smallint, 
--   @empeeno int, 
--   @fACTEmpeeNo varchar(4),
--   @firstName varchar(50), 
--   @lastName varchar(50),
--   @empeetype varchar (3), --IP - 20/05/08 - Credit Collections requires system to cater for (3) character employee type, was originally (1)
--   @password varchar(12) = null, 
--   @maxrow int, 
--   @dutyfree char(1), 
--   @loggedin smallint,
--   @defaultrate bit,
--   @upliftRate float,			-- CR36 enhancements jec 21/06/07	--IP - 22/03/11 - #3299-LW73340 - Changed upliftRate from int to float
--   @empeechange int,		--CR1117
--   @changePassword bit,
--   @return int OUTPUT

--AS
--DECLARE @old_password varchar (12)

--SELECT @old_password = isnull (password, '') FROM CourtsPerson WHERE empeeno =@empeeno    

--set @return = 0                  

--UPDATE	courtsperson  
--SET	FACTEmployeeNo = @fACTEmpeeNo,
--	branchno=@branchno , 
--	firstName=@firstName, 
--	lastName = @lastName,
--	empeetype=@empeetype , 
--	password=ISNULL(@password,@old_password), 
--	maxrow=@maxrow , 
--	dutyfree=@dutyfree,
--	loggedin = @loggedin,
--	UpliftCommissionRate=@upliftRate,		-- #9782 round(@upliftRate,4),		
--	empeechange = @empeechange
--WHERE	empeeno=@empeeno  

--if @@rowcount = 0
--  begin
--   insert into CourtsPerson 
--(     origbr , 
--      branchno , 
--      empeeno , 
--      FACTEmployeeNo,
--      firstName ,
--	  lastName, 
--      empeetype , 
--      commndue , 
--      alloccount , 
--      password , 
--      serialno , 
--      datelstaudit , 
--      maxrow , 
--      lstcommn , 
--      dutyfree,
--      loggedin,
--      UpliftCommissionRate,		-- CR36
--	  empeechange   )      --CR1117
--      values (@branchno , 
--      @branchno , 
--      @empeeno ,
--	  @fACTEmpeeNo,
--      @firstname , 
--      @lastName, 
--      @empeetype , 
--      0, 
--      0 , 
--      @password , 
--      0, 
--      getdate () , 
--      @maxrow , 
--      0 , 
--      @dutyfree,
--      @loggedin,
--	  --round(@upliftRate,4), 						
--	  @upliftRate,		-- #9782
--	  @empeechange) --CR1117
      
--    set @return =@@error
--  end   
		
--if @password IS NOT NULL 
---- updating this password to numeric to give some protection against those who can look at Showcase queries.
--	begin
--	   execute DbSetPassword 
--	   @password =@password,
--	   @empeeno =@empeeno
--	end

---- CR680 RD 28/09/05 Inserting default commission rate for employee if Assing Default Commission Rate is selected
--if @defaultrate = 1
--   BEGIN
--	INSERT INTO bailcommnbas
--		(origbr,
--		 empeeno,
--		 statuscode,
--		 collecttype,
--		 commnpercent,
--                 reposspercent,
--		 allocpercent,
--		 reppercent,
--		 minvalue,
--		 maxvalue,
--		 debitaccount,
--		 empeetype,
--		CollectionPercent)
--	SELECT  @branchno,
--		@empeeno,
--		c.statuscode,
--		c.collecttype,
--		c.commnpercent,
--		c.reposspercent,
--		c.allocpercent,
--		c.reppercent,
--		c.minvalue,
--		c.maxvalue,
--		c.debitaccount,
--		c.empeetype,
--            	c.collectionpercent
--	FROM    commnbasis c
--	WHERE   c.empeetype = @empeetype
--	AND	NOT EXISTS (SELECT * FROM bailcommnbas b 
--			    WHERE  b.empeeno = @empeeno
--			    AND    b.empeetype = @empeetype
--	                    AND    b.collecttype = c.collecttype
--                            AND    b.statuscode  = c.statuscode)

--       set @return =@@error
--   END

--IF @password IS NOT NULL 
--BEGIN
--	UPDATE courtsperson
--	SET datepasschge = DATEADD(Year,-1,GETDATE())
--	WHERE empeeno = @empeeno
--END

----IP - #9732 - LW74686
--UPDATE CMWorkListRights
--SET EmpeeType = @empeetype
--WHERE EmpeeNo = @empeeno

--UPDATE CMActionRights
--SET EmpeeType = @empeetype
--WHERE EmpeeNo = @empeeno


--SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

-- End End End End End End End End End End End End End End End End End End End End End End End End End End End End End End End End End End End End 
