update service.request
set Evaluation = CASE WHEN Evaluation = 'No Fault Found' THEN 'No Fault Found' 
                     when Evaluation = 'Misuse by the Customer' then 'Misuse by the Customer'
					 when Evaluation = 'Event or Terms Not Covered' then 'Event or Terms Not Covered'
					 when Evaluation = 'Warranty Covered' then 'Warranty Covered' ELSE NULL END
	