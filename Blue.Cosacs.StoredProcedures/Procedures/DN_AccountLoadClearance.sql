SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_AccountLoadClearance]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_AccountLoadClearance]
GO

CREATE procedure DN_AccountLoadClearance
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : DN_AccountLoadClearance.prc
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Load accounts for Delivery Authorise
-- Author       : ??
-- Date         : ??
--
-- This procedure will load details for the delivery authorise screen.
-- 
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 30/09/08  jec UAT536 set initial value of datechange=dateacctopen to avoid instances of 
--				 datechanged prior to date account opened.
-- 22/05/09  jec UAT593 update names for RF accounts. 
-- 24/11/09  ip  UAT768 return accounts with free gifts for Delivery Authorise
-- 16/02/10 jec CR1072 Malaysia Merge - FOC items
-- 24/02/11 jec CR1090 Instalment waiver
-- 16/05/11  ip  RI Integration changes - CR1212 - #3627 - Changed join between lineitem and stockitem table to now use ItemID rather than itemno
-- 31/10/11 jec #8489 CR1232 remove ability to revise cash loan accounts
-- 26/01/12 jec #9468 Time Out error in DA screen
-- 11/03/14 jec #17714 Add index for improved performance (also added indexes to lineitem and proposalflag tables - migration)
-- 26/06/14 ip  #18605 - CR15594 - Ignore Instalment required prior delivery for Ready Assist
-- =================================================================================
	-- Add the parameters for the stored procedure here

    @BranchRestriction  varchar(4),
    @includecash        integer,
    @includeHP          integer,
    @includeRF        integer,
    @includePaid       integer,
    @includeUnpaid   integer,
    @includeItems   integer,
    @holdflags          varchar(4),
	@includeGOL int,
    @return             int OUTPUT
