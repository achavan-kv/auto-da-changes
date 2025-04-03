-- Author: John Croft 	17th September 2003
/*
	View to calculate the correct "months in arrears" for Delinquency extract
*/

if OBJECT_ID('DLQ_MthsArrs') IS NOT NULL
	drop view DLQ_MthsArrs

go

create view DLQ_MthsArrs 
as select acctno,monthsinarrears=case
	when arrears=0 then 0
	when instalamount>0 then arrears/instalamount
	when instalamount=0 then 0
	end
		
from summary1
		
where  currstatus<>'S'
and accttype not in('C','S')

go

