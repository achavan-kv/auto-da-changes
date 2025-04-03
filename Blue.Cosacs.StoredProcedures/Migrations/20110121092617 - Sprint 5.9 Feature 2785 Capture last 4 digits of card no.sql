-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

--Update fintrans chequeno to only capture the last 4 digits of card numbers where payment method is Credit/Debit card. For Cheque this should not be updated and should remain
--the same as before.
update f
set chequeno = right(chequeno, 4)
from fintrans f
inner join code c on f.paymethod = c.code
where c.category = 'FPM'
and (right(c.code,1) in (3,4))
and f.chequeno!=''