as
begin
declare @status         integer,
        @query_text     varchar (256)
        
    set @return = 0            --initialise return code

    set nocount on
    
    set @query_text = 'initial select'
    
    select
        a.AcctNo,
        a.AcctType,
        ag.AgrmtNo,
        a.TermsType,
        a.AgrmtTotal,
        a.BranchNo,
        ag.CODFlag,
        a.DateAcctOpen,
        ag.DateAgrmt,
        ag.DateDepChqClr,
        ag.Deposit,
        ag.HoldProp,
        t.InstalPreDel,
        ag.EmpeeNoSale,
        a.CurrStatus,
		isnull(i.instantcredit, 'N') as InstantCredit,
        u.FullName as SalesPerson,				-- #9468 
        convert (smalldatetime, DateAcctOpen)   as DateChange,	-- UAT536 jec 30/09/08
        convert (datetime, null)        as DateProp,
        convert (integer, 0)            as EmpeeNoChange,
        convert (money, 0)              as AlreadyPaid,
        convert (tinyint,0)             as ChequeOS,
        convert (tinyint, 0)            as PaymentOS,
        c.CustId,
        convert (varchar (200),'')    as DBHandle,
        convert (tinyint, 0)            as HasString,
        convert (varchar (200), '')   as Propnotes,
        convert (varchar (1), '')     as PropResult,
        convert (varchar (2000),'')   as Notes,
        convert (varchar (60),'')     as Name,
		convert	(varchar (2), 'N')		as SubAgreement,
		convert	(varchar (2), 'N')		as FOC,
		t.isloan             as isLoan,				-- #8489
		case when ra.acctno is null then 0																				 --#18605 - CR15594
			 when ra.acctno is not null and (ra.status = 'Active' or ra.status is null) then 1 end as isReadyAssist
	
    into  #temp_acct1
    --    if a termstype is deactivated before accounts have been DA'd, account does not show in DA screen (68522)
    from  Acct a 
    inner join Agreement ag on ag.AcctNo     = a.AcctNo
    inner join CustAcct c on  c.AcctNo      = a.AcctNo
						 and   c.HldOrJnt    = 'H'
    inner join TermsTypeTable t on t.TermsType   = ISNULL(a.TermsType,'00')
    inner join delauthorise d on a.AcctNo     = d.AcctNo
    left outer join instalplan i  on  i.acctno = a.acctno  -- was TermsType t  -- 68522 jec 25/10/06
    LEFT OUTER JOIN Admin.[User] u on ag.empeenosale=u.id		-- #9468
	LEFT JOIN ReadyAssistDetails ra on ag.acctno = ra.acctno and ag.agrmtno = ra.agrmtno		--#18605 - CR15594
    where a.AcctNo      LIKE @BranchRestriction
    and   a.AcctType    != 'S'
    and   a.CurrStatus  NOT IN ('0','S') 
    --and   (a.AgrmtTotal  > 0 or @includeItems = 0)
    and   (a.AgrmtTotal  > 0 or @includeItems = 0 --IP - 24/11/09 - UAT5.2 (768)
			OR EXISTS (SELECT 1 from lineitem l
						--INNER JOIN stockinfo s ON l.itemno = s.itemno
						INNER JOIN stockinfo s on l.ItemID = s.ID	--IP
						INNER JOIN code c
						ON c.code = s.category
						WHERE c.category = 'FGC'
						AND s.itemtype = 'S'
						AND l.acctno = a.acctno
						))
	-- improve performance #9468
	and ((@includehp = 1 and a.AcctNo like '___0%' and a.AcctType != 'R')  -- include HP
		or (@includeRF = 1 and a.AcctType = 'R')						-- include RF				
		or (@includecash = 1 and a.AcctNo like '___4%')						-- include Cash
		or (@includeGOL = 1 and a.AcctType = 'L')	)					-- include Goods onn Loan?
		
	and not exists(select acctno from cancellation n where n.acctno = a.AcctNo)		-- not cancelled
	
    set @status = @@error

    if @status = 0
    begin
        set @query_text = 'create acctno index on #temp_acct1 table'
        create clustered index tindex on #temp_acct1(acctno)
		CREATE NONCLUSTERED INDEX [ix_accttype] ON [dbo].[#temp_acct1] ([AcctType]) INCLUDE ([AcctNo])			-- #17714

        set @status = @@error
    end

 -- improve performance #9468 
  
 --   if @status = 0
 --   begin
 --       set @query_text = 'remove HP'
 --       if @includehp = 0
 --            delete from  #temp_acct1
 --            where acctno like '___0%'
	--and accttype != 'R'
             
 --       set @status = @@error
 --   end
     
 --   if @status = 0
 --   begin
 --       set @query_text = 'remove cash'
 --       if @includecash = 0
 --           delete from  #temp_acct1
 --           where acctno like '___4%'
            
 --       set @status = @@error
 --   end
     
 --   if @status = 0
 --   begin
 --       set @query_text = 'remove RF'
 --       if @includeRF = 0
 --           delete from  #temp_acct1
 --           where accttype = 'R'
            
 --       set @status = @@error
 --   end

	--IF(@status = 0)
	--BEGIN
	--	SET	@query_text = 'Remove Goods On Loan'
	--	IF(@includeGOL = 0)
	--	BEGIN
	--		DELETE	
	--		FROM		#temp_acct1
	--		WHERE	accttype = 'L'
	--	END
		
	--	SET	@status = @@error
	--END

    
   if @status = 0
    begin
        set @query_text = 'remove accounts with no order lines'
        if @includeItems = 1
            begin
				--IP - 24/04/08 - UAT(321) v5.1
				--Changed so that accounts with an affinity item will not be removed
				--if the 'Exclude Accounts With No Products' checkbox is ticked.
                delete from #temp_acct1
               where not exists(select itemtype
		               from lineitem
		               where lineitem.acctno = #temp_acct1.acctno
		               and itemtype = 'S')
					   and not exists(select *
							from lineitem l
								inner join stockitem s
									--on l.itemno = s.itemno and l.stocklocn = s.stocklocn
									on l.ItemID = s.ItemID and l.stocklocn = s.stocklocn --IP - 16/05/11 - CR1212 - #3627
								inner join #temp_acct1
									on l.acctno = #temp_acct1.acctno
							where 
							l.itemtype = 'N' and
							s.category in ('11', '51', '52', '53', '54', '55', '56', '56', '58', '59'))
            end
        set @status = @@error
    end

-- improve performance #9468 
   --if @status = 0
   -- begin
   --     set @query_text = 'remove cancelled accounts'
   --     delete from #temp_acct1
   --     where exists(select acctno
	  --             from cancellation
	  --             where cancellation.acctno = #temp_acct1.acctno)
   --     set @status = @@error
   -- end

    if @status = 0
    begin
        set @query_text = 'remove non flags'
        execute @status = sploadclearance_remnonflags @holdflags = @holdflags
    end

    if @status = 0
    begin
        set @query_text = 'update account type'
        update  #temp_acct1  
        set     accttype = a.accttype
        from    accttype a, #temp_acct1 t
        where   t.accttype = a.genaccttype
        and     t.accttype = 'O'
        
        set @status = @@error

    end    

	-- improve performance #9468 
    --if @status = 0
    --begin
    --    set @query_text = 'update salesperson name'

    --    update  #temp_acct1
    --    set     salesperson = c.empeename
    --    from    courtsperson c, #temp_acct1 t
    --    where   t.empeenosale = c.empeeno
        
    --    set @status = @@error
    --end
     
    if @status = 0
    begin
        set @query_text = 'update deposit for accounts with instalment before delivery'
        update  #temp_acct1
        set     deposit = instalplan.instalamount
        from    instalplan
        where   instalpredel = 'Y'
        and     instalplan.acctno = #temp_acct1.acctno
        and		InstalmentWaived=0		-- Instalment not waived
        
        set @status = @@error
    end

    if @status = 0
    begin
        set @query_text = 'update date last updated'
        execute @status = sploadclearance_updatedate
    end
    
    if @status = 0
    begin
        set @query_text = 'update payment amounts'
        execute @status = sploadclearance_updatepayment
    end


if @status = 0
    begin
        set @query_text = 'remove deposit/instalment paid'
        if @includePaid = 0
        begin
            delete from  #temp_acct1
            where currstatus != 'U'  and acctno like '___4%' and alreadypaid >=agrmttotal

			delete from  #temp_acct1
            where currstatus != 'U'  and acctno like '___0%' and alreadypaid >=deposit
        end    
        set @status = @@error
    end

    if @status = 0
    begin
        set @query_text = 'remove deposit/instalment unpaid'
        if @includeUnpaid = 0
        begin
            delete from  #temp_acct1
            where (currstatus = 'U' or (acctno like '___0%' and alreadypaid <deposit)) and isReadyAssist = 0		--#18605 - CR15594

			delete from  #temp_acct1
            where (currstatus = 'U' or (acctno like '___4%' and codflag ='N' and alreadypaid <agrmttotal)) and isReadyAssist = 0   --#18605 - CR15594
       end
       set @status = @@error
    end


    IF(@status = 0)
    BEGIN
        SET @query_text = 'fast track RF accounts'
        IF(@includeRF = 1)
        BEGIN
			SELECT	min(p.dateprop) as dateprop,
					p.custid,
					convert(varchar (12), '') as acctno,
					convert (tinyint, 0) as delivered
			INTO	#sub
			FROM	#temp_acct1 t, custacct CA INNER JOIN acct A ON	CA.acctno = A.acctno  
											   INNER JOIN proposal P ON	CA.custid = P.custid 
																	 AND CA.acctno = P.acctno
			WHERE	CA.custid = t.custid
			AND		A.accttype = 'R'
			GROUP BY p.custid

			UPDATE	#sub
			SET		acctno = p.acctno
			FROM	proposal p
			WHERE	#sub.custid = p.custid
			AND		#sub.dateprop = p.dateprop

			UPDATE	#sub
			SET		delivered = 1
			FROM	agreement a
			WHERE	#sub.acctno = a.acctno
			AND		a.agrmttotal = (SELECT SUM(transvalue)
									FROM   fintrans f
									WHERE  #sub.acctno = f.acctno
									AND    transtypecode IN('DEL', 'GRT', 'ADD'))

			UPDATE	#temp_acct1
			SET		subagreement = 'Y'
			FROM	#sub
			WHERE	#temp_acct1.custid = #sub.custid
			AND		#sub.delivered = 1
			AND		#temp_acct1.accttype = 'R'        
        END
        
        SET @status = @@error
    END

    if @status = 0
    begin
        set @query_text = 'get installed amounts'
        select  t.*, i.instalamount
        into    #temp_hp1
        from    instalplan i, #temp_acct1 t
        where   i.acctno = t.acctno
        --and     t.accttype != 'C'
        and  t.accttype not in ('C', 'L')
        
        set @status = @@error
    end
	-- improve performance #9468  - move index to here
	if @status = 0
    begin
        set @query_text = 'create acctno index on temp_hp1 table'
        create clustered index tindex on #temp_hp1(acctno,custid)			-- #9468
        
        set @status = @@error
    end
    
    if @status = 0
    begin
        set @query_text = 'get referral details'
        select  t.acctno, r.reflresult
        into    #referral
        from    referral r,
                proposal p,
                #temp_hp1  t
        where   p.acctno = t.acctno
        and     p.custid = r.custid
        and     p.dateprop = r.dateprop
        and     p.propresult = 'D'
       -- and     t.accttype != 'C'
	and  t.accttype not in ('C', 'L')
        
        
        set @status = @@error
    end

    if @status = 0
    begin
        set @query_text = 'update from proposal table'
        update #temp_hp1
        set hasstring  = p.hasstring,
            -- dbhandle = isnull(p.propnotes, ''),
            notes      = isnull(p.notes, ''),
            propresult = isnull(p.propresult, ''),
            dateprop   = isnull(p.dateprop,'')
        from    proposal p
        where   p.custid = #temp_hp1.custid
        and     p.acctno = #temp_hp1.acctno
        
        set @status =@@error
    end
    
    -- improve performance #9468  - move index
    --if @status = 0
    --begin
    --    set @query_text = 'create acctno index on temp_hp1 table'
    --    create clustered index tindex on #temp_hp1(acctno)
        
    --    set @status = @@error
    --end
    
    if @status = 0
    begin
        set @query_text = 'delete from temp_hp1 (X, R, blank or D)'
        delete
        from    #temp_hp1
        where   propresult in ('X','R','')
        or      (propresult = 'D'
                 and acctno in (select acctno
                                from   #referral
                                where  reflresult != 'A'))
        set @status = @@error
    end

    if @status = 0
    begin
        set @query_text = 'delete from temp_hp1 (D)'
        delete
        from    #temp_hp1
        where   propresult = 'D'
        and     acctno not in (select acctno from #referral)
        
        set @status = @@error
    end
        
    if  @status = 0
    and @includecash = 1
    begin
        set @query_text = 'update customer name for cash'
        
        update  #temp_acct1  
        set     name = c.name
        from    customer c
        where   #temp_acct1.custid = c.custid
        
        set @status = @@error
    end

    if  @status = 0
    and (@includehp = 1 or @includeRF=1)		-- UAT593 
    begin
        set @query_text = 'update customer name for hp'
        
        update  #temp_hp1  
        set     name = c.name
        from    customer c
        where   #temp_hp1.custid = c.custid
        
        set @status = @@error
    end
	-- Flag  FOC accounts  CR1072  jec 16/02/10
	if @status = 0
	BEGIN
		UPDATE #temp_acct1
			set FOC='Y'
		from #temp_acct1 a
		INNER JOIN lineitem l ON a.acctno = l.acctno
		INNER JOIN stockinfo s on l.ItemID = s.ID	--IP - 16/05/11 - CR1212 - #3627
		INNER JOIN code c
		ON c.code = s.category
		WHERE c.category = 'FGC'
		AND s.itemtype = 'S'
		and a.agrmttotal=0
		
		set @status = @@error
	END
	
    if @status = 0
    begin
        set @query_text = 'final select'

        select
            AcctNo,
            AcctType,
            termstype,
            AgrmtNo,
            AgrmtTotal,
            BranchNo,
            CODFlag,
            DateAcctOpen,
            DateAgrmt,
            DateDepChqClr,
            Deposit,
            HoldProp,
            InstalPreDel,
			InstantCredit,
            custid,
            salesperson,
            datechange,
            dateprop,
            empeenochange,
            alreadypaid,
            chequeos,
            paymentos,
            hasstring,
            dbhandle,
            notes,
            name,
			SubAgreement,
			FOC,
			isLoan		-- #8489
        from    #temp_hp1
        where   accttype not in ('C', 'S', 'L')
        union
        select
            AcctNo,
            AcctType,
            termstype,
            AgrmtNo,
            AgrmtTotal,
            BranchNo,
            CODFlag,
            DateAcctOpen,
            DateAgrmt,
            DateDepChqClr,
            Deposit,
            HoldProp,
            InstalPreDel,
			InstantCredit,
            custid,
            salesperson,
            datechange,
            dateprop,
            empeenochange,
            alreadypaid,
            chequeos,
            paymentos,
            hasstring,
            dbhandle,
            notes,
            name,
			SubAgreement,
			FOC,
			isLoan		-- #8489
        from    #temp_acct1
        where   accttype in ('C', 'S', 'L')
        order by acctno

        set @status = @@error
    end

    set @return = @status

end
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

-- End End End End End End End End End End End End End End End End End End End End End End End End End End End End End
