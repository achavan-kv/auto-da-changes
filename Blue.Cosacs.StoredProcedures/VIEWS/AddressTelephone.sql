if exists (select * from information_schema.tables 
where table_name ='AddressTelephone')
drop view AddressTelephone
go
create view AddressTelephone 
as select 
	ca.custid,ca.cusaddr1,
        ca.cusaddr2,ca.cusaddr3,ca.cuspocode,ca.Email,
        --H.telno as HomePhone,
        M.telno as Mobile,W.telno as WorkPhone,
        ca.datein,
        ca.resstatus,ca.mthlyrent,ca.datemoved,
        ca.PropType,ca.empeenochange,ca.datechange,
        ca.notes,ca.deliveryarea
FROM CUSTADDRESS CA
LEFT JOIN CUSTTEL H on ca.custid = h.custid and h.datediscon is null and h.tellocn = 'H'
LEFT JOIN CUSTTEL M on ca.custid = M.custid and M.datediscon is null and M.tellocn = 'M'
LEFT JOIN CUSTTEL W on ca.custid = W.custid and W.datediscon is null and W.tellocn = 'W'

