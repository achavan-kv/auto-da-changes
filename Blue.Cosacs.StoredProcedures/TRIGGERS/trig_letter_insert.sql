--66539 30-dEC-2004 SCRIPT TO REMOVE ADD-TO LETTERS GENERATED FOR ANY rf ACCOUNTS-THESE  accounts SHOULD NOT QUALIFY FOR ADD-TO LETTERS
IF EXISTS (SELECT * FROM SYSOBJECTS WHERE  NAME = 'tr_letter_addto_removerf')
DROP TRIGGER tr_letter_addto_removerf
GO
create trigger tr_letter_addto_removerf 
on letter
for insert
as
delete from letter where exists (select * from 
 inserted,acct
where inserted.acctno = letter.acctno and acct.acctno = letter.acctno
and letter.lettercode in ('M','N','O') -- add-to letter codes
 and acct.accttype ='R' and inserted.lettercode=letter.lettercode )

