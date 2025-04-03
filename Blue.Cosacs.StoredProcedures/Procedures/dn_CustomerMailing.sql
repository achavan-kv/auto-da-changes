/*
This code brings back data according to the criteria supplied by the customer mailing screen
Its  complex. We use dn_CustomerMailingBuildstring to build the query string based on 
the supplied parametes. 
@loadfromAcctFirst is used to determine whether we are going to load up data from the account table first.
Normally if a query is likely to be more restrictive it is likely to be faster - so if we 
are restricting by letter or items purchased then we will load from the account (lineitem,letter,delivery) table first and
only load up customers existing in the account table temporary table.
Otherwise we will load up customers first then only load accounts where the custid exists in the customer temporary table
*/
--select * from setdetails

--select * from information_schema.columns where column_name like 'set%'
if exists (select * FROM sysobjects where  name ='dn_CustomerMailing')
drop procedure dn_CustomerMailing
go
create procedure dn_CustomerMailing
@CustomerCodeSet varchar(64), -- customer has this code
@NoCustomerCodeSet varchar(64), -- customer does not have this code
@AccountCodeSet varchar(64), -- customer has an account with this code
@NoAccountCodeSet varchar(64), -- customer doesn't have an account with this code
@ArrearsRestriction varchar(4) , -- 'nl' for no limitation ,'<' less than, 'ai<'  arrears/instalment <
@Arrears money , -- arrears value or arrears ratio to be < than
@maxcurrstatus char(1) , -- exclude customers with maximum current status of this leave blank '' if no restriction
@maxeverstatus char(1) , -- exclude customers with maximum current status of this leave blank '' if no restriction
@branchno int = 0, @branchset varchar(64)='', -- 0 if all branches or branchno if restriction
@accttypes  varchar(2), --Radio C for cash S for Special, H for credit --A for all 
@itemset varchar(64) , -- exclude customer who have not bought items '' if no restriction
@itemsetstartdate datetime,
@itemsetenddate datetime,
@noitemset varchar(64), -- exclude customers who have bought this item set already '' if no restriction
@noitemsetstartdate datetime,
@noitemsetenddate datetime,
@itemcatset varchar(64), -- exclude customers who have not bought items in this category '' if no restriction
@itemCatsetstartdate datetime,
@itemCatsetenddate datetime,
@noitemCatset varchar(64)   , -- exclude customers who have bought items in this category
@noitemCatsetstartdate datetime ,
@noitemCatsetenddate datetime ,
@itemsdelivered smallint, -- 1 if items delivered 2 if ordered (figures based on)
@itemstartswithset varchar(64),
@itemstartswithstartdate datetime ,
@itemstartswithenddate datetime ,
@noitemstartswithset varchar(64),
@noitemstartswithstartdate datetime ,
@noitemstartswithenddate datetime ,
@noletterset varchar(64),
@nolettersetStartdate datetime ,
@nolettersetEnddate datetime ,
@letterset varchar(64),
@lettersetstartdate datetime,
@lettersetenddate datetime,
@customerstartage smallint ,
@customerEndage smallint ,
@totals char(1), -- v for value q for quantity N for Neither
@resulttype varchar(10), --'quantities single total' Not doing as otherwise will get all sorts of reconciliation queries.
@excludecancellations smallint, 
-- to get quantities you must use delivery.....
@return int OUT
as
declare    @statement sqltext , 
           @extraclause sqltext,
           @loadfromAcctFirst varchar(8),
           @acctstatement sqltext,
           @acctwhere sqltext,    -- where clause for #account
           @custwhere sqltext,    -- where clause for #customer
           @DeleteStatement sqltext, 
           @newline  varchar(128),

           @DelLineBuilt varchar(1),    -- check Delivery/LineItem string Built only once
           @LetterBuilt varchar(1),    -- check Letter string Built only once
           @extraBuilt varchar(1),    -- check extraclause built only once
           @moreTables varchar(100),    -- jec 01/06/06
           @comma char(1),
           @deliveryorordertable varchar(24),
           @deliveryororderdate varchar(24)
   if @itemsdelivered = 1
   begin
     set @deliveryorordertable = 'delivery'
     set @deliveryororderdate = 'datedel'
   end
   else
   begin
     set @deliveryorordertable = 'lineitem'
     set @deliveryororderdate = 'dateagrmt'
   end

set nocount on

set @newline = '
'    -- keep this quote on separate line
set @loadfromAcctFirst ='FALSE'
set @extraclause =' '
set @statement =' '
set @acctstatement =' '
set @acctwhere =' '
set @custwhere =' '
set @return = 0
set @moreTables =' '
set @comma=','

-- Note: All SQL scripts generated are saved to zz_CustomerMailingScript
-- via stored procedure DN_CustomerMailingScriptSave to aid debugging
-- the stored procedure also removes scripts > 30 days old
    
-- save parameters
-- create the table if not exist - zz_CustomerMailingParms for storing parameters & debugging
-- jec 
IF not EXISTS (SELECT table_name FROM INFORMATION_SCHEMA.TABLES
	   WHERE  table_name = 'zz_CustomerMailingParms')
create table zz_CustomerMailingParms
(
ParmDate datetime,    
CustomerCodeSet varchar(64), -- customer has this code
NoCustomerCodeSet varchar(64), -- customer does not have this code
AccountCodeSet varchar(64), -- customer has an account with this code
NoAccountCodeSet varchar(64), -- customer doesn't have an account with this code
ArrearsRestriction varchar(4) , -- 'nl' for no limitation ,'<' less than, 'ai<'  arrears/instalment <
Arrears money , -- arrears value or arrears ratio to be < than@maxcurrstatus char(1) , -- exclude customers with maximum current status of this leave blank '' if no restriction
maxcurrstatus char(1) ,
maxeverstatus char(1) , -- exclude customers with maximum current status of this leave blank '' if no restriction
branchno smallint, -- 0 if all branches or branchno if restriction
accttypes  varchar(2), --Radio C for cash S for Special, H for credit --A for all 
itemset varchar(64) , -- exclude customer who have not bought items '' if no restriction
itemsetstartdate datetime,
itemsetenddate datetime,
noitemset varchar(64), -- exclude customers who have bought this item set already '' if no restriction
noitemsetstartdate datetime,
noitemsetenddate datetime,
itemcatset varchar(64), -- exclude customers who have not bought items in this category '' if no restriction
itemCatsetstartdate datetime,
itemCatsetenddate datetime,
noitemCatset varchar(64)   , -- exclude customers who have bought items in this category
noitemCatsetstartdate datetime ,
noitemCatsetenddate datetime ,
itemsdelivered smallint, -- 1 if items delivered 2 if ordered (figures based on)
itemstartswithset varchar(64),
itemstartswithstartdate datetime ,
itemstartswithenddate datetime ,
noitemstartswithset varchar(64),
noitemstartswithstartdate datetime ,
noitemstartswithenddate datetime ,
noletterset varchar(64),
nolettersetStartdate datetime ,
nolettersetEnddate datetime ,
letterset varchar(64),
lettersetstartdate datetime,
lettersetenddate datetime,
customerstartage smallint ,
customerEndage smallint ,
totals char(1), -- v for value q for quantity N for Neither
resulttype varchar(10), --'quantities single total' Not doing as otherwise will get all sorts of reconciliation queries.
excludecancellations smallint
)
/*
insert into zz_CustomerMailingParms (
ParmDate ,    
CustomerCodeSet , -- customer has this code
NoCustomerCodeSet , -- customer does not have this code
AccountCodeSet , -- customer has an account with this code
NoAccountCodeSet , -- customer doesn't have an account with this code
ArrearsRestriction  , -- 'nl' for no limitation ,'<' less than, 'ai<'  arrears/instalment <
Arrears  , -- arrears value or arrears ratio to be < than@maxcurrstatus char(1) , -- exclude customers with maximum current status of this leave blank '' if no restriction
maxcurrstatus ,
maxeverstatus  , -- exclude customers with maximum current status of this leave blank '' if no restriction
branchno , -- 0 if all branches or branchno if restriction
accttypes  , --Radio C for cash S for Special, H for credit --A for all 
itemset  , -- exclude customer who have not bought items '' if no restriction
itemsetstartdate ,
itemsetenddate ,
noitemset , -- exclude customers who have bought this item set already '' if no restriction
noitemsetstartdate ,
noitemsetenddate ,
itemcatset , -- exclude customers who have not bought items in this category '' if no restriction
itemCatsetstartdate ,
itemCatsetenddate ,
noitemCatset    , -- exclude customers who have bought items in this category
noitemCatsetstartdate  ,
noitemCatsetenddate  ,
itemsdelivered , -- 1 if items delivered 2 if ordered (figures based on)
itemstartswithset ,
itemstartswithstartdate  ,
itemstartswithenddate  ,
noitemstartswithset ,
noitemstartswithstartdate  ,
noitemstartswithenddate  ,
noletterset ,
nolettersetStartdate  ,
nolettersetEnddate  ,
letterset ,
lettersetstartdate ,
lettersetenddate ,
customerstartage  ,
customerEndage  ,
totals , -- v for value q for quantity N for Neither
resulttype , --'quantities single total' Not doing as otherwise will get all sorts of reconciliation queries.
excludecancellations 
)
values (
getdate(),
@CustomerCodeSet , 
@NoCustomerCodeSet ,
@AccountCodeSet , 
@NoAccountCodeSet , 
@ArrearsRestriction  , 
@Arrears  , 
@maxcurrstatus  ,
@maxeverstatus  , 
@branchno , 
@accttypes  , 
@itemset  , 
@itemsetstartdate ,
@itemsetenddate ,
@noitemset , 
@noitemsetstartdate ,
@noitemsetenddate ,
@itemcatset , 
@itemCatsetstartdate ,
@itemCatsetenddate ,
@noitemCatset    , 
@noitemCatsetstartdate  ,
@noitemCatsetenddate  ,
@itemsdelivered , 
@itemstartswithset ,
@itemstartswithstartdate  ,
@itemstartswithenddate  ,
@noitemstartswithset ,
@noitemstartswithstartdate  ,
@noitemstartswithenddate  ,
@noletterset ,
@nolettersetStartdate  ,
@nolettersetEnddate  ,
@letterset ,
@lettersetstartdate ,
@lettersetenddate ,
@customerstartage  ,
@customerEndage  ,
@totals , 
@resulttype , 
@excludecancellations 
)*/

-- End save Parameters End save Parameters End save Parameters End save Parameters End save 

-- ok what where are looking at is how to get the data the quickest way possible
-- either we should be loooking at restricting by customer
-- or if the customer has purchased this then we should look at restricting by account 
-- which has purchased the item first
create table #customers (custid varchar(20),
                        acctno char(12),
                        title varchar(50),
                        name varchar(60),
                        firstname varchar(30), --sp_help custaddress
                        rfcreditlimit numeric(12,2),
                        availablespend numeric(12,2),
                        cusaddr1 varchar(50),
                        cusaddr2 varchar(50),
                        cusaddr3 varchar(50),
                        cuspocode varchar(10),
                        telno varchar(30) default '',
                        email varchar(60),
                        quantity int,
                        value money )



--sp_help custaddress 

if @itemset is not null and @itemset !='nr' and @itemset !=' '
begin
      exec dn_CustomerMailingBuildstring 
        @set =@itemSet,@whereclause = @acctwhere OUT, 
        @keyfield = 'acctno',@table =@deliveryorordertable,
        @column = 'itemno',@startdate = @itemsetstartdate,
        @enddate = @itemsetenddate,@columndate = @deliveryororderdate,@totals = @totals
 --   set @DelLineBuilt = 'Y'    -- delivery/lineitem built
	set @loadfromAcctFirst = 'true'
end
 
 


if @noitemset is not null and @noitemset !='nr' and @noitemset !=' '  -- item not purchased
    and @DelLineBuilt != 'Y'
begin
      exec dn_CustomerMailingBuildstring 
        @set =@noitemSet,@whereclause = @acctwhere OUT, 
        @keyfield = 'acctno',@table =@deliveryorordertable,
        @column = 'itemno',@startdate = @noitemsetstartdate,
        @enddate = @noitemsetenddate,@notclause ='not',@columndate = @deliveryororderdate
end
--print 'Step3......'
--print 'W 2' + @acctwhere  + '/w2'

--if @accountcodeset !='' and @accountcodeset !='nr'
if @accountcodeset is not null and  @accountcodeset not in ('nr','',' ')
begin
    set @moreTables = @comma + 'acctcode'    -- jec
  if @itemsdelivered =1 
    Begin
      exec dn_CustomerMailingBuildstring 
        @set =@accountcodeset,@whereclause = @acctwhere OUT, 
        @keyfield = 'acctno',@table ='acctcode',
        @column = 'code'
    End
   else
    Begin
      exec dn_CustomerMailingBuildstring 
        @set =@accountcodeset,@whereclause = @acctwhere OUT, 
        @keyfield = 'acctno',@table ='acctcode',
        @column = 'code'
    End
end

--print 'W 3' + @acctwhere  + '/w3'
--print 'Step4......'
if @noaccountcodeset is not null and @noaccountcodeset not in ('nr',' ','')
begin
    --if @moreTables=' '
      --  set @moreTables = @comma + 'acctcode'    --jec
  if @itemsdelivered =1 
    Begin
      exec dn_CustomerMailingBuildstring 
        @set =@noaccountcodeset,@whereclause = @acctwhere OUT, 
        @keyfield = 'acctno',@table ='acctcode',
        @column = 'code',@notclause = 'not'  ,@totals = 'N'
    End
   else
    Begin
      exec dn_CustomerMailingBuildstring 
        @set =@noaccountcodeset,@whereclause = @acctwhere OUT, 
        @keyfield = 'acctno',@table ='acctcode',
        @column = 'code',@notclause = 'not',@totals = 'N'
    End
end

--print 'Step5......'
if @itemcatset is not null and @itemcatset !='nr' and @itemcatset !=' '  
begin
  if @itemsdelivered =1
  begin
--print 'Step5a......'
      exec dn_CustomerMailingBuildstring 
        @set =@itemcatSet,@whereclause = @acctwhere OUT, 
        @keyfield = 'acctno',@table ='delivery',@table2 = 'stockitem',
        @column = 'category',@startdate = @itemcatsetstartdate,
        @enddate = @itemcatsetenddate,@columndate = 'datedel',@totals = @totals
     --print 'items delivered'
  end
  set @loadfromAcctFirst = 'true'
end

--print 'Step6......'
if @noitemcatset is not null and @noitemcatset !='nr' and @noitemcatset !=' '
begin
--print 'Step6a......'
  exec dn_CustomerMailingBuildstring 
        @set =@noitemcatSet,@whereclause = @acctwhere OUT, 
        @keyfield = 'acctno',@table ='delivery',@table2='stockitem',
        @column = 'category',@startdate = @noitemcatsetstartdate,@totals ='N',
        @enddate = @noitemcatsetenddate,@notclause ='not',@columndate = 'datedel'
--  set @loadfromAcctFirst = 'true'
end
--print 'Step7......'
if @letterset is not null and @letterset !='nr' and @letterset !=' '
begin
  exec dn_CustomerMailingBuildstring 
        @set =@letterSet,@whereclause = @acctwhere OUT, 
        @keyfield = 'acctno',@table ='letter',
        @column = 'lettercode',@startdate = @lettersetstartdate,
        @enddate = @lettersetenddate,@columndate = 'dateacctlttr', @totals ='N'
        set @loadfromAcctFirst = 'true'

        set @LetterBuilt = 'Y'    -- Letter built
end
--print 'Step8......'
if @noletterset is not null and @noletterset !='nr' and @noletterset !=' '
begin
  exec dn_CustomerMailingBuildstring 
        @set =@noletterSet,@whereclause = @acctwhere OUT, 
        @keyfield = 'acctno',@table ='letter',
        @column = 'lettercode',@startdate = @nolettersetstartdate,@totals ='N',
        @enddate = @nolettersetenddate,@notclause ='not',@columndate = 'dateacctlttr'
--  set @loadfromAcctFirst = 'true'
--        set @LetterBuilt = 'Y'    -- Letter built
        
end

if @branchset !='nr' and @branchset is not null and @branchset !=' '
begin
--  set @loadfromAcctFirst = 'true'
    exec dn_CustomerMailingBuildLikestring @set =@branchset ,@whereclause  =@acctwhere OUT,
    @keyfield ='acctno', --acctno or custid
    @table = 'acct',
    @table2 ='acct',
    @column ='acctno' 
       
end




if @itemstartswithset not in ('nr','',' ') and @itemstartswithset is not null 
begin
--  set @loadfromAcctFirst = 'true'
    exec dn_CustomerMailingBuildLikestring @set =@itemstartswithset ,@whereclause  =@acctwhere OUT,
    @keyfield ='acctno', --acctno or custid
    @table = @deliveryorordertable,
    @table2 =@deliveryorordertable,
    @column ='itemno' ,
    @columndate =@deliveryororderdate , --'datedel' or 'dategrmt'
    @startdate  =@itemstartswithstartdate,
    @enddate  =@itemstartswithenddate
      
end

if @noitemstartswithset not in ('nr','',' ') and @itemstartswithset is not null 
begin
--  set @loadfromAcctFirst = 'true'
    exec dn_CustomerMailingBuildLikestring @set =@noitemstartswithset ,@whereclause  =@acctwhere OUT,
    @keyfield ='acctno', --acctno or custid
    @table = @deliveryorordertable,
    @table2 =@deliveryorordertable,
    @column ='itemno' ,
    @columndate =@deliveryororderdate , --'datedel' or 'dategrmt'
    @startdate  =@itemstartswithstartdate,
    @enddate  =@itemstartswithenddate,
    @notclause ='not'     
end




--print 'Step9......'
declare @varbranch varchar(6)
    set @varbranch = '___'

    if @accttypes ='H'
      set @varbranch =  @varbranch + '0%' 
    if @accttypes ='C'
      set @varbranch =  @varbranch + '4%' 
    if @accttypes ='S'
      set @varbranch =  @varbranch + '5%' 
  if @accttypes in ('H','C','S')
      set @acctwhere = @acctwhere + ' and acct.acctno like ' + '''' + @varbranch + ''''

  if @accttypes ='A'
  begin 
    set @varbranch =  @varbranch + '%' 
    -- todo restrict by accttype if not restricted by branch
    set @acctwhere = @acctwhere + ' and acct.acctno like ' + '''' + @varbranch + ''''
  end

  if @accttypes ='T' -- not special not S
  begin 
    set @varbranch =  @varbranch + '4%' 
    -- todo restrict by accttype if not restricted by branch
    set @acctwhere = @acctwhere + ' and (acct.acctno like ' + '''' + @varbranch + '''' 
    set @varbranch =  '___0%' 
    set @acctwhere = @acctwhere + '  or acct.acctno like ' + '''' + @varbranch + '''' + ')'

  end
  
  if @accttypes ='D' -- not cash not C
  begin 
    set @varbranch =  @varbranch + '0%' 
    -- todo restrict by accttype if not restricted by branch
    set @acctwhere = @acctwhere + ' and (acct.acctno like ' + '''' + @varbranch + '''' 
    set @varbranch =   '___5%' 
    set @acctwhere = @acctwhere + '  or acct.acctno like ' + '''' + @varbranch + '''' + ')'

  end
  if @accttypes ='I' -- not hp not H
  begin 
    set @varbranch =  @varbranch + '4%' 
    -- todo restrict by accttype if not restricted by branch
    set @acctwhere = @acctwhere + ' and (acct.acctno like ' + '''' + @varbranch + '''' 
    set @varbranch =  '___5%' 
    set @acctwhere = @acctwhere + '  or acct.acctno like ' + '''' + @varbranch + '''' + ')'

  end
--print 'Step10......'

if @ArrearsRestriction ='<' -- 'nl' for no limitation ,'<' less than, 'ai<'  arrears/instalment <
begin
   set @acctwhere = @acctwhere +
      '  and acct.arrears < ' + convert(varchar,@Arrears)   --- + ' and acct.outstbal >0 '
end

if @excludecancellations =1 -- 'nl' for no limitation ,'<' less than, 'ai<'  arrears/instalment <
begin
   set @acctwhere = @acctwhere +
      '  and not exists (select * from cancellation c where c.acctno =  acct.acctno) '
end

