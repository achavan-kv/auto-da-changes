if  exists (select * from sysobjects  where name =  'dbScorexExtract' )
drop  procedure dbScorexExtract
GO


CREATE PROCEDURE dbo.dbScorexExtract
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : dbScorexExtract.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Scorex data extract
-- Author       : Alex Ayscough
-- Date         : ?
--
-- This procedure will extract details require for Scorex to the Scorexdata table
-- 
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
--[04/Dec/2006] //CR 866 Added extra fields [PC]
-- 12/10/07 JEC Correct determination of Previous Customer & Expensive item category.
-- 17/01/08 JEC (69241) Extract data determined by ScorexExtract flag != Y 
-- 21/01/08 JEC (69448) only set instpcincome and depositpcent if less than 999.
-- 22/07/11 jec CR1254 RI Changes - use ItemId
-- ================================================
	-- Add the parameters for the stored procedure here
 
( 
	@return int OUTPUT 
)
AS

DECLARE @datestart datetime,
		@runno integer,
		@status integer

SET @status = 0
SET nocount on
set @return = 0
DECLARE @prevrundate datetime,@countrycode varchar (2)

SELECT @countrycode =countrycode FROM country
SELECT @prevrundate = Max (datestart) FROM interfacecontrol
WHERE interface = 'SCOREX' AND result in ('P','W')

   SELECT @runno = runno +1 FROM interfacecontrol
   WHERE interface = 'SCOREX' and datestart =@prevrundate
   set @status = @@error
   set @datestart =getdate()
  
   delete from Scorexdata_temp

   if @status = 0
   begin
   INSERT INTO Scorexdata_temp 
		(acctno, wrstcurrstat, wrstSETtstat, --1
		RindicSET, windicSET, ncindicSET,    --2
		staffacct, repoindic, countrycode,   --3
		paymethod, guarindic, sex,           --4
		ethnicity, hometel, worktel,         --5
		worktype, fullorpart, empmtstatus,   --6
		currresstat, prevresstat, maritalstat,--7 
		bankcode, decision, reason,           --8
		override, privclub, prevcustind, --9
		prevaddrind, bankacctind, bankacctcode, --10
		agrmtsizcode, SETagrmtsiz, title,       --11
		payfreq, postcode, postalarea,  --12
		addtoflag, mobile, spouseoccupation,--13 
		branchno, itemcount, relcount,      --14
		age, acceptscore, referscore, scorecardno, --15
		jobcount, timebank, points,         --16
		spousecount, dependants, appliccount, --17 
		timecurrempl, timeprevempl, timecurraddr, 
		timeprevaddr, instalno, bigitemcat, 
		klettrcount, origbr, SETtledaccts, 
		noofaccts, timelastdel, balofaccts,     --h
		agrtotaccts, agrmttotal, totarrears,    --g
		mthlyincome, otherpmnts, mthlyrent,     --f
		instalamount, jntmthincome, othcrtinstal, --e
		 appnumber,refempeeno, instpcincome,--d 
		depositpcent, datelastdel, dateprop,--c 
		dateborn, custid,SETagrmttotal,    --b
		jcustid, dateacctopen,  --a
		GiroInd, CreditCardInd,								--CR 866                        
		TransportType, EducationLevel, DistanceFromStore,	--CR 866                        
		Industry, JobTitle, Organisation)					--CR 866                        
		SELECT
		acct.acctno, '0','0',               --1
		'N','N','N',                     --2
		'N','N',countrycode,             --3
		'','N',customer.sex,             --4
		isnull(customer.ethnicity,''),'N','N',      --5
		'','','',                        --6
		'',proposal.PResStatus,proposal.maritalstat,      --7
		'',isnull(proposal.propresult,''),isnull(proposal.reason,''), --8
		'N','N','N',           --9
		'N','N','',    --10
		'0','Z',customer.title,--11
		'','','',            --12
		'N','N','',          --13
		acct.branchno,0,0,   --14
		customer.age,0,0,0,   --15
		1,0,proposal.points, --16
		0,proposal.dependants,1, --17
		0, 0,0,                                
		0, 0,0,
		0,acct.branchno, 0,                    --i
		0,0,0,                                 --h
		0,acct.agrmttotal,0,                   --g
		isnull(proposal.mthlyincome,0),isnull(proposal.otherpmnts,0),0,--f
		0,0,0,                                --e
		proposal.appnumber,0,99.9,               --d
		99.9,'1-jan-1900',proposal.dateprop,             --c
		customer.dateborn,customer.custid,0,  --b
		'',acct.dateacctopen, 
		case when EXISTS(SELECT * FROM DDMandate WHERE AcctNo = proposal.AcctNo) then 'Y' else 'N' end,	--GiroInd //CR 866
		case when CCardNo1 + CCardNo2 + CCardNo3 + CCardNo4  = '0000000000000000'  then 'N'                    --CR 866            
			 when ltrim(rtrim(CCardNo1 + CCardNo2 + CCardNo3 + CCardNo4)) = ''		  then  'N'				   --CR 866
			 else 'Y' end,  --CreditCardInd																	   --CR 866
		proposal.TransportType, proposal.EducationLevel, proposal.DistanceFromStore,						   --CR 866
		proposal.Industry, proposal.JobTitle, proposal.Organisation											   --CR 866	
		FROM	
	   customer, proposal,acct, country,custacct
	   WHERE proposal.acctno = acct.acctno
      AND custacct.acctno = proposal.acctno
      AND custacct.hldorjnt ='H'
      AND custacct.custid = proposal.custid 
	   AND customer.custid = proposal.custid
-- jec 69241	   AND customer.datelastscored >@prevrundate
      AND    acct.AcctType not in ('C','S')
--      AND    (proposal.DateProp > @prevrundate)
		AND    proposal.propresult !='' -- has to be scored
-- jec 69241      AND    proposal.DateProp >@prevrundate
		And ISNULL(ScorexExtract,'N')!='Y'		-- jec 69241 
      AND    proposal.DateProp = (select max(dateprop) from proposal p where p.acctno = proposal.acctno)
      set @status = @@error
      end
      
	  if @status !=0
		Print 'Error at stage 1'

	 -- Set Worst current status 
      if @status = 0
      begin
     
      UPDATE Scorexdata_temp                
      SET wrstcurrstat =isnull((SELECT  Max (statuscode) FROM
      acct, custacct, status
      WHERE custacct.custid =Scorexdata_temp.custid
      AND  Scorexdata_temp.acctno !=custacct.acctno
      AND acct.acctno = custacct.acctno
      AND custacct.hldorjnt = 'H'
      AND acct.dateacctopen < Scorexdata_temp.dateacctopen
      AND acct.currstatus != 'S'
      AND status.acctno =acct.acctno 
      AND status.statuscode between '1' AND '8'),'0')
      set @status = @@error
      end
      
	  if @status !=0
		Print 'Error at stage 2'

	-- Set Worst settled status
      if @status = 0
      begin
      
		UPDATE Scorexdata_temp 
		SET wrstSETtstat=isnull((SELECT Max (statuscode) 
		FROM  status, custacct,acct
      WHERE custacct.custid =Scorexdata_temp.custid
      AND  Scorexdata_temp.acctno !=custacct.acctno
      AND acct.acctno = custacct.acctno AND acct.currstatus ='S'
      AND custacct.hldorjnt = 'H'
      AND status.acctno = custacct.acctno
      AND status.statuscode between '1' AND '8'),'0')
      set @status = @@error
      end
      
	  if @status !=0
		Print 'Error at stage 3'


      if @status = 0
      begin
      
	   UPDATE Scorexdata_temp SET WindicSET ='Y'
	   WHERE exists (SELECT * FROM 
	   acctcode c,custacct cu
	   WHERE cu.custid = Scorexdata_temp.custid
	   AND cu.acctno = c.acctno
	   AND cu.hldorjnt ='H'
	   AND cu.acctno !=Scorexdata_temp.acctno
	   AND c.code = 'W')
	   set @status = @@error
	   end
	   
	  if @status !=0
		Print 'Error at stage 4'

	  -- Set Repo indicator 
	   if @status = 0
	   begin
	                                     
	   UPDATE Scorexdata_temp SET repoindic ='Y'
	   WHERE exists (SELECT * FROM 
	   acctcode c,custacct cu
	   WHERE cu.custid = Scorexdata_temp.custid
	   AND cu.acctno = c.acctno
	   AND cu.hldorjnt ='H'
	   AND cu.acctno !=Scorexdata_temp.acctno
	   AND (c.code = 'R' or c.code like '%REP'))	                                     
	   set @status = @@error
	   end
	   
	  if @status !=0
		Print 'Error at stage 5'

	   if @status = 0
	   begin
	   UPDATE Scorexdata_temp SET RindicSET ='Y'
	   WHERE exists (SELECT * FROM 
	   custcatcode c
	   WHERE c.custid = Scorexdata_temp.custid
	   AND c.datedeleted is null
	   AND c.code = 'R')
      set @status = @@error
      end
      
	  if @status !=0
			Print 'Error at stage 6'
	  -- Set Not circ indicator 
      if @status = 0
      begin
	   UPDATE Scorexdata_temp SET ncindicSET ='Y' -- no circulars
	   WHERE exists (SELECT * FROM 
	   custcatcode c
	   WHERE c.custid = Scorexdata_temp.custid
	   AND c.datedeleted is null
	   AND c.code = 'C')
	   set @status = @@error
	   end

	   if @status !=0
			Print 'Error at stage 7'	   

	   if @status = 0
	   begin
 	   UPDATE Scorexdata_temp SET privclub ='Y' -- no circulars
	   WHERE exists (SELECT * FROM 
	   custcatcode c
	   WHERE c.custid = Scorexdata_temp.custid
	   AND c.datedeleted is null
	   AND c.code in ('CLAC'))
	   set @status = @@error
	   end

	  if @status !=0
	 	Print 'Error at stage 8'
	  -- Set Staff indicator 
	   if @status = 0
	   begin
	   UPDATE Scorexdata_temp SET staffacct ='Y' -- no circulars
	   WHERE exists (SELECT * FROM 
	   custcatcode c
	   WHERE c.custid = Scorexdata_temp.custid
	   AND c.datedeleted is null
	   AND c.code = 'STAF')
	   set @status = @@error
	   end
	  
	   if @status !=0
	    	Print 'Error at stage 9' 
	   if @status = 0
	   begin
	   UPDATE Scorexdata_temp
    	SET paymethod=g.paymethod,
