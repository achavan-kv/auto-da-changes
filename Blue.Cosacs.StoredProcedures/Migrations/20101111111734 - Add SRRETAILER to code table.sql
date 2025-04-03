-- Add Service Retailer for Non Courts SR's'
Insert into codecat (origbr,category,catdescript,codelgth,forcenum,forcenumdesc,usermaint,
		CodeHeaderText,DescriptionHeaderText,SortOrderHeaderText,ReferenceHeaderText,AdditionalHeaderText,ToolTipText)
values (null,'SRRETAILER','Service Non courts Retailers',4,'N','N','Y',
		'Retailer Code','Retailer Name',null,null,null,null)

Insert into code (origbr,category,code,codedescript,statusflag,sortorder,reference,additional)
values (null,'SRRETAILER','OTH','Other','L',0,0,'')