if @return = 0 and @maxcurrstatus is not null
begin
    --print 'acctwhere..1.' + @acctwhere
   
   set @acctwhere = @acctwhere +
       ' and (acct.currstatus <= ' + '''' +  @maxcurrstatus + '''' + ' or acct.currstatus  in (''U'',''S'',''O'')) '
      
end 

--print 'Step11......'

declare @tables varchar(900), 
        @columns varchar(900), 
        @groupby varchar(900)
create table #accounts (acctno char(12), custid varchar(20), quantity int, transvalue money)
--select @loadfromAcctFirst as loadfromacct
--print '@loadfromAcctFirst...' + @loadfromAcctFirst + ' @totals....' + @totals
    set @acctstatement = ' insert into #accounts (acctno,custid,quantity,transvalue) ' +
                 '  select distinct '     -- distinct added  jec
    set @columns =             '  cu.acctno,cu.custid ' 
    set @tables = '  from custacct cu, agreement,acct ' + @moreTables

    if @totals = 'V' and @itemsdelivered = 1-- Values
    begin
       set @columns = @columns + ' ,0 , sum(transvalue) '
    end

    if @totals = 'V' and @itemsdelivered = 0-- Values
    begin
       set @columns = @columns + ' ,0 , sum(ordval) '
    end



    if @totals ='Q'    -- Quantities
    begin
        set @columns = @columns + ' , sum(quantity),0 '
    end
    set @groupby =''    
    if @totals != 'N' -- Neither
    begin            -- jec
    --print 'totals...1 'h + @totals +  ' columns.... ' +@columns
               set @tables = @tables + ' , ' + @deliveryorordertable + ',stockitem '
               set @groupby = ' group by cu.acctno,cu.custid '

    end        -- jec 08/06/06
    else
    begin
--        set @tables = @tables + ' , delivery,stockitem '
        set @columns = @columns + ' ,0 , 0 '   
        set @groupby =''

    end
    -- letter table required
/*    If @LetterBuilt = 'Y'
        Begin    
            set @tables = @tables + ' , letter '
        End*/


 set @acctwhere = '  where agreement.acctno = cu.acctno and acct.acctno= agreement.acctno and cu.hldorjnt =''H'' '
     + @acctwhere
    if @totals = 'Q' or @totals = 'V' 
   --??? removed 02/06/06
    --     or @totals = 'N'    -- this statement require whether totals or not? Not
        Begin
          set @acctwhere = @acctwhere + ' and ' + @deliveryorordertable +'.acctno =agreement.acctno and ' + 
             @deliveryorordertable + '.agrmtno = agreement.agrmtno ' +
          ' and ' + @deliveryorordertable + '.itemno = stockitem.itemno and ' +
         @deliveryorordertable + '.stocklocn = stockitem.stocklocn '
        End

--    set @acctstatement = @acctstatement + @newline + @columns + @newline + @tables + @newline + @acctwhere + 
 --    @newline + @groupby
