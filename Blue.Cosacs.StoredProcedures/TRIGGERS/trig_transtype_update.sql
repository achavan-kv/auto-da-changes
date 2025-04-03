
/*
-- =============================================
   Author:		Ilyas Parker
   Create date: 2008-Dec-18
   Description:	Add an audit trail for updates to the Transtype table and save the audit to the 
				'Transtypeaudit' table
   Modification:
				
   Change Control
   --------------
   Date      By  Description
   ----      --  -----------
  18/12/08   IP	 Version 1
-- =============================================
*/

if exists(select name from sysobjects where name = 'trig_transtype_update' and type = 'tr')

begin
	drop trigger trig_transtype_update
end
go

--Create a trigger for updates to the 'Transtype' table. A record will be inserted into 
--the 'Transtypeaudit' table to track the change in values to interfaceaccount, interfacesecaccount, branchsplit,
--isdeposit, interfacebalancing, IncludeinGFT, interfacesecbalancing, branchsplitbalancing columns

create trigger trig_transtype_update on transtype
for update
as


declare @transtypecode varchar(3),
		@interfaceaccount varchar(10),
		@interfaceaccountold varchar(10), --will hold the old interfaceaccount value
		@interfacesecaccount varchar(10),
		@interfacesecaccountold varchar(10), --will hold the old interfacesecaccount value
		@branchsplit smallint,
		@branchsplitold smallint, --will hold the old branchsplit value
		@isdeposit smallint,
		@isdepositold smallint, --will hold the old isdeposit value
		@interfacebalancing varchar(10),
		@interfacebalancingold varchar(10), --will hold the old interfacebalancing value
		@IncludeinGFT smallint,
		@IncludeinGFTold smallint, --will hold the old IncludeinGFT value
		@interfacesecbalancing varchar(10),
		@interfacesecbalancingold varchar(10), --will hold the old interfacesecbalancing value
		@branchsplitbalancing smallint,
		@branchsplitbalancingold smallint, --will hold the old branchsplitbalancing value
		@empeenochange int,
		@datechange datetime

--select the new updated values from the 'Inserted' table.

select @transtypecode = transtypecode,
	   @interfaceaccount = interfaceaccount,
	   @interfacesecaccount = interfacesecaccount,
	   @branchsplit = branchsplit,
	   @isdeposit = isdeposit,
	   @interfacebalancing = interfacebalancing,
	   @IncludeinGFT = IncludeinGFT,
	   @interfacesecbalancing = interfacesecbalancing,
	   @branchsplitbalancing = branchsplitbalancing,
	   @empeenochange = empeenochange
from inserted

--select the old values prior to the update from the 'Deleted' table.

select @interfaceaccountold = interfaceaccount,
	   @interfacesecaccountold = interfacesecaccount,
	   @branchsplitold = branchsplit,
       @isdepositold = isdeposit,
	   @interfacebalancingold = interfacebalancing,
	   @IncludeinGFTold = IncludeinGFT,
	   @interfacesecbalancingold = interfacesecbalancing,
	   @branchsplitbalancingold = branchsplitbalancing
from deleted

--If the new values are different to the old values then we need to insert a record
--into the 'Transtypeaudit' table recording the old and new values.

if(@interfaceaccount != @interfaceaccountold or  
   @interfacesecaccount != @interfacesecaccountold or
   @branchsplit != @branchsplitold or
   @isdeposit != @isdepositold or
   @interfacebalancing != @interfacebalancingold or
   @IncludeinGFT != @IncludeinGFTold or
   @interfacesecbalancing != @interfacesecbalancingold or
   @branchsplitbalancing != @branchsplitbalancingold)
begin
	
	insert into transtypeaudit (transtypecode, interfaceaccountold, interfaceaccountnew, 
								 interfacesecaccountold, interfacesecaccountnew, 
								 branchsplitold, branchsplitnew,
								 isdepositold, isdepositnew,
								 interfacebalancingold, interfacebalancingnew,
								 includeinold, includeinnew,
								 interfacesecbalancingold, interfacesecbalancingnew,
								 branchsplitbalancingold, branchsplitbalancingnew,
								 empeenochange, datechange)

	values(@transtypecode, @interfaceaccountold, @interfaceaccount,
		   @interfacesecaccountold, @interfacesecaccount,
		   @branchsplitold, @branchsplit,
		   @isdepositold, @isdeposit,
		   @interfacebalancingold, @interfacebalancing,
		   @IncludeinGFTold, @IncludeinGFT,
		   @interfacesecbalancingold, @interfacesecbalancing,
		   @branchsplitbalancingold, @branchsplitbalancing,
		   @empeenochange, getdate())
	
end
go