--    	depositpcent=(g.deposit/g.agrmttotal)*100,		-- updated in separate update below jec 21/01/08
      agrmttotal=g.agrmttotal
    	FROM agreement g          
    	WHERE g.acctno = Scorexdata_temp.acctno      
    	AND g.agrmtno = 1 AND g.agrmttotal >0
      set @status = @@error
      end

	  -- Set deposit percentage 	 
	if @status = 0
		begin
			UPDATE Scorexdata_temp
    		SET depositpcent=(g.deposit/g.agrmttotal)*100      
    	FROM agreement g          
    		WHERE g.acctno = Scorexdata_temp.acctno      
    		AND g.agrmtno = 1 AND g.agrmttotal >0
	-- only if depositpcent would be less than 999 (avoid overflow error in ScorexApplication.sql)
			and (g.deposit/g.agrmttotal)*100<999
      set @status = @@error
      end
      
	    if @status !=0
	    	Print 'Error at stage 10' 
      if @status = 0
      begin
      UPDATE Scorexdata_temp
    	SET paymethod=g.paymethod,
    	depositpcent=99.9,
      agrmttotal=g.agrmttotal
    	FROM agreement g          
    	WHERE g.acctno = Scorexdata_temp.acctno      
    	AND g.agrmtno = 1 AND g.agrmttotal <=0 
      set @status = @@error
      end
      
	   if @status !=0
	    	Print 'Error at stage 11' 

	  -- Set guarantor indicator 
      if @status = 0
      begin
    	UPDATE Scorexdata_temp
    	SET guarindic ='Y'
    	FROM custacct 
    	WHERE custacct.custid = Scorexdata_temp.custid
    	AND custacct.hldorjnt ='G'
    	set @status = @@error
    	end

		if @status !=0
	    	Print 'Error at stage 12' 
    	
    	if @status = 0
    	begin
    	UPDATE Scorexdata_temp
    	SET guarindic ='Y'
    	FROM proposalref  --guarantors also stored on proposalref table.
    	WHERE proposalref.acctno = Scorexdata_temp.acctno
    	AND proposalref.relation ='G'
    	set @status = @@error
    	end
    	
		if @status !=0
	    	Print 'Error at stage 13' 
	  -- Set Home telephone indicator 
    	if @status = 0
    	begin
    	UPDATE Scorexdata_temp
    	SET hometel ='Y' 
    	WHERE exists (SELECT * FROM 
    	custtel t
    	WHERE t.custid=Scorexdata_temp.custid
    	AND t.tellocn= 'H' AND t.telno !=''
    	AND t.datediscon is null)
    	set @status = @@error
    	end

		if @status !=0
	    	Print 'Error at stage 14' 

	  -- Set Home telephone indicator     	
    	if @status = 0
    	begin
    	UPDATE Scorexdata_temp
    	SET worktel ='Y' 
    	WHERE exists (SELECT * FROM 
    	custtel t
    	WHERE t.custid=Scorexdata_temp.custid
    	AND t.tellocn= 'W' AND t.telno !=''
    	AND t.datediscon is null)
    	set @status = @@error
    	end
    	
		if @status !=0
	    	Print 'Error at stage 15' 

	  -- Set Employment details    	
		if @status = 0
    	begin
    	UPDATE Scorexdata_temp
    	SET worktype =e.worktype ,
    	empmtstatus=e.empmtstatus,
    	fullorpart=e.fullorpart
    	FROM employment e
    	WHERE e.custid=Scorexdata_temp.custid AND e.dateleft is null AND e.worktype !=''
    	set @status = @@error
    	end
    	
		if @status !=0
	    	Print 'Error at stage 16' 
	  -- Set Residential/Monthly rent details 
    	if @status = 0
    	begin
    	UPDATE Scorexdata_temp
    	SET currresstat= isnull(ca.resstatus,''),
		postcode= isnull(ca.cuspocode ,''),
		mthlyrent= isnull( ca.mthlyrent ,0)
		FROM custaddress ca WHERE    	                        
      ca.custid=Scorexdata_temp.custid
      AND ca.datemoved is null
      AND ca.addtype= 'H' 
      set @status = @@error
      end
      
		if @status !=0
	    	Print 'Error at stage 17' 
      if @status = 0
      begin
 	  -- Set Post code details     
		if @countrycode not in ('T','S','T')
		   UPDATE Scorexdata_temp
       	SET postalarea=ca.cuspocode
         FROM custaddress ca WHERE    	                        
         ca.custid=Scorexdata_temp.custid
         AND ca.datemoved is null
         AND ca.addtype= 'H'
   		  set @status = @@error
      end
	  if @status !=0
	    	Print 'Error at stage 18' 

      if @status = 0
      begin
		if @countrycode= 'S'
    	   UPDATE Scorexdata_temp
       	   SET postalarea=left (ca.cuspocode, 2)
            FROM custaddress ca WHERE    	                        
            ca.custid=Scorexdata_temp.custid
            AND ca.datemoved is null
            AND ca.addtype= 'H'
            set @status = @@error
      end

	  if @status !=0
	    	Print 'Error at stage 19' 
      if @status = 0
      begin
      if @countrycode= 'Y'
    	   UPDATE Scorexdata_temp
       	SET postalarea=left (ca.cuspocode, 3)
         FROM custaddress ca WHERE    	                        
         ca.custid=Scorexdata_temp.custid
         AND ca.datemoved is null
         AND ca.addtype= 'H'
		 set @status = @@error 
      end
	  if @status !=0
	    	Print 'Error at stage 20' 
      if @status =0
      begin
      if @countrycode= 'T'
    	   UPDATE Scorexdata_temp
       	SET postalarea=left (t.telno, 3)
         FROM custtel t
       	WHERE t.custid=Scorexdata_temp.custid
       	AND t.tellocn= 'H'
       	AND t.datediscon is null
      set @status = @@error
      end
      
      if @status = 0
      begin

	  -- Set Bank details          
      UPDATE Scorexdata_temp
      SET
      bankcode=b.bankcode,
      bankacctcode = b.code,
 		timebank	= datediff(DAY,dateopened,dateprop)/30.417,
 		bankacctind= 'Y'
 		FROM bankacct b
 		WHERE Scorexdata_temp.custid = b.custid
    	AND b.code !='' 
    	set @status = @@error
    	end

    if @status !=0
	    	Print 'Error at stage 21' 
      if @status =0
	  -- Set number of previous accounts
      begin	
    	UPDATE Scorexdata_temp
		  SET noofaccts= isnull ((SELECT count (*) FROM custacct,acct WHERE
		  custacct.custid = Scorexdata_temp.custid               
		  AND custacct.acctno != Scorexdata_temp.acctno
		  AND custacct.acctno =acct.acctno
		  AND acct.dateacctopen< Scorexdata_temp.dateacctopen
		  AND custacct.hldorjnt ='H'
		  AND acct.agrmttotal> 0
		  --AND acct.currstatus !='S' 
		  AND acct.acctno like '___0%'				-- jec 12/10/07 (do not include cash accounts)
		   ),0) 
       set @status = @@error
       end
       
       if @status = 0
       begin
         UPDATE Scorexdata_temp SET prevcustind= 'Y' WHERE  noofaccts> 0	 
 		
 		-- decision,                                 
       
 			SELECT Max (datein) as datein, 
 			convert (datetime,'1-Jan-1900') as previousdatein,
 			convert (datetime,'1-Jan-1900') as previousdatemoved,
 			custaddress.custid
 			INTO
 			#custaddress
 			FROM custaddress, Scorexdata_temp
 			WHERE custaddress.custid = Scorexdata_temp.custid
 			AND custaddress.addtype='H'
					 group by custaddress.custid
 			set @status = @@error
 		end
 		
 		if @status = 0
 		begin
			create clustered index ixhashcustaddress_custid on #custaddress (custid)
			set @status = @@error
		end
		
	  -- Set Previous address/moved details 
		if @status = 0
		begin
		UPDATE #custaddress SET previousdatein= (SELECT  Max (datein)
		FROM custaddress c
		WHERE c.custid = #custaddress.custid AND c.datein != #custaddress.datein
		AND c.addtype='H' )                         
		set @status = @@error
		end
		
		if @status = 0
		begin
		UPDATE #custaddress 
		SET previousdatemoved=c.datemoved FROM custaddress c
		WHERE c.custid = #custaddress.custid AND c.datein = #custaddress.previousdatein
		set @status = @@error
		end
		
		if @status = 0
		begin
		UPDATE Scorexdata_temp SET                       
		timeprevaddr =isnull (datediff(month,   previousdatein,  previousdatemoved) , 0)
		FROM #custaddress c WHERE c.custid = Scorexdata_temp.custid
		set @status = @@error
		end
		
		if @status = 0
		begin
		UPDATE Scorexdata_temp SET                       
		timecurraddr =isnull (datediff(month,   datein,  dateprop),0)
		FROM #custaddress c WHERE c.custid = Scorexdata_temp.custid
		set @status = @@error
		end
		
		if @status = 0
		begin
		UPDATE Scorexdata_temp SET                       
		prevaddrind= 'Y'   WHERE timeprevaddr> 0
		set @status = @@error
		end
		
	  -- Set Agreement size
		if @status = 0
		begin

      SELECT *, convert (money,0) as minvalue INTO #agreementsize FROM 
      agreementsize
      set @status = @@error
      end
      
      if @status = 0
      begin
      declare @sizecode varchar (2),@maxvalue money,@prevsize varchar (2),@counter integer
      SET @counter = 1
      DECLARE size_cursor CURSOR 
     	FOR SELECT sizecode,Maxvalue
      FROM #agreementsize 
      order by maxvalue desc
      OPEN size_cursor
      FETCH NEXT FROM size_cursor INTO @sizecode,@Maxvalue
   
      WHILE (@@fetch_status <> -1)
      BEGIN
   	   IF (@@fetch_status <> -2)
      	begin         
      	   if @counter > 1
      	   begin 
      	      SET @maxvalue=@maxvalue + .01
      	      UPDATE #agreementsize SET minvalue =@Maxvalue WHERE sizecode =@prevsize
            end      	         
            
            SET @counter =@counter +1
            SET @prevsize =@sizecode      
      	
   	   END
         FETCH NEXT FROM size_cursor INTO @sizecode,@Maxvalue
      END
      CLOSE size_cursor
      DEALLOCATE size_cursor
      set @status = @@error
      end
      
      if @status = 0
      begin
  		UPDATE Scorexdata_temp SET agrmtsizcode=sizecode
  		FROM #agreementsize
  		WHERE agrmttotal< Maxvalue AND agrmttotal>minvalue
	   set @status = @@error
	   end
	   
	   if @status = 0
	   begin
  		UPDATE Scorexdata_temp SET SETagrmttotal= isnull (( 
  		  SELECT Max (acct.agrmttotal) FROM acct, custacct
  		  WHERE acct.currstatus ='S' AND custacct.acctno =acct.acctno
  		  AND custacct.hldorjnt= 'H' AND Scorexdata_temp.custid = custacct.custid
        AND acct.acctno !=Scorexdata_temp.acctno
  		  AND not exists -- not written off
  		  (
  		    SELECT * FROM 
  		    fintrans f  		  
  		    WHERE f.acctno = custacct.acctno AND f.transtypecode = 'BDW'
  		   )
         ) ,0)
       set @status = @@error
       end
       
       if @status = 0
       begin
   		UPDATE Scorexdata_temp SET SETagrmtsiz=sizecode
   		FROM #agreementsize
   		WHERE SETagrmttotal< Maxvalue AND SETagrmttotal>minvalue AND SETagrmttotal>0
   	 set @status = @@error
   	 end

	  -- Set Employment details (again)???
   	 
   	 if @status = 0
   	 begin
   	   UPDATE Scorexdata_temp SET
 --       worktype      = iSnull(Employment.worktype,''),
         payfreq       = iSnull(Employment.payfreq,''),
        fullorpart    = iSnull(Employment.fullorpart,''),
        empmtstatus   = iSnull(Employment.empmtstatus,'')
       FROM employment 
       WHERE employment.custid =  Scorexdata_temp.custid
       AND employment.dateleft is null AND employment.empmtstatus !=''
       set @status = @@error
       end

	  -- Set Addto flag
       
       if @status = 0
       begin
  		UPDATE Scorexdata_temp SET     		
  		addtoflag='Y' FROM fintrans
  		WHERE fintrans.acctno = Scorexdata_temp.acctno AND fintrans.transtypecode = 'ADD'
  		AND fintrans.transvalue > 0
      set @status = @@error
      end

	  -- Set Mobile telephone indicator
      
      if @status = 0
      begin
      UPDATE Scorexdata_temp SET
      mobile ='Y' WHERE exists (SELECT * FROM 
      custtel t
      WHERE t.custid = Scorexdata_temp.custid AND t.tellocn= 'M'
      AND t.telno !='')
      set @status = @@error
      end
      
	  -- Set Item count

      if @status = 0
      begin
   	IF @CountryCode = 'I' 
   	begin
           UPDATE Scorexdata_temp
           SET itemcount = (SELECT count (*)
           FROM LINEITEM l, stockitem s
           WHERE l.AgrmtNo =1
           AND l.AcctNo = Scorexdata_temp.acctno
           AND s.ItemType = 'S'
           --AND s.itemno =l.itemno
           AND s.ItemId =l.ItemId			-- RI
           AND s.stocklocn=l.stocklocn
           AND iskit = 0
           AND l.quantity >0 
   	     AND s.category not in (14, 24, 84))
   	     set @status = @@error
   	 end    
       ELSE
       begin
           UPDATE Scorexdata_temp
           SET itemcount = (SELECT count (*)
            FROM LINEITEM l, stockitem s
           WHERE l.AgrmtNo = 1
           AND l.AcctNo = Scorexdata_temp.acctno
           AND s.ItemType = 'S'
           --AND s.itemno =l.itemno
           AND s.ItemId =l.ItemId			-- RI
           AND s.stocklocn=l.stocklocn
			  AND l.quantity >0 
           AND iskit = 0)
           set @status = @@error
       END
       
       end
 
	  -- Set Relative count    
      if @status = 0
      begin
    	UPDATE Scorexdata_temp
       SET relcount = isnull(( SELECT count (*)
    	FROM proposalref 
    	WHERE proposalref.acctno = Scorexdata_temp.acctno
    	AND proposalref.relation in ('A','B','F','C','G','P')),0)
    	set @status = @@error
    	end
    	
    	if @status = 0
    	begin
			UPDATE	Scorexdata_temp 
			SET 	acceptscore = acceptreferscore.acceptscore,
					referscore = acceptreferscore.referscore,
					scorecardno = 1
			FROM 	acceptreferscore
			WHERE 	dateprop BETWEEN acceptreferscore.datefrom AND ISNULL(acceptreferscore.dateto,dateprop)
			
			SET @status = @@error
		end

	  -- Set Job count     
      if @status = 0
      begin
       UPDATE Scorexdata_temp 
       SET jobcount = isnull ((SELECT count (*) FROM employment
       WHERE employment.custid = Scorexdata_temp.custid 
       AND  (employment.dateleft is null or dateadd (year, 3,employment.dateleft) > getdate()) )
       ,0)
    	set @status = @@error
    	end
    	
    	if @status = 0
    	begin
    	UPDATE Scorexdata_temp 
       SET jobcount = isnull ((SELECT count (*) FROM employment
       WHERE employment.custid = Scorexdata_temp.custid 
       AND  (employment.dateleft is null or dateadd (year, 3,employment.dateleft) > getdate()))
       ,0) 
       set @status = @@error
       end
       
	  -- Set Joint customer ID
       if @status = 0
       begin
       UPDATE  Scorexdata_temp SET jcustid =custacct.custid
       FROM custacct
       WHERE custacct.acctno = Scorexdata_temp.acctno  	                                                       
       AND custacct.hldorjnt in ('S','J')
       set @status = @@error
       end
       
       if @status = 0
       begin
       UPDATE Scorexdata_temp SET spousecount= 1, appliccount=2 WHERE jcustid !=''
       set @status = @@error
       end
       
	  -- Set previous Employment details
       if @status = 0
       begin
       
       SELECT Max (dateemployed) as dateemployed, 
 		convert (datetime,'1-Jan-1900') as previousdateemployed,
 		convert (datetime,'1-Jan-1900') as previousdateleft,
 		  employment.custid
 		INTO
 		#employment
 		FROM employment, Scorexdata_temp
 		WHERE employment.custid = Scorexdata_temp.custid
 	        group by employment.custid
 		set @status = @@error
 		end
 		
 		if @status = 0
 		begin
 		create clustered index ixhash_employment on #employment (custid)
 		
 		UPDATE #employment SET previousdateemployed= (SELECT  Max (dateemployed)
 		FROM employment c
 		WHERE c.custid = #employment.custid AND c.dateemployed != #employment.dateemployed
 		 )                         
 		set @status = @@error
 		end
 		
 		if @status = 0
 		begin
 		UPDATE #employment 
 		SET previousdateleft=c.dateleft FROM employment c
 		WHERE c.custid = #employment.custid AND c.dateemployed = #employment.previousdateemployed
 		set @status = @@error
 		end
 		
 		if @status = 0
 		begin
 		UPDATE Scorexdata_temp SET                       
 		timeprevempl = isnull((SELECT datediff(month,   previousdateemployed,  previousdateleft)
 		FROM #employment c WHERE c.custid = Scorexdata_temp.custid) ,0)
 		set @status = @@error
 		end
 		
 		if @status = 0
 		begin
 		UPDATE Scorexdata_temp SET                       
 		timeprevempl=0 WHERE 		timeprevempl=1
 		set @status = @@error
 		end
 		
 		if @status = 0
 		begin
 		UPDATE Scorexdata_temp SET                       
 		timecurrempl =isnull ((SELECT datediff(month,   dateemployed,  dateprop)
 		FROM #employment c WHERE c.custid = Scorexdata_temp.custid),0)
 		set @status = @@error
 		end
 		
	  -- Set instalment amount
 		if @status = 0
 		begin
 		UPDATE  Scorexdata_temp SET
 		instalamount= instalplan.instalamount
 		,instalno= instalplan.instalno
 		FROM instalplan 
 		WHERE instalplan.acctno = Scorexdata_temp.acctno
 		set @status = @@error
 		end
 		
	  -- Set category of most expensive item 
 		if @status = 0
 		begin
       SELECT Max (ORDVAL) as ordval,lineitem.acctno,convert (varchar (18),' ') as itemno, convert (smallint, 0) as category			-- RI
       INTO #maxitems
       FROM lineitem, stockitem, Scorexdata_temp
       WHERE lineitem.acctno=Scorexdata_temp.acctno
       --AND lineitem.itemno = stockitem.itemno
       AND lineitem.ItemId = stockitem.ItemId			-- RI
       AND lineitem.stocklocn = stockitem.stocklocn
       AND stockitem.itemtype = 'S'
       group by lineitem.acctno
      set @status = @@error
      end
      
      if @status = 0
      begin
      create clustered index ix_hashmaxitems_acctno on #maxitems(acctno,itemno)
      UPDATE #Maxitems
      SET itemno = s.IUPC, category=s.category				-- RI lineitem.itemno 
      FROM lineitem INNER JOIN StockInfo s on lineitem.ItemId = s.ID			-- RI
      WHERE 
      #Maxitems.acctno = lineitem.acctno AND
      lineitem.ordval =#Maxitems.ordval
		--(To stop expensive item defaulting to highest alpha seq. itemno i.e SD or XW123 when ordval of all items is 0 - cancelled accounts)
	  and #Maxitems.ordval!=0			-- jec 12/10/07 
      set @status = @@error
      end
      
      --if @status = 0					-- updated in above  -- RI
      --begin
      --UPDATE #Maxitems 
      --SET category = stockitem.category
      --FROM stockitem
      --WHERE stockitem.itemno = #Maxitems.itemno
      --set @status = @@error
      --end
      
      if @status = 0
      begin
      UPDATE   Scorexdata_temp 
      SET bigitemcat= #Maxitems.category
      FROM #Maxitems
      WHERE Scorexdata_temp.acctno =#Maxitems.acctno
      set @status = @@error
      end
      
	  -- Set K letter count
      if @status = 0
      begin
      UPDATE Scorexdata_temp SET
      klettrcount = isnull ((SELECT count(*) FROM letter l, custacct c,acct a
      WHERE c.acctno =  l.acctno AND l.lettercode = 'K'
      AND c.custid = Scorexdata_temp.custid
      AND c.hldorjnt ='H'
      AND a.acctno =c.acctno
      AND a.dateacctopen<Scorexdata_temp.dateacctopen), 0)
      set @status = @@error
      end
      
	  -- Set settled accounts count
      if @status = 0
      begin
      UPDATE  Scorexdata_temp SET SETtledaccts=isnull ((SELECT count(*) FROM acct a, custacct c
      WHERE c.acctno =  a.acctno AND a.currstatus = 'S' AND c.hldorjnt='H'
      AND c.custid = Scorexdata_temp.custid AND c.acctno !=Scorexdata_temp.acctno
      AND a.agrmttotal> 0 
      AND a.accttype not in ('C','S')
      AND a.dateacctopen<Scorexdata_temp.dateacctopen), 0) 
      set @status = @@error
      end
      
	  -- Set latest delivery date
      if @status = 0
      begin
      UPDATE Scorexdata_temp 
      SET datelastdel=isnull((SELECT Max (datedel) FROM agreement a, custacct c,acct
      WHERE
       c.custid = Scorexdata_temp.custid AND a.acctno !=Scorexdata_temp.acctno 
      AND a.acctno = c.acctno 
      AND c.hldorjnt='H'
      AND acct.acctno = a.acctno
      --AND acct.accttype not in ('C','S')
      AND acct.dateacctopen < Scorexdata_temp.dateacctopen  ),'1-jan-1900')
      set @status = @@error
      end

	  -- Set time since last delivery       
      if @status = 0
      begin
      UPDATE Scorexdata_temp      
      SET timelastdel= datediff(month, datelastdel,DateProp) WHERE datelastdel >'1-jan-1980'
      set @status = @@error
      end
      
	  -- Set total balance of all accounts
      if @status = 0
      begin
      UPDATE Scorexdata_temp 
      SET balofaccts =isnull ((SELECT sum(outstbal) FROM acct a, custacct c
      WHERE c.acctno =  a.acctno 
      AND c.custid = Scorexdata_temp.custid 
      AND c.acctno !=Scorexdata_temp.acctno
		AND c.hldorjnt='H'
      --AND acct.accttype not in ('C','S')
      AND a.dateacctopen < Scorexdata_temp.dateacctopen ), 0) 
      set @status = @@error
      end
      
      if @status = 0
      begin
      UPDATE Scorexdata_temp 
      SET refempeeno= referral.empeeno
		FROM referral
      WHERE referral.custid =Scorexdata_temp.custid
      AND Scorexdata_temp.dateprop = referral.dateprop
      set @status = @@error
      end
      
	  -- Set total agreement total of all previous accounts
      if @status = 0
      begin
      UPDATE Scorexdata_temp 
      SET agrtotaccts =isnull ((SELECT sum(agrmttotal) FROM acct a, custacct c
      WHERE c.acctno =  a.acctno 
      AND c.custid = Scorexdata_temp.custid AND c.acctno !=Scorexdata_temp.acctno
      AND c.hldorjnt ='H'
      AND a.dateacctopen < Scorexdata_temp.dateacctopen  
      --AND a.currstatus !='S'  
      --AND a.accttype not in ('C','S')
		), 0) 
		set @status = @@error
		end

	  -- Set total arrears on all existing accounts		
		if @status = 0
		begin
      UPDATE Scorexdata_temp 
      SET totarrears =isnull ((SELECT sum(a.arrears) FROM acct a, custacct c
      WHERE c.acctno =  a.acctno  AND currstatus !='S' AND outstbal> 0
		AND a.accttype not in ('C','S')
		AND a.dateacctopen < Scorexdata_temp.dateacctopen  
      AND c.custid = Scorexdata_temp.custid AND c.acctno !=Scorexdata_temp.acctno), 0) 
      set @status = @@error
      end
      
	  -- Set Joint monthly income
      if @status = 0
      begin
		UPDATE Scorexdata_temp 
      SET jntmthincome= isnull (P.A2MthlyIncome,0) + isnull (P.A2AddIncome,0)
      FROM proposal p
      WHERE P.custid = Scorexdata_temp.custid
      AND P.acctno =Scorexdata_temp.acctno
      set @status = @@error
      end

	  -- Set total instalments on all not settled accounts      
      if @status = 0
      begin
      UPDATE Scorexdata_temp 
      SET othcrtinstal=isnull ((SELECT sum(i.instalamount) 
      FROM acct a, custacct c, instalplan i            
      WHERE c.acctno = a.acctno  
      AND a.currstatus !='S' 
      AND a.outstbal> 0
      AND i.acctno = a.acctno
     -- AND i.instalno =1
      AND c.custid = Scorexdata_temp.custid 
      AND c.hldorjnt ='H'      
      AND a.dateacctopen < Scorexdata_temp.dateacctopen  
      AND c.acctno !=Scorexdata_temp.acctno), 0) 
      set @status = @@error
      end
     
	  --  instalment as percentage of monthly income 
      if @status = 0
      begin
      UPDATE Scorexdata_temp SET instpcincome= 100*instalamount/(isnull (Scorexdata_temp.MthlyIncome,0)-  isnull (Scorexdata_temp.mthlyrent,0) 
	-isnull (proposal.OtherPmnts,0))
      FROM proposal 
      WHERE proposal.acctno= Scorexdata_temp.acctno AND  proposal.dateprop =Scorexdata_temp.dateprop                                 
      AND Scorexdata_temp.mthlyincome > 0
      AND (isnull(Scorexdata_temp.MthlyIncome,0)-  isnull(Scorexdata_temp.mthlyrent,0) -isnull(proposal.OtherPmnts,0))> 0
	  -- only if instpcincome would be less than 999 (avoid overflow error in ScorexApplication.sql)
	  and 100*instalamount/(isnull (Scorexdata_temp.MthlyIncome,0)-  isnull (Scorexdata_temp.mthlyrent,0) 
	-isnull (proposal.OtherPmnts,0))<999
      set @status = @@error
      end

      Select acctno, convert (datetime, null) as dateprop into #Minscore
      from Scorexdata_temp 
      group by acctno
      having count (acctno) > 1

      update #Minscore
      set dateprop = (select min(dateprop) from Scorexdata_temp t
      where t.Acctno =#Minscore.acctno)

      delete from Scorexdata_temp  where exists (
      select * from 
       #Minscore 
      where #Minscore.acctno =Scorexdata_temp.acctno and #Minscore.Dateprop =Scorexdata_temp.dateprop)

    
      if @status = 0
      begin  
		DELETE FROM SCOREXDATA	where exists 
		  (select * from 
		  	Scorexdata_temp s
		  	where s.acctno = Scorexdata.acctno)
      set @status = @@error
      end

	  -- Update ScorexExtract flag to Y for proposals that have been extracted	(jec 17/01/08 69241)
	  if @status = 0
      begin  
	  Update proposal
			set ScorexExtract='Y'
	  From proposal p INNER JOIN Scorexdata_temp e on p.acctno=e.acctno
	  where ISNULL(ScorexExtract,'N')!='Y'
      end 
      
      if @status = 0
      begin  
      -- RD 14/07/06 68391 Modified to add appnumber as this was not populated in scorexdata
      INSERT INTO Scorexdata 
     (age, acceptscore, referscore, acctno, addtoflag, 
      guarindic, agrmtsizcode, hometel, instpcincome, 
      agrmttotal, instalamount, instalno, jobcount, 
      itemcount, klettrcount, agrtotaccts, jntmthincome, 
      maritalstat, origbr, mobile, otherpmnts, 
      othcrtinstal, override, payfreq, paymethod, 
      postalarea, postcode, prevaddrind, prevcustind, 
      points, mthlyincome, prevresstat, privclub, 
      mthlyrent, repoindic, ncindicSET, noofaccts, 
      appliccount,  balofaccts, bankacctcode, 
      reason, relcount, scorecardno, timecurraddr, 
      timebank, timelastdel, timecurrempl, timeprevempl, 
      timeprevaddr, refempeeno, rindicSET, SETagrmtsiz, 
      windicSET, SETtledaccts, sex, totarrears, 
      title, wrstSETtstat, worktel, worktype, 
      wrstcurrstat, spousecount, spouseoccupation, staffacct, 
      bankacctind, bankcode, bigitemcat, branchno, 
      countrycode, currresstat, dateborn, datelastdel, 
      dateprop, decision, dependants, depositpcent, appnumber, -- RD 14/07/06 68391 Added appnumber
      empmtstatus, ethnicity, fullorpart,
	  GiroInd, CreditCardInd,								--CR 866                        
	  TransportType, EducationLevel, DistanceFromStore,		--CR 866                        
	  Industry, JobTitle, Organisation					    --CR 866
	  )
      
      SELECT age, acceptscore, referscore, acctno, addtoflag, 
      guarindic, agrmtsizcode, hometel, instpcincome, 
      agrmttotal, instalamount, instalno, jobcount, 
      itemcount, klettrcount, agrtotaccts, jntmthincome, 
      maritalstat, origbr, mobile, otherpmnts, 
      othcrtinstal, override, payfreq, paymethod, 
      postalarea, postcode, prevaddrind, prevcustind, 
      points, mthlyincome, prevresstat, privclub, 
      mthlyrent, repoindic, ncindicSET, noofaccts, 
      appliccount,  balofaccts, bankacctcode, 
      reason, relcount, scorecardno, timecurraddr, 
      timebank, timelastdel, timecurrempl, timeprevempl, 
      timeprevaddr, refempeeno, rindicSET, SETagrmtsiz, 
      windicSET, SETtledaccts, sex, totarrears, 
      title, wrstSETtstat, worktel, worktype, 
      wrstcurrstat, spousecount, spouseoccupation, staffacct, 
      bankacctind, bankcode, bigitemcat, branchno, 
      countrycode, currresstat, dateborn, datelastdel, 
      dateprop, decision, dependants, depositpcent, appnumber, -- RD 14/07/06 68391 Added appnumber
      empmtstatus, ethnicity, fullorpart,
	  GiroInd, CreditCardInd,								--CR 866                        
	  TransportType, EducationLevel, DistanceFromStore,		--CR 866                        
	  Industry, JobTitle, Organisation					    --CR 866
	  FROM Scorexdata_temp
      set @status = @@error
      end
      
      if @status = 0
      begin  
      
	-- Set Final decision to rejected if Referred, Declined, no decision and account has been cancelled
      UPDATE ScorexDATA 
      SET decision ='X'
      FROM cancellation
      WHERE decision in ('R','','D') AND cancellation.acctno =ScorexDATA.acctno
      set @status = @@error
      end
      
      if @status = 0
      begin  
    -- Set Final decision to rejected if Referred, Declined, no decision and account has been declined  
      UPDATE ScorexDATA 
      SET decision ='X'
      FROM proposal
      WHERE Scorexdata.decision IN ('','R','D') AND proposal.acctno =ScorexDATA.acctno
      AND propresult='D'
      AND proposal.dateprop =ScorexDATA.dateprop
      set @status = @@error
      end
      
      if @status = 0
      begin  
    -- Set Final decision to accepted if Referred, Declined, no decision and account has been accepted  
      UPDATE ScorexDATA 
      SET decision ='A'
      FROM proposal
      WHERE Scorexdata.decision in ('R','','D') AND proposal.acctno =ScorexDATA.acctno
      AND proposal.propresult = 'A' AND proposal.dateprop =ScorexDATA.dateprop
		set @status = @@error
      end
      
	  set @return = @status    
      return @return
                   
go


SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS OFF
GO

-- End End End End End End End End End End End End End End End End End End End End End End End End End End End End End