--    set @acctstatement = @acctstatement + @columns + @tables + @acctwhere + @groupby
--print 'should get to here.1.....'

    set @acctstatement = @acctstatement + @columns + @newline
    set @acctstatement = @acctstatement + @tables + @newline
    set @acctstatement = @acctstatement + @acctwhere + @newline
 
    -- in( ) is invalid sql so replace with not in('')
    set @acctstatement = replace(@acctstatement,'in( )','not in('''')')

    -- save SQL script for reference/debug  
if @loadfromAcctFirst = 'true'
begin
    set @acctstatement = @acctstatement + @groupby + @newline
    exec dn_CustomerMailingScriptSave @acctstatement, 'Stage1'
    execute sp_executesql @acctstatement
--   if @@error != 0
        -- print @acctstatement
    set @extraclause = @extraclause +  '  and exists (select * from #accounts where #accounts.custid = cu.custid)'
    set @extraBuilt = 'Y'
    
    
    --execute sp_executesql @statement   --?? removed jec 02/06/06

end    --> @loadfromAcctFirst = 'true'

--set @extraclause = '' top 100
set @statement = ' insert into #customers 
    (title,firstname,name,custid,cusaddr1,cusaddr2,cusaddr3,cuspocode,rfcreditlimit,availablespend,email)
        select distinct cu.title,cu.firstname,cu.name,c.custid
      , c.cusaddr1,c.cusaddr2,c.cusaddr3,c.cuspocode,convert(decimal,rfcreditlimit),convert(decimal,availablespend),c.email
              from customer cu, custaddress c,  custacct ca
              where cu.custid = c.custid and c.addtype =''H'' 
              and c.datemoved is null and cu.custid = ca.custid and ca.hldorjnt=''H''
              and cu.age between ' + convert(varchar,@customerstartage) + ' and ' +  convert(varchar,@customerendage)




if @CustomerCodeSet is not null and @CustomerCodeSet !='nr' and @CustomerCodeSet !=' '
Begin
---exec dn_CustomerMailingScriptSave @CustomerCodeSet, 'CustCode 1'
  exec dn_CustomerMailingBuildstring
         @set =@CustomerCodeSet,@whereclause = @custwhere OUT, 
         @keyfield = 'custid',   @column = 'code',@table ='custcatcode' ,@totals ='N'
--exec dn_CustomerMailingScriptSave @custwhere, 'CustCode 1a'
End



if @NoCustomerCodeSet is not null and @NoCustomerCodeSet !='nr' and @NoCustomerCodeSet !=' '
Begin
---exec dn_CustomerMailingScriptSave @NoCustomerCodeSet, 'CustCode 2'
  exec dn_CustomerMailingBuildstring 
  @set =@NoCustomerCodeSet,@whereclause = @custwhere OUT, 
    @keyfield = 'custid',@notclause = 'not', @column = 'code',@table ='custcatcode',@totals ='N'
--exec dn_CustomerMailingScriptSave @custwhere, 'CustCode 2a'
End




if @loadfromAcctFirst = 'true' and @extraBuilt != 'Y'
Begin
    set @extraclause = @extraclause +  '  and exists (select * from #accounts where #accounts.custid = cu.custid)'
End

if @loadFromAcctfirst != 'true'
begin   
    set @statement = @statement + @custwhere + @extraclause
    --print @statement + ' stage 3'
    exec dn_CustomerMailingScriptSave @statement, 'Stage3'
    execute sp_executesql @statement -- load up the customers first
--    if @@error !=0
	    -- print @statement

    set @acctstatement =@acctstatement + ' and exists (select * from #customers where #customers.custid = cu.custid)'
    set @acctstatement = @acctstatement + @groupby + @newline

    execute sp_executesql @acctstatement -- now load up the accounts
--    print @acctstatement
end

-- load up #accounts table
--print @acctstatement + ' stage 4'
-- what does this do!! - removed
--exec dn_CustomerMailingScriptSave @acctstatement, 'Stage4'

--PRINT @loadfromAcctFirst 
if @loadfromAcctFirst != 'true' --removing customers as we have populated the #accounts table with further restrictions
    begin
        --delete from #customers where not exists (select * from #accounts a where a.custid = #customers.custid)

       /* set @Deletestatement = ' delete from #customers where not exists (select * from #accounts a where a.custid = #customers.custid)'
        exec dn_CustomerMailingScriptSave @Deletestatement, 'Delete 1'
        execute sp_executesql @Deletestatement
        -- AA - doesn't actually work - can't explain why- I think something to do with this being in a string means
        that the parser doesn't recognise the #customer as being the same #customer 
       */
         delete from #customers where not exists (select * from #accounts a where a.custid = #customers.custid)
--        print convert(varchar,@@rowcount) + ' number of accounts removed'
    end
else
    begin
    -- if @extraclause is not null   !!removed jec 02/06/06 
    -- !! if @extraclause is null @statement does not execute
         set @statement = @statement + @custwhere + @extraclause
                
        exec dn_CustomerMailingScriptSave @statement, 'Stage5'
        execute sp_executesql @statement
        if @@error !=0
 	   print 'Stage 4:' + @statement
    end



if @ArrearsRestriction = 'ai<'
begin
/*
    delete from #customers where exists (select * from custacct c,acct a,instalplan i
        where c.acctno= a.acctno and c.custid=#customers.custid
        and a.arrears/i.instalamount > @Arrears and a.outstbal >0
       and i.acctno =a.acctno and i.instalamount >0)
*/
     -- declare @DeleteStatement sqltext
/*    set @DeleteStatement = 'delete from #customers where exists 
      (select * from custacct c,acct a,instalplan i
        where c.acctno= a.acctno and c.custid=#customers.custid and a.arrears/i.instalamount > ' 
        + convert(varchar,@Arrears) + '
        and a.outstbal >0 and i.acctno =a.acctno and i.instalamount >0) '
    
    exec dn_CustomerMailingScriptSave @Deletestatement, 'Delete 2'
    execute sp_executesql @Deletestatement*/
    delete from #customers where exists 
      (select * from custacct c,acct a,instalplan i
        where c.acctno= a.acctno and c.custid=#customers.custid and a.arrears/i.instalamount > @Arrears
        and a.outstbal >0 and i.acctno =a.acctno and i.instalamount >0)
    set @return = @@error
end


         
if @return = 0 and @maxeverstatus is not null
begin
    delete from #customers where exists 
      (select * from custacct c, status s where c.acctno= s.acctno and c.hldorjnt ='H'
        and c.custid=#customers.custid 
        and s.statuscode > @maxeverstatus and s.statuscode  not in ('U','S','O'))

     delete from #customers where exists 
      (select * from custacct c, acct a where c.acctno= a.acctno and c.hldorjnt ='H'
        and c.custid=#customers.custid 
        and a.currstatus > @maxeverstatus and a.currstatus  not in ('U','S','O'))

    /*
    set @DeleteStatement = 'delete from #customers where exists 
      (select * from custacct c, status s where c.acctno= s.acctno and c.hldorjnt =''H''
        and c.custid=#customers.custid 
        and s.statuscode > ' + '''' + @maxeverstatus + '''' + ' and s.statuscode  not in (''U'',''S'',''O'')) '
    
    exec dn_CustomerMailingScriptSave @Deletestatement, 'Delete 3'
    execute sp_executesql @statement
    */

end

-- now we have got the customers is values or quantities are required we should update the totals
  set @statement ='  update #customers set quantities = (select sum(quantity) from  '
  set @extraclause =' '
  if @totals = 'Q' 
  begin
      set @statement = ' update #customers set quantity = isnull((select sum(quantity)
      from #accounts a where a.custid =#customers.custid),0) '
      --print @statement + 'stage 5a'
      exec dn_CustomerMailingScriptSave @statement, 'Stage5a'
      execute sp_executesql @statement

  end
  if @totals = 'V' 
  begin
      set @statement = ' update #customers set value = isnull((select sum(transvalue)
      from #accounts a where a.custid =#customers.custid),0) '
      --print @statement + 'stage 5b'
      exec dn_CustomerMailingScriptSave @statement, 'Stage5b'
      execute sp_executesql @statement
  end
   -- now updating accounts. Would ideally use details from the #accounts table
   --set nocount off
  update #customers 
  set acctno= isnull(a.acctno,'') 
  from #accounts a ,acct b
   where a.custid = #customers.custid
  and a.acctno= b.acctno and b.currstatus !='S'

  update #customers 
  set acctno= isnull(a.acctno,'') 
  from #accounts a 
  where a.custid = #customers.custid
  and isnull(#customers.acctno,'') = ''

  update #customers 
  set acctno= isnull(a.acctno,'') 
  from custacct a 
  where a.custid = #customers.custid and a.hldorjnt='H'
  and isnull(#customers.acctno,'') = ''
  
  -- telno
  update #customers set telno = RTRIM(t.dialcode) + ' ' + LTRIM(t.telno)
  from custtel t where t.custid = #customers.custid and t.tellocn = 'H' and ISNULL(t.datediscon,'') = ''
  
  -- don't want commas in result set or excel will be stuff up.
  update #customers set cusaddr1 = replace(cusaddr1,',',''),
	cusaddr2 = replace(cusaddr2,',',''),
	cusaddr3 = replace(cusaddr3,',',''),
	cuspocode = replace(cuspocode,',',''),
	[name] = replace([name],',',''),
	title = replace(title,',',''),
	firstname = replace(firstname,',',''),
	telno = replace(telno,',','')
	
  select * from #customers
--  select count(*) from #accounts
--  select top 100 * from #accounts
go

-- End End End End End End End End End End End End End End End End End End End End End End End End 

