if exists (select * from sysobjects where name ='dn_CustomerMailingQueryGet')
drop procedure dn_CustomerMailingQueryGet
go
create procedure dn_CustomerMailingQueryGet
   @EmpeenoSave   int ,
   @datesaved   datetime OUT,
   @QueryName   varchar(128) ,
   @CustomerCodeSet   varchar(64) OUT,
   @NoCustomerCodeSet   varchar(64) OUT,
   @AccountCodeSet   varchar(64) OUT,
   @NoAccountCodeSet   varchar(64) OUT,
   @ArrearsRestriction   varchar(4) OUT,
   @Arrears   float OUT,
   @maxcurrstatus   char(1) OUT,
   @maxeverstatus   char(1) OUT,
   @branchset   varchar(64) OUT,
   @accttypes   varchar(2) OUT,
   @itemset   varchar(64) OUT,
   @itemsetstartdate   datetime OUT,
   @itemsetenddate   datetime OUT,
   @noitemset   varchar(64) OUT,
   @noitemsetstartdate   datetime OUT,
   @noitemsetenddate   datetime OUT,
   @itemcatset   varchar(64) OUT,
   @itemCatsetstartdate   datetime OUT,
   @itemCatsetenddate   datetime OUT,
   @noitemCatset   varchar(64) OUT,
   @noitemCatsetstartdate   datetime OUT,
   @noitemCatsetenddate   datetime OUT,
   @itemsdelivered   smallint OUT,
   @itemstartswithset   varchar(64) OUT,
   @itemstartswithstartdate   datetime OUT,
   @itemstartswithenddate   datetime OUT,
   @noitemstartswithset   varchar(64) OUT,
   @noitemstartswithstartdate   datetime OUT,
   @noitemstartswithenddate   datetime OUT,
   @noletterset   varchar(64) OUT,
   @nolettersetStartdate   datetime OUT,
   @nolettersetEnddate   datetime OUT,
   @letterset   varchar(64) OUT,
   @lettersetstartdate   datetime OUT,
   @lettersetenddate   datetime OUT,
   @customerstartage   smallint OUT,
   @customerEndage   smallint OUT,
   @totals   char(1) OUT,
   @resulttype   varchar(10) OUT,
   @excludecancellations   smallint OUT,
   @return INT out
as
   set @return =0
select
   @datesaved  = datesaved,
   @QueryName  = QueryName,
   @CustomerCodeSet  = CustomerCodeSet,
   @NoCustomerCodeSet  = NoCustomerCodeSet,
   @AccountCodeSet  = AccountCodeSet,
   @NoAccountCodeSet  = NoAccountCodeSet,
   @ArrearsRestriction  = ArrearsRestriction,
   @Arrears  = Arrears,
   @maxcurrstatus  = maxcurrstatus,
   @maxeverstatus  = maxeverstatus,
   @branchset  = branchset,
   @accttypes  = accttypes,
   @itemset  = itemset,
   @itemsetstartdate  = itemsetstartdate,
   @itemsetenddate  = itemsetenddate,
   @noitemset  = noitemset,
   @noitemsetstartdate  = noitemsetstartdate,
   @noitemsetenddate  = noitemsetenddate,
   @itemcatset  = itemcatset,
   @itemCatsetstartdate  = itemCatsetstartdate,
   @itemCatsetenddate  = itemCatsetenddate,
   @noitemCatset  = noitemCatset,
   @noitemCatsetstartdate  = noitemCatsetstartdate,
   @noitemCatsetenddate  = noitemCatsetenddate,
   @itemsdelivered  = itemsdelivered,
   @itemstartswithset  = itemstartswithset,
   @itemstartswithstartdate  = itemstartswithstartdate,
   @itemstartswithenddate  = itemstartswithenddate,
   @noitemstartswithset  = noitemstartswithset,
   @noitemstartswithstartdate  = noitemstartswithstartdate,
   @noitemstartswithenddate  = noitemstartswithenddate,
   @noletterset  = noletterset,
   @nolettersetStartdate  = nolettersetStartdate,
   @nolettersetEnddate  = nolettersetEnddate,
   @letterset  = letterset,
   @lettersetstartdate  = lettersetstartdate,
   @lettersetenddate  = lettersetenddate,
   @customerstartage  = customerstartage,
   @customerEndage  = customerEndage,
   @totals  = totals,
   @resulttype  = resulttype,
   @excludecancellations  = excludecancellations
from CustomerMailingQuery
  where
   EmpeenoSave  = @EmpeenoSave
   and QueryName =@QueryName  

   return @return
go
