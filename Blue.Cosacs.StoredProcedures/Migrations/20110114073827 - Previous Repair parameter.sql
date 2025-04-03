-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
declare @parmcategory INT

select @parmcategory= code from code c where c.category='CMC' and codedescript='Service Request'

insert into CountryMaintenance (CountryCode,ParameterCategory,[Name],Value,[Type],[Precision],
				OptionCategory,OptionListName,[Description],CodeName) 
select CountryCode,	@parmcategory,'Previous Repair',0,'numeric',2,
				'','','This is the cost of previous repairs. If the previous repair cost for a item exceeds this value, a pop-up message will appear when the allocate button is selected in the Soft Script screen. This value is also used for the Previous Repair Total Exceeded filter in the Service Management Review screen','PreviousRepair'
From country