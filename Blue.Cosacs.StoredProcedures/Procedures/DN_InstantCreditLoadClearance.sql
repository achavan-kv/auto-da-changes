SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id('[dbo].[DN_InstantCreditLoadClearance]') AND OBJECTPROPERTY(id, 'IsProcedure') = 1)
BEGIN
	DROP PROCEDURE [dbo].[DN_InstantCreditLoadClearance]
END

GO

CREATE PROCEDURE DN_InstantCreditLoadClearance
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : DN_InstantCreditLoadClearance.prc
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Load instant credit accounts for Delivery Authorise
-- Author       : ??
-- Date         : ??
-- Version		: 002
-- This procedure will load details for the instant credit authorisation screen.
-- 
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 28/01/2011 RM Created by modifying dn_accountsloadclearance for delivery authorise screen
-- 08/03/2011 IP Sprint 5.11 - #3288 - Only update deposit = instalamount if it is greater than the agreement.deposit
-- 23/05/2011 IP CR1212 - RI - #3627 - Join on StockInfo.ID and StockQuantity.ID instead of Stockitem.Itemno
-- 02/06/2011 ST #7010168 - Increase length of SalesPerson field same to length of FullName field of User table.
-- =================================================================================
	-- Add the parameters for the stored procedure here

    @BranchRestriction  varchar(4),
    @includeHP          integer,
    @includeRF        integer,
    @holdflags          varchar(4),
    @return             int OUTPUT
AS
BEGIN
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
        convert (varchar (101), '')    as SalesPerson,
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
		convert	(varchar (2), 'N')		as FOC
	
    into  #temp_acct1
    --    if a termstype is deactivated before accounts have been DA'd, account does not show in DA screen (68522)
    from  Acct a 
    inner join Agreement ag on ag.AcctNo     = a.AcctNo
    inner join CustAcct c on  c.AcctNo      = a.AcctNo
						 and   c.HldOrJnt    = 'H'
    inner join TermsTypeTable t on t.TermsType   = ISNULL(a.TermsType,'00')
    inner join delauthorise d on a.AcctNo     = d.AcctNo
    left outer join instalplan i  on  i.acctno = a.acctno  -- was TermsType t  -- 68522 jec 25/10/06
    where a.AcctNo      LIKE @BranchRestriction
    and   a.AcctType    != 'S'
    and   a.CurrStatus  NOT IN ('0','S') 
    and isnull(i.instantcredit, 'N')  = 'Y'
    

    set @status = @@error

    if @status = 0
    begin
        set @query_text = 'create acctno index on #temp_acct1 table'
        create clustered index tindex on #temp_acct1(acctno)

        set @status = @@error
    end

    if @status = 0
    begin
        set @query_text = 'remove HP'
        if @includehp = 0
             delete from  #temp_acct1
             where acctno like '___0%'
	and accttype != 'R'
             
        set @status = @@error
    end
     
  
     
    if @status = 0
    begin
        set @query_text = 'remove RF'
        if @includeRF = 0
            delete from  #temp_acct1
            where accttype = 'R'
            
        set @status = @@error
    end

   if @status = 0
    begin
        set @query_text = 'remove cancelled accounts'
        delete from #temp_acct1
        where exists(select acctno
	               from cancellation
	               where cancellation.acctno = #temp_acct1.acctno)
        set @status = @@error
    end

    if @status = 0
    begin
        set @query_text = 'remove non flags'
        execute @status = spicclearance_remnonflags @holdflags = @holdflags
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

    if @status = 0

    begin
        set @query_text = 'update salesperson name'

        update  #temp_acct1
        set     salesperson = c.FullName
        from    Admin.[User] c, #temp_acct1 t
        where   t.empeenosale = c.Id
        
        set @status = @@error
    end
     
    if @status = 0
    begin
        set @query_text = 'update deposit for accounts with instalment before delivery'
        update  #temp_acct1
        set     deposit = instalplan.instalamount
        from    instalplan
        where   instalpredel = 'Y'
        and     instalplan.acctno = #temp_acct1.acctno
        and		deposit < instalplan.instalamount	--IP - 08/03/11 - #3288
        
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
    
    if @status = 0
    begin
        set @query_text = 'create acctno index on temp_hp1 table'
        create clustered index tindex on #temp_hp1(acctno)
        
        set @status = @@error
    end
    
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
		where EXISTS (SELECT * FROM acct a1
						INNER JOIN lineitem l ON a1.acctno = l.acctno
						--INNER JOIN stockitem s ON l.itemno = s.itemno			
						INNER JOIN StockInfo s ON l.ItemID = s.ID					--IP - 23/05/11 - CR1212 - RI - #3627
						INNER JOIN StockQuantity sq on l.ItemID = sq.ID				--IP - 23/05/11 - CR1212 - RI - #3627
						AND l.stocklocn = sq.stocklocn								--IP - 23/05/11 - CR1212 - RI - #3627
						--AND l.stocklocn = s.stocklocn
						INNER JOIN code c
						ON c.code = s.category
						WHERE c.category = 'FGC'
						AND l.itemtype = 'S'
						AND a1.acctno = a.acctno
						)
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
			FOC
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
			FOC
        from    #temp_acct1
        where   accttype in ('C', 'S', 'L')
        order by acctno

        set @status = @@error
    end

    set @return = @status

END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO