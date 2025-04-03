IF OBJECT_ID('GetDuplicateCustomersSP') IS NOT NULL
	DROP PROCEDURE GetDuplicateCustomersSP
GO 
CREATE PROCEDURE GetDuplicateCustomersSP
  
-- =============================================  
-- Author:  Ilyas Parker  
-- Create date: 5th August 2014  
-- Description: Retrieves potentially duplicate customers 
-- Displays duplicate customers in the Duplicate Customers screen
--  
--   
-- Change Control  
-----------------  
-- =============================================  
 -- Add the parameters for the stored procedure here  

AS  

select 
	c1.custid as Customer, 
	c2.custid as DuplicateCustomer,
	c1.firstname, 
	c1.name, 
	c1.dateborn, 
	ROW_NUMBER() over(PARTITION BY c1.firstname, c1.name, c1.dateborn order by c1.firstname) as id,
	DENSE_RANK() over(ORDER BY c1.custid) as id2 
into 
	#duplicates
from 
	customer c1
CROSS JOIN 
	customer c2
where 
	c1.firstname = c2.firstname
	and c1.name = c2.name
	and c1.dateborn = c2.dateborn
	and c1.custid != c2.custid

select 
	Customer
into 
	#customers
from 
	#duplicates
where 
	id = 1


delete from #duplicates
where not exists(select * from #customers
					where #customers.Customer = #duplicates.Customer)



insert into
	 #duplicates
select 
	D1.DuplicateCustomer, 
	D2.DuplicateCustomer, 
	D1.firstname, 
	D1.name, 
	D1.dateborn, 	
	ROW_NUMBER() over(PARTITION BY d1.firstname, d1.name, d1.dateborn order by d1.firstname) as id,
	DENSE_RANK() over(ORDER BY d1.customer) as id2
from 
	#duplicates d1
left join 
	#duplicates d2 on d1.id2 = d2.id2
where 
	d1.DuplicateCustomer != d2.DuplicateCustomer
	and d1.id2 = d2.id2
	and d1.id < d2.id

select 
	d.Customer, 
	d.DuplicateCustomer as [Duplicate Customer], 
	d.firstname, 
	d.name,
	d.dateborn,
	cast(0 as bit) as [Resolved / Unresolved]
into 
	#final
from 
	#duplicates d

where not exists
	(select * from DuplicateCustomers dc
		where dc.Custid = d.Customer
		and dc.DuplicateCustid = d.DuplicateCustomer)

insert into #final
	select
		dc.Custid,
		dc.DuplicateCustid,
		c.firstname,
		c.name,
		c.dateborn,
		cast(1 as bit) as [Resolved / Unresolved]
from 
	DuplicateCustomers dc
	inner join customer c on dc.custid = c.custid


select 
	f.Customer as [Potential Duplicate A],
	f.[Duplicate Customer] as [Potential Duplicate B],
	f.[Resolved / Unresolved]
from 
	#final f
	order by DENSE_RANK() over(ORDER BY f.firstname, f.name, f.dateborn), f.Customer



GO


