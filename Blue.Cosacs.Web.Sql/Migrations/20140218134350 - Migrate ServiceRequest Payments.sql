-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

insert into [Service].[Payment] (PayMethod, CustomerId, Amount, EmpeeNo, RequestId, Bank, ChequeNumber, BankAccountNumber, ChargeType, CardType, CardNumber)
select f.paymethod,r.CustId,isnull(transvalue,0)*-1,empeeno,r.ServiceRequestNo,f.bankcode,f.chequeno,f.bankacctno,
		case when a.ChargeType ='C' then 'Customer' else 'Deliverer' end,case when f.paymethod in (3,4) then c.codedescript else null end,
		case when f.paymethod in (3,4) then case when isnumeric(f.chequeno)=1 then f.chequeno else null end else null end

FROM Service.Request INNER JOIN 
SR_ServiceRequest r ON Service.Request.Id = r.ServiceRequestNo
inner join SR_Resolution re on r.ServiceRequestNo=re.ServiceRequestNo
INNER JOIN SR_ChargeAcct a ON r.ServiceRequestNo = a.ServiceRequestNo
INNER JOIN acct ac ON a.AcctNo = ac.AcctNo
inner join fintrans f on f.AcctNo = ac.AcctNo and f.transtypecode in('PAY','XFR')
left outer join code c on f.paymethod= c.code and category ='FPM'
WHERE --ac.outstbal > 0 AND
 a.ChargeType in ('C', 'D')
and r.ServiceType in('C','N')
and re.ChargeTo in  ('CUS', 'DEL','SUP')

 order by r.ServiceRequestNo

